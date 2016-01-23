using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestUI
{
    class AutoBike : TronBike
    {
        const int sightRange = 4;

		public AutoBike(Point pos, int speed, Directions startDir)
			: base(pos, speed, Brushes.Black, startDir)
		{

		}

		public AutoBike(Point pos, int speed, Brush colour, Directions startDir)
            : base(pos, speed, colour, startDir)
		{

		}


        public Tuple<Point, Point> GetSightRegion()
        {
            Point P1 = position;
            Point P2 = position;
            int offset = 1 + sightRange;

            switch (direction)
            {
                case Directions.Up:
                    P1.Y -= speed;
                    P2.Y -= offset;
                    break;
                case Directions.Left:
                    P1.X -= speed;
                    P2.X -= offset;
                    break;
                case Directions.Down:
                    P1.Y += offset;
                    P2.Y += speed;
                    break;
                case Directions.Right:
                    P1.X += offset;
                    P2.X += speed;
                    break;
            }

            return new Tuple<Point, Point>(P1, P2); ;
        }
    }
}
