/**
 * \file		GamePage.xaml.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The code-behind for the Game Page.
 * \details		Provides the user interactions and display for the Game screen.
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using System.Windows.Media;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Windows.Media.Animation;




namespace EndOfLineGame
{
    /// <summary>
    /// An enumeration for player identities.
    /// </summary>
    public enum Players : int
    {
        PlayerOne = 0,
        PlayerTwo = 1
    }



    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        /// <summary>
        /// A reference to the game this page is running.
        /// </summary>
		private Game game;
        /// <summary>
        /// The sensor used for the game page.
        /// </summary>
        private KinectSensor sensor;
        /// <summary>
        /// The list of skeletons currently detected by the sensor.
        /// </summary>
        Skeleton[] skeletons = new Skeleton[0];
        /// <summary>
        /// The list of players in the game.
        /// </summary>
        private List<Player> skeletonsInFrame;


        /// <summary>
        /// Strings used for the countdown timer.
        /// </summary>
        private static List<string> countDownStrings = new List<string>() {
            "3",
            "2",
            "1",
            "Lean!"
        };




        /// <summary>
        /// The constructor for a GamePage.
        /// </summary>
        public GamePage(List<Player> gamePlayers, KinectSensor passedSensor)
        {
            InitializeComponent();

            game = new Game(myCanvas, gamePlayers);
            skeletonsInFrame = gamePlayers;
            sensor = passedSensor;
            InitializeKinect();

            game.GameOver += Game_GameOver;

        }





        /// <summary>
        /// The event handler for the game ending.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void Game_GameOver(object sender, EventArgs e)
        {
            DoubleAnimation thanksAnim = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            gameLabel.Content = "Thanks For Playing!";
            thanksAnim.Completed += ThanksAnim_Completed;
            gameLabel.BeginAnimation(Label.OpacityProperty, thanksAnim);

            game.End();
        }





        /// <summary>
        /// The event handler for the "Thanks for playing" animation ending.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void ThanksAnim_Completed(object sender, EventArgs e)
        {
            Thread.Sleep(1000);

            sensor.SkeletonFrameReady -= SkeletonLeanDetection;

            this.NavigationService.Navigate(new InitialDetectionPage());
        }





        /// <summary>
        /// Initializes the Kinect functionality for the game page.
        /// </summary>
        private void InitializeKinect()
        {
            //enable tracking the skeletons for the kinect
            sensor.SkeletonStream.Enable();
            sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
            //create an event for anytime a skeleton is present to the sensor
            sensor.SkeletonFrameReady += SkeletonLeanDetection;
        }





        /// <summary>
        /// The event handler for rendering the content of the page.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void GamePage_ContentRendered(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).KeyDown += GamePage_KeyDown;
            gameLabel.Opacity = 0;
            gameLabel.Content = countDownStrings.First();
            gameLabel.FontSize = 64;

            //Start animation
            DoubleAnimation dblAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));
            dblAnimation.Completed += Animation_Completed;
            gameLabel.BeginAnimation(Label.OpacityProperty, dblAnimation);
        }




        /// <summary>
        /// The event handler for the end of each animation in the player
        /// countdown.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            string currentText = gameLabel.Content.ToString();
            double opacity = gameLabel.Opacity;
            DoubleAnimation dblAnimation;
            int countIndex = countDownStrings.IndexOf(currentText);

            if (opacity >= 1)
            {
                //fade away
                dblAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));

                dblAnimation.Completed += Animation_Completed;
                gameLabel.BeginAnimation(Label.OpacityProperty, dblAnimation);
            }
            else
            {
                dblAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.5)));


                if (countIndex < countDownStrings.Count - 1)
                {
                    dblAnimation.Completed += Animation_Completed;
                    gameLabel.Content = countDownStrings[++countIndex];
                    gameLabel.BeginAnimation(Label.OpacityProperty, dblAnimation);
                }
                else
                {
                    gameLabel.Opacity = 0;     
                    game.Start();
                }

            }

 
        }





        /// <summary>
        /// The event handler for a key being pressed while on the game page.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
		private void GamePage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                game.TurnLeft(Players.PlayerTwo);
            }
            else if (e.Key == Key.Right)
            {
                game.TurnRight(Players.PlayerTwo);
            }
            if (e.Key == Key.A)
            {
                game.TurnLeft(Players.PlayerOne);
            }
            else if (e.Key == Key.D)
            {
                game.TurnRight(Players.PlayerOne);
            }
            else if (e.Key == Key.R)
            {
                game.Restart();
            }
            else if (e.Key == Key.Escape)
            {
                game.End();
                Window.GetWindow(this).Close();
            }
            else if (e.Key == Key.OemTilde)
            {
                Window.GetWindow(this).KeyDown -= GamePage_KeyDown;
                NavigationService.Navigate(new Uri("/InitialDetectionPage.xaml?value=", UriKind.Relative));

            }
        }





        /// <summary>
        /// The event handler for unloading the GamePage, ending the game in progress.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
		private void GamePage_Unloaded(object sender, RoutedEventArgs e)
        {
            game.End();
        }
    }
}
