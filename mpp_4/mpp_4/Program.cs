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
        static void Main()
        {
           var t = Task.Run(async() => {
                await TestGeneratorClass.Generate((Directory.GetFiles(Directory.GetCurrentDirectory() + "\\src", "*.txt")).ToList(),
                                                    "D:\\УНИК\\5 сем\\MPP(SPP)\\mpp_4\\mpp_4\\bin\\Debug\\netcoreapp3.1\\out", new List<int>() { 2, 3, 4 });
           });

           Task.WaitAll(t);
            //DEBUG
            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
