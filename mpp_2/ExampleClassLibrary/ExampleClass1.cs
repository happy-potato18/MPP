using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;

namespace ExampleClassLibrary
{
    public static class DictionaryExtension
    { 
        public static string ToString(this Dictionary<int,string> dict)
        {
            var res = "";
            foreach(var item in dict)
            {
                res += "\t" + item.Key.ToString() + ": " + item.Value + "\n";
            }
            return res;
        }
    }

        
    public class ExampleClass1
    {
        public ushort i { get; set; }
        public Single d { get; set; }
        public char ch { get; set; }
        public string str { get; set; }
        public bool b { get; set; }
        public decimal de { get; set; }
        public DateTime dt { get; set; }
        public Encoding enc { get; set; }

        public Timer timer { get; set; }
        public Dictionary<int, string> dict { get; set; }
        public ExampleClass2 ex2 { get; set; }

        public ExampleClass1(ushort _i, Single _d, char _ch, string _str)
        {
            i = _i;
            d = _d;
            ch = _ch;
            str = _str;

        }
       
        public override string ToString()
        {
            var res = "Ushort value: " + i.ToString() + "\n" +
                   "Decimal value: " + d.ToString() + "\n" +
                   "Char value: " + ch + "\n" +
                   "String value: " + str + "\n" +
                   "Boolean value: " + b.ToString() + "\n" +
                   "Decimal value: " + de.ToString() + "\n" +
                   "DateTime value: " + dt.ToString() + "\n" +
                   "Encoding value: ";
            if (enc != null)
            {
                res += enc.ToString() + "\n";
            }
            else
            {
                res += " null \n";
            }

            res += "Timer value: ";
            if (timer != null)
            {
                res += timer.ToString() + "\n";
            }
            else
            {
                res += " null \n";
            }

             res +="Dictionary<int,string>: \n" + dict.ToString() + "\n" +
                    "Example class2: ";
            if (ex2 != null)
            {
                res += ex2.ToString() + "\n";
            }
            else
            {
                res += " null \n";
            }

            return res;


        }


    }


}

