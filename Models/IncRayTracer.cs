using NMF.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMF.Expressions.Linq;
using System.Diagnostics;

namespace RayTracer.Models
{
    public class IncRayTracer
    {
        private int screenWidth;
        private int screenHeight;
        private const int MaxDepth = 5;

        public Action<int, int, System.Drawing.Color> setPixel;

        public IncRayTracer(int screenWidth, int screenHeight, Action<int, int, System.Drawing.Color> setPixel)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.setPixel = setPixel;
        }

        private class Wrap<T>
        {
            public readonly ObservingFunc<Wrap<T>, T> It;
            public Wrap(ObservingFunc<Wrap<T>, T> it) { It = it; }
        }

        public static ObservingFunc<T, U> Y<T, U>(ObservingFunc<ObservingFunc<T, U>, ObservingFunc<T, U>> f)
        {
            ObservingFunc<Wrap<ObservingFunc<T, U>>, ObservingFunc<T, U>> g = new ObservingFunc<Wrap<ObservingFunc<T,U>>,ObservingFunc<T,U>>(wx => f.Evaluate(wx.It.Evaluate(wx)));
            return g.Evaluate(new Wrap<ObservingFunc<T, U>>(new ObservingFunc<Wrap<ObservingFunc<T,U>>,ObservingFunc<T,U>>(wx => f.Evaluate(new ObservingFunc<T, U>(y => wx.It.Evaluate(wx).Evaluate(y))))));
        }

        public class TraceRayArgs
        {
            public readonly Ray Ray;
            public readonly Scene Scene;
            public readonly int Depth;

            public TraceRayArgs(Ray ray, Scene scene, int depth) { Ray = ray; Scene = scene; Depth = depth; }
        }

        private static ObservingFunc<TraceRayArgs, Color> _traceRay = new ObservingFunc<TraceRayArgs,Color>(
            (traceRayArgs) =>
                        (from isect in
                              from thing in traceRayArgs.Scene.Things
                              select thing.Intersect(traceRayArgs.Ray)
                          where isect != null
                          orderby isect.Dist
                          let d = isect.Ray.Dir
                          let pos = Vector.Plus(Vector.Times(isect.Dist, isect.Ray.Dir), isect.Ray.Start)
                          let normal = isect.Thing.Normal(pos)
                          let reflectDir = Vector.Minus(d, Vector.Times(2 * Vector.Dot(normal, d), normal))
                          let naturalColors =
                              from light in traceRayArgs.Scene.Lights
                              let ldis = Vector.Minus(light.Pos, pos)
                              let livec = Vector.Norm(ldis)
                              let testRay = new Ray() { Start = pos, Dir = livec }
                              let testIsects = from inter in
                                                   from thing in traceRayArgs.Scene.Things
                                                   select thing.Intersect(testRay)
                                               where inter != null
                                               orderby inter.Dist
                                               select inter
                              let testIsect = testIsects.FirstOrDefault()
                              let neatIsect = testIsect == null ? 0 : testIsect.Dist
                              let isInShadow = !((neatIsect > Vector.Mag(ldis)) || (neatIsect == 0))
                              where !isInShadow
                              let illum = Vector.Dot(livec, normal)
                              let lcolor = illum > 0 ? Color.Times(illum, light.Color) : Color.Make(0, 0, 0)
                              let specular = Vector.Dot(livec, Vector.Norm(reflectDir))
                              let scolor = specular > 0
                                             ? Color.Times(Math.Pow(specular, isect.Thing.Surface.Roughness),
                                                           light.Color)
                                             : Color.Make(0, 0, 0)
                              select Color.Plus(Color.Times(isect.Thing.Surface.Diffuse(pos), lcolor),
                                                Color.Times(isect.Thing.Surface.Specular(pos), scolor))
                          let reflectPos = Vector.Plus(pos, Vector.Times(.001, reflectDir))
                          let reflectColor = traceRayArgs.Depth >= MaxDepth
                                              ? Color.Make(.5, .5, .5)
                                              : Color.Times(isect.Thing.Surface.Reflect(reflectPos),
                                                            TraceRay(new TraceRayArgs(new Ray(reflectPos, reflectDir),
                                                                     traceRayArgs.Scene,
                                                                     traceRayArgs.Depth + 1)))
                          select naturalColors.Sum(reflectColor)
                         ).FirstOrDefault());

        public static INotifyValue<Color> TraceRayInc(TraceRayArgs rayArgs)
        {
            return _traceRay.Observe(rayArgs);
        }

        [ObservableProxy(typeof(IncRayTracer), "TraceRayInc")]
        public static Color TraceRay(TraceRayArgs rayArgs)
        {
            return _traceRay.Evaluate(rayArgs);
        }

        internal void Render(Scene scene)
        {
            var pixelsQuery =
                from y in Enumerable.Range(0, screenHeight)
                let recenterY = -(y - (screenHeight / 2.0)) / (2.0 * screenHeight)
                select from x in Enumerable.Range(0, screenWidth)
                       let recenterX = (x - (screenWidth / 2.0)) / (2.0 * screenWidth)
                       let point =
                           Vector.Norm(Vector.Plus(scene.Camera.Forward,
                                                   Vector.Plus(Vector.Times(recenterX, scene.Camera.Right),
                                                               Vector.Times(recenterY, scene.Camera.Up))))
                       let ray = new Ray() { Start = scene.Camera.Pos, Dir = point }
                       select new { X = x, Y = y, Color = _traceRay.Observe(new TraceRayArgs(ray, scene, 0)) };

            foreach (var row in pixelsQuery)
            {
                foreach (var pixel in row)
                {
                    pixel.Color.Successors.SetDummy();
                    setPixel(pixel.X, pixel.Y, (pixel.Color.Value ?? Color.Background).ToDrawingColor());

                    pixel.Color.ValueChanged += (o, e) => setPixel(pixel.X, pixel.Y, (pixel.Color.Value ?? Color.Background).ToDrawingColor());
                }
            }
        }

        internal static readonly Scene DefaultScene = CreateDefaultScene();

        private static Scene CreateDefaultScene()
        {
            var defaultScene = new Scene();
            defaultScene.Things.Add(new Plane()
            {
                Norm = Vector.Make(0, 1, 0),
                Offset = 0,
                Surface = Surfaces.CheckerBoard
            });
            defaultScene.Things.Add(new Sphere()
            {
                Center = Vector.Make(0, 1, 0),
                Radius = 1,
                Surface = Surfaces.Shiny
            });
            defaultScene.Things.Add(new Sphere()
            {
                Center = Vector.Make(-1, .5, 1.5),
                Radius = .5,
                Surface = Surfaces.Shiny
            });
            defaultScene.Lights.Add(new Light()
            {
                Pos = Vector.Make(-2, 2.5, 0),
                Color = Color.Make(.49, .07, .07)
            });
            defaultScene.Lights.Add(new Light()
            {
                Pos = Vector.Make(1.5, 2.5, 1.5),
                Color = Color.Make(.07, .07, .49)
            });
            defaultScene.Lights.Add(new Light()
            {
                Pos = Vector.Make(1.5, 2.5, -1.5),
                Color = Color.Make(.07, .49, .071)
            });
            defaultScene.Lights.Add(new Light()
            {
                Pos = Vector.Make(0, 3.5, 0),
                Color = Color.Make(.21, .21, .35)
            });
            defaultScene.Camera = Camera.Create(Vector.Make(3, 2, 4), Vector.Make(-1, .5, 0));
            return defaultScene;
        }
    }
}
