﻿using System;
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
    /// Логика взаимодействия для BusyProgressIndicator.xaml
    /// </summary>
    public partial class BusyProgressIndicator : UserControl
    {
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                "Progress",
                typeof(double),
                typeof(BusyProgressIndicator),
                new PropertyMetadata(
                    0.0,
                    OnProgressUpdated
                ));

        private static void OnProgressUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            (d as BusyProgressIndicator).ProgressArc.EndAngle = (double)e.NewValue * 360 / 100;
        }


        public BusyProgressIndicator()
        {
            InitializeComponent();
        }
    }
}
