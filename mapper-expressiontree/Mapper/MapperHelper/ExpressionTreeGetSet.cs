using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperUsingExpressionTree.Mapper.MapperHelper
{
    /// <summary>
    /// ExpressionTree node Getter and Setter.
    /// </summary>
    public class ExpressionTreeGetSet
    {
        public PropertyInfo Property { get; set; }
        private readonly Type _declaringType = null;
        private readonly Type _propertyType = null;
        public Func<object, object> GetDelegate;
        public Action<object, object> SetDelegate;
        private readonly Type _t = typeof(object);

        public ExpressionTreeGetSet(PropertyInfo property)
        {
            this.Property = property;
            _declaringType = Property.DeclaringType;
            _propertyType = Property.PropertyType;
            InitializeGet();
            InitializeSet();
        }

        public object Get(object instance)
        {
            return this.GetDelegate(instance);
        }

        public void Set(object instance, object value)
        {
            this.SetDelegate(instance, value);
        }

        private void InitializeSet()
        {
            var instance = Expression.Parameter(_t, "instance");
            var value = Expression.Parameter(_t, "value");

            // value as T is slightly faster than (T)value, so if it's not a value type, use that
            UnaryExpression instanceCast = _declaringType != null && (!_declaringType.IsValueType)
                ? Expression.TypeAs(instance, _declaringType)
                : Expression.Convert(instance, _declaringType);
            UnaryExpression valueCast = (!_propertyType.IsValueType)
                ? Expression.TypeAs(value, _propertyType)
                : Expression.Convert(value, _propertyType);
            this.SetDelegate =
                Expression.Lambda<Action<object, object>>(
                    Expression.Call(instanceCast, this.Property.GetSetMethod(), valueCast),
                    new ParameterExpression[] {instance, value}).Compile();
        }

        private void InitializeGet()
        {
            var instance = Expression.Parameter(_t, "instance");
            UnaryExpression instanceCast = _declaringType != null && (!_declaringType.IsValueType)
                ? Expression.TypeAs(instance, _declaringType)
                : Expression.Convert(instance, _declaringType);
            this.GetDelegate =
                Expression.Lambda<Func<object, object>>(
                        Expression.TypeAs(Expression.Call(instanceCast, this.Property.GetGetMethod()), _t), instance)
                    .Compile();
        }
    }
}
