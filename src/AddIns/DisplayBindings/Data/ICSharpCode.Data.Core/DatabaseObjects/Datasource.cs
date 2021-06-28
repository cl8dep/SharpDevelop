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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Data.SqlClient;

#endregion

namespace ICSharpCode.Data.Core.DatabaseObjects
{
    public abstract class Datasource : DatabaseObjectBase, IDatasource
    {
        #region Fields

        private IDatabaseDriver _databaseDriver = null;
        private string _providerManifestToken = string.Empty;

        private Dictionary<string, string> _connectionStringSettings = new Dictionary<string, string>();
        private string _userDefinedConnectionString = string.Empty;
        private bool _useUserDefinedConnectionString = false;

        private ObservableCollection<IDatabase> _databases = null;

        #endregion

        #region Properties

        public IDatabaseDriver DatabaseDriver
        {
            get { return _databaseDriver; }
        }

        public virtual string ProviderName
        {
            get { throw new NotImplementedException(); }
        }

        public virtual string ProviderManifestToken
        {
            get { return _providerManifestToken; }
            set
            {
                _providerManifestToken = value;
                OnPropertyChanged("ProviderManifestToken");
            }
        }

        public string ConnectionString
        {
            get 
            {
                if (_useUserDefinedConnectionString)
                    return UserDefinedConnectionString;
                
                string connectionString = string.Format("Data Source={0};", _name);

                foreach (string key in _connectionStringSettings.Keys)
                {
                    connectionString += key + "=" + _connectionStringSettings[key] + ";";
                }

                return connectionString.Remove(connectionString.Length - 1).Trim();
            }
        }

        public Dictionary<string, string> ConnectionStringSettings
        {
            get { return _connectionStringSettings; }
            set { _connectionStringSettings = value; }
        }

        public string UserDefinedConnectionString
        {
            get { return _userDefinedConnectionString; }
            set
            {
                _userDefinedConnectionString = value;
                
                OnPropertyChanged("UserDefinedConnectionString");
                
                if (_useUserDefinedConnectionString)
                    OnPropertyChanged("ConnectionString");
            }
        }

        public bool UseUserDefinedConnectionString
        {
            get { return _useUserDefinedConnectionString; }
            set
            {
                if (value && string.IsNullOrEmpty(_userDefinedConnectionString))
                    UserDefinedConnectionString = ConnectionString;
                
                _useUserDefinedConnectionString = value;
                OnPropertyChanged("UseUserDefinedConnectionString");
            }
        }

        public virtual UserControl ControlPanel
        {
            get { return null; }
        }

        public ObservableCollection<IDatabase> Databases
        {
            get { return _databases; }
            set
            {
                _databases = value;
                OnPropertyChanged("Databases");
            }
        }

        #endregion

        #region Constructor

        public Datasource(IDatabaseDriver databaseDriver)
        {
            _databaseDriver = databaseDriver;
        }

        #endregion

        #region Methods

        public bool PopulateDatabases()
        {
            try
            {
                _databaseDriver.PopulateDatabases(this);
                return true;
            }
            catch (Exception exception)
            {
                return HandlePopulateDatabasesException(exception);
            }
        }

        protected virtual bool HandlePopulateDatabasesException(Exception exception)
        {
            return false;
        }

        public string GetConnectionStringSetting(string setting)
        {
            if (_connectionStringSettings.ContainsKey(setting))
                return _connectionStringSettings[setting];
            else
                return null;
        }

        public string GetConnectionStringSetting(string setting, string defaultValue)
        {
            if (_connectionStringSettings.ContainsKey(setting))
                return _connectionStringSettings[setting];
            else
                return defaultValue;
        }

        public void SetConnectionStringSetting(string setting, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (_connectionStringSettings.ContainsKey(setting))
                    _connectionStringSettings.Remove(setting);
            }
            else
            {
                if (_connectionStringSettings.ContainsKey(setting))
                    _connectionStringSettings[setting] = value;
                else
                    _connectionStringSettings.Add(setting, value);
            }

            OnPropertyChanged("ConnectionString");
        }

        public override string ToString()
        {
            return _name;
        }

        #endregion
    }
}
