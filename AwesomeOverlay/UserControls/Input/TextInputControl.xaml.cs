using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AwesomeOverlay.UserControls.Input
{
    /// <summary>
    /// Логика взаимодействия для TextInputControl.xaml
    /// </summary>
    public partial class TextInputControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty FieldDescriptionProperty =
            DependencyProperty.Register("FieldDescription", typeof(string), typeof(TextInputControl), new PropertyMetadata(""));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(TextInputControl), new PropertyMetadata(""));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(TextInputControl), new PropertyMetadata(""));

        public event PropertyChangedEventHandler PropertyChanged;

        public TextInputControl()
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

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set {
                if (value != Value) {
                    SetValue(ValueProperty, value);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        private bool _textBoxFocused = false;
        public bool TextBoxFocused
        {
            get => _textBoxFocused;
            set {
                if (_textBoxFocused != value) {
                    _textBoxFocused = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBoxFocused"));
                }
            }
        }

        private void PlainText_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBoxFocused = true;
        }

        private void PlainText_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBoxFocused = false;
        }

        private void PlainText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PlainText.Text == "") {
                PlaceholderTextBlock.Visibility = Visibility.Visible;
            } else {
                PlaceholderTextBlock.Visibility = Visibility.Hidden;
            }
        }
    }
}
