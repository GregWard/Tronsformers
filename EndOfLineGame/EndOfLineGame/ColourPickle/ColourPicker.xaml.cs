/**
 * \file		ColourPicker.xaml.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The code-behind for the Colour Picker.
 * \details		Determines the behaviour and the Colour Picture page of the
 *              EndOfLine game.
 */



using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;



namespace EndOfLineGame
{
    /// <summary>
    /// Interaction logic for ColourPicker.xaml
    /// </summary>
    public partial class ColourPicker : Page
    {
        // The sensor associated with the colour picker.
        private KinectSensor sensor;

        // The colours available to select from.
        private List<Brush> colours;
        // The buttons used for selecting colours.
        private List<Rectangle> buttons;
        // The players in the game.
        private List<Player> players;
        // A list of buttons currently selected by the players.
        private List<int> selectedButtons;

        // The panel displaying which colour the first player has selected.
        private StackPanel playerOneArea;
        // The panel displaying which colour the second player has selected.
        private StackPanel playerTwoArea;

        // A timer counting down to when the game beings.
        private DispatcherTimer timeToPlay;
        // A label for displaying how much time remains before the game starts.
        private Label timeDisplay;
        // How long the timer lasts before starting the game.
        private int timeRemaining = 15;

        // A hashset of tracked skeletons.
        private readonly HashSet<int> trackedUsers;



        /// <summary>
        /// The constructor for a ColourPicker page.
        /// </summary>
        /// <param name="newPlayers">The players detected by the previous page.</param>
        /// <param name="sensor">The Kinect sensor used for the game.</param>
        public ColourPicker(List<Player> newPlayers, KinectSensor sensor)  
        {
            InitializeComponent();

            players = newPlayers;
            selectedButtons = new List<int>();

            Label pageTitle = new Label();
            pageTitle.Content = "Grab Your Colour";
            pageTitle.FontSize = 70;
            pageTitle.Foreground = Brushes.White;
            ColourScreen.Children.Add(pageTitle);
            Canvas.SetLeft(pageTitle,(SystemParameters.PrimaryScreenWidth / 2) - 250);
            Canvas.SetTop(pageTitle, 0);

            trackedUsers = new HashSet<int>();

            InitializePlayers();

            this.sensor = sensor;

            InitializeKinect();

            timeDisplay = new Label();
            timeDisplay.Content = timeRemaining.ToString();
            timeDisplay.FontSize = 70;
            timeDisplay.Foreground = Brushes.Wheat;

            ColourScreen.Children.Add(timeDisplay);
            Canvas.SetLeft(timeDisplay, ( Width/ 2) - 50);
            Canvas.SetBottom(timeDisplay, 60);

            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            Canvas.SetLeft(timeDisplay, ( Width/ 2) - 50);
            Canvas.SetBottom(timeDisplay, 60);

            ColourScreen.Width = Width;
            ColourScreen.Height = Height;
            double squareSize = ColourScreen.Width / 9;

            colours = new List<Brush>();

            colours.Add(Brushes.SteelBlue);
            colours.Add(Brushes.Firebrick);
            colours.Add(Brushes.DeepPink);
            colours.Add(Brushes.Moccasin);

            colours.Add(Brushes.Peru);
            colours.Add(Brushes.Yellow);
            colours.Add(Brushes.SeaGreen);
            colours.Add(Brushes.Aqua);

            

            buttons = new List<Rectangle>();

            for (int i = 0; i < 8; i++)
            {
                buttons.Add(new Rectangle() { Width = squareSize, Height = squareSize });

                buttons[i].Fill = colours[i];
                buttons[i].RadiusX = squareSize / 10;
                buttons[i].RadiusY = squareSize / 10;

                SetRectVals(buttons[i]);

                buttons[i].Tag = i;
                ColourScreen.Children.Add(buttons[i]);


                if (i < 4)
                {
                    Canvas.SetLeft(buttons[i], (1 + 2 * i) * squareSize);
                    Canvas.SetTop(buttons[i], squareSize);
                }
                else
                {
                    Canvas.SetLeft(buttons[i], (1 + 2 * (i - 4)) * squareSize);
                    Canvas.SetTop(buttons[i], 2.5 * squareSize);
                }
            }

            timeToPlay = new DispatcherTimer();
            timeToPlay.Interval = new TimeSpan(0, 0, 1);
            timeToPlay.Tick += TimeToPlay_Tick;
            timeToPlay.Start();

        }





        /// <summary>
        /// The event handler for a timer tick for the countdown timer.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event handler for the event.</param>
        private void TimeToPlay_Tick(object sender, EventArgs e)
        {
            if(timeRemaining > 0)
            {
                --timeRemaining;
                timeDisplay.Content = timeRemaining.ToString();
            }
            else
            {
                NavigateToGame();
            }
        }





        /// <summary>
        /// Closes this page and navigates to the game page.
        /// </summary>
        private void NavigateToGame()
        {
            //take the event handlers off the sensor
            sensor.SkeletonFrameReady -= SensorSkeletonFrameReady;
            sensor.DepthFrameReady -= SensorOnDepthFrameReady;
            this.interactionStream.InteractionFrameReady -= this.InteractionFrameReady;

            //disable the timer
            timeToPlay.Stop();
            timeToPlay.Tick -= TimeToPlay_Tick;

   
            NavigationService.Navigate(new GamePage(players, sensor));
            
        }





        /// <summary>
        /// Sets up the players for the colour picker.
        /// </summary>
        private void InitializePlayers()
        {
            foreach (Player p in players)
            {
                ColourScreen.Children.Add(p.Cursor);
            }

            playerOneArea = new StackPanel();
            playerTwoArea = new StackPanel();

            //lets add some elements to each of these things
            Label playerName = new Label();
            playerName.Content = "Player 1 Colour";
            playerName.FontSize = 60;
            playerName.Foreground = Brushes.Black;

            playerOneArea.HorizontalAlignment = HorizontalAlignment.Center;
            playerOneArea.VerticalAlignment = VerticalAlignment.Center;
            playerOneArea.Children.Add(playerName);

            Label playerNameTwo = new Label();
            playerNameTwo.Content = "Player 2 Colour";
            playerNameTwo.FontSize = 60;
            playerNameTwo.Foreground = Brushes.Black;

            playerTwoArea.HorizontalAlignment = HorizontalAlignment.Center;
            playerTwoArea.VerticalAlignment = VerticalAlignment.Center;
            playerTwoArea.Children.Add(playerNameTwo);

            ColourScreen.Children.Add(playerOneArea);
            ColourScreen.Children.Add(playerTwoArea);

            Canvas.SetLeft(playerOneArea, 20);
            Canvas.SetBottom(playerOneArea, 20);

            Canvas.SetRight(playerTwoArea, 20);
            Canvas.SetBottom(playerTwoArea, 20);


        }





        /// <summary>
        /// Sets up the Kinect functionality for the ColourPicker.
        /// </summary>
        private void InitializeKinect()
        {
            skeletons = new Skeleton[sensor.SkeletonStream.FrameSkeletonArrayLength];
            usersInfo = new UserInfo[InteractionFrame.UserInfoArrayLength];

            foreach (Player p in players)
            {
                trackedUsers.Add(p.Info.SkeletonTrackingId);
            }

                    

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
        }





        /// <summary>
        /// Sets the default values for a given rectangle
        /// </summary>
        /// <param name="rect">The rectangle to set the values for</param>
        void SetRectVals(Rectangle rect)
        {
            rect.StrokeThickness = 5;
            rect.Stroke = Brushes.Black;
            rect.Opacity = 1;
        }





        /// <summary>
        /// Checks to see if a rectangle is being hovered over
        /// </summary>
        /// <param name="locations">The location of mouse/hand</param>
        public void CheckHover(List<Point> locations)
        {

            foreach (Rectangle button in buttons)
            {
                if (button.StrokeThickness != 10)
                {
                    ButtonNormal(button);



                    foreach (Point location in locations)
                    {
                        int index = (int)button.Tag;

                        if (!selectedButtons.Contains(index))
                        {
                            if (IsOver(button, location))
                            {
                                ButtonHover(button);
                            }
                        }
                    }
                }
            }
        }





        /// <summary>
        /// Resets the users for the ColourPicker.
        /// </summary>
        /// <param name="playerToRemove"></param>
        private void ResetUser(Player playerToRemove)
        {
            foreach (Rectangle button in buttons)
            {
                if (button.Fill == playerToRemove.PlayerColour)
                {
                    ButtonNormal(button);
                    break;
                }
            }

            if (playerToRemove.Name == "player1")
            {
                //clear out any specifics
            }
            else if (playerToRemove.Name == "player2")
            {
                //clear out any specifics for that player
            }
        }





        /// <summary>
        /// Sets the rectangle stroke to 'normal mode'
        /// </summary>
        /// <param name="button">The Rectangle to modify</param>
        private void ButtonNormal(Rectangle button)
        {
            button.Stroke = Brushes.Black;
            button.StrokeThickness = 5;
        }





        /// <summary>
        /// Sets the rectangle stroke to 'hover mode'
        /// </summary>
        /// <param name="button">The Rectangle to modify</param>
        private void ButtonHover(Rectangle button)
        {

            button.Stroke = Brushes.IndianRed;
            button.StrokeThickness = 5;
        }





        /// <summary>
        /// Determines is a specified point is over a specified rectangle
        /// </summary>
        /// <param name="button"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool IsOver(Rectangle button, Point location)
        {
            bool isOver = false;

            double midY = Canvas.GetTop(button) + button.Height / 2;
            double midX = Canvas.GetLeft(button) + button.Width / 2;

            double distance = Math.Sqrt(Math.Pow(location.X - midX, 2) + Math.Pow(location.Y - midY, 2));

            if (distance < button.Width / 2)
            {
                isOver = true;
            }

            return isOver;
        }





        /// <summary>
        /// Checks which square a user has selected.
        /// </summary>
        /// <param name="player">The player who makes the selection.</param>
        /// <param name="point">The location of the player's cursor.</param>
        /// <param name="rect">The rectangle the user is hovering over.</param>
        public void CheckSelect(Player player, Point point, Rectangle rect)
        {
            // Iterate through each button...
            foreach (Rectangle button in buttons)
            {
                int index = (int)button.Tag;

                // Determine which button matches the one passed in...
                if (!selectedButtons.Contains(index))
                {
                    if (button == rect)
                    {
                        if (IsOver(button, point))
                        {
                            // If it's available, select that colour for the current player.
                            bool canBeChosen = true;

                            foreach(Player p in players)
                            {
                                if (p.PlayerColour == button.Fill)
                                {
                                    canBeChosen = false;
                                }
                            }

                            if (canBeChosen)
                            {
                                ButtonSelected(button, player);
                                player.PlayerColour = button.Fill;

                                if (player.Name == "player1")
                                {
                                    playerOneArea.Background = button.Fill;
                                }
                                else
                                {
                                    playerTwoArea.Background = button.Fill;
                                }
                            }
                        }
                    }
                }
            }
        }





        /// <summary>
        /// Marks a button as having been selected by the given player.
        /// </summary>
        /// <param name="button">The button to highlight.</param>
        /// <param name="player">The player who selected it.</param>
        private void ButtonSelected(Rectangle button, Player player)
        {
            foreach (Rectangle rect in buttons)
            {
                if (rect.Fill == player.PlayerColour)
                {
                    ButtonNormal(rect);
                    break;
                }
            }

            button.Stroke = Brushes.BlanchedAlmond;
            button.StrokeThickness = 10;

        }

    }
}
