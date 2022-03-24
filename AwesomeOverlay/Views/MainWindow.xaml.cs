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

namespace AwesomeOverlay.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DpiScale _dpiScale;

        public MainWindow()
        {
            InitializeComponent();

            _dpiScale = VisualTreeHelper.GetDpi(this);
            double scaleX = 1 / _dpiScale.DpiScaleX;
            double scaleY = 1 / _dpiScale.DpiScaleY;

            FrameworkElement content = this.Content as FrameworkElement;
            content.LayoutTransform = new ScaleTransform(scaleX, scaleY);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_dpiScale.DpiScaleX > 1) {
                this.Left = 0;
            }

            if (_dpiScale.DpiScaleY > 1) {
                this.Top = 0;
            }
        }
    }
}
