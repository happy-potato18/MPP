using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_3
{
    public class OpenFileService
    {
        public string FilePath { get; set; }

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Assemblies|*.dll" };
           
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }
    }
}
