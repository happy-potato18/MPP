using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TracerClass
{
    public class TraceResult : ITraceResult
    {
        public TraceResult()
        {

        }
        public TraceResult(string ClassName, string MethodName, int Time, List<TraceResult> Methods)
        {
            methodName = MethodName;
            className = ClassName;
            time = Time;
            methods = Methods;

        }
       
       
        [NonSerialized()] public bool isStopped = false;
        
        public int time { get; set; }
        public string methodName { get; set; }
        public string className { get; set; }
        public List<TraceResult> methods { get; set; }
    }
}
