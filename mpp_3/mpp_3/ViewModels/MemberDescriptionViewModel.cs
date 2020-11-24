using Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Windows.Input;
using mpp_3.ViewModels.Base;
using System.Reflection;

namespace mpp_3
{
    public class MemberDescriptionViewModel : ViewModelBase
    {
        public MemberDescriptionViewModel(Assembly _assembly, MemberType _type, string _memberName)
        {
            this.ShowCommand = new CommandBase(ShowMembers);
            this.Assembly = _assembly;
            this.Type = _type;
            this.MemberName = _memberName;
            this.UnshowedMembers();
        }
        public string MemberName { get; set; }
        public MemberType Type { get; set; }
        public Assembly Assembly { get; set; }

        public ObservableCollection<MemberDescriptionViewModel> Members { get; set; }

        public bool HasMembers { get { return this.Type != MemberType.TypeMember; } }

        public ICommand ShowCommand { get; set; }

        public bool IsMembersShowed
        {
            get
            {
                return this.Members?.Count(member => member != null) > 0;
            }
            set
            {
                if(value == true)
                {
                    ShowMembers();
                }
                else
                {
                    UnshowedMembers();
                }
            }
        }

        private void ShowMembers()
        {
            if (this.Type == MemberType.TypeMember)
                return;
            else if (this.Type == MemberType.Namespace)
            {
                this.Members =new ObservableCollection<MemberDescriptionViewModel>(
                                AssemblyStructure.AssemblyBrowser.GetNamespaceTypes(this.Assembly, this.MemberName)
                               .Select(member => new MemberDescriptionViewModel(member.Assembly,member.Type,member.MemberName))
                               );
            }
            else
            {
                this.Members = new ObservableCollection<MemberDescriptionViewModel>(
                    AssemblyStructure.AssemblyBrowser.GetTypeMembers(this.Assembly, this.MemberName)
                    .Select(member => new MemberDescriptionViewModel(member.Assembly, member.Type, member.MemberName))
                    );
            }

        }

        private void UnshowedMembers()
        {
            this.Members = new ObservableCollection<MemberDescriptionViewModel>();
            if (this.Type != MemberType.TypeMember)
                this.Members.Add(null);
        }
    }
}
