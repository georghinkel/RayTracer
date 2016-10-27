using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Camera : INotifyPropertyChanged
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

        private Vector forward;

        public Vector Forward
        {
            get
            {
                return forward;
            }
            set
            {
                if (forward != value)
                {
                    forward = value;
                    OnPropertyChanged("Forward");
                }
            }
        }

        private Vector up;

        public Vector Up
        {
            get
            {
                return up;
            }
            set
            {
                if (up != value)
                {
                    up = value;
                    OnPropertyChanged("Up");
                }
            }
        }

        private Vector right;

        public Vector Right
        {
            get
            {
                return right;
            }
            set
            {
                if (right != value)
                {
                    right = value;
                    OnPropertyChanged("Right");
                }
            }
        }

        public static Camera Create(Vector pos, Vector lookAt)
        {
            Vector forward = Vector.Norm(Vector.Minus(lookAt, pos));
            Vector down = new Vector(0, -1, 0);
            Vector right = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, down)));
            Vector up = Vector.Times(1.5, Vector.Norm(Vector.Cross(forward, right)));

            return new Camera() { Pos = pos, Forward = forward, Up = up, Right = right };
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
