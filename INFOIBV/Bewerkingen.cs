using System;
using System.Drawing;
using System.Threading.Tasks;

namespace INFOIBV
{
    class Bewerkingen
    {
        public object Paralell { get; private set; }

        public Bewerkingen() { }
        // Testje
        public double[,] ToGray(Color[,] c)
        {

            int width = c.GetLength(0);
            int height = c.GetLength(1);

            double[,] d = new double[width, height];


            Parallel.For(0, width, x =>
            {
                Parallel.For(0, height,y =>
                {
                    Color pixelColor = c[x, y];
                    int avg = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;

                    if (avg > 140)
                        avg = 255;
                    else
                        avg = 0;

                    d[x, y] = avg;
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

        public double[,] Dilation(double[,] d, int amount)
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
                            if (d[x + i, y + j] == 0)
                                totaal++;
                        }
                    }

                    if (totaal >= 4)
                        output[x, y] = 0;
                    else
                        output[x, y] = d[x, y];
                }
            }

            if (amount > 1)
                output = Dilation(output, amount - 1);
            return output;
        }


        public double[,] Erosion(double[,] d, int amount)
        {
            //  double[,] kernel = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int totaal = 0;
                    //Kernel 3x3
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (d[x + i, y + j] == 0)
                                totaal++;
                        }
                    }

                    if (totaal < 8)
                        output[x, y] = 255;

                }
            }
            if (amount > 1)
                output = Erosion(output, amount-1);
            return output;
        }

        public double[,] Edge(double[,] d)
        {
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            double[,] kernely = { { -1, -3, -1 }, { 0, 0, 0 }, { 1, 3, 1 } };
            double[,] kernelx = { { -1, 0, 1 }, { -3, 0, 3 }, { -1, 0, 1 } };

            Parallel.For(1, width - 1, x =>
           {
               Parallel.For(1, height - 1, y =>
               {
                   double xval = 0;
                   double yval = 0;

                   for (int i = 0; i < 3; i++)
                   {
                       for (int j = 0; j < 3; j++)
                       {
                           xval += kernelx[i, j] * d[-1 + i + x, -1 + j + y] / 255;
                           yval += kernely[i, j] * d[-1 + i + x, -1 + j + y] / 255;
                       }
                   }
                   double total = Math.Abs(xval) + Math.Abs(yval);
                   if (total > 0)
                       output[x, y] = d[x, y];
                   else
                       output[x, y] = 255;
               });
           });
                    return output;
        }

        public double[,] Subtract(double[,] a, double[,]b)
        {
            // when images have on position (x,y) both a black pixel, this will become white
            // the other pixels will stay the same as in image a.
            double[,] result = new double[a.GetLength(0), a.GetLength(1)];
            Parallel.For(0, a.GetLength(1), y =>
            {
                Parallel.For(0, a.GetLength(0), x =>
                {
                    if (a[x, y] == 0 && b[x, y] == 0)
                        result[x, y] = 255;
                    else
                        result[x, y] = a[x,y];
                });
            });
            return result;
        }

        public double[,] Add(double[,] a, double[,] b)
        {
            // when at least one of the images has a black pixel on (x,y), this will become black,
            // the other pixels will stay white.
            double[,] result = new double[a.GetLength(0), a.GetLength(1)];
            Parallel.For(0, a.GetLength(1), y =>
            {
                Parallel.For(0, a.GetLength(0), x =>
                {
                    if (a[x, y] == 0 || b[x, y] == 0)
                        result[x, y] = 0;
                    else
                        result[x, y] = 255;
                });
            });
            return result;
        }

        public double[,] Inverse(double[,] d)
        {
            // calculates the inverse of given image
            double[,] result = new double[d.GetLength(0), d.GetLength(1)];
            Parallel.For(0, d.GetLength(1), y =>
            {
                Parallel.For(0, d.GetLength(0), x =>
                {
                    result[x, y] = 255 - d[x, y];
                });
            });
            return result;
        }
    }
}