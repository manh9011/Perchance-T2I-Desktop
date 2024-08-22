using Microsoft.VisualBasic.Devices;
using System.Drawing.Drawing2D;

namespace Perchance
{
    public class ToolStripTrackBarControl : ProgressBar
    {
        public ToolStripTrackBar? Owner { get; set; }

        public ToolStripTrackBarControl()
        {
            typeof(Control).GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                  ?.Invoke(this, [ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true]);

            MouseUp += OnMouseUp;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            Paint += OnPaint;
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (!Enabled) return;

            float w = Width, h = Height, v = e.X, m = Maximum - Minimum;
            if (e.Button == MouseButtons.Left)
                Value = Math.Min(Maximum, Math.Max(Minimum, (int)Math.Ceiling(Minimum + v / (w - h) * m)));
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (!Enabled) return;

            Focus();
            float w = Width, h = Height, v = e.X, m = Maximum - Minimum;
            if (e.Button == MouseButtons.Left)
                Value = Math.Min(Maximum, Math.Max(Minimum, (int)Math.Ceiling(Minimum + v / (w - h) * m)));
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (!Enabled) return;

            float w = Width, h = Height, v = e.X, m = Maximum - Minimum;
            if (e.Button == MouseButtons.Left)
                Value = Math.Min(Maximum, Math.Max(Minimum, (int)Math.Ceiling(Minimum + v / (w - h) * m)));
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            var scale = Enabled ? 0 : 1f;

            // Config
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Calculate
            float w = Width, h = Height, v = Value, m = Maximum;
            float tw = h * 3 / 4, th = h * 3 / 4;
            float bw = (w - h) * v / m, bh = h * 2 / 5;

            float bx = 0, by = (h - bh) / 2;
            float tx = bw - tw / 2, ty = (h - th) / 2;
            int r = (int)Math.Round(bh / 2);

            // Clear
            e.Graphics.FillRectangle(Brushes.White, 0, 0, w, h);

            // Bar
            e.Graphics.FillRoundedRectangle(Brushes.DarkGray.WithLight(scale), new RectangleF(bw, by, w - h - bw, bh), r, r, r, r);
            e.Graphics.DrawRoundedRectangle(Pens.Gray.WithLight(scale), new RectangleF(bw, by, w - h - bw, bh - 1), r, r, r, r);

            // Bar
            e.Graphics.FillRoundedRectangle(Brushes.Silver.WithLight(scale), new RectangleF(bx, by, bw, bh), r, 0, 0, r);
            e.Graphics.DrawRoundedRectangle(Pens.DimGray.WithLight(scale), new RectangleF(bx, by, bw - 1, bh - 1), r, 0, 0, r);

            // Track
            if (MouseButtons == MouseButtons.Left && Focused)
            {
                e.Graphics.FillEllipse(new LinearGradientBrush(new PointF(tx, ty), new PointF(tx, ty + th), Color.LightGray.WithDark(scale), Color.Gray.WithDark(scale)), new RectangleF(tx, ty, tw, th));
                e.Graphics.DrawEllipse(Pens.DimGray.WithDark(scale), new RectangleF(tx, ty, tw - 1, th - 1));
            }
            else
            {
                e.Graphics.FillEllipse(new LinearGradientBrush(new PointF(tx, ty), new PointF(tx, ty + th), Color.LightGray.WithLight(scale), Color.Gray.WithLight(scale)), new RectangleF(tx, ty, tw, th));
                e.Graphics.DrawEllipse(Pens.DimGray.WithLight(scale), new RectangleF(tx, ty, tw - 1, th - 1));
            }

            // Text
            e.Graphics.DrawString(Value.ToString(), new Font(Font.FontFamily, 8, FontStyle.Bold), Brushes.Black.WithLight(scale), w, h / 2, new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
        }
    }
}