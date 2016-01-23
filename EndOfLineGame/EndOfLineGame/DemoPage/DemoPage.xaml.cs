using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;


namespace TestUI
{
    /// <summary>
    /// Event that will fire when the kinect has detected someone for five seconds
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public partial class DemoPage : Page
	{
        public enum Player : int
        {
            PlayerOne = 0,
            PlayerTwo = 1
        }

        KinectSensor sensor;
        private Game game;
        

		public DemoPage()
		{
			InitializeComponent();
			game = new DemoGame(myCanvas);
            InitializeKinect();
		}

        private void GamePage_ContentRendered(object sender, RoutedEventArgs e)
		{
			Window.GetWindow(this).KeyDown += GamePage_KeyDown;
			game.Start();
		}



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
			}
		}


		private void GamePage_Unloaded(object sender, RoutedEventArgs e)
		{
			game.End();
		}

        private void InitializeKinect()
        {
            

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                //search for sensors connected, and actually connect to the kinect that returns connected
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;

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
