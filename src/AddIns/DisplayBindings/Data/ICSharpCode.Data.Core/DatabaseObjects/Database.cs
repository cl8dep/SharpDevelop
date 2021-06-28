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
using System.ComponentModel;
using System.Linq;
using ICSharpCode.Data.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    /// <summary>
    /// Description of Database.
    /// </summary>
    public class Database : DatabaseObjectBase<DatabaseObjectBase>, IDatabase
    {
        #region Fields

        private IDatasource _datasource = null;
        private DatabaseObjectBase<ITable> _tables = null;
        private DatabaseObjectBase<IView> _views = null;
        private DatabaseObjectBase<IProcedure> _procedures = null;
        private DatabaseObjectBase<IConstraint> _constraints = null;

        #endregion

        #region Properties

        public IDatasource Datasource
        {
            get { return _datasource; }
        }

        public string ConnectionString
        {
            get { return _datasource.ConnectionString + ";Initial Catalog=" + _name; }
        }

        public DatabaseObjectsCollection<ITable> Tables
        {
            get 
            {
                if (_tables != null)
                    return _tables.Items;
                else
                    return null;
            }
        }

        public DatabaseObjectsCollection<IView> Views
        {
            get 
            {
                if (_views != null)
                    return _views.Items;
                else
                    return null;
            }
        }

        public DatabaseObjectsCollection<IProcedure> Procedures
        {
            get
            {
                if (_procedures != null)
                    return _procedures.Items;
                else
                    return null;
            }
        }

        public DatabaseObjectsCollection<IConstraint> Constraints
        {
            get
            {
                if (_constraints != null)
                    return _constraints.Items;
                else
                    return null;
            }
        }

        #endregion

        #region Constructor

        public Database(IDatasource datasource)
        {
            _datasource = datasource;
        }
        
        #endregion

        #region Methods

        public bool LoadDatabase()
        {
            try
            {
                _constraints = new DatabaseObjectBase<IConstraint>();
                _constraints.Name = "Constraints";

                _tables = new DatabaseObjectBase<ITable>();
                _tables.Name = "Tables";
                _tables.Items = Datasource.DatabaseDriver.LoadTables(this);
                Items.Add(_tables);
                OnPropertyChanged("Tables");
                OnPropertyChanged("Constraints");
                OnPropertyChanged("UserDefinedDataTypes");

                _views = new DatabaseObjectBase<IView>();
                _views.Name = "Views";
                _views.Items = Datasource.DatabaseDriver.LoadViews(this);
                Items.Add(_views);
                OnPropertyChanged("Views");

                _procedures = new DatabaseObjectBase<IProcedure>();
                _procedures.Name = "Procedures";
                _procedures.Items = Datasource.DatabaseDriver.LoadProcedures(this);
                Items.Add(_procedures);
                OnPropertyChanged("Procedures");

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Error while trying to load database '{0}'.\n\n" + exception.Message, Name), _datasource.DatabaseDriver.Name , MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
        }

        #endregion
    }
}
