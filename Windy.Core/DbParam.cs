using System.Data;

namespace Windy.Core
{
    public class DbParam
    {
        public object DbValue { get; set; }

        public DbType DbType { get; set; }

        public string Operator { get; set; }

        public DbParam(object  dbValue,DbType dbType)
        {
            this.DbType = dbType;
            this.DbValue = dbValue;

        }
    }
}