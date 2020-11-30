using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tiger.ORM.Expressions
{
    public class ExpressionAnalysis
    {
        public List<LambdaProperty> LambdaProperties { get; set; }

        public ExpressionAnalysis()
        {
            this.LambdaProperties = new List<LambdaProperty>();
        }

        public object Analysis(Expression expression, ref TigerMemberType memberType)
        {
            //LambdaExpression 
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                Expression exp = lambda.Body;
                TigerMemberType eleType = TigerMemberType.None;
                return this.Analysis(exp, ref eleType);
            }
            else if (expression is BinaryExpression)  //二进制运算
            {
                return this.Binary(expression);
            }
            else if (expression is MethodCallExpression)    //有调用方法的表达式
            {
                return this.MethodCall(expression, ref memberType);
            }
            else if (expression is ConstantExpression)  //常数值的表达式
            {
                return this.Constant(expression, ref memberType);
            }
            else if (expression is MemberExpression)        //访问字段或属性
            {
                return this.Member(expression, ref memberType);
            }
            else if (expression is UnaryExpression)     //一元运算符的表达式
            {
                UnaryExpression ue = (UnaryExpression)expression;
                Expression mex = ue.Operand;
                return this.Analysis(mex, ref memberType);
            }
            else if (expression is NewExpression)
            {
                NewExpression ne = (NewExpression)expression;
                return this.New(ne);
            }
            else if (expression is MemberInitExpression)
            {
                MemberInitExpression mie = (MemberInitExpression)expression;
                object entity = Expression.Lambda(mie).Compile().DynamicInvoke();
                ReadOnlyCollection<MemberBinding> bindings = mie.Bindings;
                foreach (MemberBinding item in bindings)
                {
                    PropertyInfo property = item.Member as PropertyInfo;
                    LambdaProperty lambdaProperty = new LambdaProperty()
                    {
                        Property = property,
                        Value = property.GetValue(entity)
                    };
                    this.LambdaProperties.Add(lambdaProperty);
                }
            }
            return null;
        }

        //二进制运算
        private object Binary(Expression exp)
        {
            BinaryExpression expression = exp as BinaryExpression;
            TigerMemberType leftType = TigerMemberType.None;
            TigerMemberType rightType = TigerMemberType.None;
            //left expression
            object left = this.Analysis(expression.Left, ref leftType);
            //operation 
            string operation = this.GetOperator(exp.NodeType);
            //right expression
            object right = this.Analysis(expression.Right, ref rightType);


            //propertyInfo = value
            var isKeyOperValue = (leftType == TigerMemberType.Key && rightType == TigerMemberType.Value);
            //value  = property
            var isValueOperKey = (rightType == TigerMemberType.Key && leftType == TigerMemberType.Value);

            PropertyInfo property = null;
            object value = null;
            if (isKeyOperValue)
            {
                property = left as PropertyInfo;
                value = right;
            }
            else if (isValueOperKey)
            {
                property = right as PropertyInfo;
                value = left;
            }
            else
                return null;

            this.LambdaProperties.Add(new LambdaProperty()
            {
                Property = property,
                Operation = operation,
                Value = value
            });
            return null;
        }

        //有调用方法的表达式
        private object MethodCall(Expression exp, ref TigerMemberType memberType)
        {
            MethodCallExpression mce = (MethodCallExpression)exp;
            string methodName = mce.Method.Name;
            if (methodName == "Contains")
            {
                return this.Contains(mce);
            }
            else if (methodName == "StartsWith")
            {
                return this.StartsWith(mce);
            }
            else if (methodName == "EndWith")
            {
                return this.EndWith(mce);
            }
            else if (methodName == "ToString")
            {
                memberType = TigerMemberType.Value;
                return this.ToString(mce, ref memberType);
            }
            else if (methodName.StartsWith("To"))
            {
                memberType = TigerMemberType.Value;
                return this.MethodTo(methodName, mce, ref memberType);
            }
            return null;
        }
        #region "MethodCall"
        private object Contains(MethodCallExpression mce)
        {
            TigerMemberType leftType = TigerMemberType.None;
            TigerMemberType rightType = TigerMemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.LambdaProperties.Add(new LambdaProperty
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = "%" + right + "%"
            });
            return null;
        }

        private object StartsWith(MethodCallExpression mce)
        {
            TigerMemberType leftType = TigerMemberType.None;
            TigerMemberType rightType = TigerMemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.LambdaProperties.Add(new LambdaProperty
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = "%" + right
            });
            return null;
        }

        private object EndWith(MethodCallExpression mce)
        {
            TigerMemberType leftType = TigerMemberType.None;
            TigerMemberType rightType = TigerMemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.LambdaProperties.Add(new LambdaProperty
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = right + "%"
            });
            return null;
        }

        private object ToString(MethodCallExpression mce, ref TigerMemberType type)
        {
            return this.Analysis(mce.Object, ref type);
        }

        private object MethodTo(string methodName, MethodCallExpression mce, ref TigerMemberType type)
        {
            var value = this.Analysis(mce.Arguments[0], ref type);
            if (methodName == "ToDateTime")
                return Convert.ToDateTime(value).ToString();
            else if (methodName.StartsWith("ToInt"))
                return Convert.ToInt32(value).ToString();
            return value;
        }
        #endregion

        //常数值的表达式
        private object Constant(Expression exp, ref TigerMemberType memberType)
        {
            memberType = TigerMemberType.Value;
            ConstantExpression ce = (ConstantExpression)exp;
            if (ce.Value == null)
                return null;
            else
                return ce.Value;
        }

        //访问字段或属性
        private object Member(Expression exp, ref TigerMemberType memberType)
        {
            MemberExpression me = (MemberExpression)exp;
            if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")  //value
            {
                if (me.Expression is ConstantExpression)
                {
                    var conExp = me.Expression as ConstantExpression;
                    if (conExp != null)
                    {
                        memberType = TigerMemberType.Value;
                        object dynamicInvoke = (me.Member as PropertyInfo).GetValue((me.Expression as ConstantExpression).Value);
                        if (dynamicInvoke == null)
                            return null;
                        return dynamicInvoke;
                    }
                }
                else if (me.Expression is MemberExpression)
                {
                    var mem = me.Expression as MemberExpression;
                    if (mem.Member.MemberType == MemberTypes.Property)
                    {
                        memberType = TigerMemberType.Key;
                        PropertyInfo property = me.Member as PropertyInfo;
                        return property;
                    }
                    else
                    {
                        memberType = TigerMemberType.Value;
                        var memberInfos = new Stack<MemberInfo>();
                        while (exp is MemberExpression)
                        {
                            var memberExpr = exp as MemberExpression;
                            memberInfos.Push(memberExpr.Member);
                            exp = memberExpr.Expression;
                        }

                        var constExpr2 = mem.Expression as ConstantExpression;
                        object objReference = constExpr2.Value;
                        while (memberInfos.Count > 0)
                        {
                            var mi = memberInfos.Pop();
                            if (mi.MemberType == MemberTypes.Property)
                            {
                                objReference = objReference.GetType().GetProperty(mi.Name).GetValue(objReference, null);
                            }
                            else if (mi.MemberType == MemberTypes.Field)
                            {
                                objReference = objReference.GetType().GetField(mi.Name).GetValue(objReference);
                            }
                        }

                        return objReference;
                    }
                }
                else if (me.Expression is NewExpression)
                {
                    object entity = this.New(me.Expression);
                    object value = (me.Member as PropertyInfo).GetValue(entity);
                    memberType = TigerMemberType.Value;
                    return value;
                }
                else
                {
                    if (me.NodeType == ExpressionType.MemberAccess)
                    {
                        object value = Expression.Lambda(me).Compile().DynamicInvoke();
                        memberType = TigerMemberType.Value;
                        return value;
                    }
                }
            }
            else    //key
            {
                memberType = TigerMemberType.Key;
                PropertyInfo property = me.Member as PropertyInfo;
                return property;
            }

            return null;
        }

        //New表达式
        private object New(Expression exp)
        {
            NewExpression ne = (NewExpression)exp;
            object[] arg = new object[ne.Arguments.Count];
            for (int i = 0; i < ne.Arguments.Count; i++)
            {
                ConstantExpression ce = (ConstantExpression)ne.Arguments[i];
                arg[i] = ce.Value;
            }

            object obj = ne.Constructor.Invoke(arg);
            return obj;
        }

        private object MemberInit(Expression exp)
        {
            //MemberInitExpression
            return null;
        }

        /// 根据条件生成对应的sql查询操作符
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.LambdaProperties.Add(new LambdaProperty()
                    {
                        Operation = "AND"
                    });
                    return "";
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.LambdaProperties.Add(new LambdaProperty()
                    {
                        Operation = "OR"
                    });
                    return "";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    throw new Exception(string.Format("不支持{0}此种运算符查找！" + expressiontype));
            }
        }
    }
}
