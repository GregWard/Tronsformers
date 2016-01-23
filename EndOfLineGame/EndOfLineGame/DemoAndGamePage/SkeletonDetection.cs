/**
 * \file		SkeletonDetection.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The skeleton detection functions.
 * \details		Defines the skeleton detection functionality for the
 *              DemoPage and GamePage pages.
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

using Microsoft.Kinect;



namespace EndOfLineGame
{
    public partial class DemoPage : Page
    {
        /// <summary>
        /// The skeletons seen by the Kinect sensor.
        /// </summary>
        Skeleton[] skeletons = new Skeleton[0];
        /// <summary>
        /// A dictionary 
        /// </summary>
        Dictionary<int, long> skeletonsInFrame = new Dictionary<int, long>();




        /// <summary>
        /// The event handler for detecting skeletons with the Kinect.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="skeletonFrameReadyEventArgs">The arguments for the event.</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {

            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {

                if (skeletonFrame != null)
                {
                    try
                    {

                        skeletonFrame.CopySkeletonDataTo(skeletons);

                        foreach (Skeleton skel in skeletons)
                        {
                            if (skel.TrackingId != 0)
                            {
                                if (!skeletonsInFrame.ContainsKey(skel.TrackingId))
                                {
                                    //get the skeleton ID and the time we captured it...
                                    skeletonsInFrame.Add(skel.TrackingId, skeletonFrame.Timestamp);
                                }
                                else
                                {
                                    //ensure the skeleton has been around for 5 seconds so we know they may actually want to play
                                    long timeStampToCompare = skeletonsInFrame[skel.TrackingId] + 4000;

                                    if (skeletonFrame.Timestamp > timeStampToCompare)
                                    {
                                        game.End();
                                        sensor.SkeletonFrameReady -= SensorSkeletonFrameReady;


                                        this.NavigationService.Navigate(new Uri("../InitialDetectionPage/InitialDetectionPage.xaml", UriKind.Relative));
                                        break;

                                    }
                                }
                            }
                        }


                    }
                    catch (InvalidOperationException)
                    {
                        // SkeletonFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                        Console.WriteLine("Kinect threw bad information for skeleton garbage. Potential Error with kinect...");

                    }
                }

            }
        }

    }





    public partial class GamePage : Page
    {
        /// <summary>
        /// The event handler for interpreting and responding to skeletons leaning.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="skeletonFrameReadyEventArgs">The arguments for the event.</param>
        private void SkeletonLeanDetection(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {
            Player playerToTrack = null;
            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    try
                    {
                        skeletonFrame.CopySkeletonDataTo(skeletons);

                        foreach (Skeleton skel in skeletons)
                        {
                            //check if we are looking at a skeleton to track
                            if (skeletonsInFrame.Count > 0)
                            {
                                if (skeletonsInFrame[0].Info.SkeletonTrackingId == skel.TrackingId)
                                {
                                    playerToTrack = skeletonsInFrame[0];
                                }
                                else if (skeletonsInFrame.Count > 1)
                                {
                                    if (skeletonsInFrame[1].Info.SkeletonTrackingId == skel.TrackingId)
                                    {
                                        playerToTrack = skeletonsInFrame[1];
                                    }
                                }

                                //if we've found a player to track...let's track em
                                if (playerToTrack != null)
                                {
                                    double shoulderY = skel.Joints[JointType.ShoulderCenter].Position.Y;
                                    double shoulderX = skel.Joints[JointType.ShoulderCenter].Position.X;
                                    double hipsX = skel.Joints[JointType.HipCenter].Position.X;
                                    double hipsY = skel.Joints[JointType.HipCenter].Position.Y;
                                    //
                                    //
                                    //
                                    double roll = Math.Atan((hipsX - shoulderX) / (hipsY - shoulderY));

                                    if (playerToTrack.CanLean && playerToTrack.bike != null)
                                    {
                                        if (roll < -Math.PI / 15)
                                        {
                                            playerToTrack.bike.TurnLeft();
                                            playerToTrack.CanLean = false;
                                            // I can't turn left - DZ
                                            //if enum != left  =>  enum = left & call left turn
                                        }
                                        else if (roll > Math.PI / 15)
                                        {
                                            playerToTrack.bike.TurnRight();
                                            playerToTrack.CanLean = false;
                                        }
                                    }
                                    else
                                    {
                                        if ((roll > -Math.PI / 18) && (roll < Math.PI / 18))
                                        {
                                            playerToTrack.CanLean = true;
                                        }

                                    }
                                }

                                    }

                                }


                            }



                    catch (InvalidOperationException)
                    {
                        // SkeletonFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                        Console.WriteLine("Kinect threw bad information for skeleton garbage. Potential Error with kinect...");

                    }
                }

            }
        }
    }
}
