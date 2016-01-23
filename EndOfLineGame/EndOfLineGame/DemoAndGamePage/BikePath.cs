/**
 * \file		BikePath.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		Defines a path left behind by a bike.
 * \details		Defines the creation, maintenance, and retrieval of the
 *              trail of lines left by a bike in the game.
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Threading;



namespace EndOfLineGame
{
    /// <summary>
    /// The trail path that follows a Bike.
    /// </summary>
    class BikePath
    {
        /// <summary>
        /// The points at which the bike has turned (the corners of the path.)
        /// </summary>
        private List<Point> turnPoints;
        /// <summary>
        /// A mutex to protect the list of points in the path.
        /// </summary>
		private Mutex mut;


        /// <summary>
        /// The constructor for the BikePath.
        /// </summary>
        /// <param name="startPoint">The start point of the path.</param>
        public BikePath(Point startPoint)
        {
            turnPoints = new List<Point>();

            turnPoints.Add(startPoint);
            turnPoints.Add(startPoint);

            mut = new Mutex();
        }





        /// <summary>
        /// Moves the "head" segment of the trail (the one
        /// that the bike is attached to.)
        /// </summary>
        /// <param name="newPos">The new position of the leading
        /// edge of the trail.</param>
        public void Move(Point newPos)
        {
            mut.WaitOne();
            turnPoints[turnPoints.Count - 1] = newPos;
            mut.ReleaseMutex();
        }





        /// <summary>
        /// Creates a turn in the trail, starting a new trail segment and
        /// setting the last one as constant.
        /// </summary>
        /// <param name="newPos">The new location of the new leading edge
        /// of the trail.</param>
        public void Turn(Point newPos)
        {
            mut.WaitOne();
            turnPoints[turnPoints.Count - 1] = newPos;
            turnPoints.Add(newPos);
            mut.ReleaseMutex();
        }





        /// <summary>
        /// Gets the points that make up this trail.
        /// </summary>
        /// <returns>The collection of points that make up this trail.</returns>
		public PointCollection GetPoints()
        {
            PointCollection drawPoints = new PointCollection();

            mut.WaitOne();
            foreach (Point p in turnPoints)
            {
                drawPoints.Add(p);
            }
            mut.ReleaseMutex();

            return drawPoints;
        }
    }
}
