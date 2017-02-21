using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common
{
    public class Db
    {
        /// <summary>
        /// Gets the data helper. 
        /// <para>If MySQL DB Please Add：[name="ConnectionString" providerName="Cocon90.Db.Mysql.dll|Cocon90.Db.Mysql.DbDriver" connectionString="Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;"] in ConnectionStrings nodes.</para> 
        /// <para>If MsSql DB Please Add：[name="ConnectionString" providerName="Cocon90.Db.SqlServer.dll|Cocon90.Db.SqlServer.DbDriver" connectionString="Server=127.0.0.1;Database=test;Uid=sa;Pwd=123456;"] in ConnectionStrings nodes.</para> 
        /// </summary>
        public static DataHelper GetDataHelper(string connectionStringName = "ConnectionString")
        {
            var driver = GetDriver(connectionStringName);
            return GetDataHelper(driver);
        }
        public static DataHelper GetDataHelper(string dllPath, string driverTypeClassFullName, string connectionString)
        {
            var driver = GetDriver(dllPath, driverTypeClassFullName, connectionString);
            return GetDataHelper(driver);
        }
        public static DataHelper GetDataHelper(Driver.BaseDriver driver)
        {
            var dh = new DataHelper(driver);
            return dh;
        }
        public static BaseDriver GetDriver(Type driverType, string connectionString)
        {
            try
            {
                var driver = (BaseDriver)Activator.CreateInstance(driverType, new object[] { connectionString });
                return driver;
            }
            catch (Exception ex) { throw ex; }
        }
        public static BaseDriver GetDriver(string dllPath, string driverTypeClassFullName, string connectionString)
        {
            var ass = System.Reflection.Assembly.LoadFrom(dllPath);
            var type = ass.GetType(driverTypeClassFullName);
            if (type == null) throw new Exceptions.DriverNotFoundException("Driver " + driverTypeClassFullName + " not found in library " + dllPath + ".");
            var driver = GetDriver(type, connectionString);
            return driver;
        }
        public static BaseDriver GetDriver(string connectionStringName = "ConnectionString")
        {
            var conf = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];
            if (conf == null) throw new KeyNotFoundException("ConnectionStringName " + connectionStringName + " not found.");
            var providerArr = conf.ProviderName?.Split('|');
            return GetDriver(providerArr[0], providerArr[1], conf.ConnectionString);
        }

    }
}
