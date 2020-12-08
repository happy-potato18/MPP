using System;
using System.Collections.Generic;
using System.Linq;

namespace mpp_5
{
    public class DependencyConfiguration
    {
        public  Dictionary<Type, List<TypeDef>> Configurations { get; private set; }
        
        public DependencyConfiguration()
        {
            Configurations = new Dictionary<Type, List<TypeDef>>();
        }

        public List<TypeDef> GetConfigurationForAbstraction(Type abstractionType)
        {
            try
            {
                return Configurations[abstractionType];
            }
            catch(KeyNotFoundException)
            {
                return null;
            }
            
        }

        private void Register(Type abstractionType, TypeDef implementationValue)
        {
           
            var abstractionKey = abstractionType;
            if (Configurations.ContainsKey(abstractionKey))
            {
                if (!Configurations[abstractionKey]
                                   .Any(typeDef => typeDef.Type == implementationValue.Type))
                {
                    Configurations[abstractionKey].Add(implementationValue);
                }
            }
                
            else
            {
                Configurations.Add(abstractionKey, new List<TypeDef>() { implementationValue });
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
