using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ColorPicker.Controls
{
    internal class PieShape : Shape
    {
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The inner radius of this pie piece
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The outer radius of this pie piece
        /// </summary>
        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The padding arround this pie piece
        /// </summary>
        public double Padding
        {
            get { return (double)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PushOutProperty =
            DependencyProperty.Register("PushOut", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The distance to 'push' this pie piece out from the center
        /// </summary>
        public double PushOut
        {
            get { return (double)GetValue(PushOutProperty); }
            set { SetValue(PushOutProperty, value); }
        }

        public static readonly DependencyProperty AngleDeltaProperty =
            DependencyProperty.Register("AngleDelta", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The angle delta of this pie piece in degrees
        /// </summary>
        public double AngleDelta
        {
            get { return (double)GetValue(AngleDeltaProperty); }
            set { SetValue(AngleDeltaProperty, value); }
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The start angle from the Y axis vector of this pie piece in degrees
        /// </summary>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterX", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The X coordinate of center of the circle from which this pie piece is cut
        /// </summary>
        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterY", typeof(double), typeof(PieShape),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The Y coordinate of center of the circle from which this pie piece is cut
        /// </summary>
        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        static PieShape()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PieShape), new FrameworkPropertyMetadata(typeof(PieShape)));
        }

        /// <summary>
        /// Defines the shape
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                // Creates a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawGeometry(context);
                }

                // Freezes the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        /// <summary>
        /// Draws the pie piece
        /// </summary>
        private void DrawGeometry(StreamGeometryContext context)
        {
            if (AngleDelta <= 0)
            {
                return;
            }

            double outerStartAngle = StartAngle;
            double outerAngleDelta = AngleDelta;
            double innerStartAngle = StartAngle;
            double innerAngleDelta = AngleDelta;
            Point arcCenter = new Point(CenterX, CenterY);
            Size outerArcSize = new Size(OuterRadius, OuterRadius);
            Size innerArcSize = new Size(InnerRadius, InnerRadius);

            if (AngleDelta >= 360 && Padding <= 0)
            {
                Point outerArcTopPoint = ComputeCartesianCoordinate(arcCenter, outerStartAngle, OuterRadius + PushOut);
                Point outerArcBottomPoint = ComputeCartesianCoordinate(arcCenter, outerStartAngle + 180, OuterRadius + PushOut);
                Point innerArcTopPoint = ComputeCartesianCoordinate(arcCenter, innerStartAngle, InnerRadius + PushOut);
                Point innerArcBottomPoint = ComputeCartesianCoordinate(arcCenter, innerStartAngle + 180, InnerRadius + PushOut);

                context.BeginFigure(innerArcTopPoint, true, true);
                context.LineTo(outerArcTopPoint, true, true);
                context.ArcTo(outerArcBottomPoint, outerArcSize, 0, false, SweepDirection.Clockwise, true, true);
                context.ArcTo(outerArcTopPoint, outerArcSize, 0, false, SweepDirection.Clockwise, true, true);
                context.LineTo(innerArcTopPoint, true, true);
                context.ArcTo(innerArcBottomPoint, innerArcSize, 0, false, SweepDirection.Counterclockwise, true, true);
                context.ArcTo(innerArcTopPoint, innerArcSize, 0, false, SweepDirection.Counterclockwise, true, true);
            }
            // Else draws as always
            else
            {
                if (Padding > 0)
                {
                    // Offsets the angle by the padding
                    double outerAngleVariation = (180 * (Padding / OuterRadius)) / Math.PI;
                    double innerAngleVariation = (180 * (Padding / InnerRadius)) / Math.PI;

                    outerStartAngle += outerAngleVariation;
                    outerAngleDelta -= outerAngleVariation * 2;
                    innerStartAngle += innerAngleVariation;
                    innerAngleDelta -= innerAngleVariation * 2;
                }

                Point outerArcStartPoint = ComputeCartesianCoordinate(arcCenter, outerStartAngle, OuterRadius + PushOut);
                Point outerArcEndPoint = ComputeCartesianCoordinate(arcCenter, outerStartAngle + outerAngleDelta, OuterRadius + PushOut);
                Point innerArcStartPoint = ComputeCartesianCoordinate(arcCenter, innerStartAngle, InnerRadius + PushOut);
                Point innerArcEndPoint = ComputeCartesianCoordinate(arcCenter, innerStartAngle + innerAngleDelta, InnerRadius + PushOut);

                bool largeArcOuter = outerAngleDelta > 180.0;
                bool largeArcInner = innerAngleDelta > 180.0;

                context.BeginFigure(innerArcStartPoint, true, true);
                context.LineTo(outerArcStartPoint, true, true);
                context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArcOuter, SweepDirection.Clockwise, true, true);
                context.LineTo(innerArcEndPoint, true, true);
                context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArcInner, SweepDirection.Counterclockwise, true, true);
            }
        }

        private static Point ComputeCartesianCoordinate(Point center, double angle, double radius)
        {
            // Converts to radians
            double radiansAngle = (Math.PI / 180.0) * (angle - 90);
            double x = radius * Math.Cos(radiansAngle);
            double y = radius * Math.Sin(radiansAngle);
            return new Point(x + center.X, y + center.Y);
        }
    }
}
