using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Tiger.ORM.Utilities
{
    //public class AppConfig
    //{
    //    public string ConnectionName { get; set; }

    //    public string ConnectionString { get; set; }

    //    public string ProviderName { get; set; }

    //    private ConnectionStringSettingsCollection _connectionStringSettings;
        
    //    public AppConfig()
    //    {
    //        _connectionStringSettings = ConfigurationManager.ConnectionStrings;
    //    }

    //    public void Initialize(string nameOrConnectionString)
    //    {
    //        ConnectionStringSettings setting = _connectionStringSettings[nameOrConnectionString];
    //        if (setting == null)
    //        {
    //            ConnectionName = "TigerDefault";
    //            ConnectionString = nameOrConnectionString;
    //            ProviderName = "System.Data.SqlClient";
    //        }
    //        else
    //        {
    //            Check.NotEmpty(setting.Name, "connection name");
    //            Check.NotEmpty(setting.ConnectionString, "connection string");
    //            Check.NotEmpty(setting.ProviderName, "provider name");
    //            ConnectionName = setting.Name;
    //            ConnectionString = setting.ConnectionString;
    //            ProviderName = setting.ProviderName;
    //        }

    //    }
    //}
}
