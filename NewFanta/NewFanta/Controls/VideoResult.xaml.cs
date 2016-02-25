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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewFanta.Controls
{
    /// <summary>
    /// Interaction logic for VideoResult.xaml
    /// </summary>
    public partial class VideoResult : UserControl
    {
        public VideoResult()
        {
            InitializeComponent();
        }

        public event EventHandler Repeat;
        public event EventHandler Continue;

        private void OnRepeat(object sender, RoutedEventArgs e)
        {
            if (Repeat != null)
                Repeat(this, new EventArgs());
        }

        private void OnContinue(object sender, RoutedEventArgs e)
        {
            if (Continue != null)
                Continue(this, new EventArgs());
        }

        private void PreviewVideoMediaEnded(object sender, RoutedEventArgs e)
        {
            PreviewVideo.Position = TimeSpan.FromSeconds(0);
            PreviewVideo.Play();
        }
    }
}
