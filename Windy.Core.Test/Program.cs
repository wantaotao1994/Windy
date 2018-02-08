using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Windy.Core.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IDbConnection dbConnection =
                new SqlConnection("your  connection string"); 
           
            using (dbConnection)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();


                //增加
                var  reslut1 =    dbConnection.Insert<Test>(new Test(){
                    name= "insert",
                    pwd = "2017",
                    state =20,
                    Date = DateTime.Now
                });

             
                // 更新
                 var  reslut2 =    dbConnection.Update<Test>(r=>r.pwd=="2017",new {
                        user_name="",
                        pwd = "update",
                        state =20,
                        datetime = DateTime.Now
                    });
                var result4 = dbConnection.Query<Test>(r=>r.state==20);
              
                var  reslut3 =    dbConnection.Delete<Test>(r=>r.state==20);

                stopwatch.Stop();

                Console.WriteLine(stopwatch.ElapsedMilliseconds);
            }
        }
    }

    public class Test
    {

        
        /// <summary>
        /// 
        /// </summary>
        [ColumnName("id",isGenerated:true)]
        public long Id { get; set; }

        [ColumnName("user_name")]
        public string name { get; set; }


        [ColumnName("datetime")] 
        public DateTime Date { get; set; }



        [ColumnName("pwd")]
        public string pwd { get; set; }

        [ColumnName("state")]
        public int state { get; set; }

    }
}