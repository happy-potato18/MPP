using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriterClass
{
    public class Writer : IWriter
    {
        public void Write(string info, Stream OutputStream)
        {
            var sw = new StreamWriter(OutputStream);
            sw.AutoFlush = true;
            sw.Write(info);
            sw.Close();
        }
    }
}
