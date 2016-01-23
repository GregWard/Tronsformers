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
    public partial class MainWindow : Window
    {

        private void WelcomeDone(object sender, EventArgs e)
        {
            if ((string)Hello.Content != "OSB, drag a coin to the slot to lockyourself in.")
            {
                Hello.Content = "OSB, drag a coin to the slot to lockyourself in.";

                DoubleAnimation dblAnim = new DoubleAnimation(0, 1,new Duration(new TimeSpan(0, 0, 2)));
                dblAnim.Completed += DblAnim_Completed;

                Hello.BeginAnimation(Label.OpacityProperty, dblAnim);
               
            }
        }

        private void DblAnim_Completed(object sender, EventArgs e)
        {
            entryCoin = new Coin();

            canvas.Children.Add(entryCoin.CoinShape);

            Canvas.SetLeft(entryCoin.CoinShape, this.Width/2);
            Canvas.SetTop(entryCoin.CoinShape, this.Height / 2);

            DoubleAnimation dblAnim = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0,0,300)));
            entryCoin.CoinShape.BeginAnimation(Ellipse.OpacityProperty, dblAnim);
        }
    }
}
