using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracerClass
{
   
    public interface ITracer
    {
      
        void StartTrace();
      
        void StopTrace();     
        ConcurrentDictionary<int,ThreadTraceStruct> GetTraceResult();
    }
}
