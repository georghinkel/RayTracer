using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Models
{
    static class Surfaces
    {
        // Only works with X-Z plane.
        public static readonly Surface CheckerBoard =
            new Surface(pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? Color.Make(1, 1, 1)
                                    : Color.Make(0, 0, 0),
                        pos => Color.Make(1, 1, 1),
                        pos => ((Math.Floor(pos.Z) + Math.Floor(pos.X)) % 2 != 0)
                                    ? .1
                                    : .7)
            {
                Roughness = 150
            };


        public static readonly Surface Shiny =
            new Surface(pos => Color.Make(1, 1, 1),
                        pos => Color.Make(.5, .5, .5),
                        pos => .6)
            {
                Roughness = 50
            };
    }
}
