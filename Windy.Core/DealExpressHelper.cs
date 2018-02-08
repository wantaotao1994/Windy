using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Windy.Core
{
    public class DealExpressHelper
    {
        public DealExpressHelper(Expression exp)
        {
            DealExpress(exp);
        }

        public IList<string> OperateList => _operateList;

        public IList<object> ParamList => _paramList;

        private int _paramNum = 0;
        private readonly IList<string> _operateList = new List<string>();
        private readonly IList<object> _paramList = new List<object>();


        /// <summary>
        /// 返回解析 lambda后面的语句
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Dictionary<string, DbParam> GetLambdaDbParams(out string   condition)
        {
            
            Dictionary<string,DbParam> dictionary = new Dictionary<string, DbParam>();

            _operateList.Add("");  //增加一个空的元素号匹配
            StringBuilder  stringBuilder = new StringBuilder();
            for (int i = 0; i < _paramList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    stringBuilder.Append(" " + ParamList[i]);
                    stringBuilder.Append(OperateList[i]);
                    stringBuilder.Append("@" + ParamList[i] + i);
                    stringBuilder.Append(OperateList[i + 1]);
                }
                else
                {
                    int lastNum = i - 1;

                    DbParam dbParam = new DbParam(ParamList[i], TypeHelper.SqlTypeMap[ParamList[i].GetType()]);
                    
                    
                    dictionary.Add(ParamList[lastNum].ToString()+lastNum,dbParam);
                    // stringBuilder.Add(ParamList[lastNum].ToString() + lastNum, ParamList[i]);
                }
            } 
            condition = stringBuilder.ToString();
            
            return dictionary;
        }

        private  void DealExpress(Expression exp)
         {
             if (exp is LambdaExpression)
            {
                LambdaExpression l_exp = exp as LambdaExpression;
                 DealExpress(l_exp.Body);
            }
            else if (exp is UnaryExpression)
            {
                 DealUnaryExpression(exp as UnaryExpression);
            }
            else if (exp is BinaryExpression)
            {
                 DealBinaryExpression(exp as BinaryExpression);
            }
            else if (exp is MemberExpression)
            {
                object memberValue = DealMemberExpression(exp as MemberExpression);
                _paramList.Add(memberValue);
                _paramNum++;
            }
            else  if (exp is ConstantExpression)
            {
                object constantValue = DealConstantExpression(exp as ConstantExpression);
                _paramList.Add(constantValue);
             //   dic.Add(constantValue+paramNum,constantValue);
                _paramNum++;
            }
            else
            {
              
            }
        }

        private  void DealUnaryExpression(UnaryExpression exp)
        {
            DealExpress(exp.Operand);
        }

        private  object DealConstantExpression(ConstantExpression exp)
        {
            object vaule = exp.Value;
            string v_str = string.Empty;
            if (vaule == null)
            {
                return "NULL";
            }
            if (vaule is string)
            {
                v_str = string.Format("{0}", vaule.ToString());
            }
            else if (vaule is DateTime)
            {
                DateTime time = (DateTime) vaule;
                v_str = string.Format("{0}", time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                 return vaule;
            }
            return v_str;
        }

        private  void DealBinaryExpression(BinaryExpression exp)
        { 
            DealExpress(exp.Left);
            
          string  operate=  GetOperStr(exp.NodeType);
            _operateList.Add(operate);
             DealExpress(exp.Right);
        
        }

        private  object DealMemberExpression(MemberExpression exp)
        {
           
           
            var attribute = (ColumnNameAttribute[]) exp.Member.GetCustomAttributes(typeof(ColumnNameAttribute), true);
           // return attribute.Length > 0 ? attribute[0].ColumnName : exp.Member.Name;
            if ( exp.Expression.GetType().Name=="TypedParameterExpression")
            {
                
               
                return attribute.Length > 0 ? attribute[0].ColumnName : exp.Member.Name;
            
            }
            else if(exp.Expression.GetType().Name=="FieldExpression"||exp.Expression.GetType().Name=="ConstantExpression")
            {
                            UnaryExpression cast = Expression.Convert(exp, typeof(object));
                            object obj = Expression.Lambda<Func<object>>(cast).Compile().Invoke();
                            return obj;
            }
            else
            {
               return exp.Expression.GetType().Name;
            }
           //return attribute.Length > 0 ? attribute[0].ColumnName : exp.Member.Name;
            
            //return exp.Member.Name;
        }

        private  string GetOperStr(ExpressionType e_type)
        {
            switch (e_type)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return "";
        }


        private  object Eval(MemberExpression member)
        {
            UnaryExpression cast = Expression.Convert(member, typeof(object));
            object obj = Expression.Lambda<Func<object>>(cast).Compile().Invoke();
            return obj;
        }
    }
}