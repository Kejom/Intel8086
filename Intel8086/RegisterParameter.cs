using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Intel8086
{
    public class RegisterParameter : INotifyPropertyChanged
    {
        private string _Value;

        public string Value
        {
            get { return _Value; }
            set { _Value = value; RaisePropertyChanged("Value"); }
        }

        public RegisterParameter(string Value)
        {
            this.Value = Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
