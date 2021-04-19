using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YR.Util
{
    public static class DataTableExtensions
    {
        #region ToList<T>
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">來源資料表</param>
        /// <param name="ignoreNoSruceColumn">是否忽略無來源資料表</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table, bool ignoreNoSruceColumn) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties, ignoreNoSruceColumn);
                result.Add(item);
            }

            return result;
        } 

        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            result = ToList<T>(table, false);

            return result;
        }

        public static List<T> ToList<T>(this DataTable table, Dictionary<string, string> mappings) where T : new()
        {
            List<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties, mappings);
                result.Add(item);
            }

            return result;
        }
        #endregion

        public static T ToItem<T>(this DataRow pdr) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            T result = new T();

            result = CreateItemFromRow<T>(pdr, properties);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pdr"></param>
        /// <param name="ignoreNoSruceColumn">true.若來源DataRow無欄位,將忽略</param>
        /// <returns></returns>
        public static T ToItem<T>(this DataRow pdr, bool ignoreNoSruceColumn) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            T result = new T();

            result = CreateItemFromRow<T>(pdr, properties, ignoreNoSruceColumn);
            return result;
        }


        #region CreateItemFromRow
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="properties"></param>
        /// <param name="ignoreNoSruceColumn">true.若來源DataRow無欄位,將忽略</param>
        /// <returns></returns>
        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties, bool ignoreNoSruceColumn) where T : new()
        {
            string msg="";
            string propertyName="";
            try
            {
                T item = new T();
                foreach (var property in properties)
                {
                    propertyName=property.Name;
                    if (!row.Table.Columns.Contains(property.Name))
                    {
                        if (ignoreNoSruceColumn)
                            continue;
                        else
                            throw (new Exception(string.Format("查找不到此DataColumn [{0}]", property.Name)));
                    }

                    if (row[property.Name] != DBNull.Value)
                        property.SetValue(item, row[property.Name], null);
                }
                return item;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(propertyName))
                    msg = propertyName+"欄位發生異常錯誤如下\n";
                msg += ex.Message;
                throw new Exception(msg);
            }
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            item = CreateItemFromRow<T>(row, properties, false);

            return item;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties, Dictionary<string, string> mappings) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (mappings.ContainsKey(property.Name))
                    property.SetValue(item, row[mappings[property.Name]], null);
            }
            return item;
        } 
        #endregion
    }
}
