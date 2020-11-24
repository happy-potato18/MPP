using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriterClass
{
    public interface IWriter
    {
        void Write(string info, Stream OutputStream);
              
    }
}
