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

namespace TestUI
{
    public static class KinectHelper
    {
        public static StackPanel CreateCursor(double height, double width, SolidColorBrush color, string owner)
        {
            StackPanel cursor = new StackPanel();

            Ellipse cursorImage = new Ellipse();

            cursorImage.Height = height;
            cursorImage.Width = width;
            cursorImage.Fill = color;
            cursorImage.Name = "cursorIcon";
            cursor.Name = owner;

            cursor.Children.Add(cursorImage);

            return cursor;
        }
    }
}
