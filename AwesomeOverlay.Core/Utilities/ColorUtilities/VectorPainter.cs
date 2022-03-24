using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.Core.Utilities.ColorUtilities
{
    /// <summary>
    /// This class contains static methods which allow you to color all parts of svg image in certain color
    /// </summary>
    public static class VectorPainter
    {
        /// <summary>
        /// Colors vector image in destColor using animation
        /// </summary>
        /// <param name="vectorDrawing">Coloring vector</param>
        /// <param name="destColor">Destination color</param>
        /// <param name="animation">Animation for coloring (to and from values are unnessassary to define and will be changed for coloring purposes)</param>
        public static void PaintVector(Drawing vectorDrawing, Color destColor, ColorAnimation animation = null)
        {
            List<GeometryDrawing> geometries = new List<GeometryDrawing>();

            if (vectorDrawing is GeometryDrawing geometryDrawing)
                geometries.Add(geometryDrawing);
            else if (vectorDrawing is DrawingGroup drawingGroup)
                geometries.AddRange(GetDrawableGeometries(drawingGroup));
            else
                throw new ArgumentException($"Vector drawign must be either {typeof(GeometryDrawing)} or {typeof(DrawingGroup)}");

            geometries = geometries.Where(g => g.Brush != null && g.Brush is SolidColorBrush).ToList();
            if (geometries.Count == 0)
                return;

            if (animation == null) {
                foreach (GeometryDrawing drawing in geometries) {
                    (drawing.Brush as SolidColorBrush).Color = destColor;
                }

                return;
            }

            animation.To = destColor;
            foreach (GeometryDrawing drawing in geometries) { 
                drawing.Brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
        }

        private static IEnumerable<GeometryDrawing> GetDrawableGeometries(DrawingGroup drawingGroup)
        {
            foreach (Drawing drawing in drawingGroup.Children) {
                if (drawing is DrawingGroup childDrawingGroup) {
                    foreach (GeometryDrawing geometryDrawing in GetDrawableGeometries(childDrawingGroup)) {
                        yield return geometryDrawing;
                    }
                } else if (drawing is GeometryDrawing geometryDrawing) {
                    yield return geometryDrawing;
                } else {
                    continue;
                }
            }
        }
    }
}
