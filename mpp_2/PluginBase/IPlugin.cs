using System;

namespace PluginBase
{
    public interface IPlugin
    {
        Type GeneratedType { get; }
        object GenerateValue(Type t);
    }
}
