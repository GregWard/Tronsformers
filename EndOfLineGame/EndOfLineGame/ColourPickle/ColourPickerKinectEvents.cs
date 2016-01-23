/**
 * \file		ColourPickerKinectEvents.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The Kinect events connected to the Colour Picker.
 * \details		All gestures, motions, and actions triggered by the Kinect
 *              while on the ColourPicker page.
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
    public partial class ColourPicker : Page
    {
        // The skeletons detected by the Kinect.
        Skeleton[] skeletons = new Skeleton[0];
        // The user info for the players detected by the Kinect.
        UserInfo[] usersInfo = new UserInfo[0];
        
        InteractionStream interactionStream;




        /// <summary>
        /// The event handler for following user skeletons.
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
                        var accelerometerReading = sensor.AccelerometerGetCurrentReading();
                        interactionStream.ProcessSkeleton(skeletons, accelerometerReading, skeletonFrame.Timestamp);
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





        /// <summary>
        /// The event handler for depth tracking on the Colour Picker.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="depthImageFrameReadyEventArgs">The arguments for the event.</param>
        private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs depthImageFrameReadyEventArgs)
        {
            using (DepthImageFrame depthFrame = depthImageFrameReadyEventArgs.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {

                    try
                    {
                        interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                    }
                    catch (InvalidOperationException)
                    {
                        // DepthFrame functions may throw when the sensor gets
                        // into a bad state.  Ignore the frame in that case.
                        Console.WriteLine("Kinect threw bad information for depth garbage. Potential Error with kinect...");
                    }
                }
            }
        }





        /// <summary>
        /// The event handler for interaction frame events.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            Player playerToUse = null;
            List<Point> RectanglePoints = new List<Point>();
            // Check for a null userInfos since we may still get posted events
            // from the stream after we have unregistered our event handler and
            // deleted our buffers.
            if (this.usersInfo != null)
            {

                UserInfo[] localUserInfos = null;
                long timestamp = 0;


                using (InteractionFrame interactionFrame = e.OpenInteractionFrame())
                {
                    if (interactionFrame != null)
                    {
                        // Copy interaction frame data so we can dispose interaction frame
                        // right away, even if data processing/event handling takes a while.
                        interactionFrame.CopyInteractionDataTo(this.usersInfo);
                        timestamp = interactionFrame.Timestamp;
                        localUserInfos = this.usersInfo;
                    }
                }

                if (localUserInfos != null)
                {

                    var currentUserSet = new HashSet<int>();
                    var usersToRemove = new HashSet<Player>();

                    // Keep track of current users in scene
                    foreach (var info in localUserInfos)
                    {
                        if (info.SkeletonTrackingId == 0)
                        {
                            // Only look at user information corresponding to valid users
                            continue;
                        }



                        foreach (Player player in players)
                        {
                            if (player.Info.SkeletonTrackingId == info.SkeletonTrackingId)
                            {
                                playerToUse = player;
                            }
                        }

                        currentUserSet.Add(info.SkeletonTrackingId);
                        this.trackedUsers.Add(info.SkeletonTrackingId);

                        // Perform hit testing and look for Grip and GripRelease events
                        foreach (var handPointer in info.HandPointers)
                        {

                            if (handPointer.IsPrimaryForUser)
                            {
                                double xUI = handPointer.X * this.Width;
                                double yUI = handPointer.Y * this.Height;

                                RectanglePoints.Add(new Point(xUI+50, yUI+50));

                                if (handPointer.HandEventType == InteractionHandEventType.Grip)
                                {
                                    playerToUse.Grab();
                                    //check the rectangle and see what colour we're on.
                                    foreach (UIElement elem in ColourScreen.Children)
                                    {
                                        if (elem is Rectangle)
                                        {
                                            Rectangle tmp = (Rectangle)elem;

                                            CheckSelect(playerToUse, new Point(xUI+50, yUI+50), tmp);
                                        }
                                    }
                                }
                                else if(handPointer.HandEventType == InteractionHandEventType.GripRelease)
                                {
                                    playerToUse.Release();
                                }

                                if (playerToUse != null)
                                {
                                    if (xUI + 100 <= this.Width && xUI >= 0)
                                    {
                                        Canvas.SetLeft(playerToUse.Cursor, xUI);
                                    }
                                    if (yUI + 100 <= this.Height && yUI >= 0)
                                    {
                                        Canvas.SetTop(playerToUse.Cursor, yUI);
                                    }

                                }
                            }
                        }
                    }
                    foreach (Player player in players)
                    {
                        if (!currentUserSet.Contains(player.Info.SkeletonTrackingId))
                        {
                            usersToRemove.Add(player);
                            ResetUser(player);
                        }
                    }

                    foreach (Player player in usersToRemove)
                    {
                        ColourScreen.Children.Remove(player.Cursor);
                        players.Remove(player);
                        if (players.Count == 0)
                        {
                            //start a timer that will mark the time and go back if there's no one there within 5 seconds
                            
                            break;
                        }
                        sensor.SkeletonStream.AppChoosesSkeletons = false;

                    }

                    CheckHover(RectanglePoints);


                }
            }
        }
    }
}
