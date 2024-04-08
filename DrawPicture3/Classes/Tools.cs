using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPicture3.Classes
{
    public static class Tools
    {
        private static readonly Random random = new Random();

        public static readonly int MaxPolygons = 250;
        public static int MaxWidth = 200;
        public static int MaxHeight = 200;

        public static int Next(int min,int max)
        {
            return random.Next(min, max);
        }

        public static int Next(int max)
        {
            return random.Next(max);
        }

        //是否会变异
        public static bool WillMutate(int mutationRate)
        {
            if (Next(mutationRate) == 1)
            {
                return true;
            }
            return false;
        }

        public static int Between(int min,int max,int value)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

    }
}
