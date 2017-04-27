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
        /// Gets the boolean. 
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns><c>true</c> if on, 1, true, yes, t, <c>false</c> otherwise.</returns>
        public static bool GetBoolean(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return Convert.ToBoolean(dh.GetScalar(tsqlParamed, paramKeyAndValue));
        }

        /// <summary>
        /// Gets the date time. if convert failed threw exception.
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>DateTime.</returns>
        public static DateTime GetDateTime(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return Convert.ToDateTime(dh.GetScalar(tsqlParamed, paramKeyAndValue));
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
            return new DateTimeOffset(Convert.ToDateTime(dh.GetScalar(tsqlParamed, paramKeyAndValue)));
        }

        /// <summary>
        /// Gets the float. if convert failed threw exception.
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Single.</returns>
        public static float GetFloat(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return Convert.ToSingle(dh.GetScalar(tsqlParamed, paramKeyAndValue));
        }

        /// <summary>
        /// Gets the double. if convert failed threw exception.
        /// </summary>
        /// <param name="dh">The dh.</param>
        /// <param name="tsqlParamed">The TSQL paramed.</param>
        /// <param name="paramKeyAndValue">The parameter key and value.</param>
        /// <returns>System.Double.</returns>
        public static double GetDouble(this DataHelper dh, string tsqlParamed, params Params[] paramKeyAndValue)
        {
            return Convert.ToDouble(dh.GetScalar(tsqlParamed, paramKeyAndValue));
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
            return Convert.ToInt32(dh.GetScalar(tsqlParamed, paramKeyAndValue));
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
            return Convert.ToDecimal(dh.GetScalar(tsqlParamed, paramKeyAndValue));
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
            return Convert.ToInt64(dh.GetScalar(tsqlParamed, paramKeyAndValue));
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
                    if (!list.Contains(row[0].ValueOfString())) { list.Add(row[0].ValueOfString()); }
                }
                else { list.Add(row[0].ValueOfString()); }
            }
            return list;
        }
    }
}
