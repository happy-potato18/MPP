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
            await MultipleFilesTestsGenetator.GenerateAsync((Directory.GetFiles(Directory.GetCurrentDirectory() + "\\src", "*.cs")).ToList(),
                                                     "D:\\УНИК\\5 сем\\MPP(SPP)\\mpp_4\\mpp_4\\bin\\Debug\\netcoreapp3.1\\out", new List<int>() { 2, 6, 6 });
            
            Console.WriteLine("Done!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.ReadLine();
        }
    }
}
