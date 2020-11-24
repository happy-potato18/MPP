using NUnit.Framework;
using FakerClass;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace _2mppTestsProject
{
    public class ExampleClass1
    {
        public string str { get; set; }

        public ExampleClass1(string _str)
        {
            str = _str;
        }
    }

    public class ExampleClass2
    {
        public Stack<int> stack { get; set; }
               
    }
    public class ExampleClass3
    {
        public ExampleClass2 ex2 { get; set; }

    }

    public class ExampleClass4
    {
        public Timer timer { get; set;}

    }

    public class ExampleClass5
    {
        public int i { get; set; }

    }


    public class Tests
    {

        [TestFixture]
        public class Faker_Tests
        {
            private Faker _faker;

            [SetUp]
            public void SetUp()
            {
                _faker = new Faker();
            }

            [Test]
            public void FillObjct_GenerateReferenceTypeValue()
            {
                var obj =(ExampleClass1)_faker.Create<ExampleClass1>();
                Assert.IsTrue(obj.str.Length > 0);
               

            }

            [Test]
            public void FillObjct_GenerateIenumerableValue()
            {
                var obj = (ExampleClass2)_faker.Create<ExampleClass2>();
                Assert.IsTrue(obj.stack.Count > 0);


            }

            [Test]
            public void FillObjct_GenerateDTOwithNestedDTO()
            {
                var obj = (ExampleClass3)_faker.Create<ExampleClass3>();
                Assert.IsTrue(obj.ex2 != null);


            }

            [Test]
            public void FillObjct_GenerateSystemObjectValue()
            {
                var obj = (ExampleClass4)_faker.Create<ExampleClass4>();
                Assert.IsTrue(obj.timer == null);


            }

            [Test]
            public void FillObjct_GenerateValueTypeValue()
            {
                var obj = (ExampleClass5)_faker.Create<ExampleClass5>();
                Assert.IsTrue(obj.i != 0);


            }
        }
    }
}