using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace INFOIBV
{
    public class Bewerkingen
    {
        public double[,] GrayValues(Color[,] pic)
        {
            double[,] result = new double[pic.GetLength(0), pic.GetLength(1)];

            //DO YOUR STUFF
            Parallel.For(0, pic.GetLength(1), y =>
            {
                Parallel.For(0, pic.GetLength(0), x =>
    {


    }
}