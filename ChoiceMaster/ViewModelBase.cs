using System.ComponentModel;

namespace ChoiceMaster
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        internal void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
