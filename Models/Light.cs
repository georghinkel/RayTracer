using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Light : INotifyPropertyChanged
    {
        private Vector pos;

        public Vector Pos
        {
            get
            {
                return pos;
            }
            set
            {
                if (pos != value)
                {
                    pos = value;
                    OnPropertyChanged("Pos");
                }
            }
        }

        private Color color;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    OnPropertyChanged("Color");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
