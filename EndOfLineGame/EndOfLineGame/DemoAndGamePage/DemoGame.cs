/**
 * \file		DemoGame.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The basic description of a Demo of Tronsformers.
 * \details		Describes the gameplay functionality of a demo game, adding functionality
 *              for adding bikes to demonstrate the basic gameplay of Tronsformers.
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
    /// A demonstration of Tronsformers.
    /// </summary>
    public class DemoGame : Game
    {
        /// <summary>
        /// The constructor for a demo game.
        /// </summary>
        /// <param name="pageCanvas">The canvas to draw the canvas upon.</param>
        public DemoGame(Canvas pageCanvas)
            : base(pageCanvas, null)
        {

        }





        /// <summary>
        /// Sets up a pair of demo bikes to move randomly.
        /// </summary>
        /// <param name="startPoints"></param>
        protected override void InitializeBikes(List<Point> startPoints)
        {
            // Clear the list.
            bikes = new List<TronBike>();

            List<Brush> colours = new List<Brush>();

            colours.Add(Brushes.SteelBlue);
            colours.Add(Brushes.Firebrick);
            colours.Add(Brushes.DeepPink);
            colours.Add(Brushes.Moccasin);

            colours.Add(Brushes.Peru);
            colours.Add(Brushes.Yellow);
            colours.Add(Brushes.SeaGreen);
            colours.Add(Brushes.Aqua);

            Random rng = new Random();

            int p1Colour = rng.Next(colours.Count);
            int p2Colour = rng.Next(colours.Count);

            while (p1Colour == p2Colour)
            {
                p2Colour = rng.Next(colours.Count);
            }

            //no worries, this is an ambi-turner
            bikes.Add(new CPUBike(startPoints[0], bikeSpeed, colours[p1Colour], Directions.Right));

            bikes.Add(new CPUBike(startPoints[1], bikeSpeed, colours[p2Colour], Directions.Left));
        }
    }
}
