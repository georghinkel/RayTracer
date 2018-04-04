using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Color : INotifyPropertyChanged
    {
        private double r;

        public double R
        {
            get
            {
                return r;
            }
            set
            {
                if (r != value)
                {
                    r = value;
                    OnPropertyChanged("R");
                }
            }
        }

        private double g;

        public double G
        {
            get
            {
                return g;
            }
            set
            {
                if (g != value)
                {
                    g = value;
                    OnPropertyChanged("G");
                }
            }
        }

        private double b;

        public double B
        {
            get
            {
                return b;
            }
            set
            {
                if (b != value)
                {
                    b = value;
                    OnPropertyChanged("B");
                }
            }
        }

        public Color(double r, double g, double b) { R = r; G = g; B = b; }

        public static Color Make(double r, double g, double b) { return new Color(r, g, b); }

        public static Color Times(double n, Color v)
        {
            if (v == null) return null;
            return new Color(n * v.R, n * v.G, n * v.B);
        }
        public static Color Times(Color v1, Color v2)
        {
            return new Color(v1.R * v2.R, v1.G * v2.G, v1.B * v2.B);
        }

        public static Color Plus(Color v1, Color v2)
        {
            if (v1 == null) return v2;
            if (v2 == null) return v1;
            return new Color(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B);
        }
        public static Color Minus(Color v1, Color v2)
        {
            if (v2 == null) return v1;
            if (v1 == null) return new Color(-v2.R, -v2.G, -v2.B);
            return new Color(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B);
        }

        public static readonly Color Background = Make(0, 0, 0);
        public static readonly Color DefaultColor = Make(0, 0, 0);

        private double Legalize(double d)
        {
            return d > 1 ? 1 : d;
        }

        public System.Drawing.Color ToDrawingColor()
        {
            return System.Drawing.Color.FromArgb((int)(Legalize(R) * 255), (int)(Legalize(G) * 255), (int)(Legalize(B) * 255));
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
