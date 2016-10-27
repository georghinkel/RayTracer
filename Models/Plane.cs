using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Plane : SceneObject
    {
        private Vector norm;

        public Vector Norm
        {
            get
            {
                return norm;
            }
            set
            {
                if (norm != value)
                {
                    norm = value;
                    OnPropertyChanged("Norm");
                }
            }
        }

        private double offset;

        public double Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (offset != value)
                {
                    offset = value;
                    OnPropertyChanged("Offset");
                }
            }
        }

        public override ISect Intersect(Ray ray)
        {
            double denom = Vector.Dot(Norm, ray.Dir);
            if (denom > 0) return null;
            return new ISect()
            {
                Thing = this,
                Ray = ray,
                Dist = (Vector.Dot(Norm, ray.Start) + Offset) / (-denom)
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Norm;
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
