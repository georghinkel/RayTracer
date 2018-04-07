using NMF.Collections.ObjectModel;
using NMF.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class Scene : INotifyPropertyChanged
    {
        private Camera camera;

        public Camera Camera
        {
            get
            {
                return camera;
            }
            set
            {
                if (camera != value)
                {
                    camera = value;
                    OnPropertyChanged("Camera");
                }
            }
        }

        public IListExpression<SceneObject> Things { get; private set; }
        public IListExpression<Light> Lights { get; private set; }

        public Scene()
        {
            Things = new ObservableList<SceneObject>();
            Lights = new ObservableList<Light>();
        }

        public IEnumerable<ISect> Intersect(Ray r)
        {
            return from thing in Things
                   select thing.Intersect(r);
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
