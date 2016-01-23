/**
 * \file		App.xaml.cs
 * \author		Colin McMillan, Aaron Vos, Greg Ward
 * \date		2015 December
 * \brief		The code behind for the game app.
 * \details		All code behind functions for the EndOfLineGame app.
 */


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect;



namespace EndOfLineGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The sensor used for the application.
        /// </summary>
        public static KinectSensor AppSensor;



        /// <summary>
        /// The event handler for exiting the application, which ensures the Kinect
        /// shuts down properly.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (AppSensor != null)
            {
                AppSensor.Stop();
            }

        }
    }
}
