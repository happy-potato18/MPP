using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializatorClass
{
    public interface ISerializator<T>
    {
        string MySerialize(ConcurrentDictionary<int,T> threads);
        
    }
}
