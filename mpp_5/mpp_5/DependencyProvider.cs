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

        private object GetOrCreateSingletonInstance(Type implType, List<object> argumentsList)
        {
            object instance = new object();
            object locker = new object();
            if (!_singletonCache.ContainsKey(implType))
            {
                lock (locker)
                {
                    if (!_singletonCache.ContainsKey(implType))
                    {

                        _singletonCache.Add(implType, CreateObject(implType, argumentsList));
                    }
                }
            }
            else
            {
                instance = _singletonCache[implType];
            }

            return instance;

        }

        private object CreateObject(Type implType, List<object> argumentsList)
        {
            if (argumentsList.Count != 0)
                return Activator.CreateInstance(implType, argumentsList.ToArray());
            else
                return Activator.CreateInstance(implType);
        }

        private object ProduceCurrentImplementation(TypeDef impl)
        {
            Type implType = impl.Type;
            var parameters = implType.GetConstructors()
                              .OrderByDescending(constr => constr.GetParameters().Length)
                              .First().GetParameters();
            var argumentsList = new List<object>();
            //List<dynamic> resolvedNestedImpls;
            if (parameters.Length != 0)
            {
                foreach (var parameter in parameters)
                {
                    if (_dependencyConfiguration.Configurations.TryGetValue(parameter.ParameterType, out List<TypeDef> nestedImpls))
                    {
                        var abstractionType = parameter.ParameterType;
                        argumentsList.Add(ResolveAbstractionConfig(abstractionType));
                    }
                    else
                    {
                        argumentsList.Add(default);
                    }
                }
            }
            
            if (impl.IsSingleton)
                return GetOrCreateSingletonInstance(implType, argumentsList);
            else
                return CreateObject(implType, argumentsList);
        }
        
        private dynamic ResolveAbstractionConfig(Type abstractionType)
        {
            var config = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType);
            if (config.Count > 1)
            {
                var resolvedImplsList = new List<object>();
                foreach(var implClass in config)
                {
                   resolvedImplsList.Add(ProduceCurrentImplementation(implClass));
                }
                return resolvedImplsList[0];
            }
            else
            {
                var implClass =config[0];
                object instance = ProduceCurrentImplementation(implClass);
                return instance;
            }

            //else if (abstractionType.IsGenericType)
            //{

            //    return default;
            //}
           


                      
        }

        private IEnumerable<TAbstraction> ProduceMultipleImplementations<TAbstraction>()
        {
           
            var implClassList = _dependencyConfiguration.GetConfigurationForAbstraction(typeof(TAbstraction));
            var resolvedImpls = new List<TAbstraction>();
            foreach (var implClass in implClassList)
            {
                resolvedImpls.Add((TAbstraction)ProduceCurrentImplementation(implClass));
            }

            return resolvedImpls;
           
        }
      
        public dynamic Resolve<TAbstraction>()
        {
            if(typeof(TAbstraction).GetInterface("IEnumerable") != null)
            {
                var abstrType = typeof(TAbstraction).GetGenericArguments()[0];
                var method = typeof(DependencyProvider).GetMethod("ProduceMultipleImplementations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var resolvedImplsList = method.MakeGenericMethod(new Type[] { abstrType }).Invoke(this,new object[] { });
                return resolvedImplsList;
            }
            else
                return ResolveAbstractionConfig(typeof(TAbstraction));
        }

    }
}