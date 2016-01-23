using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;

namespace EndOfLineGame
{
    /// <summary>
    /// Interaction logic for InitialDetection.xaml
    /// </summary>
    public partial class InitialDetectionPage : Page
    {
        Coin entryCoin;
        CoinSlot coinbox;

        Skeleton[] skeletons = new Skeleton[0];
        UserInfo[] usersInfo = new UserInfo[0];
        KinectSensor sensor;
        InteractionStream interactionStream;

        DispatcherTimer playersMissing;
        int playersMissingCount;

        private readonly HashSet<int> trackedUsers = new HashSet<int>();
        private List<Player> players = new List<Player>();


        public InitialDetectionPage()
        {
            InitializeComponent();

            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;

            InitializeKinect();

            InitializeWelcomeText();
            

            playersMissing = new DispatcherTimer();

            playersMissing.Tick += PlayersMissing_Tick;
            playersMissing.Interval = new TimeSpan(0, 0, 1);
        }

        private void PlayersMissing_Tick(object sender, EventArgs e)
        {
            if (playersMissingCount > 5)
            {
                //disconnect the kinect event handlers

                sensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
                sensor.DepthFrameReady -= SensorOnDepthFrameReady;
                interactionStream.InteractionFrameReady -= this.InteractionFrameReady;
                playersMissing.Tick -= PlayersMissing_Tick;
                playersMissingCount = 0;

                NavigationService.Navigate(new Uri(@"../DemoAndGamePage/DemoPage.xaml", UriKind.Relative));
                
            }
            else
            {
                ++playersMissingCount;
            }
        }

        private void InitializeWelcomeText()
        {
            Hello.Height = this.Height;
            Hello.Width = this.Width;
            Hello.HorizontalContentAlignment = HorizontalAlignment.Center;
            Hello.VerticalContentAlignment = VerticalAlignment.Center;
            
        }

        private void InitialDetectPage_Loaded(object sender, RoutedEventArgs e)
        {

            Hello.Content = "To start, drag the coin to the slot to lock yourself in.";
            DoubleAnimation lastStage = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 2)));
            Hello.BeginAnimation(Label.OpacityProperty, lastStage);

            entryCoin = new Coin();
            coinbox = new CoinSlot();

            canvas.Children.Add(coinbox.Slot);
            canvas.Children.Add(entryCoin.CoinShape);

            Canvas.SetLeft(coinbox.Slot, (this.Width / 2) - 150);
            Canvas.SetTop(coinbox.Slot, (this.Height / 2) + 100);

            Canvas.SetLeft(entryCoin.CoinShape, this.Width / 2);
            Canvas.SetTop(entryCoin.CoinShape, this.Height / 4);

            DoubleAnimation entry = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 2, 500)));

            DoubleAnimationUsingKeyFrames readySetGo = new DoubleAnimationUsingKeyFrames();

            entryCoin.CoinShape.BeginAnimation(Ellipse.OpacityProperty, entry);
            coinbox.Slot.BeginAnimation(Rectangle.OpacityProperty, entry);
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
                    usersInfo = new UserInfo[InteractionFrame.UserInfoArrayLength];

                    //enable tracking the skeletons for the kinect
                    sensor.SkeletonStream.Enable();

                    //measure the depth
                    sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);

                    //set for seating...TESTING PURPOSES
                    sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;

                    //create an event for anytime a skeleton is present to the sensor
                    sensor.SkeletonFrameReady += SensorSkeletonFrameReady;

                    //for measuring the depth of our interaction with a user
                    sensor.DepthFrameReady += SensorOnDepthFrameReady;
                    //kinectRegion.KinectSensor = sensor;

                    this.interactionStream = new InteractionStream(sensor, new DummyInteractionClient());
                    this.interactionStream.InteractionFrameReady += this.InteractionFrameReady;

                    sensor.Start();

                    break;
                }
            }

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            playersMissing.Stop();
        }
    }
}
