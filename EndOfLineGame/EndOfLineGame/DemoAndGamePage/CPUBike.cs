/**
 * \file		CPUBike.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		Defines a computer controlled bike.
 * \details		Outlines the behaviours of an automated bike for the Tronsformers game.
 *              Automated bikes intermittently make random turns, but also dodge fairly
 *              effectively around walls and trails.
 */




using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace EndOfLineGame
{

    /// <summary>
    /// A computer-controlled TronBike.
    /// </summary>
    public class CPUBike : TronBike
    {
        /// <summary>
        /// How far in front of a bike that the CPU looks and
        /// responds to walls.
        /// </summary>
        const int sightRange = 10;

        /// <summary>
        /// A random number generator used for decision-making.
        /// </summary>
        static Random decisionGenerator = null;// = new Random();
        /// <summary>
        /// A buffer around the borders of the screen where the bike will
        /// not turn towards the border (reducing the number of early collisions with
        /// the wall borders.)
        /// </summary>
        public const int DangerZone = 20;


        /// <summary>
        /// The percentage probability of turning at any given time.
        /// </summary>
        private const int chanceToTurn = 10;



        /// <summary>
        /// A constructor for a CPU bike (using the default colour.)
        /// </summary>
        /// <param name="pos">The position at which the bike starts.</param>
        /// <param name="speed">The speed of the bike.</param>
        /// <param name="startDir">The starting direction of the bike.</param>
        public CPUBike(Point pos, int speed, Directions startDir)
            : base(pos, speed, startDir)
        {
            if (decisionGenerator == null)
            {
                decisionGenerator = new Random();
            }
        }





        /// <summary>
        /// A constructor for a CPU bike.
        /// </summary>
        /// <param name="pos">The position at which the bike starts.</param>
        /// <param name="speed">The speed of the bike.</param>
        /// <param name="colour">The colour of the bike and its trail.</param>
        /// <param name="startDir">The starting direction of the bike.</param>>
        public CPUBike(Point pos, int speed, Brush colour, Directions startDir)
            : base(pos, speed, colour, startDir)
        {
            if (decisionGenerator == null)
            {
                decisionGenerator = new Random();
            }
        }





        /// <summary>
        /// Gets a collision box for the bike's "line of sight", the range at which
        /// trails in front of the bike are detected.
        /// </summary>
        /// <returns>The points that make up the vision range.</returns>
        public Tuple<Point, Point> GetSightCheckRegion()
        {
            Point P1 = position;
            Point P2 = position;
            double offset = collisionSensitivity;

            switch (direction)
            {
                case Directions.Up:
                    P1.Y -= speed + sightRange;
                    P2.Y -= offset;
                    break;
                case Directions.Left:
                    P1.X -= speed + sightRange;
                    P2.X -= offset;
                    break;
                case Directions.Down:
                    P1.Y += offset;
                    P2.Y += speed + sightRange;
                    break;
                case Directions.Right:
                    P1.X += offset;
                    P2.X += speed + sightRange;
                    break;
            }

            return new Tuple<Point, Point>(P1, P2);
        }





        /// <summary>
        /// Processes the CPUBike's reaction to seeing a wall or trail -
        /// predominantly, turning away from it, unless the bike is too close to the
        /// borders of the game space.
        /// </summary>
        /// <param name="xBound">The x limit of the game board.</param>
        /// <param name="yBound">The y limit of the game board.</param>
        public void ReactToWall(double xBound, double yBound)
        {
            bool leftBanned = false;
            bool rightBanned = false;

            CheckBounds(xBound, yBound, out rightBanned, out leftBanned);

            if (decisionGenerator.Next(100) >= 50 && !leftBanned)
            {
                TurnLeft();
            }
            else if (!rightBanned)
            {
                TurnRight();
            }
        }





        /// <summary>
        /// Creates randomized behaviour in the CPUBikes, turning in a random
        /// direction at random times (unless a border is too close.)
        /// </summary>
        /// <param name="xBound">The x limit of the game board.</param>
        /// <param name="yBound">The y limit of the game board.</param>
        public void AutoPilot(double xBound, double yBound)
        {
            int decision = decisionGenerator.Next(0, 100);

            bool leftBanned = false;
            bool rightBanned = false;

            CheckBounds(xBound, yBound, out rightBanned, out leftBanned);

            if (decision < (chanceToTurn / 2) && !leftBanned)
            {
                TurnLeft();
            }
            else if (decision >= (100 - chanceToTurn / 2) && !rightBanned)
            {
                TurnRight();
            }
        }





        /// <summary>
        /// A function that checks whether a CPUBike is in the "DangerZone", where they
        /// are too close to the borders of the screen and should not turn towards the wall.
        /// </summary>
        /// <param name="xBound">The x limit of the game board.</param>
        /// <param name="yBound">The y limit of the game board.</param>
        /// <param name="rightBanned">Whether turning right should be banned.</param>
        /// <param name="leftBanned">Whether turning left should be banned.</param>
        private void CheckBounds(double xBound, double yBound, out bool rightBanned, out bool leftBanned)
        {
            rightBanned = false;
            leftBanned = false;

            if (position.X < DangerZone)
            {
                if (direction == Directions.Up)
                {
                    leftBanned = true;
                }
                else if (direction == Directions.Down)
                {
                    rightBanned = true;
                }
            }
            else if (position.X > xBound - DangerZone)
            {
                if (direction == Directions.Up)
                {
                    rightBanned = true;
                }
                else if (direction == Directions.Down)
                {
                    leftBanned = true;
                }
            }

            if (position.Y > yBound - DangerZone)
            {
                if (direction == Directions.Left)
                {
                    leftBanned = true;
                }
                else if (direction == Directions.Right)
                {
                    rightBanned = true;
                }
            }
            else if (position.Y < DangerZone)
            {
                if (direction == Directions.Left)
                {
                    rightBanned = true;
                }
                else if (direction == Directions.Right)
                {
                    leftBanned = true;
                }
            }
        }
    }
}
