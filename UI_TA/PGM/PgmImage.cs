using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_TA.PGM
{
    public class PgmImage
    {
        public int width;
        public int height;
        public int maxVal;
        public byte[][] pixels;

        public PgmImage(int width, int height, int maxVal,
          byte[][] pixels)
        {
            this.width = width;
            this.height = height;
            this.maxVal = maxVal;
            this.pixels = pixels;
        }
    }
}
