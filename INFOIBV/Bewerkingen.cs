﻿using System.Drawing;

namespace INFOIBV
{
    class Bewerkingen
    {
        public Bewerkingen() { }
        // Testje
        public double[,] ToGray(Color[,] c)
        {

            int width = c.GetLength(0);
            int height = c.GetLength(1);

            double[,] d = new double[width, height];


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = c[x, y];
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    if (avg > 140)
                        avg = 255;
                    else
                        avg = 0;

                    d[x, y] = avg;
                }
            }
            return d;
        }

        public Color[,] ToColor(double[,] d)
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

        public double[,] Dilation(double[,] d)
        {
            //  double[,] kernel = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            double[,] output = new double[d.GetLength(0),d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            for (int x = 1; x < width-1; x++)
            {
                for (int y = 1; y < height-1; y++)
                {
                    int totaal = 0;
                                                        //Kernel 3x3
                    for(int i = -1; i < 2; i++)
                    {
                        for(int j = -1; j<2; j++)
                        {
                            if (d[x + i, y + j] < 140)
                                totaal++;
                        }
                    }

                    if (totaal>8)
                        output[x, y] = 0;
                    else
                        output[x, y] = 255;
                }
            }

            return output;
        }
        
    }
}