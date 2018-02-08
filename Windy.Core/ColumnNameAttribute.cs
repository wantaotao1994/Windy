using System;

namespace Windy.Core
{
    
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnNameAttribute:Attribute
    {
        public bool IsGenerated { get; set; }

        public string ColumnName { get; set; }

        public ColumnNameAttribute(string  name,bool isGenerated=false)
        {
            this.ColumnName = name;
            this.IsGenerated = isGenerated;
        }

    }
}