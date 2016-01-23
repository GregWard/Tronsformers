/**
 * \file		TronBike.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The definition of a bike.
 * \details		Defines the behaviours and controls of a bike for the Tronsformers game.
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
    /// Enumerates the four cardinal directions that a bike can travel in.
    /// </summary>
	public enum Directions { Up, Down, Left, Right };



    /// <summary>
    /// A bike for use in Tronsformers.
    /// </summary>
    public class TronBike
    {
        /// <summary>
        /// Offsets collision detection, so a bike doesn't collide with itself.
        /// </summary>
        protected const double collisionSensitivity = 0.001;
        /// <summary>
        /// The thickness of bike trails (and the outer walls of the map.)
        /// </summary>
        public const int trailThickness = 10;

        /// <summary>
        /// The position of the bike.
        /// </summary>
        protected Point position;
        /// <summary>
        /// The speed of the bike.
        /// </summary>
        protected int speed;
        /// <summary>
        /// The trail of lines that the bike has left.
        /// </summary>
        private BikePath tail;
        /// <summary>
        /// The direction the bike is facing.
        /// </summary>
        protected Directions direction;
        /// <summary>
        /// Whether the bike is dead or not.
        /// </summary>
        private bool dead;
        /// <summary>
        /// The colour of the bike and its trail.
        /// </summary>
		private Brush bikeColour;
        /// <summary>
        /// The path representing the bike and its trail.
        /// </summary>
		private Path myPath;
        /// <summary>
        /// The starting point of the bike path.
        /// </summary>
		private Point startingPoint;
        /// <summary>
        /// The re-colourable sprite used when the bike is travelling vertically.
        /// </summary>
		private WriteableBitmap verticalSprite;
        /// <summary>
        /// The re-colourable sprite used when the bike is travelling horizontally.
        /// </summary>
		private WriteableBitmap horizontalSprite;
        /// <summary>
        /// The current image of the bike.
        /// </summary>
		private Image bike;


        /// <summary>
        /// The cooldown between when a bike can turn and the number of frames
        /// before it can next turn.
        /// </summary>
        private int turnCooldown = 0;



        /// <summary>
        /// A constructor for a TronBike, using the default colour.
        /// </summary>
        /// <param name="pos">The position to create the bike at.</param>
        /// <param name="speed">The speed of the bike.</param>
        /// <param name="startDir">The starting direction of the bike.</param>
		public TronBike(Point pos, int speed, Directions startDir)
			: this(pos, speed, Brushes.Black, startDir)
		{

		}





        /// <summary>
        /// A constructor for a TronBike.
        /// </summary>
        /// <param name="pos">The position to start the bike at.</param>
        /// <param name="speed">The speed of the bike.</param>
        /// <param name="colour">The colour of the bike and its trail.</param>
        /// <param name="startDir">The starting direction of the bike.</param>
		public TronBike(Point pos, int speed, Brush colour, Directions startDir)
		{
			bikeColour = colour;
			position = pos;
			this.speed = speed;
			tail = new BikePath(position);
			direction = startDir;
			dead = false;
			startingPoint = pos;

			myPath = new Path();
			myPath.Stroke = colour;
			myPath.StrokeThickness = trailThickness;

			bike = new Image();
			ColourImages();
			SetBikeImg();
		}





        /// <summary>
        /// Recolours the bike's sprites to match its selected colour.
        /// </summary>
		private void ColourImages()
		{
			BitmapImage vertical = new BitmapImage();
			vertical.BeginInit();
			vertical.UriSource = new Uri(@"images\BikeVertical.png", UriKind.Relative);
			vertical.EndInit();

			verticalSprite= new WriteableBitmap(vertical);
			byte[] vData = verticalSprite.ToByteArray();


			for (int i = 0; i < vData.Length; i= i + 4 )
			{

				if (vData[i] == 255 && vData[i + 1] == 255 && vData[i + 2] == 255)
				{
					vData[i] = ((SolidColorBrush)bikeColour).Color.B;
					vData[i + 1] = ((SolidColorBrush)bikeColour).Color.G;
					vData[i + 2] = ((SolidColorBrush)bikeColour).Color.R;
				}
				else if (vData[i] == 0xd4 && vData[i + 1] == 0xd4 && vData[i + 2] == 0xd4)
				{
					double scale = ((double)0xd4) / 0xFF;
					vData[i] = (byte)(((SolidColorBrush)bikeColour).Color.B * scale);
					vData[i + 1] = (byte)(((SolidColorBrush)bikeColour).Color.G * scale);
					vData[i + 2] = (byte)(((SolidColorBrush)bikeColour).Color.R * scale);
				}
			}

			verticalSprite.FromByteArray(vData);



			BitmapImage horizontal = new BitmapImage();
			horizontal.BeginInit();
			horizontal.UriSource = new Uri(@"images\BikeHorizontal.png", UriKind.Relative);
			horizontal.EndInit();


			horizontalSprite = new WriteableBitmap(horizontal);
			byte[] hData = horizontalSprite.ToByteArray();


			for (int i = 0; i < hData.Length; i = i + 4)
			{

				if (hData[i] == 0xFF && hData[i + 1] == 0xFF && hData[i + 2] == 0xFF)
				{
					hData[i] = ((SolidColorBrush)bikeColour).Color.B;
					hData[i + 1] = ((SolidColorBrush)bikeColour).Color.G;
					hData[i + 2] = ((SolidColorBrush)bikeColour).Color.R;
				}
				else if (hData[i] == 0xd4 && hData[i + 1] == 0xd4 && hData[i + 2] == 0xd4)
				{
					double scale = ((double)0xd4) / 0xFF;
					hData[i] = (byte)(((SolidColorBrush)bikeColour).Color.B*scale);
					hData[i + 1] = (byte)(((SolidColorBrush)bikeColour).Color.G * scale);
					hData[i + 2] = (byte)(((SolidColorBrush)bikeColour).Color.R * scale);
				}
			}

			horizontalSprite.FromByteArray(hData);

		}





        /// <summary>
        /// Sets the bike image to match its current direction of travel.
        /// </summary>
		private void SetBikeImg()
		{
			BitmapImage bikeBmp = new BitmapImage();
			

			
			if (direction == Directions.Down)
			{
				bike.Source = new TransformedBitmap(verticalSprite, new ScaleTransform(1, -1));
			}
			else if (direction == Directions.Up)
			{
				bike.Source = verticalSprite;
			}
			else if (direction == Directions.Left)
			{
				bike.Source = horizontalSprite;
			}
			else if (direction == Directions.Right)
			{
				bike.Source = new TransformedBitmap(horizontalSprite,new ScaleTransform(-1,1));
			}
		}





        /// <summary>
        /// The bike's image.
        /// </summary>
		public Image BikeImage
		{
			get
			{
				return bike;
			}
		}





        /// <summary>
        /// Whether the bike is dead or not.
        /// </summary>
		public bool Dead
		{
			get
			{
				return dead;
			}
			set
			{
				dead = true;
			}
		}





        /// <summary>
        /// The position of the bike.
        /// </summary>
        public Point Position
        {
            get
            {
                return position;
            }
        }





        /// <summary>
        /// The colour of the bike and its trail.
        /// </summary>
		public Brush Colour
		{
			get
			{
				return bikeColour;
			}
			set
			{
				bikeColour = value;
			}
		}





        /// <summary>
        /// Moves the bike forward in its current direction, adjusting its
        /// trail to follow.
        /// </summary>
        public void Move()
        {
            if (!dead)
            {
                if (direction == Directions.Down)
                {
                    position.Y += speed;
                }
                else if (direction == Directions.Up)
                {
                    position.Y -= speed;
                }
                else if (direction == Directions.Left)
                {
                    position.X -= speed;
                }
                else if (direction == Directions.Right)
                {
                    position.X += speed;
                }


                if (turnCooldown > 0)
                {
                    turnCooldown -= speed;
                }

                tail.Move(position);
            }
        }





        /// <summary>
        /// Turns the bike to the left.
        /// </summary>
        public void TurnLeft()
        {
			if (!dead && turnCooldown <= 0)
			{
				switch (direction)
				{
					case Directions.Up:
						direction = Directions.Left;
						break;
					case Directions.Left:
						direction = Directions.Down;
						break;
					case Directions.Down:
						direction = Directions.Right;
						break;
					case Directions.Right:
						direction = Directions.Up;
						break;
				}

                /* The turn cooldown prevents further left turns until this bike is
                clear of its own path.*/
                turnCooldown = trailThickness * 4;

				SetBikeImg();
				tail.Turn(position);
			}
        }





        /// <summary>
        /// Turns the bike to the right.
        /// </summary>
		public void TurnRight()
		{
			if (!dead && turnCooldown <= 0)
			{
				switch (direction)
				{
					case Directions.Up:
						direction = Directions.Right;
						break;
					case Directions.Left:
						direction = Directions.Up;
						break;
					case Directions.Down:
						direction = Directions.Left;
						break;
					case Directions.Right:
						direction = Directions.Down;
						break;
                }

                /* The turn cooldown prevents further right turns until this bike is
                clear of its own path.*/
                turnCooldown = trailThickness * 4;

				SetBikeImg();
				tail.Turn(position);
			}
		}





        /// <summary>
        /// Gets the collision detection box for the bike.
        /// </summary>
        /// <returns>The two opposite corners of the bike.</returns>
		public Tuple<Point, Point> GetCollisionRegion()
		{
			Point P1 = position;
			Point P2 = position;
			double offset = collisionSensitivity;

			switch (direction)
			{
				case Directions.Up:
					P1.Y -= speed;
					P2.Y -= offset;
					break;
                case Directions.Left:
					P1.X -=  speed;
					P2.X -= offset;
					break;
                case Directions.Down:
					P1.Y += offset;
					P2.Y +=  speed;
					break;
                case Directions.Right:
					P1.X += offset;
					P2.X += speed;
					break;
			}

			return new Tuple<Point, Point>(P1, P2); ;
		}





        /// <summary>
        /// Gets all points left behind by the bike.
        /// </summary>
        /// <returns>A collection of points representing the trail of the
        /// bike.</returns>
		public PointCollection GetTailPoints()
		{
			return tail.GetPoints();
		}





        /// <summary>
        /// The path left by the bike.
        /// </summary>
		public Path Path
		{
			get
			{
				return myPath;
			}
		}





        /// <summary>
        /// The path representing the bike and its trail.
        /// </summary>
		public void Draw()
		{
			PathFigure path = new PathFigure();
			PathFigureCollection pathColl = new PathFigureCollection();
			PolyLineSegment points = new PolyLineSegment();
			PathSegmentCollection pathSeg = new PathSegmentCollection();
			PathGeometry pathGeo = new PathGeometry();
			Pen thinPen = new Pen(Brushes.Transparent, 0.0001);


			points.Points = tail.GetPoints();
			path.StartPoint = startingPoint;
			pathSeg.Add(points);

			path.StartPoint = points.Points.ElementAt(0);
			path.Segments = pathSeg;
			pathColl.Add(path);
			pathGeo.Figures = pathColl;

			
			myPath.Data = pathGeo;

		}
    }
}
