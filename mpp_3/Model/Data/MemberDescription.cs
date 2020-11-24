using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Data
{
    public class MemberDescription
    {

        public MemberDescription(string _memberName, MemberType _type, Assembly _assembly )
        {
            Type = _type;
            MemberName = _memberName;
            Assembly = _assembly;
        }
        
        public string MemberName { get; set; }
        public MemberType Type { get; set; }
        public Assembly Assembly {get;set;}

    }

}
