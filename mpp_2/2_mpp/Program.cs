using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleClassLibrary;
using FakerClass;
using System.Linq.Expressions;
using System.Reflection;

namespace mpp_2
{
    public class Foo
    {
        public string city { get; set; }
        public int citizensCount { get; set; }

        public string ShowInfo()
        {
            return city + ": " + citizensCount.ToString();
        }
    }

    public class IntGenerator
    {
        public int CountGenerator()
        {
            return new Random().Next(100,2000);

        }

    }


    class Program
    {
        private delegate string CityGeneratorDelegate();
        public static string CityGenerator()
        {
            return "Minsk";

        }

        static void Main(string[] args)
        {
            var config = new FakerConfig();
            config.Add<Foo, string, CityGeneratorDelegate>(foo => foo.city, CityGenerator);
            config.Add<Foo, int, Func<int>>(foo => foo.citizensCount, new IntGenerator().CountGenerator);
            var faker = new Faker(config);
            var ex = (Foo)faker.Create<Foo>();
            Console.WriteLine(ex.citizensCount.ToString() + " " + ex.city);
           //Console.WriteLine(typeof(IntGenerator).GetMethod("CountGenerator").ToString());
            Console.ReadLine();
        }
    }
}

