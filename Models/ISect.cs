using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class ISect : INotifyPropertyChanged
    {
        private SceneObject thing;

        public SceneObject Thing
        {
            get
            {
                return thing;
            }
            set
            {
                if (thing != value)
                {
                    thing = value;
                    OnPropertyChanged("Thing");
                }
            }
        }

        private Ray ray;

        public Ray Ray
        {
            get
            {
                return ray;
            }
            set
            {
                if (ray != value)
                {
                    ray = value;
                    OnPropertyChanged("Ray");
                }
            }
        }

        private double dist;

        public double Dist
        {
            get
            {
                return dist;
            }
            set
            {
                if (dist != value)
                {
                    dist = value;
                    OnPropertyChanged("Dist");
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
