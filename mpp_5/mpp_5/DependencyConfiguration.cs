using System;
using System.Collections.Generic;
using System.Linq;

namespace mpp_5
{
    public class DependencyConfiguration
    {
        private Dictionary<Type, List<TypeDef>> configurations;
        
        public DependencyConfiguration()
        {
            configurations = new Dictionary<Type, List<TypeDef>>();
        }

        private void Register(Type abstractionType, TypeDef implementationValue)
        {
            if ((abstractionType.IsClass) &&
                ((implementationValue.Type.BaseType == abstractionType) ||
                  (implementationValue.Type.GetInterface(abstractionType.Name) != null)
                )
             )
            {
                var abstractionKey = abstractionType;
                if (configurations.ContainsKey(abstractionKey))
                    configurations[abstractionKey].Add(implementationValue);
                else
                {
                    if (!configurations[abstractionKey]
                                     .Any(typeDef => typeDef.Type == implementationValue.Type))
                    {
                        configurations.Add(abstractionKey, new List<TypeDef>() { implementationValue });
                    }

                }
            }

        }

        public void RegisterSingleton(Type abstraction, Type implementation)
        {
            var implementationValue = new TypeDef(implementation, true);
            Register(abstraction, implementationValue);
        }

        public void RegisterSingleton<TAbstraction, TImplementation>() 
        {
            RegisterSingleton(typeof(TAbstraction), typeof(TImplementation));

        }
        public void RegisterTransient(Type abstraction, Type implementation)
        {
            var implementationValue = new TypeDef(implementation);
            Register(abstraction, implementationValue);
                       
        }

        public void RegisterTransient<TAbstraction, TImplementation>()
        {
            RegisterTransient(typeof(TAbstraction), typeof(TImplementation));
        }
    }

}
