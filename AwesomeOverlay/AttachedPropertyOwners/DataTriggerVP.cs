using AwesomeOverlay.Core.Utilities.ColorUtilities;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    /// <summary>
    /// When value of <see cref="BindingProperty"/> will be changed to the value, specified in <see cref="ValueProperty"/>, 
    /// the drawing, stored in the Source property of the image current properties are attached, will be painted from <see cref="FromColorProperty"/> to <see cref="ToColorProperty"/>
    /// and when value of <see cref="BindingProperty"/> will be changed to the value that is not equal to <see cref="ValueProperty"/>, the drawing will be painted from <see cref="ToColorProperty"/>
    /// to <see cref="FromColorProperty"/>. Painting can be done with color animation, specified in <see cref="AnimationProperty"/>.
    /// </summary>
    class DataTriggerVP
    {
        private static object DrawingImageCoerced(DependencyObject d, object baseValue) {
            if (baseValue is DrawingImage image && image != null)
                return image.Clone();

            return baseValue;
        }

        private static void DrawingImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Image image))
                throw new Exception($"Properties of DataTriggerVP can be attached only to {typeof(Image)} controls");

            DrawingImage drawingImage = e.NewValue as DrawingImage;
            image.Source = drawingImage;

            UpdateDrawingImage(d);
        }

        private static void UpdateDrawingImage(DependencyObject d)
        {
            DrawingImage drawingImage = GetDrawingImage(d);
            if (drawingImage == null)
                return;

            if (GetBinding(d) == GetValue(d)) {
                VectorPainter.PaintVector(drawingImage.Drawing, GetToColor(d).Color, GetAnimation(d));
            } else {
                VectorPainter.PaintVector(drawingImage.Drawing, GetFromColor(d).Color, GetAnimation(d));
            }
        }

        #region Properties
        public static readonly DependencyProperty DrawingImageProperty =
            DependencyProperty.RegisterAttached("DrawingImage", typeof(DrawingImage), typeof(DataTriggerVP), new PropertyMetadata(default(DrawingImage), DrawingImageChanged, DrawingImageCoerced));

        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.RegisterAttached("Binding", typeof(bool), typeof(DataTriggerVP), new PropertyMetadata(false, (d, e) => UpdateDrawingImage(d)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.RegisterAttached("Value", typeof(bool), typeof(DataTriggerVP), new PropertyMetadata(false, (d, e) => UpdateDrawingImage(d)));

        public static readonly DependencyProperty ToColorProperty =
            DependencyProperty.RegisterAttached("ToColor", typeof(SolidColorBrush), typeof(DataTriggerVP), new PropertyMetadata(default(SolidColorBrush), (d, e) => UpdateDrawingImage(d)));

        public static readonly DependencyProperty FromColorProperty =
            DependencyProperty.RegisterAttached("FromColor", typeof(SolidColorBrush), typeof(DataTriggerVP), new PropertyMetadata(default(SolidColorBrush), (d, e) => UpdateDrawingImage(d)));

        public static readonly DependencyProperty AnimationProperty =
            DependencyProperty.RegisterAttached("Animation", typeof(ColorAnimation), typeof(DataTriggerVP), new PropertyMetadata(default(ColorAnimation)));
        #endregion

        #region Getters and setters
        public static ColorAnimation GetAnimation(DependencyObject d) =>
          (ColorAnimation)d.GetValue(AnimationProperty);

        public static void SetAnimation(DependencyObject d, ColorAnimation value) =>
          d.SetValue(AnimationProperty, value);


        public static SolidColorBrush GetToColor(DependencyObject d) =>
          (SolidColorBrush)d.GetValue(ToColorProperty);

        public static void SetToColor(DependencyObject d, SolidColorBrush value) =>
          d.SetValue(ToColorProperty, value);


        public static SolidColorBrush GetFromColor(DependencyObject d) =>
          (SolidColorBrush)d.GetValue(FromColorProperty);

        public static void SetFromColor(DependencyObject d, SolidColorBrush value) =>
          d.SetValue(FromColorProperty, value);


        public static bool GetValue(DependencyObject d) =>
          (bool)d.GetValue(ValueProperty);

        public static void SetValue(DependencyObject d, bool value) =>
          d.SetValue(ValueProperty, value);


        public static bool GetBinding(DependencyObject d) =>
          (bool)d.GetValue(BindingProperty);

        public static void SetBinding(DependencyObject d, bool value) =>
          d.SetValue(BindingProperty, value);


        public static DrawingImage GetDrawingImage(DependencyObject d) =>
          (DrawingImage)d.GetValue(DrawingImageProperty);

        public static void SetDrawingImage(DependencyObject d, DrawingImage value) =>
          d.SetValue(DrawingImageProperty, value);
        #endregion
    }
}
