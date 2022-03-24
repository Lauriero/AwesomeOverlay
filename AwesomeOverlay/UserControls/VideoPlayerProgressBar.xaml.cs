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

namespace AwesomeOverlay.UserControls
{
    /// <summary>
    /// Логика взаимодействия для VideoPlayerProgressBar.xaml
    /// </summary>
    public partial class VideoPlayerProgressBar : UserControl
    {

        public TimeSpan VideoDuration
        {
            get { return (TimeSpan)GetValue(VideoDurationProperty); }
            set { SetValue(VideoDurationProperty, value); }
        }

        /// <summary>
        /// Property for duration of playing video
        /// </summary>
        public static readonly DependencyProperty VideoDurationProperty =
            DependencyProperty.Register("VideoDuration", typeof(TimeSpan), typeof(VideoPlayerProgressBar), new PropertyMetadata(default(TimeSpan)));

        public TimeSpan Progress
        {
            get { return (TimeSpan)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// Property to set and get current playing progress
        /// Bind to MediaElement Progress property
        /// </summary>
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
                                            "Progress",
                                            typeof(TimeSpan),
                                            typeof(VideoPlayerProgressBar),
                                            new PropertyMetadata(
                                                default(TimeSpan),
                                                OnProgressUpdated    
                                            ));

        private static void OnProgressUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoPlayerProgressBar instance = d as VideoPlayerProgressBar;
            instance.ProgressBar.Width = ((TimeSpan)e.NewValue).TotalMilliseconds * instance.ActualWidth / instance.VideoDuration.TotalMilliseconds;
            instance.Circle.Margin = new Thickness(-((TimeSpan)e.NewValue).TotalMilliseconds * 10 / instance.VideoDuration.TotalMilliseconds, 0, 0, 0);
        }

        public VideoPlayerProgressBar()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Circle_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        private void DockPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Circle.Visibility = Visibility.Visible;
        }

        private void DockPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Progress != TimeSpan.Zero)
                Circle.Visibility = Visibility.Hidden;
        }
    }
}
