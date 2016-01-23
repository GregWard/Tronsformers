/**
 * \file		Coin.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The definition of the intro screen coin.
 * \details		Defines the behaviour and attributes of the coin
 *              used to start the game.
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




namespace EndOfLineGame
{
    /// <summary>
    /// A coin used to start a round of EndOfLine.
    /// </summary>
    class Coin
    {
        /// <summary>
        /// The ellipse defining the Coin's outline.
        /// </summary>
        Ellipse coinShape;
        /// <summary>
        /// Whether the coin is presently gripped or not.
        /// </summary>
        bool isGripped;
        /// <summary>
        /// The name of the player holding the string.
        /// </summary>
        string grippedBy;
        


        /// <summary>
        /// The name of the player holding the string.
        /// </summary>
        public string GrippedBy
        {
            get { return grippedBy; }
            set { grippedBy = value; }
        }



        /// <summary>
        /// Whether the coin has been gripped or not.
        /// </summary>
        public bool IsGripped
        {
            get { return isGripped;}
            set { isGripped = value; }
        }



        /// <summary>
        /// The ellipse defining the coin's outline.
        /// </summary>
        public Ellipse CoinShape
        {
            get { return coinShape; }
        }





        /// <summary>
        /// The constructor for creating a new coin.
        /// </summary>
        public Coin()
        {
            coinShape = new Ellipse();
            coinShape.Width = 100;
            coinShape.Height = 100;

            coinShape.Fill = new ImageBrush(new BitmapImage(new Uri(@"images\BigRoundCoin.png", UriKind.Relative)));

            coinShape.Opacity = 0;
            coinShape.Name = "Quarter";

            isGripped = false;
            grippedBy = "";
        }
    }
}
