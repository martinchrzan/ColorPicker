using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColorPicker.Controls
{
    public class CircularMenuItem : Button
    {
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, UpdateItemRendering));

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, UpdateItemRendering));

        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.Register("CenterX", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double CenterX
        {
            get { return (double)GetValue(CenterXProperty); }
            set { SetValue(CenterXProperty, value); }
        }

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.Register("CenterY", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double CenterY
        {
            get { return (double)GetValue(CenterYProperty); }
            set { SetValue(CenterYProperty, value); }
        }

        public static readonly DependencyProperty OuterRadiusProperty =
            DependencyProperty.Register("OuterRadius", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double OuterRadius
        {
            get { return (double)GetValue(OuterRadiusProperty); }
            set { SetValue(OuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public new static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public new double Padding
        {
            get { return (double)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty EdgeOuterRadiusProperty =
            DependencyProperty.Register("EdgeOuterRadius", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double EdgeOuterRadius
        {
            get { return (double)GetValue(EdgeOuterRadiusProperty); }
            set { SetValue(EdgeOuterRadiusProperty, value); }
        }

        public static readonly DependencyProperty EdgeInnerRadiusProperty =
            DependencyProperty.Register("EdgeInnerRadius", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double EdgeInnerRadius
        {
            get { return (double)GetValue(EdgeInnerRadiusProperty); }
            set { SetValue(EdgeInnerRadiusProperty, value); }
        }

        public static readonly DependencyProperty EdgePaddingProperty =
            DependencyProperty.Register("EdgePadding", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double EdgePadding
        {
            get { return (double)GetValue(EdgePaddingProperty); }
            set { SetValue(EdgePaddingProperty, value); }
        }

        public static readonly DependencyProperty EdgeBackgroundProperty =
            DependencyProperty.Register("EdgeBackground", typeof(Brush), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Brush EdgeBackground
        {
            get { return (Brush)GetValue(EdgeBackgroundProperty); }
            set { SetValue(EdgeBackgroundProperty, value); }
        }

        protected static readonly DependencyPropertyKey AngleDeltaPropertyKey =
            DependencyProperty.RegisterReadOnly("AngleDelta", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(360.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty AngleDeltaProperty = AngleDeltaPropertyKey.DependencyProperty;

        public double AngleDelta
        {
            get { return (double)GetValue(AngleDeltaProperty); }
            protected set { SetValue(AngleDeltaPropertyKey, value); }
        }

        protected static readonly DependencyPropertyKey StartAnglePropertyKey =
            DependencyProperty.RegisterReadOnly("StartAngle", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty StartAngleProperty = StartAnglePropertyKey.DependencyProperty;

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            protected set { SetValue(StartAnglePropertyKey, value); }
        }

        protected static readonly DependencyPropertyKey RotationPropertyKey =
            DependencyProperty.RegisterReadOnly("Rotation", typeof(double), typeof(CircularMenuItem),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RotationProperty = RotationPropertyKey.DependencyProperty;

        public double Rotation
        {
            get { return (double)GetValue(RotationProperty); }
            protected set { SetValue(RotationPropertyKey, value); }
        }

        protected static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(System.Drawing.Color), typeof(CircularMenuItem));

        public System.Drawing.Color Color
        {
            get { return (System.Drawing.Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        static CircularMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularMenuItem), new FrameworkPropertyMetadata(typeof(CircularMenuItem)));
        }

        private static void UpdateItemRendering(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CircularMenuItem item = d as CircularMenuItem;
            if (item != null)
            {
                var angleDelta = 360.0 / item.Count;
                var startAngle = angleDelta * item.Index;
                var rotation = startAngle + angleDelta / 2;

                item.AngleDelta = angleDelta;
                item.StartAngle = startAngle;
                item.Rotation = rotation;
            }
        }
    }
}
