using DrawPicture3.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.AST
{
    public class DnaPolygon
    {
        public List<DnaPoint> Points { get; set; }
        public DnaBrush Brush { get; set; }

        public void Init()
        {
            Points = new List<DnaPoint>();
            var origin = new DnaPoint();
            origin.Init();

            for(int i = 0; i < Settings.ActivePointsPerPolygonMin; i++)
            {
                var point = new DnaPoint();
                point.X = origin.X + Tools.Next(-3, 3);
                point.X = Tools.Between(0, Tools.MaxWidth, point.X);
                point.Y = origin.Y + Tools.Next(-3, 3);
                point.Y = Tools.Between(0, Tools.MaxHeight, point.Y);

                Points.Add(point);
            }
            Brush = new DnaBrush();
            Brush.Init();
        }
        public DnaPolygon Clone()
        {
            var newPolygon = new DnaPolygon();
            newPolygon.Brush = Brush.Clone();
            newPolygon.Points = new List<DnaPoint>();
            foreach (DnaPoint point in Points)
            {
                newPolygon.Points.Add(point.Clone());
            }
            return newPolygon;
        }


        public void Mutate(DnaDrawing drawing)
        {
            if (Tools.WillMutate(Settings.ActiveAddPointMutationRate))
            {
                AddPoint(drawing);
            }

            if (Tools.WillMutate(Settings.ActiveRemovePointMutationRate))
            {
                RemovePoint(drawing);
            }

            Brush.Mutate(drawing);
            Points.ForEach(x => x.Mutate(drawing));
        }

        private void RemovePoint(DnaDrawing drawing)
        {
            if (Points.Count > Settings.ActivePointsPerPolygonMin)
            {
                if (drawing.PointCount > Settings.ActivePointsMin)
                {
                    int index = Tools.Next(0, Points.Count);
                    Points.RemoveAt(index);
                    drawing.SetDirty();
                }
            }
        }

        private void AddPoint(DnaDrawing drawing)
        {
            //在2点之间插入一个点
            if (Points.Count < Settings.ActivePointsPerPolygonMax)
            {
                if(drawing.PointCount < Settings.ActivePointsMax)
                {
                    var newPoint = new DnaPoint();
                    int index = Tools.Next(1, Points.Count - 1);

                    DnaPoint prev = Points[index - 1];
                    DnaPoint next = Points[index];

                    newPoint.X = (prev.X + next.X) / 2;
                    newPoint.Y = (prev.Y + next.Y) / 2;

                    Points.Insert(index, newPoint);
                    drawing.SetDirty();
                }
            }
        }
    }
}
