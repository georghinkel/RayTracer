using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Sphere : SceneObject
    {
        private Vector center;

        public Vector Center
        {
            get
            {
                return center;
            }
            set
            {
                if (center != value)
                {
                    center = value;
                    OnPropertyChanged("Center");
                }
            }
        }

        private double rad;

        public double Radius
        {
            get
            {
                return rad;
            }
            set
            {
                if (rad != value)
                {
                    rad = value;
                    OnPropertyChanged("Radius");
                }
            }
        }

        public override ISect Intersect(Ray ray)
        {
            Vector eo = Vector.Minus(Center, ray.Start);
            double v = Vector.Dot(eo, ray.Dir);
            double dist;
            if (v < 0)
            {
                dist = 0;
            }
            else
            {
                double disc = Math.Pow(Radius, 2) - (Vector.Dot(eo, eo) - Math.Pow(v, 2));
                dist = disc < 0 ? 0 : v - Math.Sqrt(disc);
            }
            if (dist == 0) return null;
            return new ISect()
            {
                Thing = this,
                Ray = ray,
                Dist = dist
            };
        }

        public override Vector Normal(Vector pos)
        {
            return Vector.Norm(Vector.Minus(pos, Center));
        }
    }
}
