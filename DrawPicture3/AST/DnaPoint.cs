using DrawPicture3.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.AST
{
    [Serializable]
    public class DnaPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void Init()
        {
            X = Tools.Next(Tools.MaxWidth);
            Y = Tools.Next(Tools.MaxHeight);
        }

        public DnaPoint Clone()
        {
            return new DnaPoint
            {
                X = X,
                Y = Y
            };
        }

        /// <summary>
        /// 突变
        /// </summary>
        public void Mutate(DnaDrawing drawing)
        {
            //大幅度改变顶点位置 
            if (Tools.WillMutate(Settings.ActiveMovePointMaxMutationRate))
            {
                X = Tools.Next(0, Tools.MaxWidth);
                Y = Tools.Next(0, Tools.MaxHeight);
                drawing.SetDirty();
            }

            //中等幅度改变顶点位置
            if (Tools.WillMutate(Settings.ActiveMovePointMidMutationRate))
            {
                X += Tools.Next(-Settings.ActiveMovePointRangeMid, Settings.ActiveMovePointRangeMid);
                X = Tools.Between(0, Tools.MaxWidth, X);

                Y += Tools.Next(-Settings.ActiveMovePointRangeMid, Settings.ActiveMovePointRangeMid);
                Y = Tools.Between(0, Tools.MaxHeight, Y);
                drawing.SetDirty();
            }

            //小幅度改变顶点位置
            if (Tools.WillMutate(Settings.ActiveMovePointMinMutationRate))
            {
                X += Tools.Next(-Settings.ActiveMovePointRangeMin, Settings.ActiveMovePointRangeMin);
                X = Tools.Between(0, Tools.MaxWidth, X);

                Y += Tools.Next(-Settings.ActiveMovePointRangeMin, Settings.ActiveMovePointRangeMin);
                Y = Tools.Between(0, Tools.MaxHeight, Y);
                drawing.SetDirty();
            }



        }

    }
}
