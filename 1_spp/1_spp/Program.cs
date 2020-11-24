using SerializatorClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TracerClass;
using WriterClass;

namespace _1_spp
{
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }

        public void MyMethod()
        {
            _tracer.StartTrace();

           
             foreach(char k in "as;lf;lkdl")
            {
                
                Console.WriteLine("Hello!");
                
            }
            _bar.InnerMethod();

            foreach (char k in "as;lf;lkdl")
            {
               
                Console.WriteLine("Hello222!");
                
            }

            _tracer.StopTrace();
           
        }

        public void MyMethod2()
        {
            _tracer.StartTrace();
           
            foreach (char k in "as;lf;lkdl")
            {
                
                Console.WriteLine("Hello!");
               
            }
            _bar.InnerMethod();

            foreach (char k in "as;lf;lkdl")
            {
                
                Console.WriteLine("Hello222!");
               
            }

            _tracer.StopTrace();
            
            

        }
    }

    public class Bar
    {
        private ITracer _tracer;

        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void InnerMethod()
        {
            _tracer.StartTrace();

            foreach (char k in "as;lf;lkdl")
            {
                Console.WriteLine("Hello3333!");
                
            }

            _tracer.StopTrace();
        }
    }


    class Program
    {

        public static void ThreadCode()
        {
            var tr = new Tracer();
            var boo = new Foo(tr);
            boo.MyMethod();
            boo.MyMethod2();
           
        }
        static void Main(string[] args)
        {
           
            for (int i = 0; i < 5; i++)
            {
                Thread myThread = new Thread(ThreadCode);
                myThread.Start();
            }
           
            var tr = new Tracer();
            var a = tr.GetTraceResult();
           
            var ser1 = new SerializatorXML<ThreadTraceStruct>();
            var ser2 = new SerializatorJSON<ThreadTraceStruct>();
            var ret1 = ser1.MySerialize(a);
            var ret2 = ser2.MySerialize(a);
            var writer = new Writer();
            writer.Write(ret1,Console.OpenStandardOutput());
            writer.Write(ret1, new FileStream("xml.txt", FileMode.OpenOrCreate, FileAccess.Write));
            Console.WriteLine("\n____________________________________________\n");
            writer.Write(ret2, Console.OpenStandardOutput());
            writer.Write(ret2, new FileStream("json.txt", FileMode.OpenOrCreate, FileAccess.Write));
            Console.ReadKey();



        }
    }
}
