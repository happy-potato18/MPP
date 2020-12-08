using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mpp_5
{
  
    public class DependencyProvider
    {
        private readonly DependencyConfiguration _dependencyConfiguration;
        private Dictionary<Type,object> _singletonCache;

               
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

        private object ProduceSingleImplementation(Type singleAbstractionType, TypeDef impl)
        {
            Type implType = impl.Type;
            var dependencies = implType.GetConstructors()
                              .OrderByDescending(constr => constr.GetParameters().Length)
                              .First().GetParameters();
            var argumentsList = new List<object>();
            if (dependencies.Length != 0)
            {
                foreach (var dependency in dependencies)
                {
                    if (_dependencyConfiguration.Configurations.ContainsKey(dependency.ParameterType))
                    {
                        //foreach(var depImpl in dependencyImpls)
                        //resolvedDependencies.Add(CreateImplementationInstance(depImpl));
                        argumentsList.Add(ProduceImplementations(dependency.ParameterType));
                    }
                    else
                    {
                        argumentsList.Add(default);
                    }
                }
            }
            if (impl.IsSingleton)
            {

                return GetOrCreateSingletonInstance(implType, argumentsList);
            }
            else
            {
                return CreateObject(implType, argumentsList);
            }
        }
        
        private dynamic ProduceImplementations(Type abstractionType)
        {

            if (abstractionType.GetInterface("IEnumerable") != null)
            {
                var implClassList = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType.GetGenericArguments()[0]);
                var resolvedImpls = new List<object>();
                foreach (var implClass in implClassList)
                {
                    resolvedImpls.Add(ProduceSingleImplementation(abstractionType,implClass));
                }

                return resolvedImpls;
            }
            //else if (abstractionType.IsGenericType)
            //{

            //    return default;
            //}
            else
            {
                var implClass = _dependencyConfiguration.GetConfigurationForAbstraction(abstractionType)[0];
                object instance = ProduceSingleImplementation(abstractionType, implClass);
                return instance;
            }
                      
        }

      
        public dynamic Resolve<TAbstraction>() 
        {
                       
           return ProduceImplementations(typeof(TAbstraction));
        }

    }
}