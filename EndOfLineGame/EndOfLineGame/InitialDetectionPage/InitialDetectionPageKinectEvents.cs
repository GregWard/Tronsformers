/**
 * \file		InitialDetectionPageKinectEvents.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The Kinect events for the player start page.
 * \details		The Kinect events used to run the InitialDetectionPage, where the
 *              player uses a coin to start the game.
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
    public partial class InitialDetectionPage : Page
    {

        /// <summary>
        /// The event handler for detecting a skeleton on the InitialDetectionPage.
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
        /// The event handler for depth detection for the Kinect.
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
        /// The event handler for Kinect interactions for the InitialDetectionPage.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void InteractionFrameReady(object sender, InteractionFrameReadyEventArgs e)
        {
            Player playerToUse = null;
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


                        if (!this.trackedUsers.Contains(info.SkeletonTrackingId))
                        {
                            if (players.Count == 0)
                            {

                                players.Add(new Player(info, "player1", Brushes.SteelBlue));

                                canvas.Children.Add(players[0].Cursor);
                            }
                            else
                            {
                                if (players.Count == 1)
                                {
                                    string name = "player2";

                                    // Default the player's colour.
                                    SolidColorBrush playerColour = Brushes.Firebrick;

                                    if (players[0].Name == "player2")
                                    {
                                        name = "player1";
                                        // Default the player's colour.
                                        playerColour = Brushes.SteelBlue;
                                    }


                                    players.Add(new Player(info, name, playerColour));
                                    
                                    canvas.Children.Add(players[1].Cursor);

                                    sensor.SkeletonStream.AppChoosesSkeletons = true;
                                    sensor.SkeletonStream.ChooseSkeletons(players[0].Info.SkeletonTrackingId, players[1].Info.SkeletonTrackingId);
                                }
                            }

                            //re initialize timer so we are not counting players missing
                            playersMissing.Stop();
                            playersMissingCount = 0;
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

                                double coinTop = 0;
                                double coinLeft = 0;
                               
                                Ellipse coinElement = null;
                                // canvas.FindName("Quarter");

                                foreach (UIElement elem in canvas.Children)
                                {
                                    if (elem is Ellipse && ((Ellipse)elem).Name == "Quarter")
                                    {
                                        coinElement = (Ellipse)elem;
                                        coinTop = Canvas.GetTop(coinElement);
                                        coinLeft = Canvas.GetLeft(coinElement);
                                    }
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
                                if (coinElement != null)
                                {
                                    if (handPointer.HandEventType != InteractionHandEventType.None)
                                    {
                                        if (handPointer.HandEventType == InteractionHandEventType.Grip)
                                        {
                                            playerToUse.Grab();

                                            if (!entryCoin.IsGripped)
                                            {
                                                if (((xUI <= coinLeft + 60) && (xUI >= coinLeft - 60)) && ((yUI <= coinTop + 60) && (yUI >= coinTop - 60)))
                                                {
                                                    entryCoin.IsGripped = true;
                                                    entryCoin.GrippedBy = playerToUse.Name;
                                                }
                                            }

                                        }
                                        else if (handPointer.HandEventType == InteractionHandEventType.GripRelease)
                                        {
                                            playerToUse.Release();
                                            if ((entryCoin.IsGripped) && (entryCoin.GrippedBy == playerToUse.Name))
                                            {
                                                entryCoin.GrippedBy = "";
                                                entryCoin.IsGripped = false;
                                                //do not let the coin exceed the bounds of the screen. This must be set explicitly and not
                                                //in reference to the top/left of the players cursor due to it still exceeding boundary

                                                if (xUI + 100 <= this.Width && xUI >= 0)
                                                {
                                                    Canvas.SetLeft(coinElement, xUI);
                                                }

                                                if (yUI + 100 <= this.Height && yUI >= 0)
                                                {
                                                    Canvas.SetTop(coinElement, yUI);
                                                }

                                                double slotThresholdX = xUI - Canvas.GetLeft(coinbox.Slot);
                                                double slotThresholdY = yUI - Canvas.GetTop(coinbox.Slot);

                                                if (  (slotThresholdX >= 0 && slotThresholdX < 400) && (slotThresholdY >= 0 && slotThresholdY < 400))
                                                {
                                                        canvas.Children.Clear();
                                                        NavigationService.Navigate(new ColourPicker(players, sensor));
                                                    playersMissing.Stop();
                                                    playersMissing.Tick -= PlayersMissing_Tick;
                                                    playersMissingCount = 0;
                                                    

                                                }

                                            }
                                        }
                                    }

                                    if (entryCoin.IsGripped && (entryCoin.GrippedBy == playerToUse.Name))
                                    {

                                        //do not let the coin exceed the bounds of the screen. This must be set explicitly and not
                                        //in reference to the top/left of the players cursor due to it still exceeding boundary

                                        if (xUI + 100 <= this.Width && xUI >= 0)
                                        {
                                            Canvas.SetLeft(coinElement, xUI);
                                        }

                                        if (yUI + 100 <= this.Height && yUI >= 0)
                                        {
                                            Canvas.SetTop(coinElement, yUI);
                                        }
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
                        }
                    }

                    foreach (Player player in usersToRemove)
                    {
                        canvas.Children.Remove(player.Cursor);
                        players.Remove(player);
                        if (players.Count == 0)
                        {
                            //start a timer that will mark the time and go back if there's no one there within 5 seconds
                            playersMissing.Start();
                            break;
                        }
                        sensor.SkeletonStream.AppChoosesSkeletons = false;

                    }


                }
            }
        }
    }
}
