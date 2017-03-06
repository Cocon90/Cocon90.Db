using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Helper;
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
        /// <para>If MySQL DB Please Add：[name="ConnectionString" providerName="${app}\Cocon90.Db.Mysql.dll|Cocon90.Db.Mysql.DbDriver" connectionString="Server=127.0.0.1;Port=3306;Database=world;Uid=root;Pwd=123456;"] in ConnectionStrings nodes.</para> 
        /// <para>If MsSql DB Please Add：[name="ConnectionString" providerName="${app}\Cocon90.Db.SqlServer.dll|Cocon90.Db.SqlServer.DbDriver" connectionString="Server=127.0.0.1;Database=test;Uid=sa;Pwd=123456;"] in ConnectionStrings nodes.</para> 
        /// <para>If Sqlite DB Please Add：[name="ConnectionString" providerName="${app}\Cocon90.Db.Sqlite.dll|Cocon90.Db.Sqlite.DbDriver" connectionString="Data Source=${app}\testdb.db3"] in ConnectionStrings nodes.</para> 
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
            if (providerArr == null || providerArr.Length < 2)
                throw new Exceptions.ConnectionException("The connection string '" + connectionStringName + "' providerName is made up of two parts: dll path | db driver namespace. like ${app}\\Cocon90.Db.SqlServer.dll|Cocon90.Db.SqlServer.DbDriver");
            var dllPath = HandlerConnectionString(providerArr[0]);
            var connectionString = HandlerConnectionString(conf.ConnectionString);
            return GetDriver(dllPath, providerArr[1], connectionString);
        }
        /// <summary>
        /// 取得加工后的连接语句
        /// </summary>
        /// <returns></returns>
        private static string HandlerConnectionString(string connString)
        {
            var appSetting = (System.Configuration.ConfigurationManager.AppSettings["IsConnectionStringEncry"] + "").Trim().ToLower();
            if (appSetting == "1" || appSetting == "true" || appSetting == "on")
            {
                connString = Des(connString, "chinapsu");
            }
            if (connString.ToLower().Contains("${app}"))
            {
                var dir = System.AppDomain.CurrentDomain.BaseDirectory;
                dir = dir.TrimEnd('\\');
                connString = connString.ToLower().Replace("${app}", dir);
            }
            foreach (var item in Enum.GetNames(typeof(Environment.SpecialFolder)))
            {
                if (connString.Contains("${" + item + "}"))
                {
                    Environment.SpecialFolder folder = Environment.SpecialFolder.CommonApplicationData;
                    if (Enum.TryParse<Environment.SpecialFolder>(item, out folder))
                    {
                        var dir = System.Environment.GetFolderPath(folder);
                        connString = connString.Replace("${" + item + "}", dir.TrimEnd('\\'));
                    }
                }
            }
            return connString;
        }

        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        private static string Des(string decryptString, string decryptKey8Letter)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey8Letter);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
    }
}
