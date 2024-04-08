using DrawPicture3.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.AST
{
    [Serializable]
    public class DnaDrawing
    {
        public List<DnaPolygon> Polygons { get; set; }

        //标记 用于判断图像是否发生改变 从而重新计算适应度
        public bool IsDirty { get; private set; }

        public int PointCount
        {
            get
            {
                int point = 0;
                foreach(DnaPolygon p in Polygons)
                {
                    point += p.Points.Count;
                }
                return point;
            }
        }

        public void Init()
        {
            Polygons = new List<DnaPolygon>();

            for (int i = 0; i < Settings.ActivePolygonsMin; i++)
            {
                AddPolygon();
            }
            SetDirty();
        }


        public DnaDrawing Clone()
        {
            var drawing = new DnaDrawing();
            drawing.Polygons = new List<DnaPolygon>();
            foreach (DnaPolygon polygon in Polygons)
            {
                drawing.Polygons.Add(polygon.Clone());
            }
            return drawing;
        }

        public void Mutate()
        {
            if (Tools.WillMutate(Settings.ActiveAddPolygonMutationRate))
            {
                AddPolygon();
            }

            if (Tools.WillMutate(Settings.ActiveRemovePolygonMutationRate))
            {
                RemovePolygon();
            } 

            if (Tools.WillMutate(Settings.ActiveMovePolygonMutationRate))
            {
                MovePolygon();
            }

            foreach(DnaPolygon polygon in Polygons)
            {
                polygon.Mutate(this);
            }
        }

        public void MovePolygon()
        {
            if (Polygons.Count < 1)
                return;

            int index = Tools.Next(0, Polygons.Count);
            DnaPolygon poly = Polygons[index];
            Polygons.RemoveAt(index);
            index = Tools.Next(0, Polygons.Count);
            Polygons.Insert(index, poly);
            SetDirty();
        }

        public void RemovePolygon()
        {
            if (Polygons.Count > Settings.ActivePolygonsMin)
            {
                int index = Tools.Next(0, Polygons.Count);
                Polygons.RemoveAt(index);
                SetDirty();
            }
        }

        public void AddPolygon()
        {
            if (Polygons.Count < Settings.ActivePolygonsMax)
            {
                var newPolygon = new DnaPolygon();
                newPolygon.Init();

                int index = Tools.Next(0, Polygons.Count);

                Polygons.Insert(index, newPolygon);
                SetDirty();
            }
        }

        public void SetDirty()
        {
            IsDirty = true;
        }
    }
}
