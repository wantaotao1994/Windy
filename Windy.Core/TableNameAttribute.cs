using System;

namespace Winter.ORM.OrmAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableNameAttribute:Attribute
    {
        public string TableName { get; set; }

        public TableNameAttribute(string  tableName)
        {
            this.TableName = tableName;
        }
    }
}