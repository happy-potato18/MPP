using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public static class MultipleFilesTestsGenetator
    {
        private static bool isAllOpened = false;
        private static bool isAllProcessed = false;

        private static void ReadAndAddToTheNextQueueFileAsync(string file, BlockingCollection<List<string>> content)
        {
            //DEBUG
            var fileName = file.Substring(file.Length - 5);
            //DEBUG
            Console.WriteLine("Start reading task " + fileName);
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
                        var t = SingleFileContentTestGenerator.GenerateTestFile(temp);
                        outputContent.TryAdd(t);
                        //DEBUG
                        Console.WriteLine("Particular file " + i.ToString() + "is processed");
                    }));


                }
            }

            Task.WaitAny(tasks.ToArray());

        }

        private static void MakeWritingTasks(BlockingCollection<List<string>> contentToWrite, int limit, string pathToFolder)
        {
            Console.WriteLine("I am in MakeWritingTasks()");
            if (limit > contentToWrite.Count)
                limit = contentToWrite.Count;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < limit; i++)
            {
                List<string> temp = new List<string>();
                var FILE_NAME = pathToFolder + "\\" + Guid.NewGuid().ToString() + ".cs";
                if (contentToWrite.TryTake(out temp))
                {
                    tasks.Add(Task.Run(() =>
                    {
                        File.WriteAllLines(FILE_NAME, temp);
                        //DEBUG
                        Console.WriteLine("File " + FILE_NAME + "is wrote to folder");
                    }));

                }


            }

            Task.WaitAll(tasks.ToArray());


        }

        private static void ProcessAllOpening(List<string> fileNames, int limit, BlockingCollection<List<string>> FilesContents)
        {
            while (fileNames.Count != 0)
            {
                MakeOpeningTasks(fileNames, limit, FilesContents);
            }

            isAllOpened = true;

        }

        private static void ProcessAllGenerating(BlockingCollection<List<string>> srcFilesContents,
                                                 int limit, BlockingCollection<List<string>> processedFilesContents)
        {
            while ((srcFilesContents.Count != 0) || (!isAllOpened))
            {
                MakeProcessingTasks(srcFilesContents, limit, processedFilesContents);
            }
            isAllProcessed = true;

        }

        private static void ProcessAllWriting(BlockingCollection<List<string>> processedFilesContents,
                                                int limit, string outputFolderPath)
        {
            while ((processedFilesContents.Count != 0) || (!isAllProcessed))
            {
                MakeWritingTasks(processedFilesContents, limit, outputFolderPath);
            }
            isAllProcessed = true;

        }
        /// <summary>
        /// limits[0] - limit for simulteniously file opening
        /// limits[1] - limit for simulteniously file content processing
        /// limits[2] - limit for simulteniously writing in new files
        /// To make limitless queue for particular action pass zero as parameter.
        /// </summary>
        public static Task GenerateAsync(List<string> srcFilesNames, string outputFolderPath, List<int> limits)
        {
            BlockingCollection<List<string>> srcFilesContents = new BlockingCollection<List<string>>();
            BlockingCollection<List<string>> generatedTestsFilesContents = new BlockingCollection<List<string>>();

            var task1 = Task.Run(() => { ProcessAllOpening(srcFilesNames, limits[0], srcFilesContents); });
            var task2 = Task.Run(() => { ProcessAllGenerating(srcFilesContents, limits[1], generatedTestsFilesContents); });
            var task3 = Task.Run(() => { ProcessAllWriting(generatedTestsFilesContents, limits[2], outputFolderPath); ; });
            return Task.WhenAll(task1, task2, task3);


        }

    }
}
