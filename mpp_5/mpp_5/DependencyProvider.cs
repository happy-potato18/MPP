using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mpp_5
{
  
    public class DependencyProvider
    {
        private readonly DependencyConfiguration _dependencyConfiguration;
        private readonly Dictionary<Type,object> _singletonCache;

        ///<summary>
        /// Throws exception when regestered implementation does nor inherit from corresponding abstraction
        ///    </summary>   
        private bool ValidateConfigurationOrThrowException(DependencyConfiguration configuration)
        {
            foreach (var dc in configuration.Configurations)
            {
                var abstractionType = dc.Key;
                foreach(var implementationTypeDef in dc.Value)
                {
                    if (!( ( (abstractionType.IsClass) || (abstractionType.IsInterface) ) &&
                       ((implementationTypeDef.Type.BaseType == abstractionType) ||
                        (implementationTypeDef.Type.GetInterface(abstractionType.Name) != null) ||
                        (implementationTypeDef.Type == abstractionType)
                       )
                     ))
                    {
                        throw new IneligibleImplementationException
                           (String.Format("Type {0} is ineligble implementation for type {1}.", implementationTypeDef.Type.Name, abstractionType.Name));
                    }
                                               
                }
               
            }
            return true;
                       
        }
        public DependencyProvider(DependencyConfiguration dependencyConfiguration)
        {
             if(ValidateConfigurationOrThrowException(dependencyConfiguration))
             {
                _dependencyConfiguration = dependencyConfiguration;
                _singletonCache = new Dictionary<Type, object>();
            }
           
        }

        ///<summary>
        /// Returns current implementation instance from the cache, if exists, otherwise creates new instance and adds it to the cache
        ///    </summary>  
        private object GetOrCreateSingletonInstance(Type implType, List<object> argumentsList)
        {
            dynamic instance = null;
            object locker = new object();
            if (!_singletonCache.ContainsKey(implType))
            {
                lock (locker)
                {
                    if (!_singletonCache.ContainsKey(implType))
                    {
                        instance = CreateObject(implType, argumentsList);
                        _singletonCache.Add(implType, instance);
                        
                    }
                }
            }
            else
                return _singletonCache[implType];
            
            return instance;

        }

        ///<summary>
        /// Creates and returns instance of "instance per dependency" implementation
        ///    </summary>  
        private object CreateObject(Type implType, List<object> argumentsList)
        {
            if (argumentsList.Count != 0)
                return Activator.CreateInstance(implType, argumentsList.ToArray());
            else
                return Activator.CreateInstance(implType);
        }


        ///<summary>
        /// Checking dependency lifetime type and produce implementation
        ///    </summary>
        private object ProduceCurrentImplementation(TypeDef impl)
        {
            Type implType = impl.Type;
            var parameters = implType.GetConstructors()
                              .OrderByDescending(constr => constr.GetParameters().Length)
                              .First().GetParameters(); // get the most advanced constructor
            var argumentsList = new List<object>();
            if (parameters.Length != 0)
            {
                foreach (var parameter in parameters)
                {
                    // if dependency also were registered in config
                    if (_dependencyConfiguration.Configurations.TryGetValue(parameter.ParameterType, out List<TypeDef> nestedImpls)) 
                    {
                        //if has named dependency in constructor(marked with KeyDependencyAttribute)
                        if(parameter.GetCustomAttributes(typeof(DependencyKeyAttribute),false).Length != 0 )
                        {
                            var dependencyKey =(DependencyKeyAttribute)parameter.GetCustomAttributes(typeof(DependencyKeyAttribute), false)[0];
                            foreach(var namedImpl in nestedImpls)
                            {
                                if (namedImpl.Name == dependencyKey.DependencyName)
                                {
                                    argumentsList.Add(ProduceCurrentImplementation(namedImpl));
                                    break;
                                }
                            }
                            
                        }
                        else
                        {
                            var abstractionType = parameter.ParameterType;
                            argumentsList.Add(ResolveAbstractionConfig(abstractionType));
                        }
                       
                    }
                    else
                        argumentsList.Add(default);
                }
            }
            
            if (impl.IsSingleton)
                return GetOrCreateSingletonInstance(implType, argumentsList);
            else
                return CreateObject(implType, argumentsList);
        }

        ///<summary>
        /// Produce implementation(s) only for closed generic type, where
        /// <paramref name="impl"/> is regestered open generic dependency
        /// <paramref name="closedGenericType"/> is passed in Resolve() closed generic type
        /// <paramref name="dependency"/> is implementation which is passed as parameter in constructor
        ///    </summary>
        private object ProduceCurrentGenericImplementation(TypeDef impl, Type closedGenericType, object dependency)
        {
            if(impl.IsSingleton)
                 return GetOrCreateSingletonInstance(closedGenericType,new List<object>() {dependency});
            else 
                return CreateObject(closedGenericType, new List<object>() { dependency });
        }

        ///<summary>
        /// Retrive implementation(s) for  <paramref name="abstractionType"/>
        ///    </summary>
        private dynamic ResolveAbstractionConfig(Type abstractionType)
        {
            var config = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType);
            //if open generic were registered but actually in Resolve() were passed closed generic type
            if(config == null) 
            {
                if (abstractionType.IsGenericType)
                {
                    try
                    {
                        var implClass = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType.GetGenericTypeDefinition())[0]; // retrieve config for open generic
                        Type nestedType = abstractionType.GetGenericArguments()[0];
                        var nestedTypeImpl = _dependencyConfiguration.GetConfigurationForAbstraction(nestedType)[0]; // retrieve config for nested type in passed closed generic
                        object resolvedNestedImpl = ProduceCurrentImplementation(nestedTypeImpl); // produce nested type implementation
                        Type closedGenericType = implClass.Type.MakeGenericType(new Type[] { nestedType });
                        var genericImpl = ProduceCurrentGenericImplementation(implClass, closedGenericType, resolvedNestedImpl);  // produce closed generic type implementation
                        return genericImpl;
                    }
                    catch (UnregisteredAbstractionTypeException)
                    {
                        return null;
                    }
                   
                   
                }
                else
                {
                    throw new UnregisteredAbstractionTypeException(String.Format("Type {0} has not been registered.", abstractionType.Name));
                }
                
                
            }
            
            // if multiple implementations exist for current abstraction
            if (config.Count > 1)
            {
                var resolvedImplsList = new List<object>();
                foreach(var implClass in config)
                {
                   resolvedImplsList.Add(ProduceCurrentImplementation(implClass));
                }
                return resolvedImplsList[0];
            }
            else  // if single implementation exists for current abstraction
            {
                var implClass = config[0];
                object instance = ProduceCurrentImplementation(implClass);
                return instance;
            }            
                      
        }

        ///<summary>
        ///Dynamically invoked method for multiple dependencies
        ///    </summary>
        private IEnumerable<TAbstraction> ResolveAbstractionMultipleConfigs<TAbstraction>()
        {
           
            var implClassList = _dependencyConfiguration.GetConfigurationForAbstraction(typeof(TAbstraction));
            var resolvedImpls = new List<TAbstraction>();
            foreach (var implClass in implClassList)
                 resolvedImpls.Add((TAbstraction)ProduceCurrentImplementation(implClass));
           
            return resolvedImpls;
           
        }
         

        public dynamic Resolve<TAbstraction>()
        {
            //dynamically invoke method when TAbstraction is IEnumerable<TRealAbstraction>
            if(typeof(TAbstraction).GetInterface("IEnumerable") != null)
            {
                var abstrType = typeof(TAbstraction).GetGenericArguments()[0];
                var method = typeof(DependencyProvider).GetMethod("ResolveAbstractionMultipleConfigs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var resolvedImplsList = method.MakeGenericMethod(new Type[] { abstrType }).Invoke(this,new object[] { });
                return resolvedImplsList;
            }           
            else
                return ResolveAbstractionConfig(typeof(TAbstraction));
        }

        public dynamic ResolveNamed<TAbstraction>(int dependencyName)
        {
            var abstractionType = typeof(TAbstraction);
            try
            {
                var implClass = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType).Find(impl => impl.Name == dependencyName);
                object instance = ProduceCurrentImplementation(implClass);
                return instance;
            }
            catch(UnregisteredAbstractionTypeException)
            {
                return null;
            }          
            
        }

    }
}