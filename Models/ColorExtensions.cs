using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMF.Expressions;
using NMF.Expressions.Linq;

namespace RayTracer.Models
{
    public static class ColorExtensions
    {
        [ObservableProxy(typeof(ColorSum), "Create")]
        public static Color Sum(this INotifyEnumerable<Color> colors, Color seed)
        {
            return colors.Aggregate(seed, Color.Plus);
        }

        private class ColorSum : ObservableAggregate<Color, Color, Color>
        {
            public static INotifyValue<Color> Create(INotifyEnumerable<Color> colors, Color seed)
            {
                return new ColorSum(seed, colors);
            }

            private Color seed;

            public ColorSum(Color seed, INotifyEnumerable<Color> colors)
                : base(colors, seed)
            {
                this.seed = seed;
            }

            protected override void AddItem(Color item)
            {
                Accumulator = Color.Plus(Accumulator, item);
            }

            protected override void RemoveItem(Color item)
            {
                Accumulator = Color.Minus(Accumulator, item);
            }

            protected override void ResetAccumulator()
            {
                Accumulator = seed;   
            }

            public override Color Value
            {
                get { return Accumulator; }
            }
        }

    }
}
