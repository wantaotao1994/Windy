using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Winter.ORM.OrmAttributes;

namespace Windy.Core
{
    public static class CustomAttributeHelper
    {
        /// <summary>
        /// Cache Data
        /// </summary>
        private static readonly Dictionary<string, string> Cache = new Dictionary<string, string>();


        /// <summary>
        /// 获取tablename
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableNameAttributeValue(this Type type)
        {

            TableNameAttribute[] tableAttributes =
                (TableNameAttribute[]) type.GetCustomAttributes(typeof(TableNameAttribute), true);
            string tableName = "";
            if (tableAttributes.Length > 0)
            {
                tableName = tableAttributes[0].TableName;
            }
            else
            {
                tableName = type.Name;
            }

            return tableName;
        }




        /// <summary>
        /// 获取columname
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetColumNameAttributeValue(PropertyInfo propertyInfo)
        {

            ColumnNameAttribute[] columnAtrributes =
                (ColumnNameAttribute[]) propertyInfo.GetCustomAttributes(typeof(ColumnNameAttribute), true);
            string name = "";
            if (columnAtrributes.Length > 0)
            {
                name = columnAtrributes[0].ColumnName;
            }
            else
            {
                name = propertyInfo.Name;
            }

            return name;

        }
    }
}