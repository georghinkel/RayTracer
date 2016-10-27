using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public abstract class SceneObject : INotifyPropertyChanged
    {
        private Surface surface;

        public Surface Surface
        {
            get
            {
                return surface;
            }
            set
            {
                if (surface != value)
                {
                    surface = value;
                    OnPropertyChanged("Surface");
                }
            }
        }

        public abstract ISect Intersect(Ray ray);
        public abstract Vector Normal(Vector pos);

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
