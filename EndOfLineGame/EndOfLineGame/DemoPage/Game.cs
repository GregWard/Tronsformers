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
	public class Game
	{

        public enum Player : int
        {
            PlayerOne = 0,
            PlayerTwo = 1
        }
        protected const int borderBuffer = 10;
        public const int bikeSpeed = 2;

        protected List<TronBike> bikes;

        bool areCPUBikesActive;

		private Canvas canvas;


		private bool breakP;
		private bool started;
		private Thread moveThread, drawThread;
		private volatile bool killThreads;


		public Game(Canvas pageCanvas)
		{
			canvas = pageCanvas;

		    Rectangle border = new Rectangle();
			border.Width = canvas.Width;
			border.Height = canvas.Height;
			border.Stroke = Brushes.Black;
			border.StrokeThickness = TronBike.trailThickness;


            areCPUBikesActive = false;

			canvas.Children.Add(border);

			killThreads = false;
			breakP = false;
		}


		public void Start()
		{
			Point leftStart = new Point(canvas.Width / 5, canvas.Height / 2);
			Point rightStart = new Point(4 * canvas.Width / 5, canvas.Height / 2);

            InitializeBikes(new List<Point> { leftStart, rightStart } );

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

			drawThread = new Thread(new ThreadStart(Draw));
			drawThread.Start();

			Thread countdownThread = new Thread(new ThreadStart(CountDown));
			countdownThread.Start();

		}



        protected virtual void InitializeBikes(List<Point> startPoints)
        {
            // Clear the list.
            bikes = new List<TronBike>();

            //no worries, this is an ambi-turner
            bikes.Add(new TronBike(startPoints[0], bikeSpeed, Brushes.SteelBlue, Directions.Right));

            bikes.Add(new TronBike(startPoints[1], bikeSpeed, Brushes.Firebrick, Directions.Left));
        }



		private void MoveBikePics()
		{
            foreach (TronBike bike in bikes)
            {
                Canvas.SetTop(bike.BikeImage, bike.Position.Y - bike.BikeImage.ActualHeight / 2);
                Canvas.SetLeft(bike.BikeImage, bike.Position.X - bike.BikeImage.ActualWidth / 2);
            }
		}





		public void CountDown()
		{
			moveThread = new Thread(new ThreadStart(Move));
			moveThread.Start();
			started = true;
		}


		public void Restart()
		{
			if (started)
			{
				
				killThreads = true;
				foreach(TronBike bike in bikes)
                {
                    bike.Dead = true;
                }

				moveThread.Join();
				drawThread.Join();

				
				
				killThreads = false;
                
                foreach(TronBike bike in bikes)
                {
                    canvas.Children.Remove(bike.Path);
                    canvas.Children.Remove(bike.BikeImage);
                }

				Start();
			}
		}


		private void DrawBikes()
		{
			foreach(TronBike bike in bikes)
            {
                bike.Draw();
            }
			MoveBikePics();
		}



		private void MoveBikes()
		{
            foreach(TronBike bike in bikes)
            {
                if (!bike.Dead)
                {
                    bike.Move();
                }
            }


            CollisionCheck();

            // Code for CPU bikes.
            if (areCPUBikesActive)
            {
                if (DateTime.Now.Millisecond % 2 == 0)
                {
                    VisionCheck();
                }
                if (DateTime.Now.Millisecond % 10 == 0)
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


			if (breakP)
			{
				breakP = false;
			}
		}

		public void InvokeMove()
		{
			if (!canvas.CheckAccess())
			{
				canvas.Dispatcher.Invoke(() => MoveBikes());
			}
		}

		public void InvokeDraw()
		{
			if (!canvas.CheckAccess())
			{
				if (!killThreads)
				{
					canvas.Dispatcher.BeginInvoke(
						System.Windows.Threading.DispatcherPriority.Normal,
						new Action(() =>{
							DrawBikes();
						}));
				}
			}
		}


		private void Draw()
		{
			while (!killThreads)
			{
				
				InvokeDraw();

				Thread.Sleep(10);
			}
		}



		private void Move()
		{
			while (!killThreads)
			{
                InvokeMove();

				Thread.Sleep(10);
			}
		}





		private void CollisionCheck()
		{
			bool collided = false;
			List<PointCollection> tails = new List<PointCollection>(); ;

            foreach (TronBike bike in bikes)
            {
                tails.Add(bike.GetTailPoints());
            }



			foreach (TronBike bike in bikes)
			{
				collided = false;

				if (bike.Dead)
				{
                    continue;
				}

				Tuple<Point, Point> bikePos;

                bikePos = bike.GetCollisionRegion();

				foreach (PointCollection tail in tails)
				{
                    if (IsBikeInBounds(bikePos.Item1) && IsBikeInBounds(bikePos.Item2))
                    {
                        for (int i = tail.Count - 1; i > 0; i--)
                        {

                            Point p1 = tail[i];
                            Point p2 = tail[i - 1];

                            if (p1.X == p2.X)
                            {
                                //check against a vertical line
                                if (p1.X >= bikePos.Item1.X && p1.X <= bikePos.Item2.X)
                                {
                                    double min = Math.Min(p1.Y, p2.Y);
                                    double max = Math.Max(p1.Y, p2.Y);

                                    if (min <= bikePos.Item1.Y && max >= bikePos.Item1.Y)
                                    {
                                        collided = true;
                                        break;
                                    }
                                }
                            }
                            else if (p1.Y == p2.Y)
                            {
                                //check against a horizontal line
                                if (p1.Y >= bikePos.Item1.Y && p1.Y <= bikePos.Item2.Y)
                                {
                                    double min = Math.Min(p1.X, p2.X);
                                    double max = Math.Max(p1.X, p2.X);

                                    if (min <= bikePos.Item1.X && max >= bikePos.Item1.X)
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
                        bike.Dead = true;
                    }
				}

				if (collided)
				{
                    bike.Dead = true;
				}

			}

		}




        private void VisionCheck()
        {
			bool wallSeen = false;
			List<PointCollection> tails = new List<PointCollection>(); ;

            foreach (TronBike bike in bikes)
            {
                tails.Add(bike.GetTailPoints());
            }


			foreach (TronBike bike in bikes)
			{
				wallSeen = false;

				if (bike.Dead)
				{
                    continue;
				}

				Tuple<Point, Point> visionRange;

                visionRange = ((CPUBike)bike).GetSightCheckRegion();

				foreach (PointCollection tail in tails)
				{
					if (IsBikeInBounds(visionRange.Item1) && IsBikeInBounds(visionRange.Item2))
					{
						for (int i = tail.Count - 1; i > 0; i--)
						{

							Point p1 = tail[i];
							Point p2 = tail[i - 1];

							if (p1.X == p2.X)
							{
								//check against a vertical line
								if (p1.X >= visionRange.Item1.X && p1.X <= visionRange.Item2.X)
								{
									double min = Math.Min(p1.Y, p2.Y);
									double max = Math.Max(p1.Y, p2.Y);

									if (min <= visionRange.Item1.Y && max >= visionRange.Item1.Y)
									{
										wallSeen = true;
										break;
									}
								}
							}
							else if (p1.Y == p2.Y)
							{
								//check against a horizontal line
								if (p1.Y >= visionRange.Item1.Y && p1.Y <= visionRange.Item2.Y)
								{
									double min = Math.Min(p1.X, p2.X);
									double max = Math.Max(p1.X, p2.X);

									if (min <= visionRange.Item1.X && max >= visionRange.Item1.X)
									{
										wallSeen = true;
										break;
									}
								}
							}
						}
					}
					else
					{
						wallSeen = true;
					}
				}


                if (wallSeen)
                {

                    ((CPUBike)bike).ReactToWall(canvas.Width, canvas.Height);
                }
			}
        }
        


		

		private bool IsBikeInBounds(Point position)
		{
			bool inBounds = true;
			Size canvasSize = canvas.DesiredSize;

			if (position.X <= 0 + borderBuffer || position.X >= canvasSize.Width - borderBuffer)
			{
				inBounds = false;
			}
			else if (position.Y <= 0 + borderBuffer || position.Y >= canvasSize.Height - borderBuffer)
			{
				inBounds = false;
			}


			return inBounds;
		}


		

		

		internal void End()
		{
			killThreads = true;

            foreach (TronBike bike in bikes)
            {
                bike.Dead = true;
            }

			drawThread.Join();
			moveThread.Join();
		}




		internal void TurnLeft(Player player)
		{
            switch (player)
            {
                case Player.PlayerOne:
                    bikes[0].TurnLeft();
                    break;
                case Player.PlayerTwo:
                    bikes[1].TurnLeft();
                    break;
                default:
                    break;
            }
		}

		internal void TurnRight(Player player)
        {
			if (bikes.Count > (int)player)
			{
				bikes[(int)player].TurnRight();
			}
		}
	}
}
