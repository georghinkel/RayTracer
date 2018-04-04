using RayTracer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracer
{
    public partial class RayTracerForm : Form
    {
        Bitmap bitmapPoll;
        Bitmap bitmapInc;
        PictureBox pictureBoxPoll;
        PictureBox pictureBoxInc;
        const int width = 20;
        const int height = 20;

        public RayTracerForm()
        {
            bitmapPoll = new Bitmap(width, height);
            bitmapInc = new Bitmap(width, height);

            pictureBoxPoll = new PictureBox();
            pictureBoxPoll.Location = new Point(0, 0);
            pictureBoxPoll.Size = new Size(300, 300);
            pictureBoxPoll.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            pictureBoxPoll.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPoll.Image = bitmapPoll;

            pictureBoxInc = new PictureBox();
            pictureBoxInc.Location = new Point(300, 0);
            pictureBoxInc.Size = new Size(300, 300);
            pictureBoxInc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pictureBoxInc.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxInc.Image = bitmapInc;

            ClientSize = new System.Drawing.Size(600, 324);
            Controls.Add(pictureBoxPoll);
            Controls.Add(pictureBoxInc);
            Text = "Ray Tracer";
            Load += RayTracerForm_Load;
            Resize += RayTracerForm_Resize;

            Show();
        }

        void RayTracerForm_Resize(object sender, EventArgs e)
        {
            pictureBoxPoll.Width = Width / 2;
            pictureBoxInc.Width = pictureBoxPoll.Width;
            pictureBoxInc.Left = pictureBoxPoll.Width;
        }

        private void RayTracerForm_Load(object sender, EventArgs e)
        {
            this.Show();
            var rayTracerPoll = new PollRayTracer(width, height, (int x, int y, System.Drawing.Color color) =>
            {
                bitmapPoll.SetPixel(x, y, color);
            });
            var rayTracerInc = new IncRayTracer(width, height, (int x, int y, System.Drawing.Color color) =>
            {
                bitmapInc.SetPixel(x, y, color);
            });

            var scene = IncRayTracer.DefaultScene;

            var watch = new Stopwatch();
            watch.Start();
            rayTracerPoll.Render(scene);
            watch.Stop();
            pictureBoxPoll.Refresh();
            MessageBox.Show(string.Format("Normal Ray trace took {0}ms", watch.ElapsedMilliseconds));
            watch.Restart();
            rayTracerInc.Render(scene);
            watch.Stop();
            pictureBoxInc.Refresh();
            MessageBox.Show(string.Format("Incremental rayvtrace took {0}ms", watch.ElapsedMilliseconds));
            pictureBoxPoll.Invalidate();

            watch.Restart();
            scene.Lights[0].Color = RayTracer.Models.Color.Make(.47, .47, .07);
            watch.Stop();
            MessageBox.Show(string.Format("Setting new color took {0}ms", watch.ElapsedMilliseconds));
            pictureBoxInc.Invalidate();
            
            watch.Restart();
            rayTracerPoll.Render(scene);
            watch.Stop();
            MessageBox.Show(string.Format("Rerendering took {0}ms", watch.ElapsedMilliseconds));
            pictureBoxPoll.Invalidate();
        }
    }
}
