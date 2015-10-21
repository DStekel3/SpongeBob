using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace INFOIBV
{
    class Bewerkingen
    {
        public Bewerkingen() { }

        public double[,] ToGray(Color[,] c)
        {

            int width = c.GetLength(0);
            int height = c.GetLength(1);

            double[,] pixels = new double[width, height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = c[x, y];
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    if (avg > 130)
                        avg = 255;
                    else
                        avg = 0;

                    pixels[x, y] = avg;
                }
            }
            return pixels;
        }

        public Color[,] ToColorArray(double[,] d)
        {
            int width = d.GetLength(0);
            int height = d.GetLength(1);
            Color[,] c = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color newColor = Color.FromArgb((int)d[x, y], (int)d[x, y], (int)d[x, y]);
                    c[x, y] = newColor;
                }
            }

            return c;
        }
    }
}