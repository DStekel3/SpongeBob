using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace INFOIBV
{
    class Bewerkingen
    {
        public object Paralell { get; private set; }

        public Bewerkingen() { }



        public double[,] ToGray(Color[,] c)
        {
            // Get the average value of each pixel
            int width = c.GetLength(0);
            int height = c.GetLength(1);

            double[,] d = new double[width, height];

            Parallel.For(0, width, x =>
            {
                Parallel.For(0, height, y =>
                {
                    Color pixelColor = c[x, y];
                     int avg = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.5 + pixelColor.B * 0.2);
          //           int avg = (pixelColor.R + pixelColor.G + pixelColor.B)/3;

                    if (avg > 255)
                        avg = 255;
                   

                    d[x, y] = avg;
                });
            });
            return d;
        }

        public double[,] ToBinary(double[,] d, int th)
        {
            // Convert a greyscale image to a binary image depending on threshhold th
            int width = d.GetLength(0);
            int height = d.GetLength(1);

            double[,] output = new double[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (d[x, y] > th)
                        output[x, y] = 255;
                }
            }
            
            return output;
        }

        public Color[,] ToColor(double[,] d)
        {
            // Convert a double array to a Color array so the Image can be displayed
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

        public double[,] DoubleThresHold(double[,] d, int l, int u)
        {
            // Convert a greyscale image to a binary image depending on threshhold th
            int width = d.GetLength(0);
            int height = d.GetLength(1);

            double lower = l;
            double upper = u;

            double[,] output = new double[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (d[x, y] >= lower && d[x, y] <= upper)
                        output[x, y] = 255;
                }
            }

            return output;
        }

        public double[,] BinaryMedian(double[,] d)
        {
            int width = d.GetLength(0);
            int height = d.GetLength(1);
            double[,] result = new double[width, height];

            int[,] median = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            List<double> vals = new List<double>();

            Parallel.For(1, height - 1, y =>
              {
                  Parallel.For(1, width - 1, x =>
                  {
                      int black = 0;
                      int white = 0;
                      for(int u= -1; u <= 1; u++)
                      {
                          for(int v=-1;v<=1;v++)
                          {
                              if (d[x + u, y + v] == 0)
                                  black++;
                              else
                                  white++;
                          }
                          if (black > white)
                              result[x, y] = 0;
                          else
                              result[x, y] = 255;
                      }
                  });
              });

            return result;
        }

        public double[,] Dilation(double[,] d, int th, int amount)
        {
            // Dilate the double array depending on threshhold th, amount times
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

                    if (totaal >=  th)
                        output[x, y] = 0;
                    else
                        output[x, y] = d[x, y];
                }
            }

            if (amount > 1)
                output = Dilation(output, th, amount - 1);
            return output;
        }


        public double[,] Erosion(double[,] d, int th, int amount)
        {
            // Erode the double array depending on threshhold th, amount times
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    int totaal = 0;                         //The total number of pixels in the picture covered by the kernel that are 0
                    //Kernel 3x3
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (d[x + i, y + j] == 0)
                                totaal++;
                        }
                    }

                    if (totaal < th)
                        output[x, y] = 255;
                    else
                        output[x, y] = d[x, y];

                }
            }
            if (amount > 1)
                output = Erosion(output, th, amount - 1);

            return output;
        }

        public double[,] Edge(double[,] d)
        {
            // Get the edges
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

        public double[,] Subtract(double[,] a, double[,] b)
        {
            // when images have on position (x,y) both a black pixel, this will become white
            // the other pixels will stay the same as in image a.
            double[,] result = new double[a.GetLength(0), a.GetLength(1)];
            Parallel.For(0, a.GetLength(1), y =>
            {
                Parallel.For(0, a.GetLength(0), x =>
                {
                      if (a[x, y] == 0 && a[x, y] == b[x, y])
                        result[x, y] = 255;
                    else
                          result[x, y] = a[x, y];
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

        public double[,] Smooth(double[,] d, int amount)
        {
            // Smooth
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            double[,] kernel = { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };

            Parallel.For(1, width - 1, x =>
            {
                Parallel.For(1, height - 1, y =>
                {
                    double val = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            val += (kernel[i, j] * d[-1 + i + x, -1 + j + y]) / 16;

                        }
                    }

                    output[x, y] = val;
                });
            });
            if (amount > 1)
                output = Smooth(output, amount - 1);
            return output;
        }

        public double[,] Sharp(double[,] d)
        {
            // Smooth
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            double[,] kernel = { { -1, -1, -1 }, { -1, 9, -1 }, { -1, 1, -1 } };

            Parallel.For(1, width - 1, x =>
            {
                Parallel.For(1, height - 1, y =>
                {
                    double val = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            val += (kernel[i, j] * d[-1 + i + x, -1 + j + y]);

                        }
                    }

                    output[x, y] = val;
                });
            });
            return output;
        }

        public double[,] Closing(double[,] d, int amount)
        {
            d = Dilation(d, 1, 1);
            d = Erosion(d, 1, 1);
            
            if (amount > 1)
                d = Closing(d, amount - 1);

            return d;
        }

        public double[,] HoughTransform(double[,] d)
        {
            /* Perform Hough Line Transform on img */
            int w = d.GetLength(0);
            int h = d.GetLength(1);

            /* the middle of img is made the origin */

            double pmax = Math.Sqrt(((w / 2) * (w / 2)) + ((h / 2) * (h / 2)));
            double tmax = Math.PI * 2;

            // step sizes
            double dp = pmax / (double)w;
            double dt = tmax / (double)h;

            int[,] A = new int[w * 2, h * 2]; // accumulator array

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (d[x, h - y - 1] == 255) // pixel is white - h-y flips incoming img
                    {
                        // book claims it's j = 1, i think it should be j = 0
                        for (int j = 0; j < h; j++)
                        {
                            double row = ((double)(x - (w / 2)) * Math.Cos(dt * (double)j)) + ((double)(y - (h / 2)) * Math.Sin(dt * (double)j));
                            // find index k of A closest to row
                            int k = (int)((row / pmax) * w);
                            if (k >= 0 && k < w) A[k, j]++;
                        }
                    }
                }
            }

            // find max of A, to normalize colors
            int amax = 0;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (A[x, y] > amax) amax = A[x, y];
                }
            }

            double[,] res = new double[w, h];
            // make us a greyscale bitmap
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int b = 0;
                    if (amax != 0) b = (int)(((double)A[x, y] / (double)amax) * 255.0);
                    res[x, y] = b;
                }
            }
            return res;
        }
        public double[,] Opening(double[,] d, int amount)
        {
            d = Erosion(d, 1, 1);
            d = Dilation(d, 1, 1);

            if (amount > 1)
                d = Opening(d, amount - 1);

            return d;
        }
    }
}