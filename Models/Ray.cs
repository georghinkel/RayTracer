using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Ray : INotifyPropertyChanged
    {
        private Vector start;

        public Ray() { }

        public Ray(Vector start, Vector dir)
        {
            Start = start;
            Dir = dir;
        }

        public Vector Start
        {
            get
            {
                return start;
            }
            set
            {
                if (start != value)
                {
                    start = value;
                    OnPropertyChanged("Start");
                }
            }
        }

        private Vector dir;

        public Vector Dir
        {
            get
            {
                return dir;
            }
            set
            {
                if (dir != value)
                {
                    dir = value;
                    OnPropertyChanged("Dir");
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
