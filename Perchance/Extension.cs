using System.Drawing.Drawing2D;
using System.Reflection;

namespace Perchance
{
    public static class Extension
    {
        public static SolidBrush WithDark(this Brush brush, float percOfDarkDark)
        {
            if (brush is SolidBrush br)
                return new SolidBrush(ControlPaint.Dark(br.Color, percOfDarkDark));

            throw new InvalidOperationException("Only Solid brush can apply this extension!");
        }

        public static Pen WithDark(this Pen pen, float percOfDarkDark) => new Pen(ControlPaint.Dark(pen.Color, percOfDarkDark));

        public static Color WithDark(this Color color, float percOfDarkDark) => ControlPaint.Dark(color, percOfDarkDark);

        public static SolidBrush WithLight(this Brush brush, float percOfLightLight)
        {
            if (brush is SolidBrush br)
                return new SolidBrush(ControlPaint.Light(br.Color, percOfLightLight));

            throw new InvalidOperationException("Only Solid brush can apply this extension!");
        }

        public static Pen WithLight(this Pen pen, float percOfLightLight) => new Pen(ControlPaint.Light(pen.Color, percOfLightLight));

        public static Color WithLight(this Color color, float percOfLightLight) => ControlPaint.Light(color, percOfLightLight);

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, RectangleF bounds, int tl, int tr, int br, int bl)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            using (GraphicsPath path = RoundedRect(bounds, tl, tr, br, bl))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF bounds, int tl, int tr, int br, int bl)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));
            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            using (GraphicsPath path = RoundedRect(bounds, tl, tr, br, bl))
                graphics.FillPath(brush, path);
        }

        public static GraphicsPath RoundedRect(RectangleF bounds, int tl, int tr, int br, int bl)
        {
            var diameter1 = tl * 2;
            var diameter2 = tr * 2;
            var diameter3 = br * 2;
            var diameter4 = bl * 2;

            var arc1 = new RectangleF(bounds.Location, new Size(diameter1, diameter1));
            var arc2 = new RectangleF(bounds.Location, new Size(diameter2, diameter2));
            var arc3 = new RectangleF(bounds.Location, new Size(diameter3, diameter3));
            var arc4 = new RectangleF(bounds.Location, new Size(diameter4, diameter4));
            var path = new GraphicsPath();

            // tl
            if (tl == 0)
                path.AddLine(arc1.Location, arc1.Location);
            else
                path.AddArc(arc1, 180, 90);

            // tr
            arc2.X = bounds.Right - diameter2;
            if (tr == 0)
                path.AddLine(arc2.Location, arc2.Location);
            else
                path.AddArc(arc2, 270, 90);

            // br
            arc3.X = bounds.Right - diameter3;
            arc3.Y = bounds.Bottom - diameter3;
            if (br == 0)
                path.AddLine(arc3.Location, arc3.Location);
            else
                path.AddArc(arc3, 0, 90);

            // bl
            arc4.X = bounds.Right - diameter4;
            arc4.Y = bounds.Bottom - diameter4;
            arc4.X = bounds.Left;
            if (bl == 0)
                path.AddLine(arc4.Location, arc4.Location);
            else
                path.AddArc(arc4, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
