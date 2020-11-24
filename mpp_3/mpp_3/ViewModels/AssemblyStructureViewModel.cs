using Data;
using mpp_3.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace mpp_3
{
    public class AssemblyStructureViewModel : ViewModelBase
    {
        public AssemblyStructureViewModel()
        {
            OpenfileService = new OpenFileService();
        }
       
        public string PathToDll { get; set; }

        private Assembly assembly;
        public OpenFileService OpenfileService { get; set; }
        public string ErrorMessages { get; set; }
        public ICommand OpenFileCommand 
        {
            get
            {
                return new CommandBase(OpenFile);
            }
        }

        public ObservableCollection<MemberDescriptionViewModel> Collection { get; set; }

        private void OpenFile()
        {
           if (OpenfileService.OpenFileDialog())
           {
                PathToDll = OpenfileService.FilePath;
                this.Collection = null;
                assembly = null;
               try
               {
                    assembly = Assembly.LoadFrom(PathToDll);
               }
               catch
               {
                    
                    ErrorMessages += "Сannot load an Assembly from the specified file!\n";
                    return;
               }
               
                try
                {
                    this.Collection = new ObservableCollection<MemberDescriptionViewModel>(AssemblyStructure.AssemblyBrowser.GetNamespaces(assembly)
                   .Select(namesp => new MemberDescriptionViewModel(assembly, MemberType.Namespace, namesp.MemberName)));
                }
                catch
                {
                    ErrorMessages += "Assembly were loaded, but some issues occured during parsing its metadata!\n";
                    return;
                }
               
           }
                         
        }
    }
}
