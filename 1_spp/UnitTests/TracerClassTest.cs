using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TracerClass;

namespace UnitTests
{
    public class Foo
    {
       
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
           
        }
       
        public void CopyList()
        {
            _tracer.StartTrace();

            var arr = new List<int>() { 1, 23, 4, 5, 34, 4, 34, 55, 3232, 1234, 1, 67, 34, 6, 4, 6, 409, 555, 5456 };
            var arr2 = new List<int>();
            foreach (int a in arr)
            {
               arr2.Add( a + 56);
               
            }
            _tracer.StopTrace();



        }
    }

    public class Foo3
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo3(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }

        public void MyMethod()
        {
            _tracer.StartTrace();


            var arr = new List<int>() { 1, 23, 4, 5, 34, 4, 34, 55, 3232, 1234, 1, 67, 34, 6, 4, 6, 409, 555, 5456 };
            var arr2 = new List<int>();
            foreach (int a in arr)
            {
                arr2.Add(a + 56);

            }
            _bar.InnerMethod();

           
            foreach (int a in arr2)
            {
                arr.Add(a*2);

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

            var arr = new List<int>() { 1, 23, 4, 5, 34, 4, 34, 55, 3232, 1234, 1, 67, 34, 6, 4, 6, 409, 555, 5456 };
            var arr2 = new List<int>();
            foreach (int a in arr)
            {
                arr2.Add(a + 2);

            }

            _tracer.StopTrace();
        }
    }



    [TestClass]
    public class TracerClassTest
    {

        [TestMethod]
        public void TraceStruct_Methods_ClassName_ReceivingClassNameFromTracer()
        {
            // Arrange
            string expectedClassName = "Foo";
            var worker = new Tracer();
            var boo = new Foo(worker);
                       
            // Act
            boo.CopyList();
            var tester = new Tracer();
            
            // Assert
            var receivedStruct = tester.GetTraceResult();
            Assert.AreEqual(expectedClassName, receivedStruct.ToArray()[0].Value.methods[0].className, "Class name is not as expected!");
        }

        [TestMethod]
        public void TraceStruct_Methods_MethodName_ReceivingMethodNameFromTracer()
        {
            // Arrange
            string expectedMethodName = "CopyList";
            var worker = new Tracer();
            var boo = new Foo(worker);

            // Act
            boo.CopyList();
            var tester = new Tracer();

            // Assert
            var receivedStruct = tester.GetTraceResult();
            Assert.AreEqual(expectedMethodName, receivedStruct.ToArray()[0].Value.methods[0].methodName, "Method name is not as expected!");
        }

        [TestMethod]
        public void TraceStruct_EmptyOnItialization()
        {
            // Arrange
            var expected = new ConcurrentDictionary<int, ThreadTraceStruct>();
            var worker = new Tracer();
            var boo = new Foo(worker);

            // Act
            var tester = new Tracer();

            // Assert
            var received = tester.GetTraceResult();
            Assert.AreEqual(expected.IsEmpty, received.IsEmpty, "Received structure is not empty!");
        }

        [TestMethod]
        public void TraceStruct_Methods_Time_ReceivingMethodNameFromTracer()
        {
            // Arrange
            long expectedDifference = 2;
            var worker = new Tracer();
            var boo = new Foo(worker);
            Stopwatch stopwatch = new Stopwatch();

            // Act
            var tester = new Tracer();
            stopwatch.Start();
            boo.CopyList();
            stopwatch.Stop();

            // Assert
            var receivedStruct = tester.GetTraceResult();
            var receivedDifference = Math.Abs(stopwatch.ElapsedMilliseconds - receivedStruct.ToArray()[0].Value.methods[0].time);
            Assert.IsTrue((expectedDifference >= receivedDifference),
                "Allowable time difference is " + expectedDifference.ToString() + " but received is " + receivedDifference.ToString());
                
        }

    }
}
