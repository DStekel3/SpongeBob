using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;


namespace INFOIBV
{
    class Bewerkingen
    {
        public List<Object> objects = new List<Object>();

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
            foreach(Object o in objects)
                o.calc_isMiddle(d);
            c = DrawObjects(c);
            return c;
        }

        public Color[,] DrawResult(Color[,] d)
        {
            d = DrawObjects(d);
            return d;
        }

        public double[,] Dilation(double[,] d, int amount)
        {
            // Dilate the double array  amount times
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (d[x, y] == 255)
                        //Kernel 3x3
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                output[x + i, y + j] = 255;
                            }
                        }
                }
            }

            if (amount > 1)
                output = Dilation(output, amount - 1);
            return output;
        }

        public double[,] Erosion(double[,] d, int amount)
        {
            // Erode the double array  amount times
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
                            if (d[x + i, y + j] == 255)
                                totaal++;
                        }
                    }

                    if (totaal >= 9)
                        output[x, y] = 255;
                    else
                        output[x, y] = 0;

                }
            }
            if (amount > 1)
                output = Erosion(output, amount - 1);

            return output;
        }

        public double[,] ErosionPlus(double[,] d, Object o)
        {
            // Perform erosion with a 3x3 "+" kernel until the next erosion would completely remove the object. This will thin objects and find their middle points
            double[,] kernel = { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int min_x = o.min_x;
            int max_x = o.max_x;
            int min_y = o.min_y;
            int max_y = o.max_y;

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            for (int x = min_x; x <= max_x; x++)
            {
                for (int y = min_y; y < max_y; y++)
                {
                    int totaal = 0;                         //The total number of pixels in the picture covered by the kernel that are 0
                                                            //Kernel 3x3
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (d[x + i, y + j] * kernel[i + 1, j + 1] == 255)
                                totaal++;
                        }
                    }

                    if (totaal == 5)
                        output[x, y] = 255;
                    else
                        output[x, y] = 0;

                }
            }
            int count = 0;
            for (int x = min_x; x <= max_x; x++)
            {
                for (int y = min_y; y < max_y; y++)
                {
                    if (output[x, y] == 255)
                    {
                        count++;
                    }
                }
            }


            if (count > 1)
                return ErosionPlus(output, o);
            else if (count == 1)
                return output;
            else
                return d;
        }

        public double[,] Middle(double[,] d)
        {
            //Outputs an image with for each object found, only it's middle point and its bounding box
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];
            foreach (Object o in objects)
                output = Add(output, ErosionPlus(d, o));

            return output;
        }
        
        public double[,] Edge(double[,] d)
        {
            // Get the edges
            double[,] output = new double[d.GetLength(0), d.GetLength(1)];

            int width = d.GetLength(0);
            int height = d.GetLength(1);

            //using a kernel for vertical edges and a kernel for horizontal edges
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

        public double[,] Add(double[,] a, double[,] b)
        {
            // when at least one of the images has a black pixel on (x,y), this will become black,
            // the other pixels will stay white.
            double[,] result = new double[a.GetLength(0), a.GetLength(1)];
            Parallel.For(0, a.GetLength(1), y =>
            {
                Parallel.For(0, a.GetLength(0), x =>
                {
                    result[x, y] = Math.Min(255, a[x, y] + b[x, y]);
                });
            });
            return result;
        }

        public double[,] Inverse(double[,] d)
        {
            // calculates the inverse of given image d
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

        public double[,] Perimeter(double[,] d)
        {
            //Seeks objects and calculates the perimeter
            for (int y = 0; y < d.GetLength(1); y++)
            {
                for (int x = 0; x < d.GetLength(0); x++)
                {
                    if (d[x, y] == 255)
                    {
                        var s = GetPerimeter(x, y, d);
                        d = s.Item1;
                        Object o = s.Item2;
                        if (o.area > 30 && !Intersect(o, d)) // add object with area > 30 and don't intersect with the boundaries of the image
                            objects.Add(o);
                    }
                }
            }
            return d;
        }

        private bool Intersect(Object o, double[,] d)
        {
            // checks if object intersects with the boundaries of the image
            if (o.min_x == 0
                || o.max_x == d.GetLength(0) - 1
                || o.min_y == 0
                || o.max_y == d.GetLength(1) - 1)
                return true;
            return false;
        }

        private Tuple<double[,], Object> GetPerimeter(int x, int y, double[,] d)
        {
            // calculates the perimeter of a certain object by measuring the length of object boundary (calculating the length of it's edge).
            // we exclude diagonal steps, so we use city block distance
            // when we're done with a pixel, we set it's value to 0, to avoid using this pixel again in our calculation.
            // returns resulting double[,] array and object.

            int max_x = d.GetLength(0) - 1; // max x-position of the image
            int max_y = d.GetLength(1) - 1; // max y-position of the image
            int cur_x = x; // current x-position used for calculation
            int cur_y = y; // current y-position used for calculation
            double score = 0; // length of perimeter
            Object o = new Object(x, y);
            while (!(cur_x == x && cur_y == y && score > 0))
            {
                double cur_score = score;
                d[cur_x, cur_y] = 0;

                //  check top
                if (cur_y != 0 && d[cur_x, cur_y - 1] == 255)
                {
                    score++;
                    cur_y--;
                    if (cur_y < o.min_y)
                        o.min_y = cur_y;
                }

                // check right
                else if (cur_x != max_x && d[cur_x + 1, cur_y] == 255)
                {
                    score++;
                    cur_x++;
                    if (cur_x > o.max_x)
                        o.max_x = cur_x;
                }
                // check bottom
                else if (cur_y != max_y && d[cur_x, cur_y + 1] == 255)
                {
                    score++;
                    cur_y++;
                    if (cur_y > o.max_y)
                        o.max_y = cur_y;
                }

                // check left
                else if (cur_x != 0 && d[cur_x - 1, cur_y] == 255)
                {
                    score++;
                    cur_x--;
                    if (cur_x < o.min_x)
                        o.min_x = cur_x;
                }

                if (cur_score == score)
                    break;
            }
            o.perimeter = score;
            o.calc_properties(); // calculate certain properties of the object
            return new Tuple<double[,], Object>(d, o);
        }

        public Color[,] DrawObjects(Color[,] d)
        {
            //Draws found objects
            foreach (Object o in objects)
            {
                if (o.isSquare && o.area > 400 && o.isMiddle && o.Perim)
                {
                    for (int x = o.min_x; x <= o.max_x; x++)
                    {
                        d[x, o.min_y] = Color.Red;
                        d[x, o.max_y] = Color.Red;
                    }
                    for (int y = o.min_y; y <= o.max_y; y++)
                    {
                        d[o.min_x, y] = Color.Red;
                        d[o.max_x, y] = Color.Red;
                    }
                }
            }
            return d;
        }

        public double[,] Opening(double[,] d, int amount)
        {
            //performs an opening on d amount times
            d = Erosion(d, 1);
            d = Dilation(d, 1);

            if (amount > 1)
                d = Opening(d, amount - 1);

            return d;
        }
    }

    public class Object
    {
        public int min_x, max_x, min_y, max_y, area;
        public double perimeter;
        public bool isSquare = false;
        public bool isMiddle = false;
        public bool Perim = false;

        public Object(int a, int b)
        {
            min_x = a; max_x = a;
            min_y = b; max_y = b;
        }

        public void calc_properties()
        {
            area = (max_x - min_x) * (max_y - min_y); // calculates the area of the surrounding bounding box of the object
            calc_isSquare(); // checks if the bounding box is a square 
            calc_isPerim(); // checks if the perimeter of the object == perimeter of the bounding box
        }

        public void calc_isSquare()
        {
            //check whether the boundingbox is square
            int width = max_x - min_x;
            int height = max_y - min_y;
            if (Math.Abs(width - height) < 10)
                isSquare = true;
            else
                isSquare = false;
        }

        public void calc_isPerim()
        {
            // checks if the perimeter of the object is equal to the perimeter of the bounding box
            // for plus-signs the difference between these should be very low
            if (Math.Abs((perimeter / 1) - (2*(max_x- min_x)+(2*(max_y- min_y)))) < 20)
                Perim = true;
            else
                Perim = false;
        }

        public void calc_isMiddle(double[,] d)
        {
            // after using ErosionPlus, where we use erosion on the object until it's almost gone, 
            // we check the pixels left in the image. 
            // For a plus-sign these pixels should be in the middle of the object-area.
            int count = 0;
            for (int y = min_y; y <= max_y; y++)
            {
                for (int x = min_x; x <= max_x; x++)
                {
                    if (d[x, y] == 255)
                    {
                        count++;
                        int x_pos = x;
                        int y_pos = y;
                        if (Math.Abs(((max_x - min_x) / 2 + min_x) - x_pos) < 2) // diff between middle-point(x-) and the location of the pixel
                        {
                            if (Math.Abs(((max_y - min_y) / 2 + min_y) - y_pos) < 2) // diff between middle-point(y) and the location of the pixel
                            {
                                isMiddle = true;
                            }
                        }
                       
                    }
                }
            }
            if (count > 4) // if more than given amount of pixels is left in the image, we decide the object isn't a plus-sign. For plus-signs, the pixels left should be very low.
                isMiddle = false;
        }

    }
}