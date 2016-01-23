using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;

namespace TestUI
{
    public class DemoGame : Game
    {
        public DemoGame(Canvas pageCanvas)
            : base(pageCanvas)
        {

        }

        protected override void InitializeBikes(List<Point> startPoints)
        {
            // Clear the list.
            bikes = new List<TronBike>();

            //no worries, this is an ambi-turner
            bikes.Add(new CPUBike(startPoints[0], bikeSpeed, Brushes.SteelBlue, Directions.Right));

            bikes.Add(new CPUBike(startPoints[1], bikeSpeed, Brushes.Firebrick, Directions.Left));
        }
    }
}
