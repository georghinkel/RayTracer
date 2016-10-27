using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    public class PollRayTracer
    {
        private int screenWidth;
        private int screenHeight;
        private const int MaxDepth = 5;

        public Action<int, int, System.Drawing.Color> setPixel;

        public PollRayTracer(int screenWidth, int screenHeight, Action<int, int, System.Drawing.Color> setPixel)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.setPixel = setPixel;
        }

        private class Wrap<T>
        {
            public readonly Func<Wrap<T>, T> It;
            public Wrap(Func<Wrap<T>, T> it) { It = it; }
        }

        public static Func<T, U> Y<T, U>(Func<Func<T, U>, Func<T, U>> f)
        {
            Func<Wrap<Func<T, U>>, Func<T, U>> g = wx => f(wx.It(wx));
            return g(new Wrap<Func<T, U>>(wx => f(y => wx.It(wx)(y))));
        }

        class TraceRayArgs
        {
            public readonly Ray Ray;
            public readonly Scene Scene;
            public readonly int Depth;

            public TraceRayArgs(Ray ray, Scene scene, int depth) { Ray = ray; Scene = scene; Depth = depth; }
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
                       let computeTraceRay = (Func<Func<TraceRayArgs, Color>, Func<TraceRayArgs, Color>>)
                        (f => traceRayArgs =>
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
                                                            f(new TraceRayArgs(new Ray()
                                                            {
                                                                Start = reflectPos,
                                                                Dir = reflectDir
                                                            },
                                                                               traceRayArgs.Scene,
                                                                               traceRayArgs.Depth + 1)))
                          select naturalColors.Aggregate(reflectColor,
                                                         (color, natColor) => Color.Plus(color, natColor))
                         ).DefaultIfEmpty(Color.Background).First())
                       let traceRay = Y(computeTraceRay)
                       select new { X = x, Y = y, Color = traceRay(new TraceRayArgs(ray, scene, 0)) };

            foreach (var row in pixelsQuery)
                foreach (var pixel in row)
                    setPixel(pixel.X, pixel.Y, pixel.Color.ToDrawingColor());
        }
    }
}
