using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace mpp_3
{
    [AddINotifyPropertyChangedInterface]
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
