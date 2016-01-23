/**
 * \file		CoinSlot.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The slot used to start the game.
 * \details		The coin slot used to start the EndOfLine game by dropping
 *              a coin in.
 */



using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace EndOfLineGame
{
    /// <summary>
    /// The constructor for a coin slot.
    /// </summary>
    class CoinSlot
    {
        /// <summary>
        /// The shape of the rectangular border of the CoinSlot.
        /// </summary>
        Rectangle slot;



        /// <summary>
        /// The constructor for a CoinSlot.
        /// </summary>
        public CoinSlot()
        {
            slot = new Rectangle();

            slot.Height = 300;
            slot.Width = 300;

            slot.Fill = new ImageBrush(new BitmapImage(new Uri(@"images\Slot.png", UriKind.Relative)));
            slot.Name = "CoinSlot";

            slot.Opacity = 0;
        }





        /// <summary>
        /// The shape of the rectangular border of the CoinSlot.
        /// </summary>
        public Rectangle Slot
        {
            get
            {
                return slot;
            }
        }
    }
}
