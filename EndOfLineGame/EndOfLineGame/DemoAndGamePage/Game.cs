/**
 * \file		Game.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The basic description of a Game of Tronsformers.
 * \details		Describes the gameplay functionality of a game, including
 *              initializing the bikes, making them move, and detecting collisions.
 */



using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;



namespace EndOfLineGame
{
    /// <summary>
    /// A game of Tronsformers.
    /// </summary>
	public class Game
    {
        /// <summary>
        /// A zone around the edges of the screen, to prevent the appearance
        /// of bike sprites crossing over the border.
        /// </summary>
        protected const int borderBuffer = 10;
        /// <summary>
        /// The speed (in pixels/frame) that bikes move at.
        /// </summary>
        public const int bikeSpeed = 2;

        /// <summary>
        /// A list of bikes involved in the game.
        /// </summary>
        protected List<TronBike> bikes;

        /// <summary>
        /// Whether there are any CPU controlled bikes.
        /// </summary>
        bool areCPUBikesActive;

        /// <summary>
        /// The canvas to draw to.
        /// </summary>
		private Canvas canvas;

        /// <summary>
        /// Whether the game has started or not.
        /// </summary>
		private bool started;
        /// <summary>
        /// The thread responsible for moving the bikes.
        /// </summary>
        private Thread moveThread;
        /// <summary>
        /// The thread responsible for drawing game objects.
        /// </summary>
        private Thread drawThread;
        /// <summary>
        /// A flag indicating that, when true, indicates that all threads
        /// should be stopped ASAP.
        /// </summary>
		private volatile bool killThreads;

        /// <summary>
        /// The event handler delegate for the game ending.
        /// </summary>
        public event EventHandler GameOver;

        /// <summary>
        /// The list of players
        /// </summary>
        private List<Player> thePlayers;

        /// <summary>
        /// The constructor for a Game.
        /// </summary>
        /// <param name="pageCanvas">The canvas to draw to.</param>
        public Game(Canvas pageCanvas, List<Player> players)
        {
            canvas = pageCanvas;

            Rectangle border = new Rectangle();
            border.Width = canvas.Width;
            border.Height = canvas.Height;
            border.Stroke = Brushes.Black;
            border.StrokeThickness = TronBike.trailThickness;

            thePlayers = players;

            areCPUBikesActive = false;

            canvas.Children.Add(border);

            killThreads = false;
        }





        /// <summary>
        /// Starts a round of Tronsformers!!
        /// </summary>
		public void Start()
        {
            Point leftStart = new Point(canvas.Width / 5, canvas.Height / 2);
            Point rightStart = new Point(4 * canvas.Width / 5, canvas.Height / 2);

            Point topStart = new Point(canvas.Width / 2, canvas.Height / 5);
            Point bottomStart = new Point(canvas.Width / 2, 4 * canvas.Height / 5);

            Point topLeftStart = new Point(canvas.Width / 5, canvas.Height / 5);
            Point topRightStart = new Point(4 * canvas.Width / 5, canvas.Height / 5);
            Point bottomLeftStart = new Point(canvas.Width / 5, 4 * canvas.Height / 5);
            Point bottomRightStart = new Point(4 * canvas.Width / 5, 4 * canvas.Height / 5);

            InitializeBikes(new List<Point> { leftStart, rightStart, topStart, bottomStart,
                topLeftStart, topRightStart, bottomLeftStart, bottomRightStart });

            foreach (TronBike bike in bikes)
            {
                canvas.Children.Add(bike.Path);

                if (bike is CPUBike)
                {
                    areCPUBikesActive = true;
                }
            }

            foreach (TronBike bike in bikes)
            {
                canvas.Children.Add(bike.BikeImage);
            }

            MoveBikePics();

            if (drawThread == null || drawThread.IsAlive == false)
            {
                drawThread = new Thread(new ThreadStart(Draw));
                drawThread.Name = "Draw Thread";
                drawThread.Start();
            }



            if (moveThread == null || moveThread.IsAlive == false)
            {
                moveThread = new Thread(new ThreadStart(Move));
                moveThread.Name = "Move Thread";
                moveThread.Start();
            }

            started = true;

        }





        /// <summary>
        /// Creates two bikes and places them facing each other, ready to start the game.
        /// </summary>
        /// <param name="startPoints">The points to start the bikes at.</param>
        protected virtual void InitializeBikes(List<Point> startPoints)
        {
            // Clear the list.
            bikes = new List<TronBike>();

            //no worries, this is an ambi-turner

            if (thePlayers.Count == 0)
            {
                bikes.Add(new CPUBike(startPoints[0], bikeSpeed, Brushes.IndianRed, Directions.Right));
            }
            else
            {
                bikes.Add(new TronBike(startPoints[0], bikeSpeed, thePlayers[0].PlayerColour, Directions.Right));
                thePlayers[0].bike = bikes[0];
            }
            

            if (thePlayers.Count > 1)
            {
                bikes.Add(new TronBike(startPoints[1], bikeSpeed, thePlayers[1].PlayerColour, Directions.Left));
                thePlayers[1].bike = bikes[1];
            }
            else
            {
                bikes.Add(new CPUBike(startPoints[1], bikeSpeed, Brushes.NavajoWhite, Directions.Left));
            }
        }





        /// <summary>
        /// Moves the sprites for the bikes to their current location.
        /// </summary>
		private void MoveBikePics()
        {
            foreach (TronBike bike in bikes)
            {
                Canvas.SetTop(bike.BikeImage, bike.Position.Y - bike.BikeImage.ActualHeight / 2);
                Canvas.SetLeft(bike.BikeImage, bike.Position.X - bike.BikeImage.ActualWidth / 2);
            }
        }





        /// <summary>
        /// Resets the game to its starting conditions.
        /// </summary>
		public void Restart()
        {
            if (started)
            {

                killThreads = true;
                foreach (TronBike bike in bikes)
                {
                    bike.Dead = true;
                }

                moveThread.Join();
                drawThread.Join();



                killThreads = false;

                foreach (TronBike bike in bikes)
                {
                    canvas.Children.Remove(bike.Path);
                    canvas.Children.Remove(bike.BikeImage);
                }

                Start();
            }
        }





        /// <summary>
        /// Draws all bikes in the game.
        /// </summary>
		private void DrawBikes()
        {
            foreach (TronBike bike in bikes)
            {
                bike.Draw();
            }
            MoveBikePics();
        }





        /// <summary>
        /// Moves all of the bikes in the game.
        /// </summary>
		private void MoveBikes()
        {
            int deadBikes = 0;

            foreach (TronBike bike in bikes)
            {
                if (!bike.Dead)
                {
                    bike.Move();
                }
                else
                {
                    deadBikes++;
                }
            }


            if (deadBikes != bikes.Count)
            {

                CollisionCheck();

                // Code for CPU bikes.
                if (areCPUBikesActive)
                {
                    if (DateTime.Now.Millisecond % 2 == 0)
                    {
                        VisionCheck();
                    }
                    if (DateTime.Now.Millisecond % 5 == 0)
                    {
                        foreach (TronBike bike in bikes)
                        {
                            if (bike is CPUBike)
                            {
                                ((CPUBike)bike).AutoPilot(canvas.Width, canvas.Height);
                            }
                        }
                    }
                }
            }
            else
            {
                if (GameOver != null)
                {
                    GameOver(this, new EventArgs());
                }
            }
        }





        /// <summary>
        /// Invokes MoveBikes() on the canvas' thread, so that the bikes
        /// can be drawn.
        /// </summary>
		public void InvokeMove()
        {
            if (!canvas.CheckAccess())
            {
                if (!killThreads)
                {
                    canvas.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() => {
                            MoveBikes();
                        }));
                }
            }
        }





        /// <summary>
        /// Invokes DrawBikes() on the canvas' thread, so they can be properly
        /// moved and have their collision checked.
        /// </summary>
		public void InvokeDraw()
        {
            if (!canvas.CheckAccess())
            {
                if (!killThreads)
                {
                    canvas.Dispatcher.BeginInvoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(() => {
                            DrawBikes();
                        }));
                }
            }
        }





        /// <summary>
        /// The function used in the draw thread, which repeatedly draws the game objects.
        /// </summary>
		private void Draw()
        {
            while (!killThreads)
            {

                InvokeDraw();

                Thread.Sleep(10);
            }
        }





        /// <summary>
        /// The function used in the move thread, which repeatedly moves the
        /// bikes.
        /// </summary>
		private void Move()
        {
            while (!killThreads)
            {
                InvokeMove();

                Thread.Sleep(10);
            }
        }





        /// <summary>
        /// Checks each bike's position against the positions of the bike trails,
        /// and triggers a collision if appropriate.
        /// </summary>
		private void CollisionCheck()
        {
            bool collided = false;
            List<PointCollection> tails = new List<PointCollection>(); ;

            foreach (TronBike bike in bikes)
            {
                tails.Add(bike.GetTailPoints());
            }

            int bikeNum = 0;

            foreach (TronBike bike in bikes)
            {
                collided = false;

                if (bike.Dead)
                {
                    bikeNum++;
                    continue;
                }

                Tuple<Point, Point> bikePos;

                bikePos = bike.GetCollisionRegion();

                collided = CheckCollision(tails, bikePos, bikeNum++);

                if (collided)
                {
                    bike.Dead = true;
                }

            }

        }


        private bool CheckCollision(List<PointCollection> tails, Tuple<Point, Point> checkRange, int bikeNum)
        {
            bool collided = false;
            int tailNum = 0;

            foreach (PointCollection tail in tails)
            {
                if (IsBikeInBounds(checkRange.Item1) && IsBikeInBounds(checkRange.Item2))
                {
                    for (int i = tail.Count - 1; i > 0; i--)
                    {
                        Point p1 = tail[i];
                        Point p2 = tail[i - 1];

                        if (i >= tail.Count - 2 && bikeNum == tailNum)
                        {
                            continue;
                        }

                        if (p1.X == p2.X)
                        {
                            //vertical line
                            if (p1.X + TronBike.trailThickness / 2 >= checkRange.Item1.X &&
                                p1.X - TronBike.trailThickness / 2 <= checkRange.Item2.X)
                            {
                                double min = Math.Min(p1.Y, p2.Y) - TronBike.trailThickness;
                                double max = Math.Max(p1.Y, p2.Y) + TronBike.trailThickness;

                                if (min <= checkRange.Item1.Y && max >= checkRange.Item1.Y)
                                {
                                    collided = true;
                                    break;
                                }
                            }

                        }
                        else if (p1.Y == p2.Y)
                        {
                            //check against a horizontal line
                            if (p1.Y + TronBike.trailThickness / 2 >= checkRange.Item1.Y &&
                                p1.Y - TronBike.trailThickness / 2 <= checkRange.Item2.Y)
                            {
                                double min = Math.Min(p1.X, p2.X) - TronBike.trailThickness;
                                double max = Math.Max(p1.X, p2.X) + TronBike.trailThickness;

                                if (min <= checkRange.Item1.X && max >= checkRange.Item1.X)
                                {
                                    collided = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    collided = true;
                }

                tailNum++;
            }

            return collided;
        }


        /// <summary>
        /// Performs a vision check for all CPU-controlled bikes, checking for trails
        /// or walls in front of them and calling their ReactToWall() function when necessary.
        /// </summary>
        private void VisionCheck()
        {
            bool wallSeen = false;
            List<PointCollection> tails = new List<PointCollection>(); ;

            foreach (TronBike bike in bikes)
            {
                tails.Add(bike.GetTailPoints());
            }

            int bikeNum = 0;

            foreach (TronBike bike in bikes)
            {
                if (bike is CPUBike)
                {
                    wallSeen = false;

                    if (bike.Dead)
                    {
                        bikeNum++;
                        continue;
                    }

                    Tuple<Point, Point> visionRange;

                    visionRange = ((CPUBike)bike).GetSightCheckRegion();

                    wallSeen = CheckCollision(tails, visionRange, bikeNum++);

                    if (wallSeen)
                    {

                        ((CPUBike)bike).ReactToWall(canvas.Width, canvas.Height);
                    }
                }
            }
        }





        /// <summary>
        /// Checks whether a bike is (or will be) within the bounds of the arena.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the bike is within allowable bounds, false otherwise.</returns>
        private bool IsBikeInBounds(Point position)
        {
            bool inBounds = true;
            Size canvasSize = canvas.DesiredSize;

            if (position.X <= 0 + borderBuffer || position.X >= canvasSize.Width - 2 * borderBuffer)
            {
                inBounds = false;
            }
            else if (position.Y <= 0 + borderBuffer || position.Y >= canvasSize.Height - 2 * borderBuffer)
            {
                inBounds = false;
            }


            return inBounds;
        }





        /// <summary>
        /// Ends the game, stopping all side threads.
        /// </summary>
        internal void End()
        {
            killThreads = true;

            if (bikes != null)
            {
                foreach (TronBike bike in bikes)
                {
                    bike.Dead = true;
                }
            }

            while (drawThread != null && drawThread.IsAlive)
            {
                drawThread.Join();
            }
            while (moveThread != null && moveThread.IsAlive)
            {
                moveThread.Join();
            }
        }





        /// <summary>
        /// Turns a given player bike left.
        /// </summary>
        /// <param name="player">The player - PlayerOne or PlayerTwo - to turn.</param>
		internal void TurnLeft(Players player)
        {
            switch (player)
            {
                case Players.PlayerOne:
                    bikes[0].TurnLeft();
                    break;
                case Players.PlayerTwo:
                    bikes[1].TurnLeft();
                    break;
                default:
                    break;
            }
        }





        /// <summary>
        /// Turns a given player bike right.
        /// </summary>
        /// <param name="player">The player - PlayerOne or PlayerTwo - to turn.</param>
		internal void TurnRight(Players player)
        {
            if (bikes.Count > (int)player)
            {
                bikes[(int)player].TurnRight();
            }
        }
    }
}
