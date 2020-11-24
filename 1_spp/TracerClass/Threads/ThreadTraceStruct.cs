 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TracerClass
{
    [Serializable]
    public class ThreadTraceStruct
    {
       
       
        public ThreadTraceStruct()
        {

        }

        public ThreadTraceStruct(int wholeTime, List<TraceResult> Methods)
        {
            WholeTime = wholeTime;
            methods = Methods;
        }

       

        //public override string ToString()
        //{
        //    return _wholeTime.ToString();
        //}


        public int WholeTime { get; set; }
       
        public List<TraceResult> methods { get; set; }

    }
}
