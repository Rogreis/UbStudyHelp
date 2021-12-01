using System.ComponentModel;

namespace UbStudyHelp.Classes
{
    /// <summary>
    /// Provide data for the each item in the index shown to users
    /// </summary>
    public class IndexTextblockEntry : INotifyPropertyChanged
    {
        private string text;
        private string hypelink;
        private double marginValue;

        public IndexTextblockEntry(string text, string hypelink, double marginValue)
        {
            this.text = text;
            this.hypelink = hypelink;
            this.marginValue = marginValue;
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                FirePropertyChanged("Text");
            }
        }

        public string Hyperlink
        {
            get { return hypelink; }
            set
            {
                hypelink = value;
                FirePropertyChanged("Hyperlink");
            }
        }

        public double MarginValue
        {
            get { return marginValue; }
            set
            {
                marginValue = value;
                FirePropertyChanged("MarginValue");
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
