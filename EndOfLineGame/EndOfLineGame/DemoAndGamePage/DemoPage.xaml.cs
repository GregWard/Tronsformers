/**
 * \file		DemoPage.xaml.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The code-behind for the Demo Page.
 * \details		Provides the user interactions and display for the demo screen.
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Threading;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;




namespace EndOfLineGame
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class DemoPage : Page
    {
        /// <summary>
        /// The game object being run for the demo.
        /// </summary>
		private Game game;
        // The sensor used for the demo page.
        private KinectSensor sensor;


        /// <summary>
        /// The constructor for the Demo Page.
        /// </summary>
		public DemoPage()
        {
            InitializeComponent();
            game = new DemoGame(myCanvas);
            game.GameOver += Game_GameOver;
            InitializeKinect();
            
        }





        /// <summary>
        /// The event handler for the game ending.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the evnt.</param>
        private void Game_GameOver(object sender, EventArgs e)
        {
            Thread.Sleep(500);
            game.Restart();
        }





        /// <summary>
        /// The event handler for when the page is first rendered.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event handler.</param>
		private void GamePage_ContentRendered(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).KeyDown += GamePage_KeyDown;
            game.Start();
        }





        /// <summary>
        /// The event handler for keyboard events (mostly used in debug, as opposed
        /// to the actual Kinect-enabled program.)
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event handler.</param>
		private void GamePage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                game.Restart();
            }
            else if (e.Key == Key.Escape)
            {
                game.End();
                Window.GetWindow(this).KeyDown -= GamePage_KeyDown;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    Window.GetWindow(this).Close();
                }
            }
        }





        /// <summary>
        /// The event handler for unloading the page event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event handler.</param>
		private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            game.End();
        }





        /// <summary>
        /// Initializes Kinect functionality for the demo page.
        /// </summary>
        private void InitializeKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                //search for sensors connected, and actually connect to the kinect that returns connected
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;

                    App.AppSensor = potentialSensor;

                    skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
                    //enable tracking the skeletons for the kinect
                    sensor.SkeletonStream.Enable();

                    //measure the depth
                    sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                    //set for seating...TESTING PURPOSES
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;

                    //create an event for anytime a skeleton is present to the sensor
                    sensor.SkeletonFrameReady += SensorSkeletonFrameReady;

                    sensor.Start();

                    break;
                }
            }


        }
    }
}
