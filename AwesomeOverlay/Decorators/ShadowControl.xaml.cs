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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AwesomeOverlay.Decorators
{
    /// <summary>
    /// Логика взаимодействия для ShadowControl.xaml
    /// </summary>
    [ContentProperty("Child")]
    public partial class ShadowControl : UserControl
    {
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register("Child", typeof(object), typeof(ShadowControl), new PropertyMetadata());

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ShadowControl), new PropertyMetadata(default(CornerRadius)));

        public static readonly DependencyProperty ShadowDirectionProperty =
            DependencyProperty.Register("ShadowDirection", typeof(double), typeof(ShadowControl), new PropertyMetadata(315.0));

        public static readonly DependencyProperty ShadowDepthProperty =
            DependencyProperty.Register("ShadowDepth", typeof(double), typeof(ShadowControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(ShadowControl), new PropertyMetadata(5.0));

        public static readonly DependencyProperty ShadowOpacityProperty =
           DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(ShadowControl), new PropertyMetadata(1.0));

        public static readonly DependencyProperty ShadowColorProperty =
            DependencyProperty.Register("ShadowColor", typeof(Color), typeof(ShadowControl), new PropertyMetadata(Colors.Black));


        public ShadowControl()
        {
            InitializeComponent();
        }


        public object Child
        {
            get { return (object)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public double ShadowDirection
        {
            get { return (double)GetValue(ShadowDirectionProperty); }
            set { SetValue(ShadowDirectionProperty, value); }
        }

        public double ShadowDepth
        {
            get { return (double)GetValue(ShadowDepthProperty); }
            set { SetValue(ShadowDepthProperty, value); }
        }

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        public Color ShadowColor
        {
            get { return (Color)GetValue(ShadowColorProperty); }
            set { SetValue(ShadowColorProperty, value); }
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (Child is Border border) {
                Shadower.CornerRadius = border.CornerRadius;
            } else {
                Shadower.CornerRadius = CornerRadius;                
            }
        }
    }
}
