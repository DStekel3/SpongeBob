using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;


namespace INFOIBV
{
    class Bewerkingen
    {
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

        public Color[,] ToColor(double[,] d, Score s)
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

            for (int x = s.x; x <= s.x + s.w; x++)
            {
                c[x, s.y - 1] = Color.FromArgb(255, 0, 0);
                c[x, s.y] = Color.FromArgb(255, 0, 0);
                c[x, s.y + 1] = Color.FromArgb(255, 0, 0);

                c[x, s.y + s.h] = Color.FromArgb(255, 0, 0);
                c[x, s.y + s.h - 1] = Color.FromArgb(255, 0, 0);
                c[x, s.y] = Color.FromArgb(255, 0, 0);
                c[x, s.y + s.h + 1] = Color.FromArgb(255, 0, 0);
            }
            for (int y = s.y; y <= s.y + s.h; y++)
            {
                c[s.x - 1, y] = Color.FromArgb(255, 0, 0);
                c[s.x, y] = Color.FromArgb(255, 0, 0);
                c[s.x + 1, y] = Color.FromArgb(255, 0, 0);

                c[s.x + s.w - 1, y] = Color.FromArgb(255, 0, 0);
                c[s.x + s.w, y] = Color.FromArgb(255, 0, 0);
                c[s.x + s.w + 1, y] = Color.FromArgb(255, 0, 0);
            }

            return c;
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
                      for (int u = -1; u <= 1; u++)
                      {
                          for (int v = -1; v <= 1; v++)
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
                            if (d[x + i, y + j] == 255)
                                totaal++;
                        }
                    }

                    if (totaal < th)
                        output[x, y] = 0;
                    else
                        output[x, y] = 255;
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

            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < height - 1; y++)
                {
                    int totaal = 0;                         //The total number of pixels in the picture covered by the kernel that are 255
                    if (x != 0 && x != width - 1 && y != 0 && y != height - 1)
                    {
                        //Kernel 3x3
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                if (d[x + i, y + j] == 255)
                                    totaal++;
                            }
                        }
                    }
                    if (totaal > th)
                        output[x, y] = 255;
                    else
                        output[x, y] = d[x, y];

                }
            }
            if (amount > 1)
                output = Erosion(output, th, amount - 1);

            return output;
        }

        public double[,] ColorFilter(Color[,] d)
        {
            int w = d.GetLength(0);
            int h = d.GetLength(1);

            double[,] res = new double[w, h];
            for(int x = 0;x< w;x++)
            {
                for (int y = 0;y< h;y++)
                {
                    Color z = d[x, y];
                    if (z.R >= 70 && z.R <= 140 &&
                        z.G >= 90 && z.G <= 130 &&
                        z.B >= 0 && z.B <= 50)
                        res[x, y] = 255;
                }
            }
            return res;
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
                       output[x, y] = 0;
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

        public Score FindSquares(double[,] d)
        {
            Score s = SquareTest(d);
            return s;
        }


        public Tuple<int,int> HoughLine(double[,] d)
        {
            var r = Hough(d);
            var t = GetMaximumHough(r);
            
            return t;

        }

        public Tuple<int,int> GetMaximumHough(double[,] r)
        {
            int max = 0;
            int rho = -1;
            int theta = -1;
            for (int t = 0; t < r.GetLength(0); t++)
            {
                for (int u = 0; u < r.GetLength(1); u++)
                {
                    if (r[t, u] > max)
                    {
                        theta = u;
                        rho = t;
                    }
                }
            }
            return new Tuple<int, int>(theta, rho);
        }

        public double[,] My_Hough(double[,] d)
        {
            int w = d.GetLength(0);
            int h = d.GetLength(1);
            
                int rho_max = (int)Math.Floor(Math.Sqrt(w * w + h * h)) + 1;
                int[,] accarray = new int[rho_max, 180];
                double[] theta = new double[180];

                double i = 0;
                for(int index=0;index<180;index++)
                {
                    theta[index] = i;
                    i += Math.PI / 180;
                }

                double rho;
                int rho_int;
                for(int n=0;n< w;n++)
                {
                    for(int m=0;m< h;m++)
                    {
                        if(d[n,m] == 255)
                        {
                            for(int k=0;k<180;k++)
                            {
                                rho = (m * Math.Cos(theta[k])) + (n * Math.Sin(theta[k]));
                                rho_int = (int)Math.Round(rho / 2 + rho_max / 2);
                                accarray[rho_int, k]++;
                            }
                        }
                    }
                }

                int amax = 0;
                for (int x = 0; x < rho_max; x++)
                {
                    for (int y = 0; y < 180; y++)
                    {
                        if (accarray[x, y] > amax) amax = accarray[x, y];
                    }
                }
                double[,] res = new double[w, h];
                int highest = 0;
                int _x = -1;
                int _y = -1;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < 180; y++)
                    {
                        int b = 0;
                        if (amax != 0) b = (int)(((double)accarray[x, y] / (double)amax) * 255.0);
                        res[x, y] = b;
                        if (accarray[x, y] > highest)
                        {
                            highest = accarray[x, y];
                            _x = x;
                            _y = y;
                        }
                    }
                }
                return res;
            
        }

        public double[,] Hough(double[,] d)
        {
            /* Perform Hough Line Transform on img */

            /* the middle of img is made the origin */
            int w = d.GetLength(0);
            int h = d.GetLength(1);
            double[,] res = new double[w, h];
            double pmax = Math.Sqrt(((w / 2) * (w / 2)) + ((h / 2) * (h / 2)));
            double tmax = Math.PI * 2;

            // step sizes
            double dp = pmax / (double)w;
            double dt = tmax / (double)h;

            int[,] A = new int[w, h]; // accumulator array

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

            // make us a greyscale bitmap
            int highest = 0;
            int _x = -1;
            int _y = -1;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int b = 0;
                    if (amax != 0) b = (int)(((double)A[x, y] / (double)amax) * 255.0);
                    res[x, y] = b;
                    if (A[x,y] > highest)
                    {
                        highest = A[x,y];
                        _x = x;
                        _y = y;
                    }
                }
            }
            return res;
            //return new Tuple<int,int>(_x, _y);
        }

        public Score SquareTest(double[,] z)
        {
            int w = z.GetLength(0);
            int h = z.GetLength(1);

            Score best = new Score(-1, -1, -1, -1, -1);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (z[x, y] == 255)
                    {
                        //if pixel is white
                        for (int n = 100; n <= 400; n++)
                        {
                            int width = n;
                            int height = n;
                            Score s = GetScoreSquare(z, x, y, width, height, 0.0);
                            if (s == null)
                                break;
                            else if (s.score > 40 && s.score > best.score)
                                best = s;
                        }
                    }
                }
            }
            return best;
        }

        public Score GetScoreSquare(double[,] d, int x, int y, int w, int h, double angle)
        {
            int min_h = 50;
            int min_w = 50;
            int max_h = 400;
            int max_w = 400;

            if (x + w >= d.GetLength(0))
                return null;
            if (y + h >= d.GetLength(1))
                return null;

            int res = 0;
            for (int a = x; a <= x + w; a++)
            {
                if (d[a, y] == 255)
                    res += 2;
                else
                    res--;
                if (d[a, y + h] == 255)
                    res += 2;
                else
                    res--;
            }
            for (int b = y; b <= y + h; b++)
            {
                if (d[x, b] == 255)
                    res += 2;
                else
                    res--;
                if (d[x + w, b] == 255)
                    res += 2;
                else
                    res--;
            }
            return new Score(x, y, w, h, res);
        }

        public double ToRadians(int x)
        {
            return x / 180 * Math.PI;
        }

        public double[,] Perimeter(double[,] d)
        {
            for(int y =0;y<d.GetLength(1);y++)
            {
                for(int x=0;x<d.GetLength(0);x++)
                {
                    if(d[x,y] == 255)
                    {
                        var s = GetPerimeter(x, y, d);
                        d = s.Item1;
                    }
                }
            }
            return d;
        }

        private Tuple<double[,], double> GetPerimeter(int x, int y, double[,] d)
        {
            int max_x = d.GetLength(0)-1;
            int max_y = d.GetLength(1)-1;
            int cur_x = x;
            int cur_y = y;
            double score = 0;
            while(!(cur_x == x && cur_y == y && score>0))
            {
                double cur_score = score;
                // check right
                if (cur_x != max_x && d[cur_x + 1, cur_y] == 255)
                {
                    d[cur_x, cur_y] = 0;
                    score++;
                    cur_x++;
                }
                // check bottom
                else if (cur_y != max_y && d[cur_x, cur_y + 1] == 255)
                {
                    d[cur_x, cur_y] = 0;
                    score++;
                    cur_y++;
                }
                // check left
                else if (cur_x != 0 && d[cur_x - 1, cur_y] == 255)
                {
                    d[cur_x, cur_y] = 0;
                    score++;
                    cur_x--;
                }

                //  check top
                else if (cur_y != 0 && d[cur_x, cur_y - 1] == 255)
                {
                    d[cur_x, cur_y] = 0;
                    score++;
                    cur_y--;
                }
                if (cur_score == score)
                    break;
            }
            return new Tuple<double[,], double>(d, score);
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

    public class Score
    {
        public int x, y, w, h, score;
        public Score(int _x, int _y, int _w, int _h, int _score)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            score = _score;
        }
    }
}