using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace MapperUsingExpressionTree.Mapper.MapperHelper
{
    /// <summary>
    /// For creating new instance of a dynamic class object using ExpressionTree.
    /// </summary>
    public static class New
    {
        /// <summary>
        /// Create instance of type dynamic.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static dynamic DynamicInstance(this Type obj)
        {
            return Instance<dynamic>(obj);
        }

        /// <summary>
        /// Create any instance of type TOut can be object , dynamic etc.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TOut Instance<TOut>(this Type obj)
        {
            return (TOut) Creator<TOut>(obj).DynamicInvoke();
        }

        private static Func<TOut> Creator<TOut>(Type obj)
        {
            if (obj == typeof (string))
                return Expression.Lambda<Func<TOut>>(Expression.Constant(string.Empty)).Compile();

            if (HasDefaultConstructor(obj))
                return Expression.Lambda<Func<TOut>>(Expression.New(obj)).Compile();

            return () => (TOut) FormatterServices.GetUninitializedObject(obj);
        }

        private static bool HasDefaultConstructor(Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
    }
}
