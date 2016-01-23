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
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;

namespace TestUI
{
    public partial class DemoPage : Page
    {
        Skeleton[] skeletons = new Skeleton[0];
        Dictionary<int, long> skeletonsInFrame = new Dictionary<int, long>();
        /// <summary>
        /// How I will handle the skeletons tracking for this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="skeletonFrameReadyEventArgs"></param>
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
                                        NavigationService.Navigate(new InitialDetectionPage());
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
}
