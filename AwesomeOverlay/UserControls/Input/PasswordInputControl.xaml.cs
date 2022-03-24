using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
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

namespace AwesomeOverlay.UserControls.Input
{
    /// <summary>
    /// Логика взаимодействия для TextInputControl.xaml
    /// </summary>
    public partial class PasswordInputControl : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty FieldDescriptionProperty =
            DependencyProperty.Register("FieldDescription", typeof(string), typeof(PasswordInputControl), new PropertyMetadata(""));

        public static readonly DependencyProperty PlaceholderProperty = 
            DependencyProperty.Register("Placeholder", typeof(string), typeof(PasswordInputControl), new PropertyMetadata(""));

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(SecureString), typeof(PasswordInputControl), new PropertyMetadata(default(SecureString)));

        public static readonly DependencyProperty ClearTriggerProperty = 
            DependencyProperty.Register("ClearTrigger", typeof(object), typeof(PasswordInputControl), new PropertyMetadata(default, TriggerPropertyChanged));

        private static void TriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PasswordInputControl).PassBox.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PasswordInputControl()
        {
            InitializeComponent();
        }

        public string FieldDescription
        {
            get { return (string)GetValue(FieldDescriptionProperty); }
            set { SetValue(FieldDescriptionProperty, value); }
        }

        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        /// <summary>
        /// When this property changed password box cleared
        /// </summary>
        public object ClearTrigger
        {
            get { return (object)GetValue(ClearTriggerProperty); }
            set { SetValue(ClearTriggerProperty, value); }
        }
       
        public SecureString Value
        {
            get { return (SecureString)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private bool _textBoxFocused = false;
        public bool TextBoxFocused
        {
            get => _textBoxFocused;
            set {
                _textBoxFocused = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBoxFocused"));
            }
        }

        private void PassBox_GotFocus(object sender, RoutedEventArgs e) =>
            TextBoxFocused = true;

        private void PassBox_LostFocus(object sender, RoutedEventArgs e) =>
            TextBoxFocused = false;

        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PassBox.Password == "") {
                PlaceholderTextBlock.Visibility = Visibility.Visible;
            } else {
                PlaceholderTextBlock.Visibility = Visibility.Hidden;
            }

            Value = PassBox.SecurePassword;
        }
    }
}
