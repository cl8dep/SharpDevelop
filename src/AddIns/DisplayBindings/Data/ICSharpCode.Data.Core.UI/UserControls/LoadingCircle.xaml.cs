﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#region Usings

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for LoadingCircle.xaml
    /// </summary>
    public partial class LoadingCircle : UserControl
    {
        #region Fields

        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register("IsActivated", typeof(bool), typeof(LoadingCircle), new FrameworkPropertyMetadata(false, IsActivatedChanged));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets if the LoadingCircle is activated.
        /// </summary>
        public bool IsActivated
        {
            get { return (bool)GetValue(LoadingCircle.IsActivatedProperty); }
            set { SetValue(LoadingCircle.IsActivatedProperty, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoadingCircle()
        {
            InitializeComponent();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Returns the standard size (20x20).
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(Math.Min(20, constraint.Width), Math.Min(20, constraint.Height));
        }
        
        /// <summary>
        /// Called to arange and size the content of the LoadingCircle.
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // Save ellipse for later re-creation
            Ellipse backgroundEllipse = BackgroundEllipse;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(backgroundEllipse);
            double lengthOfOneSide = Math.Min(arrangeBounds.Height, arrangeBounds.Width);
            double outerRadius = lengthOfOneSide / 2d;
            double innerRadius = outerRadius - outerRadius / 2.4;
            BackgroundEllipse.Height = BackgroundEllipse.Width = lengthOfOneSide;
            BackgroundEllipse.StrokeThickness = outerRadius - innerRadius;
            CanvasRotateTransformation.CenterX = CanvasRotateTransformation.CenterY = outerRadius;

            Point middlePoint = new Point(outerRadius, outerRadius);

            int numberOfStrokes = (int)Math.Round(lengthOfOneSide * 26d / 20d);
            double angleFactorPerStroke = 360d / (double)numberOfStrokes;
            double transparentFactorPerStroke = 1d / (double)numberOfStrokes;
            double currentTransparentFactor = 1;
            for (double angle = 0, i = 1; angle < 360; angle += angleFactorPerStroke, i++)
            {
                double angleInRad = Math.PI * angle / 180d;
                Line line = new Line();
                line.Opacity = currentTransparentFactor;
                line.Stroke = this.Foreground;
                line.StrokeThickness = 3;
                line.X1 = middlePoint.X + outerRadius * Math.Cos(angleInRad);
                line.Y1 = middlePoint.Y + outerRadius * Math.Sin(angleInRad);
                line.X2 = middlePoint.X + innerRadius * Math.Cos(angleInRad);
                line.Y2 = middlePoint.Y + innerRadius * Math.Sin(angleInRad);
                MainCanvas.Children.Add(line);
                currentTransparentFactor -= transparentFactorPerStroke;
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Starts/Stops animation.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsActivatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadingCircle lc = (LoadingCircle)d;
            if ((bool)e.NewValue)
            {
                ((Storyboard)lc.FindResource("OpacityDoubleAnimationStoryboardToUnvisible")).Stop(lc);
                ((Storyboard)lc.FindResource("RenderDoubleAnimationStoryboard")).Begin(lc, true);
                ((Storyboard)lc.FindResource("OpacityDoubleAnimationStoryboardToVisible")).Begin(lc, true);
            }
            else
            {
                ((Storyboard)lc.FindResource("OpacityDoubleAnimationStoryboardToVisible")).Stop(lc);
                ((Storyboard)lc.FindResource("OpacityDoubleAnimationStoryboardToUnvisible")).Begin(lc, true);
            }
        }

        /// <summary>
        /// Stops the rotation animation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeactivationCompleted(object sender, EventArgs e)
        {
            ((Storyboard)this.FindResource("RenderDoubleAnimationStoryboard")).Stop(this);
        }

        #endregion
    }
}
