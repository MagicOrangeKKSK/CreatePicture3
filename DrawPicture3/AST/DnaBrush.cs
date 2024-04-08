using DrawPicture3.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.AST
{
    [Serializable]
    public class DnaBrush
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }

        public void Init()
        {
            Red = Tools.Next(255);
            Green = Tools.Next(255);
            Blue = Tools.Next(255);
            Alpha = Tools.Next(10, 60);
        }

        public DnaBrush Clone()
        {
            return new DnaBrush
            {
                Alpha = Alpha,
                Red = Red,
                Green = Green,
                Blue = Blue
            };
        }

        public void Mutate(DnaDrawing drawing)
        {
            //每个颜色都有可能突变
            if (Tools.WillMutate(Settings.ActiveRedMutationRate))
            {
                Red = Tools.Next(Settings.ActiveRedRangeMin, Settings.ActiveRedRangeMax);
                drawing.SetDirty();
            }

            if (Tools.WillMutate(Settings.ActiveRedMutationRate))
            {
                Green = Tools.Next(Settings.ActiveGreenRangeMin, Settings.ActiveGreenRangeMax);
                drawing.SetDirty();
            }

            if (Tools.WillMutate(Settings.ActiveRedMutationRate))
            {
                Blue = Tools.Next(Settings.ActiveBlueRangeMin, Settings.ActiveBlueRangeMax);
                drawing.SetDirty();
            }

            if (Tools.WillMutate(Settings.ActiveRedMutationRate))
            {
                Alpha = Tools.Next(Settings.ActiveAlphaRangeMin, Settings.ActiveAlphaRangeMax);
                drawing.SetDirty();
            }

        }
    }
}
