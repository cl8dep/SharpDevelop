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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType;
using System;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations
{
    public partial class RelationContener : UserControl
    {
        public RelationContener(RelationBase relationControl)
        {
            RelationControl = relationControl;
            InitializeComponent();
        }

        public RelationBase RelationControl {get; private set;}

        public TypeBaseDesigner FromTypeDesigner 
        {
            get { return RelationControl.FromTypeDesigner; }
        }
        public TypeBaseDesigner ToTypeDesigner 
        {
            get { return RelationControl.ToTypeDesigner; }
        }

        internal void OnMove()
        {
            RelationControl.OnMove();
            if (GetBindingExpression(Canvas.LeftProperty) != null)
                GetBindingExpression(Canvas.LeftProperty).UpdateTarget();
            if (GetBindingExpression(Canvas.TopProperty) != null)
                GetBindingExpression(Canvas.TopProperty).UpdateTarget();
        }

        public void OnRemove()
        {
            if (Removed != null)
                Removed();
        }
        public event Action Removed;
    }
}
