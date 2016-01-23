using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestUI
{
    public class CPUBike : TronBike
    {
        const int sightRange = 10;
        static Random decisionGenerator = new Random();
        public const int DangerZone = 20;


        public CPUBike(Point pos, int speed, Directions startDir)
            : base(pos, speed, startDir)
        {

        }

        public CPUBike(Point pos, int speed, Brush colour, Directions startDir)
            : base(pos, speed, colour, startDir)
        {

        }


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


        public void AutoPilot(double xBound, double yBound)
        {
            int decision = decisionGenerator.Next(0, 100);

            bool leftBanned = false;
            bool rightBanned = false;

            CheckBounds(xBound, yBound, out rightBanned, out leftBanned);

            if (decision < 5 && !leftBanned)
            {
                TurnLeft();
            }
            else if (decision >= 95 && !rightBanned)
            {
                TurnRight();
            }
        }


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
