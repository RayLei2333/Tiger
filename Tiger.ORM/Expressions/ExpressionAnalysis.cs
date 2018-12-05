using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tiger.ORM.Adapter;
using Tiger.ORM.Utilities;

namespace Tiger.ORM.Expressions
{
    public class ExpressionAnalysis
    {
        public List<LambdaWhereEntity> WhereEntities = null;

        private ISqlTimeHandle _timeHandle;

        public ExpressionAnalysis()
        {
            this.WhereEntities = new List<LambdaWhereEntity>();
        }

        internal ExpressionAnalysis(ISqlTimeHandle timeHandle) : this()
        {
            this._timeHandle = timeHandle;
        }

        public object Analysis(Expression expression, ref MemberType memberType)
        {
            //LambdaExpression 
            if (expression is LambdaExpression)
            {
                LambdaExpression lambda = expression as LambdaExpression;
                Expression exp = lambda.Body;
                MemberType eleType = MemberType.None;
                return this.Analysis(exp, ref eleType);
            }
            else if (expression is BinaryExpression)
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
            return null;
        }

        private object Binary(Expression exp)
        {
            BinaryExpression expression = exp as BinaryExpression;
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            //left expression
            object left = this.Analysis(expression.Left, ref leftType);
            //operation 
            string operation = this.GetOperator(exp.NodeType);
            //right expression
            object right = this.Analysis(expression.Right, ref rightType);


            //propertyInfo = value
            var isKeyOperValue = (leftType == MemberType.Key && rightType == MemberType.Value);
            //value  = property
            var isValueOperKey = (rightType == MemberType.Key && leftType == MemberType.Value);

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

            this.WhereEntities.Add(new LambdaWhereEntity()
            {
                Property = property,
                Operation = operation,
                Value = value
            });
            return null;
        }

        private object MethodCall(Expression exp, ref MemberType memberType)
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
                memberType = MemberType.Value;
                return this.ToString(mce, ref memberType);
            }
            else if (methodName.StartsWith("To"))
            {
                memberType = MemberType.Value;
                this.MethodTo(methodName, mce, ref memberType);
            }


            return null;
        }

        private object Constant(Expression exp, ref MemberType memberType)
        {
            memberType = MemberType.Value;
            ConstantExpression ce = (ConstantExpression)exp;
            if (ce.Value == null)
                return null;
            else
                return ce.Value;
        }

        private object Member(Expression exp, ref MemberType memberType)
        {
            MemberExpression me = (MemberExpression)exp;
            if (me.Expression == null || me.Expression.NodeType.ToString() != "Parameter")  //value
            {
                var conExp = me.Expression as ConstantExpression;
                if (conExp != null)
                {
                    memberType = MemberType.Value;
                    object dynamicInvoke = (me.Member as PropertyInfo).GetValue((me.Expression as ConstantExpression).Value);
                    if (dynamicInvoke == null)
                        return null;
                    return dynamicInvoke;
                }
                else
                {
                    var mem = me.Expression as MemberExpression;
                    if (mem == null)
                    {
                        if (me.Member.ReflectedType.FullName == "System.DateTime")
                        {
                            if (me.ToString() == "DateTime.Now")
                            {
                                memberType = MemberType.Value;
                                //if (_timeHandle != null)
                                //    return _timeHandle.TimeHandel(DateTime.Now.ToString("yyyy-MM-dd"), "120");
                                return DateTime.Now.ToString();
                            }
                        }
                        //TODO://
                    }
                    else if (mem.Member.MemberType == MemberTypes.Property)
                    {
                        memberType = MemberType.Key;
                        PropertyInfo property = me.Member as PropertyInfo;
                        return property;

                    }
                    else
                    {
                        memberType = MemberType.Value;
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
            }
            else    //key
            {
                memberType = MemberType.Key;
                PropertyInfo property = me.Member as PropertyInfo;
                return property;
            }

            return null;
        }

        #region Method Call 
        private object Contains(MethodCallExpression mce)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.WhereEntities.Add(new LambdaWhereEntity
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = "%" + right + "%"
            });


            return null;
        }

        private object StartsWith(MethodCallExpression mce)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.WhereEntities.Add(new LambdaWhereEntity
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = "%" + right
            });
            return null;
        }

        private object EndWith(MethodCallExpression mce)
        {
            MemberType leftType = MemberType.None;
            MemberType rightType = MemberType.None;
            var left = this.Analysis(mce.Object, ref leftType);
            var right = this.Analysis(mce.Arguments[0], ref rightType);
            this.WhereEntities.Add(new LambdaWhereEntity
            {
                Property = left as PropertyInfo,
                Operation = "LIKE",
                Value = right + "%"
            });
            return null;
        }

        private object ToString(MethodCallExpression mce, ref MemberType type)
        {
            return this.Analysis(mce.Object, ref type);
        }

        private object MethodTo(string methodName, MethodCallExpression mce, ref MemberType type)
        {
            var value = this.Analysis(mce.Arguments[0], ref type);
            if (methodName == "ToDateTime")
                return Convert.ToDateTime(value).ToString();
            else if (methodName.StartsWith("ToInt"))
                return Convert.ToInt32(value).ToString();
            return value;
        }

        #endregion



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
                    this.WhereEntities.Add(new LambdaWhereEntity()
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
                    this.WhereEntities.Add(new LambdaWhereEntity()
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


        private void DateTimeHandle()
        {

        }
    }
}
