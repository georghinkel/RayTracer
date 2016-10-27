using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Surface : INotifyPropertyChanged
    {
        public Func<Vector, Color> Diffuse { get; private set; }
        public Func<Vector, Color> Specular { get; private set; }
        public Func<Vector, double> Reflect { get; private set; }

        public Surface(Func<Vector, Color> diffuse, Func<Vector, Color> specular, Func<Vector, double> reflect)
        {
            Diffuse = diffuse;
            Specular = specular;
            Reflect = reflect;
        }

        private double roughness;

        public double Roughness
        {
            get
            {
                return roughness;
            }
            set
            {
                if (roughness != value)
                {
                    roughness = value;
                    OnPropertyChanged("Roughness");
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
