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

namespace AwesomeOverlay.Views.Users
{
    /// <summary>
    /// Логика взаимодействия для DetailedUserView.xaml
    /// </summary>
    public partial class DetailedUserView : UserControl
    {
        public DetailedUserView()
        {
            InitializeComponent();
        }

        private void UserCard_MouseEnter(object sender, MouseEventArgs e)
        {
            ControlButtonsPanel.Visibility = Visibility.Visible;
            UserServiceIcon.Visibility = Visibility.Collapsed;
        }

        private void UserCard_MouseLeave(object sender, MouseEventArgs e)
        {
            UserServiceIcon.Visibility = Visibility.Visible;
            ControlButtonsPanel.Visibility = Visibility.Collapsed;
        }
    }
}
