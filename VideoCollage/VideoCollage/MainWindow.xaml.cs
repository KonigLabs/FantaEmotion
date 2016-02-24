using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VideoCollage.ViewModels;

namespace VideoCollage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();          
          
        }

      

        private void MediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = TimeSpan.FromSeconds(0);
            ((MediaElement)sender).Play();

        }

        private void MediaElement_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = TimeSpan.FromSeconds(0);
            ((MediaElement)sender).Play();
        }
    }

    
}
