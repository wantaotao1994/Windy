using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Windy.Core
{
    public static class DbManager
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConnection"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task<int> InsertAsync<T>(this IDbConnection dbConnection, T t) where T : new()
        {
            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) await ((DbConnection) dbConnection).OpenAsync();
            IDbCommand command = dbConnection.CreateCommand();
            //Get  tableName  from class  atributes
            string tableName = typeof(T).GetTableNameAttributeValue();
            StringBuilder columList = new StringBuilder();
            StringBuilder paramList = new StringBuilder();
            Dictionary<string, DbParam> dictionary = TypeHelper.GetPropertiesValue(t);
            foreach (var item in dictionary)
            {
                columList.Append(item.Key + ",");
                paramList.Append(("@" + item.Key + ","));
                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.DbType = item.Value.DbType;
                dataParameter.Value = item.Value.DbValue;
                dataParameter.ParameterName = "@" + item.Key;
                command.Parameters.Add(dataParameter);
            }

            columList.Remove(columList.Length - 1, 1);
            paramList.Remove(paramList.Length - 1, 1);
            string sqlCommand = $"insert into [{tableName}]  ({columList}) values ({paramList}) ";


            command.CommandText = sqlCommand;
            //    command.CommandText = "INSERT INTO test    (user_name , pwd) VALUES ('2','3')";
            return await ((DbCommand) command).ExecuteNonQueryAsync();
        }


        public static int Insert<T>(this IDbConnection dbConnection, T t) where T : new()
        {
            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) (dbConnection).Open();
            IDbCommand command = dbConnection.CreateCommand();
            //Get  tableName  from class  atributes
            string tableName = typeof(T).GetTableNameAttributeValue();
            StringBuilder columList = new StringBuilder();
            StringBuilder paramList = new StringBuilder();
            Dictionary<string, DbParam> dictionary = TypeHelper.GetPropertiesValue(t);
            foreach (var item in dictionary)
            {
                columList.Append(item.Key + ",");
                paramList.Append(("@" + item.Key + ","));
                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.DbType = item.Value.DbType;
                dataParameter.Value = item.Value.DbValue;
                dataParameter.ParameterName = "@" + item.Key;
                command.Parameters.Add(dataParameter);
            }

            columList.Remove(columList.Length - 1, 1);
            paramList.Remove(paramList.Length - 1, 1);
            string sqlCommand = $"insert into [{tableName}]  ({columList}) values ({paramList}) ";


            command.CommandText = sqlCommand;
            //    command.CommandText = "INSERT INTO test    (user_name , pwd) VALUES ('2','3')";
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConnection"></param>
        /// <param name="obj"></param>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection dbConnection, Expression<Func<T, bool>> lambdaExpression,object  obj) where T : new()
        {
            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) (dbConnection).Open();
            Dictionary<string, DbParam> dic;
            DealExpressHelper expressHelper = new DealExpressHelper(lambdaExpression);
            string condition = "";
            string tableName = typeof(T).GetTableNameAttributeValue();
            dic = expressHelper.GetLambdaDbParams(out condition);
            
            StringBuilder columList = new StringBuilder();

            
            Dictionary<string, DbParam> dictionary = TypeHelper.GetAnonymouseValues(obj);

            //此处写的很烂  有必要重新写  @auhor winter
            
            foreach (var item in dictionary)
            {
                columList.Append(  " "+item.Key + " = ");
                
                columList.Append(  "@"+item.Key + ",");
            }
            columList.Remove(columList.Length - 1, 1);
            string sql = $"update [{tableName}]  set  {columList} where  {condition}  ";
            
            
            IDbCommand  command=  GetExcuteCommand(ref dbConnection,sql,dic);

            foreach (var itemDbParam in dictionary)
            {
               
                    IDbDataParameter dataParameter = command.CreateParameter();

                    dataParameter.Value = itemDbParam.Value.DbValue;
                    dataParameter.DbType = itemDbParam.Value.DbType;
                    dataParameter.ParameterName = "@"+itemDbParam.Key;

                    command.Parameters.Add(dataParameter);
                
            }
            
             return  command.ExecuteNonQuery();
        }


        /// <summary>
        /// 得到第一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConnection"></param>
        /// <param name="obj"></param>
        /// <param name="lambdaExpression"></param>
        /// <returns></returns>
        public static T GetEnity<T>(this IDbConnection dbConnection, Expression<Func<T, bool>> lambdaExpression) where T : new()
        {
            Dictionary<string, DbParam> dic;
            DealExpressHelper expressHelper = new DealExpressHelper(lambdaExpression);
            string condition = "";
            string tableName = typeof(T).GetTableNameAttributeValue();
            dic = expressHelper.GetLambdaDbParams(out condition);
            string sql = $"select top 1 * from  [{tableName}] where  {condition}  ";

            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) (dbConnection).Open();

            IDbCommand  command=  GetExcuteCommand(ref dbConnection,sql,dic);
            
            IDataReader dataReader = command.ExecuteReader();


            using (dataReader)
            {
                T result =  TypeHelper.ReaderToModel<T>(dataReader);
                
                return result;
            }

        }


        /// <summary>
        /// 得到所有
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConnection"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> Query<T>(this IDbConnection dbConnection, Expression<Func<T, bool>> lambdaExpression)
            where T : new()
        {
            Dictionary<string, DbParam> dic;
            DealExpressHelper expressHelper = new DealExpressHelper(lambdaExpression);
            string condition = "";
            string tableName = typeof(T).GetTableNameAttributeValue();
            dic = expressHelper.GetLambdaDbParams(out condition);
            string sql = $"select  * from  [{tableName}] where  {condition}  ";

            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) (dbConnection).Open();

            IDbCommand  command=  GetExcuteCommand(ref dbConnection,sql,dic);
            
            IDataReader dataReader = command.ExecuteReader();


            using (dataReader)
            {
             List<T> result =  TypeHelper.DataReaderMapToList<T>(dataReader);
                
                return result;
            }

        }



        /// <summary>
        /// 删除某一条记录
        /// </summary>
        /// <param name="dbConnection"></param>
        /// <param name="lambdaExpression"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        public static int Delete<T>(this  IDbConnection  dbConnection,Expression<Func<T, bool>> lambdaExpression)
        {
            
            Dictionary<string, DbParam> dic;
            DealExpressHelper expressHelper = new DealExpressHelper(lambdaExpression);
            string condition = "";
            string tableName = typeof(T).GetTableNameAttributeValue();
            dic = expressHelper.GetLambdaDbParams(out condition);
            string sql = $"delete  from  [{tableName}] where  {condition}  ";

            bool wasClosed = dbConnection.State == ConnectionState.Closed;

            if (wasClosed) (dbConnection).Open();

            IDbCommand  command=  GetExcuteCommand(ref dbConnection,sql,dic);
            
            return command.ExecuteNonQuery();
            
        }


        private static IDbCommand GetExcuteCommand(ref IDbConnection dbConnection, string  sqlText,Dictionary<string, DbParam>  dictionary)
        {
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlText;
            foreach (var itemDbParam in dictionary)
            {
                IDbDataParameter dataParameter = command.CreateParameter();

                dataParameter.Value = itemDbParam.Value.DbValue;
                dataParameter.DbType = itemDbParam.Value.DbType;
                dataParameter.ParameterName = "@"+itemDbParam.Key;

                command.Parameters.Add(dataParameter);
            }

            return command;


        }
    }
}