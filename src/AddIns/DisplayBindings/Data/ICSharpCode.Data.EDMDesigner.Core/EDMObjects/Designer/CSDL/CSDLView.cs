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
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL
{
    public class CSDLView
    {
        private IndexableUIBusinessTypeObservableCollection<EntityType, UIEntityType> _entityTypes;
        private UIEntityType _hierarchyType;
        private ObservableCollection<UIEntityType> _hierarchyTypes;
        private IndexableUIBusinessTypeObservableCollection<ComplexType, UIComplexType> _complexTypes;

        public EDMView EDMView { get; internal set; }
        public CSDLContainer CSDL { get; internal set; }

        #region Entity Types
        public IndexableUIBusinessTypeObservableCollection<EntityType, UIEntityType> EntityTypes
        {
            get
            {
                if (_entityTypes == null)
                {
                    _entityTypes = new IndexableUIBusinessTypeObservableCollection<EntityType, UIEntityType>(CSDL.EntityTypes.Select(et => new UIEntityType(this, et)));
                    CSDL.EntityTypes.ItemAdded += newEntityType => _entityTypes.GetAndAddIfNotExist(newEntityType, net => new UIEntityType(this, net));
                    CSDL.EntityTypes.ItemRemoved += oldEntityType => _entityTypes.Remove(_entityTypes[oldEntityType]);
                }
                return _entityTypes;
            }
        }

        internal void EntityTypesPropertyChanged()
        {
            EntityTypes.Refresh();
        }

        public UIEntityType HierarchyType
        {
            get { return _hierarchyType; }
            set
            {
                _hierarchyType = value;
                HierarchyTypes.Clear();
                if (value == null)
                    return;
                value.Bold = true;
                EntityType baseType;
                while ((baseType = value.BusinessInstance.BaseType) != null)
                    value = new UIEntityType(this, baseType, value);
                HierarchyTypes.Add(value);
            }
        }

        public ObservableCollection<UIEntityType> HierarchyTypes
        {
            get
            {
                if (_hierarchyTypes == null)
                    _hierarchyTypes = new ObservableCollection<UIEntityType>();
                return _hierarchyTypes;
            }
        }
        #endregion Entity Types

        #region Complex Types
        public IndexableUIBusinessTypeObservableCollection<ComplexType, UIComplexType> ComplexTypes
        {
            get
            {
                if (_complexTypes == null)
                {
                    _complexTypes = new IndexableUIBusinessTypeObservableCollection<ComplexType, UIComplexType>(CSDL.ComplexTypes.Select(ct => new UIComplexType(this, ct)));
                    CSDL.ComplexTypes.ItemAdded += newComplexType => _complexTypes.GetAndAddIfNotExist(newComplexType, nct => new UIComplexType(this, nct));
                    CSDL.ComplexTypes.ItemRemoved += oldComplexType => _complexTypes.Remove(_complexTypes[oldComplexType]);
                }
                return _complexTypes;
            }
        }
        #endregion Complex Types

        #region CSDL Functions
        public ObservableCollection<Function> Functions
        {
            get
            {
                return CSDL.Functions;
            }
        }
        #endregion CSDL Functions

        public UIAssociation AddAssociation(string associationName, string navigationProperty1Name, UIEntityType navigationProperty1EntityType, Cardinality navigationProperty1Cardinality, string navigationProperty2Name, UIEntityType navigationProperty2EntityType, Cardinality navigationProperty2Cardinality)
        {
            var association = CSDL.AddAssociation(associationName, navigationProperty1Name, navigationProperty1EntityType.BusinessInstance, navigationProperty1Cardinality, navigationProperty2Name, navigationProperty2EntityType.BusinessInstance, navigationProperty2Cardinality);
            return new UIAssociation { NavigationProperty1 = navigationProperty1EntityType.Properties[association.PropertyEnd1] as UIRelatedProperty, NavigationProperty2 = navigationProperty2EntityType.Properties[association.PropertyEnd2] as UIRelatedProperty };
        }

        public UIEntityType AddEntityType(string entityTypeName, UIEntityType baseType)
        {
            return EntityTypes[CSDL.AddEntityType(entityTypeName, baseType != null ? null : string.Concat(entityTypeName, "Set"), baseType == null ? null : baseType.BusinessInstance)];
        }

        public UIComplexType AddComplexType(string complexTypeName)
        {
            return ComplexTypes[CSDL.AddComplexType(complexTypeName)];
        }

        public void DeleteType(IUIType uiType)
        {
            var entityType = uiType.BusinessInstance as EntityType;
            if (entityType != null)
                CSDL.EntityTypes.Remove(entityType);
            var complexType = uiType.BusinessInstance as ComplexType;
            if (complexType != null)
                CSDL.ComplexTypes.Remove(complexType);
            OnTypeDeleted(uiType);
        }

        protected virtual void OnTypeDeleted(IUIType uiType)
        {
            if (TypeDeleted != null)
                TypeDeleted(uiType);
        }

        public event Action<IUIType> TypeDeleted;
    }
}
