/**
 * \file		Player.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		Defines a player.
 * \details		Defines a player, as an entity connected to a particular bike,
 *              with a Kinect-tracked skeleton.
 */



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

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;




namespace EndOfLineGame
{
    /// <summary>
    /// The constructor for a player.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The user's Kinect info.
        /// </summary>
        UserInfo myInfo;
        /// <summary>
        /// The cursor representing the player's hand.
        /// </summary>
        StackPanel cursor;
        /// <summary>
        /// The image/shape used for the player's hand.
        /// </summary>
        Ellipse cursorImage;
        /// <summary>
        /// The name of the player (Player 1/Player 2).
        /// </summary>
        string name;
        /// <summary>
        /// Whether the player is allowed to lean at this time to change
        /// directions.
        /// </summary>
        bool canLean;

        /// <summary>
        /// The colour the player's picked, which is used for their bike
        /// and its trail.
        /// </summary>
        Brush playerColour;


        /// <summary>
        /// The bike that belongs to the player.
        /// </summary>
        public TronBike bike;


        /// <summary>
        /// The cursor representing the player's hand.
        /// </summary>
        public StackPanel Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }




        /// <summary>
        /// The image/shape used for the player's hand.
        /// </summary>
        public Ellipse CursorImage
        {
            get { return cursorImage; }
            set { cursorImage = value; }
        }




        /// <summary>
        /// The user's Kinect info.
        /// </summary>
        public UserInfo Info
        {
            get { return myInfo; }
            set { myInfo = value; }
        }




        /// <summary>
        /// The name of the player (Player 1/Player 2).
        /// </summary>
        public string Name
        {
            get { return name; }
        }




        /// <summary>
        /// Whether the player is allowed to lean at this time to change
        /// directions.
        /// </summary>
        public bool CanLean
        {
            get { return canLean; }
            set { canLean = value; }
        }




        /// <summary>
        /// The colour the player's picked, which is used for their bike
        /// and its trail.
        /// </summary>
        public Brush PlayerColour
        {
            get { return playerColour; }
            set { playerColour = value; }
        }





        /// <summary>
        /// The constructor for a player.
        /// </summary>
        /// <param name="info">The player's information.</param>
        /// <param name="name">The name of the player.</param>
        /// <param name="color">The player's colour.</param>
        public Player(UserInfo info, string name,Brush color)
        {
            myInfo = info;

            Label playerCursorLabel = new Label();

            playerCursorLabel.Content = name;
            playerCursorLabel.Foreground = new SolidColorBrush(Colors.GhostWhite);
            playerCursorLabel.FontSize = 32;
            playerCursorLabel.HorizontalAlignment = HorizontalAlignment.Center;

            cursor = new StackPanel();

            cursorImage = new Ellipse();

            cursorImage.Height = 100;
            cursorImage.Width = 100;
            cursorImage.Fill = new ImageBrush(new BitmapImage(new Uri(@"images/Hand.png", UriKind.Relative)));

            cursor.Children.Add(cursorImage);

            Canvas.SetZIndex(cursor, 1);

            cursor.Children.Add(playerCursorLabel);

            
            this.name = name;
            canLean = true;
            playerColour = color;

        }





        /// <summary>
        /// Changes the hand's image when the user makes a grabbing motion.
        /// </summary>
        public void Grab()
        {
            cursorImage.Fill = new ImageBrush(new BitmapImage(new Uri(@"images/ClosedHand.png", UriKind.Relative)));
        }





        /// <summary>
        /// Changes the hand's image when the user makes a release motion.
        /// </summary>
        public void Release()
        {
            cursorImage.Fill = new ImageBrush(new BitmapImage(new Uri(@"images/Hand.png", UriKind.Relative)));
        }

    }
}
