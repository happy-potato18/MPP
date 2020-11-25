using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TestGeneratorLib;

namespace mpp_4
{
    class Program
    {
        static async Task Main()
        {
            await TestGeneratorClass.Generate((Directory.GetFiles(Directory.GetCurrentDirectory() + "\\src", "*.txt")).ToList(),
                                                     "D:\\УНИК\\5 сем\\MPP(SPP)\\mpp_4\\mpp_4\\bin\\Debug\\netcoreapp3.1\\out", new List<int>() { 3, 4, 4 });

            Console.WriteLine("Done!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.ReadLine();
        }
    }
}
