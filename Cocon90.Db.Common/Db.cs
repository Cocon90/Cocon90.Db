using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cocon90.Db.Common
{
    public class Db
    {
        /// <summary>
        /// Gets the data helper. 
        /// <para>If MySQL DB dbType is "Mysql" dbConnString  like "Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;" </para> 
        /// <para>If MsSql DB dbType is "SqlServer" dbConnString  like "Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;" </para> 
        /// <para>If Sqlite DB dbType is "Sqlite" dbConnString  like "Data Source=${app}\\testdb.db3" </para> 
        /// </summary>
        public static DataHelper GetDataHelper(string jsonConfigFile = "appsettings.json", string dbTypeSectionName = "dbType", string dbConnStringSectionName = "dbConnString")
        {
            var driver = GetDriver(jsonConfigFile, dbTypeSectionName, dbConnStringSectionName);
            return GetDataHelper(driver);
        }
        public static DataHelper GetDataHelper(string dbTypeName, string dbConnString)
        {
            var driver = GetDriver(dbTypeName, dbConnString);
            return GetDataHelper(driver);
        }
        public static DataHelper GetDataHelper(Driver.BaseDriver driver)
        {
            var dh = new DataHelper(driver);
            return dh;
        }
        internal static BaseDriver GetDriver(Type driverType, string connectionString)
        {
            try
            {
                connectionString = HandlerConnectionString(connectionString);
                var driver = (BaseDriver)Activator.CreateInstance(driverType, new object[] { connectionString });
                return driver;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static BaseDriver GetDriver(string dbTypeName, string dbConnString)
        {
            var assName = string.Format("Cocon90.Db.{0}", dbTypeName);
            var ass = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(assName));
            var driverTypeClassFullName = assName + ".DbDriver";
            var type = ass.GetType(driverTypeClassFullName);
            if (type == null) throw new Exceptions.DriverNotFoundException("Driver " + driverTypeClassFullName + " not found in assembly '" + assName + ".dll'");
            var driver = GetDriver(type, dbConnString);
            return driver;
        }
        /// <summary>
        /// Get dbdriver from config file.
        /// </summary>
        internal static BaseDriver GetDriver(string jsonConfigFile = "appsettings.json", string dbTypeSectionName = "dbType", string dbConnStringSectionName = "dbConnString")
        {
            var cfgPath = Path.Combine(AppContext.BaseDirectory, jsonConfigFile);
            if (!File.Exists(cfgPath)) throw new FileNotFoundException("The json configuration file '" + jsonConfigFile + "' was not found.");
            var setting = new ConfigurationBuilder().AddJsonFile(cfgPath, optional: true).Build();
            var dbConn = setting.GetSection(dbConnStringSectionName).Value;
            if (string.IsNullOrWhiteSpace(dbConn)) throw new KeyNotFoundException("'dbConnStringSectionName' " + dbConnStringSectionName + " not found in '" + cfgPath + "'");
            var dbType = setting.GetSection(dbTypeSectionName).Value;
            if (string.IsNullOrWhiteSpace(dbType)) throw new KeyNotFoundException("'dbTypeSectionName' " + dbTypeSectionName + " not found in '" + cfgPath + "'");
            var dllPath = Path.Combine(AppContext.BaseDirectory, string.Format("Cocon90.Db.{0}.dll", dbType));
            return GetDriver(dbType, dbConn);
        }

        private static string HandlerConnectionString(string connectionString)
        {
            return connectionString?.ToLower()?.Replace("${app}", AppContext.BaseDirectory);
        }
    }
}
