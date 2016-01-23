/**
 * \file		DummyInteractionClient.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		Defines an interaction interface client.
 * \details		Defines a extension of the IInteractionClient interface
 *              to allow the Kinect to interpret user actions for the
 *              EndOfLineGame.
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
    /// The interaction client for the EndOfLine Game.
    /// </summary>
    public class DummyInteractionClient : IInteractionClient
    {
        /// <summary>
        /// The constructor for the DummyInteractionClient.
        /// </summary>
        /// <param name="skeletonTrackingId">The ID of the skeleton to track.</param>
        /// <param name="handType">The type of hand (left/right).</param>
        /// <param name="x">The x position of the hand.</param>
        /// <param name="y">The y position of the hand.</param>
        /// <returns></returns>
        public InteractionInfo GetInteractionInfoAtLocation(
            int skeletonTrackingId,
            InteractionHandType handType,
            double x,
            double y)
        {
            var result = new InteractionInfo();
            result.IsGripTarget = true;
            result.IsPressTarget = true;
            result.PressAttractionPointX = 0.5;
            result.PressAttractionPointY = 0.5;
            result.PressTargetControlId = 1;

            return result;
        }
    }
}

