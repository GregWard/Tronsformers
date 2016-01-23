using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;


namespace TestUI
{
    class BikePath
    {
        private List<Point> turnPoints;
		private Mutex mut;
		//private Point currentHead;

        public BikePath(Point startPoint)
        {
            turnPoints = new List<Point>();

            turnPoints.Add(startPoint);
            turnPoints.Add(startPoint);

			mut = new Mutex();
        }


        public void Move(Point newPos)
        {
			mut.WaitOne();
            turnPoints[turnPoints.Count - 1] = newPos;
			mut.ReleaseMutex();
        }

        public void Turn(Point newPos)
        {
			mut.WaitOne();
            turnPoints[turnPoints.Count - 1] = newPos;
            turnPoints.Add(newPos);
			mut.ReleaseMutex();
        }

		public PointCollection GetPoints()
		{
			PointCollection drawPoints = new PointCollection();

			mut.WaitOne();
			foreach(Point p in turnPoints)
			{
				drawPoints.Add(p);
			}
			mut.ReleaseMutex();

			return drawPoints;
		}
    }
}
