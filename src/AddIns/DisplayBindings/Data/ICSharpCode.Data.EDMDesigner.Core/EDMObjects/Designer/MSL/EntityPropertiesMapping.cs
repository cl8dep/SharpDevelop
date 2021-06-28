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

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class EntityPropertiesMapping : PropertiesMapping
    {
        public EntityPropertiesMapping(EntityType entityType, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
            : base(entityType, table)
        {
        }

        private bool _tpc;
        public bool TPC 
        {
            get { return _tpc; }
            set 
            { 
                _tpc = value;
                if (! value)
                    EntityType.Mapping.RemoveTPCMapping();
            }
        }

        public override IEnumerable<PropertyMapping> Mappings
        {
            get
            {
                IEnumerable<ScalarProperty> scalarProperties;
                if (TPC)
                    scalarProperties = EntityType.AllScalarProperties;
                else
                {
                    scalarProperties = EntityType.ScalarProperties;
                    if (EntityType.BaseType != null)
                        scalarProperties = EntityType.BaseType.Keys.Union(scalarProperties);
                }
                return scalarProperties.Except(EntityType.Mapping.ConditionsMapping.OfType<PropertyConditionMapping>().Select(pcm => pcm.CSDLProperty)).Select(property => new PropertyMapping(property, EntityType.Mapping, Table));
            }
        }
    }
}
