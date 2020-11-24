using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public static class TestGeneratorClass
    {
        private static bool isAllOpened = false;
        private static bool isAllProcessed = false;

        private static void ReadAndAddToTheNextQueueFileAsync(string file, BlockingCollection<List<string>> content)
        {
            var fileName = file.Substring(file.Length - 5);
            var fileContent = File.ReadAllLines(file);
            var res = content.TryAdd(fileContent.ToList());
            //DEBUG
            Console.WriteLine("Stop reading task " + fileName);
            
        }
        private static void MakeOpeningTasks(List<string> srcFiles, int limit, BlockingCollection<List<string>> content)
        {
            List<Task> tasks = new List<Task>();
            if (limit >= srcFiles.Count)
                limit = srcFiles.Count;
            for (int i = 0; i < limit; i++)
            {
                var temp = srcFiles[0];
                tasks.Add(Task.Run(() =>
                {
                    ReadAndAddToTheNextQueueFileAsync(temp, content);
                }));
                srcFiles.RemoveAt(0);

            }

            Task.WaitAny(tasks.ToArray());

             //return Task.WhenAll();

        }

      
        private static void MakeProcessingTasks(BlockingCollection<List<string>> content, int limit,
                                                      BlockingCollection<List<string>> outputContent)
        {
            List<Task> tasks = new List<Task>();
           
           if (limit > content.Count)
               limit = content.Count;
            for (int i = 0; i < limit; i++)
            {
                List<string> temp = new List<string>();
                if (content.TryTake(out temp))
                {
                    tasks.Add(Task.Run(() =>
                    {
                        //DEBUG
                        temp.Add("shhhhhhhhhhhhh");
                        outputContent.TryAdd(temp);
                        //DEBUG
                        Console.WriteLine("Particular file " + i.ToString() + "is processed");
                    }));


                }
            }   

            Task.WaitAny(tasks.ToArray());
        }

        private static void MakeWritingTasks(BlockingCollection<List<string>> contentToWrite, int limit, string pathToWriteInFolder)
        {
            Console.WriteLine("I am in MakeWritingTasks()");
            if (limit > contentToWrite.Count)
                    limit = contentToWrite.Count;
            List<Task> tasks = new List<Task>();
                for (int i = 0; i < limit; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        List<string> temp = new List<string>();
                        //DEBUG
                        var FILE_NAME = pathToWriteInFolder + "\\" + Guid.NewGuid().ToString() + ".txt";
                        if (contentToWrite.TryTake(out temp))
                        {
                            File.WriteAllLinesAsync(FILE_NAME, temp);
                        }

                        //DEBUG
                        Console.WriteLine("File " + FILE_NAME + "is wrote to folder");
                    }));

                }

            Task.WaitAll();


        }

        private static void Func1(List<string> inl, int limit, BlockingCollection<List<string>> outl )
        {
            while (inl.Count != 0)
            {
                MakeOpeningTasks(inl, limit, outl);
            }

            isAllOpened = true;
           
        }

        private static void Func2(BlockingCollection<List<string>> inl, int limit, BlockingCollection<List<string>> outl)
        {
            while((inl.Count != 0) || (!isAllOpened))
            {
                MakeProcessingTasks(inl, limit, outl);
            }
            isAllProcessed = true;

        }
        /// <summary>
        /// limits[0] - limit for simulteniously file opening
        /// limits[1] - limit for simulteniously file content processing
        /// limits[2] - limit for simulteniously writing in new files
        /// To make limitless queue for particular action pass zero as parameter.
        /// </summary>
        public static Task Generate(List<string> srcFiles, string outFolderPath, List<int> limits)
        {
            BlockingCollection<List<string>> srcFilesContents = new BlockingCollection<List<string>>();
            BlockingCollection<List<string>> generatedTestsFilesStrings = new BlockingCollection<List<string>>();
            
            var task1 = Task.Run(() => { Func1(srcFiles,limits[0],srcFilesContents); });

            var task2 = Task.Run(() => { Func2(srcFilesContents, limits[1], generatedTestsFilesStrings); });
            return  Task.WhenAll(task1, task2);
          
            //MakeWritingTasks(generatedTestsFilesStrings, limits[2], outFolderPath);



        }

    }
}
