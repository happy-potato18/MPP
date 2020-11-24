using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleClassLibrary
{

    public static class QueueExtension
    {
        public static string ToString(this Queue<int> stack)
        {
            var res = "";
            foreach (var item in stack)
            {
                res += "\t" + item.ToString()+ ", ";
            }
            return res.Trim(',');
        }
    }

    public class ExampleClass2
    {
        public long i { get; set; }
        public double d { get; set; }
        public Queue<int> queue { get; set; }
        private int K { get; set; }

        public ExampleClass1 ex1 { get; set; }
        public ExampleClass2(long _i)
        {
            i = _i;

        }

        public int Multiply()
        {
            return K * K;
        }

        public override string ToString()
        {
            var res ="Long value: " + i.ToString() + ", " +
                   "Double value: " + d.ToString() + ", " +
                   "Stack value: " + queue.ToString() + " " +
                   "Example class1: ";
            if(ex1 != null)
            {
                res += ex1.ToString() + "\n";
            }
            else
            {
                res += " null \n";
            }
            return res; 
        }


    }
}

