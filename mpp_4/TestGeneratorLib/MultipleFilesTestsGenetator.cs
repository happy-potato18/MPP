using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public static class MultipleFilesTestsGenetator
    {
        private static bool isAllOpened = false;
        private static bool isAllProcessed = false;

        private static async Task ReadAndAddToTheNextQueueFileAsync(string file, BlockingCollection<Tuple<string, List<string>>> content)
        {
            var fileName =file.Substring(file.LastIndexOf('\\')+1,file.LastIndexOf(".")-file.LastIndexOf('\\')-1);
            //DEBUG
            //Console.WriteLine("2)Starts reading task" + fileName);
            var fileContent = await File.ReadAllLinesAsync(file);
            var res = content.TryAdd(new Tuple<string,List<string>>(fileName,fileContent.ToList()));
            //DEBUG
            //Console.WriteLine("4)Stops reading task " + fileName);

        }

        private static void MakeOpeningTasks(List<string> srcFiles, int limit, BlockingCollection<Tuple<string, List<string>>> content)
        {
            List<Task> tasks = new List<Task>();
            if (limit >= srcFiles.Count)
                limit = srcFiles.Count;
            for (int i = 0; i < limit; i++)
            {
                var temp = srcFiles[0];
                //DEBUG
                //Console.WriteLine("1)adds reading task" + temp.Substring(temp.LastIndexOf('\\')));
                tasks.Add(ReadAndAddToTheNextQueueFileAsync(temp, content));
                //DEBUG
                //Console.WriteLine("3)added reading task " + temp.Substring(temp.LastIndexOf('\\')));
                srcFiles.RemoveAt(0);
            }

            Task.WaitAll(tasks.ToArray());
        }


        private static void MakeProcessingTasks(BlockingCollection<Tuple<string, List<string>>> content, int limit,
                                                      BlockingCollection<Tuple<string, List<string>>> outputContent)
        {
            List<Task<Tuple<string,List<string>>>> tasks = new List<Task<Tuple<string, List<string>>>>();

            if (limit >= content.Count)
                limit = content.Count;
            int i = 0;
            var temp = new Tuple<string, List<string>>(null,null);
            while(i < limit)
            {
                if (content.TryTake(out temp))
                {
                    i++;
                    //DEBUG
                    //Console.WriteLine("5)start processing" + temp.Item1);
                    tasks.Add(SingleFileContentTestGenerator.GenerateTestFileAsync(temp));
                    //DEBUG
                    //Console.WriteLine("2");
                }
            }
            foreach(var task in tasks)
            {
                //DEBUG
                //Console.WriteLine("3");
                var generatedContent = task.Result;
                outputContent.TryAdd(generatedContent);
                //DEBUG
                //Console.WriteLine("6)close processing "+ temp.Item1);
            }
        }

        private static async Task WriteToFileAsync(string filename, List<string> content)
        {
            await File.WriteAllLinesAsync(filename, content);
            //DEBUG
            //Console.WriteLine("8)END WRITING"+ filename);
        }

        private static void MakeWritingTasks(BlockingCollection<Tuple<string, List<string>>> contentToWrite, int limit, string pathToFolder)
        {
            if (limit >= contentToWrite.Count)
                limit = contentToWrite.Count;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < limit; i++)
            {              
              
                if (contentToWrite.TryTake(out Tuple<string,List<string>> temp))
                {
                    //DEBUG
                    //Console.WriteLine("7)START WRITING" + temp.Item1);
                    var testFileName = pathToFolder + "\\" + temp.Item1 + "Test.cs";
                    tasks.Add(WriteToFileAsync(testFileName, temp.Item2));
                }
            } 
            Task.WaitAll(tasks.ToArray());
        }

        private static void ProcessAllOpening(List<string> fileNames, int limit, BlockingCollection<Tuple<string, List<string>>> FilesContents)
        {
            while (fileNames.Count != 0)
            {
                MakeOpeningTasks(fileNames, limit, FilesContents);
            }
            isAllOpened = true;           
        }

        private static void ProcessAllGenerating(BlockingCollection<Tuple<string, List<string>>> srcFilesContents,
                                                 int limit, BlockingCollection<Tuple<string, List<string>>> processedFilesContents)
        {
            while ((srcFilesContents.Count != 0) || (!isAllOpened))
            {
                MakeProcessingTasks(srcFilesContents, limit, processedFilesContents);
            }
            isAllProcessed = true;           
        }

        private static void ProcessAllWriting(BlockingCollection<Tuple<string, List<string>>> processedFilesContents,
                                                int limit, string outputFolderPath)
        {
            while ((processedFilesContents.Count != 0) || (!isAllProcessed))
            {
                MakeWritingTasks(processedFilesContents, limit, outputFolderPath);
            }
            
        }
        /// <summary>
        /// limits[0] - limit for simulteniously file opening
        /// limits[1] - limit for simulteniously file content processing
        /// limits[2] - limit for simulteniously writing in new files
        /// To make limitless queue for particular action pass zero as parameter.
        /// </summary>
        public static Task GenerateAsync(List<string> srcFilesNames, string outputFolderPath, List<int> limits)
        {
            BlockingCollection<Tuple<string,List<string>>> srcFilesContents = new BlockingCollection<Tuple<string, List<string>>>();
            BlockingCollection<Tuple<string, List<string>>> generatedTestsFilesContents = new BlockingCollection<Tuple<string, List<string>>>();

            var task1 = Task.Run(() => { ProcessAllOpening(srcFilesNames, limits[0], srcFilesContents); });
            var task2 = Task.Run(() => { ProcessAllGenerating(srcFilesContents, limits[1], generatedTestsFilesContents); });
            var task3 = Task.Run(() => { ProcessAllWriting(generatedTestsFilesContents, limits[2], outputFolderPath); ; });
            return Task.WhenAll(task1, task2, task3);
        }

    }
}
