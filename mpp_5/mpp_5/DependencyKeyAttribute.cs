using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_5
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : System.Attribute
    {
        public int DependencyName { get; set; } = -1;

        public DependencyKeyAttribute()
        { }

        public DependencyKeyAttribute(int dependencyName)
        {
            DependencyName = dependencyName;
        }
    }
}
