using Cocon90.Db.Common.Data;
using Cocon90.Db.Common.Data.Schema;
using Cocon90.Db.Common.Driver;
using Cocon90.Db.Common.Helper;
using Cocon90.Db.Common.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace System
{
    public static class DataHelperBaseExtension
    {
        /// <summary>
        /// Gets the boolean. if value is all,1,true,yes,t  return true.otherwise is false.
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns><c>true</c> if on, 1, true, yes, t, <c>false</c> otherwise.</returns>
        public static bool GetBoolean(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return TypeConverter.ToBool(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "");
        }

        /// <summary>
        /// Gets the date time. if convert failed return '1970-01-01 0:0:0'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetDateTime(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            DateTime temp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            if (DateTime.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the date time offset. if convert failed return 'DateTimeOffset.MinValue'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DateTimeOffset.</returns>
        public static DateTimeOffset GetDateTimeOffset(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            DateTimeOffset temp = DateTimeOffset.MinValue;
            if (DateTimeOffset.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the float. if convert failed return '-1'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Single.</returns>
        public static float GetFloat(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            float temp = -1;
            if (float.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the double. if convert failed return '-1'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Double.</returns>
        public static double GetDouble(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            double temp = -1;
            if (double.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the int. if convert failed return '-1'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Int32.</returns>
        public static int GetInt(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            int temp = -1;
            if (int.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the decimal. if convert failed return '-1'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal GetDecimal(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            decimal temp = -1;
            if (decimal.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the long. if convert failed return '-1'
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Int64.</returns>
        public static long GetLong(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            long temp = -1;
            if (long.TryParse(dh.GetScalar(tsqlParamed, paramKeyAndValue) + "", out temp)) return temp;
            return temp;
        }

        /// <summary>
        /// Gets the string. if convert failed return [string.Empty]
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.String.</returns>
        public static string GetString(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return dh.GetScalar(tsqlParamed, paramKeyAndValue) + "";
        }

        /// <summary>
        /// Gets the table. 
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DataTable.</returns>
        public static MDataTable GetTable(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            MDataTable dt = null;
            var ds = dh.GetDataSet(tsqlParamed, paramKeyAndValue);
            if (ds.Tables != null && ds.Tables.Count > 0)
            { dt = ds.Tables[0]; }
            return dt;
        }

        /// <summary>
        /// Gets the list string.
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> GetListString(this DataHelper dh, bool isDistinct, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            List<string> list = new List<string>();
            var dt = GetTable(dh, tsqlParamed, paramKeyAndValue);
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) { return list; }
            foreach (MRow row in dt.Rows)
            {
                if (isDistinct)
                {
                    if (!list.Contains(row[0] + "")) { list.Add(row[0] + ""); }
                }
                else { list.Add(row[0] + ""); }
            }
            return list;
        }
    }
}
