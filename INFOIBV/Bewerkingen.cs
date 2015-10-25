using System;
using System.Drawing;
using System.Threading.Tasks;

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


            Parallel.For (0, width, x =>
            {
                Parallel.For (0, height, y => { 
                    Color pixelColor = c[x, y];
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    d[x, y] = avg;
                });
            });
            return d;
        }

        public double[,] Sharpening(double[,] c)
        {
            double[,] kernel = new double[,]{
                {-1,-1,-1},
                {-1,9,-1},
                {-1,-1,-1}
            };
            int width = c.GetLength(0);
            int height = c.GetLength(1);
            double[,] d = new double[width, height];

            Parallel.For(1, height-1, y =>
            {
                Parallel.For(1, width-1, x => {
                    double sum = 0;
                    for (int t = -1; t <=1; t++)
                    {
                        for (int u = -1; u <=1; u++)
                        {
                            try {
                                sum += c[x + t, y + u] * kernel[t+1, u+1];
                            }
                            catch(IndexOutOfRangeException e)
                            { }
                        }
                    }
                    if (sum > 255)
                        sum = 255;
                    else if (sum < 0)
                        sum = 0;
                    d[x, y] = sum;
                });
            });
            return d;
        }

        public double[,] Smoothing(double[,] c)
        {
            double[,] kernel = new double[,]{
                {0.11,0.11,0.11},
                {0.11,0.11,0.11},
                {0.11,0.11,0.11}
            };
            int width = c.GetLength(0);
            int height = c.GetLength(1);
            double[,] d = new double[width, height];

            Parallel.For(1, height - 1, y =>
            {
                Parallel.For(1, width - 1, x => {
                    double sum = 0;
                    for (int t = -1; t <= 1; t++)
                    {
                        for (int u = -1; u <= 1; u++)
                        {
                            try
                            {
                                sum += c[x + t, y + u] * kernel[t + 1, u + 1];
                            }
                            catch (IndexOutOfRangeException e)
                            { }
                        }
                    }
                    if (sum > 255)
                        sum = 255;
                    else if (sum < 0)
                        sum = 0;
                    d[x, y] = sum;
                });
            });
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




    }
}