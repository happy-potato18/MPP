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
        private static async Task MakeOpeningTasks(List<string> srcFiles, int limit, BlockingCollection<List<string>> content)
        {
            //DEBUG
            Console.WriteLine("I am in MakeOpeningTasks()");
            while (srcFiles.Count != 0)
            {
                if (limit >= srcFiles.Count)
                    limit = srcFiles.Count;

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < limit; i++)
                {
                    //DEBUG
                    var fileName = srcFiles[i].Substring(srcFiles[i].Length - 5);
                    Console.WriteLine("Start reading: " + fileName);
                    var fileContent = await File.ReadAllLinesAsync(srcFiles[i]);
                    //DEBUG 
                    Console.WriteLine("Stop reading: " + fileName);
                    srcFiles.RemoveAt(i);
                    tasks.Add(Task.Run(() =>
                    {
                        //DEBUG
                        Console.WriteLine("Start task: " + fileName);
                        //DEBUG VARIABLE
                        var res = content.TryAdd(fileContent.ToList());
                        Console.WriteLine("Stop task: " + fileName);
                        //DEBUG
                    }));

                }

                Task.WaitAny(tasks.ToArray());
                //Task t =  Task.WhenAll(tasks);
                //try
                //{
                //    t.Wait();
                //}
                //catch { }

            }


            isAllOpened = true;

        }

        //IT Will be async after creating real processing async method to call inside it

        private static void MakeProcessingTasks(BlockingCollection<List<string>> content, int limit,
                                                      BlockingCollection<List<string>> outputContent)
        {
            while (content.Count != 0 || !isAllOpened)
            {
                Console.WriteLine("I am in MakeProcessingTasks()");
                if (limit > content.Count)
                    limit = content.Count;

                List<Task> tasks = new List<Task>();
                for (int i = 0; i < limit; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        List<string> temp = new List<string>();
                        //DEBUG
                        Console.WriteLine("I am trying to read value");
                        if (content.TryTake(out temp))
                        {
                            //DEBUG
                            Console.WriteLine("I have already read the value");
                            //DEBUG
                            temp.Add("shhhhhhhhhhhhh");
                            outputContent.TryAdd(temp);
                        }
                        //DEBUG
                        Console.WriteLine("Particular file " + i.ToString() + "is processed");
                    }));


                }

                Task.WaitAny(tasks.ToArray());

            }


            isAllProcessed = true;

        }

        private static void MakeWritingTasks(BlockingCollection<List<string>> contentToWrite, int limit, string pathToWriteInFolder)
        {
            Console.WriteLine("I am in MakeWritingTasks()");
            while (contentToWrite.Count != 0 || !isAllProcessed)
            {
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


            }

            Task.WaitAll();


        }
        /// <summary>
        /// limits[0] - limit for simulteniously file opening
        /// limits[1] - limit for simulteniously file content processing
        /// limits[2] - limit for simulteniously writing in new files
        /// To make limitless queue for particular action pass zero as parameter.
        /// </summary>
        public static async Task Generate(List<string> srcFiles, string outFolderPath, List<int> limits)
        {
            BlockingCollection<List<string>> srcFilesContents = new BlockingCollection<List<string>>();
            BlockingCollection<List<string>> generatedTestsFilesStrings = new BlockingCollection<List<string>>();
            //DEBUG
            Console.WriteLine("I am in Generate Method");


            await MakeOpeningTasks(srcFiles, limits[0], srcFilesContents);
            MakeProcessingTasks(srcFilesContents, limits[1], generatedTestsFilesStrings);
            MakeWritingTasks(generatedTestsFilesStrings, limits[2], outFolderPath);



        }

    }
}
