using DrawPicture3.AST;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.Classes
{
    public class Renderer
    {
        public static void Render(DnaDrawing drawing,Graphics g,int scale)
        {
            g.Clear(Color.Black);
            foreach(DnaPolygon polygon in drawing.Polygons)
            {
                Render(polygon, g, scale);
            }
        }

        private static void Render(DnaPolygon polygon ,Graphics g,int scale)
        {
            using (Brush brush = GetGdiBrush(polygon.Brush))
            {
                Point[] points = GetGdiPoints(polygon.Points, scale);
                g.FillPolygon(brush, points);
            }
        }

        private static Point[] GetGdiPoints(IList<DnaPoint> points,int scale)
        {
            Point[] pts = new Point[points.Count];
            int i = 0;
            foreach(DnaPoint pt in points)
            {
                pts[i++] = new Point(pt.X * scale, pt.Y * scale);
            }
            return pts;
        }

        private static Brush GetGdiBrush(DnaBrush b)
        {
            return new SolidBrush(Color.FromArgb(b.Alpha, b.Red, b.Green, b.Blue));
        }
    }
}

