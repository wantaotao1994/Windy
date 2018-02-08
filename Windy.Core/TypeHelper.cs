using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Linq;

namespace Windy.Core
{
    public static class TypeHelper
    {
        private static readonly Dictionary<string, PropertyInfo[]> PropertyInfos =
            new Dictionary<string, PropertyInfo[]>();


        /// <summary>
        /// type map
        /// </summary>
        public static readonly Dictionary<Type, DbType> SqlTypeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(object)] = DbType.Object
        };


        /// <summary>
        /// 将datareader 转换成  对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ReaderToModel<T>(IDataReader dr) where T : new()
        {
            if ((dr != null) && dr.Read())
            {
                T local = new T();

                string tableName = local.GetType().FullName;
                PropertyInfo[] propertyInfos;
                if (PropertyInfos.Keys.Contains(tableName))
                {
                    propertyInfos = PropertyInfos[tableName];
                }
                else
                {
                    propertyInfos = local.GetType().GetProperties();
                    PropertyInfos.Add(tableName, propertyInfos);
                }


                int fieldCount = dr.FieldCount;


                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    string name = CustomAttributeHelper.GetColumNameAttributeValue(propertyInfo);
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (dr[name] != DBNull.Value)
                        {
                            Type propertyType = propertyInfo.PropertyType;
                            if (propertyType.IsGenericType &&
                                propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter converter = new NullableConverter(propertyType);
                                propertyType = converter.UnderlyingType;
                            }

                            if (propertyType.IsEnum)
                            {
                                object obj2 = Enum.ToObject(propertyType, dr[name]);
                                propertyInfo.SetValue(local, obj2, null);
                            }
                            else
                            {
                                object obj3 = Convert.ChangeType(dr[name], propertyType);
                                if (propertyType.Equals(typeof(string)) && (obj3 == null))
                                {
                                    obj3 = string.Empty;
                                }
                                else if (propertyType.Equals(typeof(DateTime)))
                                {
                                    propertyInfo.SetValue(local, obj3, null);
                                }
                                else
                                {
                                    propertyInfo.SetValue(local, obj3, null);
                                }
                            }
                        }
                    }
                }


                return local;
            }

            return default(T);
        }


        public static List<T> DataReaderMapToList<T>(IDataReader dr) where T : new()
        {
            List<T> list = new List<T>();
            T obj = new T();
            string tableName = obj.GetType().FullName;
            PropertyInfo[] propertyInfos;
            if (PropertyInfos.Keys.Contains(tableName))
            {
                propertyInfos = PropertyInfos[tableName];
            }
            else
            {
                propertyInfos = obj.GetType().GetProperties();
                PropertyInfos.Add(tableName, propertyInfos);
            }

            while (dr.Read())
            {
                T local2 = default(T);
                T local = (local2 == null) ? Activator.CreateInstance<T>() : (local2 = default(T));
                int fieldCount = dr.FieldCount;

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    string name = CustomAttributeHelper.GetColumNameAttributeValue(propertyInfo);
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (dr[name] != DBNull.Value)
                        {
                            Type propertyType = propertyInfo.PropertyType;
                            if (propertyType.IsGenericType &&
                                propertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                            {
                                NullableConverter converter = new NullableConverter(propertyType);
                                propertyType = converter.UnderlyingType;
                            }

                            if (propertyType.IsEnum)
                            {
                                object obj2 = Enum.ToObject(propertyType, dr[name]);
                                propertyInfo.SetValue(local, obj2, null);
                            }
                            else
                            {
                                object obj3 = Convert.ChangeType(dr[name], propertyType);
                                if (propertyType.Equals(typeof(string)) && (obj3 == null))
                                {
                                    obj3 = string.Empty;
                                }
                                else if (propertyType.Equals(typeof(DateTime)))
                                {
                                    propertyInfo.SetValue(local, obj3, null);
                                }
                                else
                                {
                                    propertyInfo.SetValue(local, obj3, null);
                                }
                            }
                        }
                    }
                }

                list.Add(local);
            }

            return list;
        }

        public static Dictionary<string, DbParam> GetAnonymouseValues(object obj)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties();

            Dictionary<string, DbParam> dictionary = new Dictionary<string, DbParam>();
            foreach (PropertyInfo item in propertyInfos)
            {
                object value = item.GetValue(obj);
                if (value != null)
                {
                    DbType dbType = SqlTypeMap[item.PropertyType];

                    dictionary[item.Name] = new DbParam(value, dbType);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// 得到 列名 跟值
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, DbParam> GetPropertiesValue<T>(T t)
        {
            //缓存反射
            string tableName = t.GetType().FullName;
            PropertyInfo[] propertyInfos;
            if (PropertyInfos.ContainsKey(tableName))
            {
                propertyInfos = PropertyInfos[tableName];
            }
            else
            {
                propertyInfos = t.GetType().GetProperties();

                PropertyInfos.Add(tableName, propertyInfos);
            }

            Dictionary<string, DbParam> dictionary = new Dictionary<string, DbParam>();
            foreach (PropertyInfo item in propertyInfos)
            {
                object value = item.GetValue(t);
                if (value != null)
                {
                    var attrs = (ColumnNameAttribute[]) item.GetCustomAttributes(typeof(ColumnNameAttribute), true);


                    var a = typeof(string);
                    DbType dbType = SqlTypeMap[item.PropertyType];
                    if (attrs.Length > 0)
                    {
                        if (attrs[0].IsGenerated == false)
                        {
                            dictionary[attrs[0].ColumnName] = new DbParam(value, dbType);
                        }
                    }

                    else
                    {
                        dictionary[item.Name] = new DbParam(value, dbType);
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }

            return Convert.ChangeType(value, conversionType);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return ((obj == null) || (obj is DBNull));
        }
    }
}