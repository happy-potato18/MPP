using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TracerClass
{
    public class Tracer : ITracer
    {

        private string GetMethodName()
        {
            return new StackTrace(2).GetFrame(0).GetMethod().Name;
        }

        private string GetClassName()
        {
            return new StackTrace(2).GetFrame(0).GetMethod().DeclaringType.Name;
        }

        
        public static ConcurrentDictionary<int, ThreadTraceStruct> ThreadsInfo = new ConcurrentDictionary<int, ThreadTraceStruct>();
        private DateTime _start;
        
        public ConcurrentDictionary<int,ThreadTraceStruct> GetTraceResult()
        {
          
            while(true)
            {
                bool isEnded = true;
                if (ThreadsInfo.IsEmpty)
                    return ThreadsInfo;
                foreach (var thread in ThreadsInfo)
                {
                    if((!thread.Value.methods[thread.Value.methods.Count-1].isStopped) || (thread.Value == null ))
                    {
                        isEnded = false;
                        break;
                    }
                        
                }
                if(isEnded)
                {
                    foreach (var thread in ThreadsInfo)
                    {

                        thread.Value.WholeTime = thread.Value.methods.Sum(item => item.time);
                    }
                    break;
                }
               

            }
            
            return ThreadsInfo;
        }
       
        public void StartTrace()
        {
            _start = DateTime.Now;
            int ThreadId = Thread.CurrentThread.ManagedThreadId;
            try 
            {
                ThreadsInfo.TryAdd(ThreadId, new ThreadTraceStruct(0, new List<TraceResult>()));                
            }
            finally
            {
                var methods = ThreadsInfo[ThreadId].methods;
                while(true)
                {
                    if ( (methods.Count == 0) || (methods[methods.Count - 1].isStopped) )
                    {
                        methods.Add(new TraceResult(GetClassName(), GetMethodName(), 0, new List<TraceResult>()));
                        break;
                    }
                    else
                        methods = methods[methods.Count - 1].methods;
                }

            }
          
        }

        public void StopTrace()
        {
            var time = DateTime.Now.Subtract(_start).Milliseconds;

            var methods = ThreadsInfo[Thread.CurrentThread.ManagedThreadId].methods;
            TraceResult previousMethod = methods[methods.Count - 1];

            while (true)
            {
                if ( (methods.Count == 0) || (methods[methods.Count - 1].isStopped) )
                {
                    previousMethod.isStopped = true;
                    previousMethod.time = time;
                    break;

                }
                else
                {
                    previousMethod = methods[methods.Count - 1];
                    methods = methods[methods.Count - 1].methods;
                   
                }
               
            }


        }

        


    }
}
