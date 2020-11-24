using PluginBase;
using System;

namespace CharPlugin
{
    public class CharGenerator : IPlugin
    {
        public Type GeneratedType { get => typeof(char); }

        public object GenerateValue(Type t)
        {
            return (char)new Random().Next(Byte.MinValue+56, Byte.MaxValue + 1);
            
        }
    }
}
