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
using System.Globalization;
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public abstract class RelationToXYConverterBase : IValueConverter
    {
        protected const double MIN_SIZE = 30;
        protected const double MORE_SIZE_PER_ITEM = 5;
        protected const double ARROW_ANGLE = Math.PI / 6.0;
        protected const double ARROW_HEIGHT = 10;
        protected const double ARROW_WIDTH = 5.0;
        protected const double ARROW_LINE_MIN_ANGLE = Math.PI / 6.0;
        protected const double ARROW_LINE_MAX_ANGLE = Math.PI / 3.0;

        public abstract object Convert(object value, System.Type targetType, object parameter, CultureInfo culture);

        protected double GetAngle(double x1, double x2, double y1, double y2)
        {
            return (y2 - y1) / (Math.Abs(x2 - x1) + Math.Abs(y2 - y1)) * Math.PI / 2.0;
        }
        protected void FillArrow(double x1, double x2, double y1, double y2, out double x1Arrow, out double x2Arrow, out double y1Arrow, out double y2Arrow)
        {
            double angle = GetAngle(x1, x2, y1, y2);
            FillArrow(angle, x1, x2, y1, y2, out x1Arrow, out x2Arrow, out y1Arrow, out y2Arrow);
        }

        protected void FillArrow(double angle, double x1, double x2, double y1, double y2, out double x1Arrow, out double x2Arrow, out double y1Arrow, out double y2Arrow)
        {
            if (x1 > x2)
                angle = Math.PI - angle;
            x1Arrow = x2 - ARROW_HEIGHT * Math.Cos(angle - ARROW_ANGLE);
            x2Arrow = x2 - ARROW_HEIGHT * Math.Cos(angle + ARROW_ANGLE);
            y1Arrow = y2 - ARROW_HEIGHT * Math.Sin(angle - ARROW_ANGLE);
            y2Arrow = y2 - ARROW_HEIGHT * Math.Sin(angle + ARROW_ANGLE);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
