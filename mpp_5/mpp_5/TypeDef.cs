using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_5
{
    public class TypeDef
    {
        ///<summary>
        ///Set <paramref name="isSingleton"/> parameter as true to create singleton-typed object afterwards
        ///    </summary>
        public TypeDef(Type type, bool isSingleton = false)
        {
            Type = type;
            IsSingleton = isSingleton;
        }
        public Type Type { get; set; }
        public bool IsSingleton { get; set; }
    }
}
