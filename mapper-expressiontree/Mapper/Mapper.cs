using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MapperUsingExpressionTree.Mapper.MapperHelper;

namespace MapperUsingExpressionTree.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Mapper<TEntity> where TEntity : class, new()
    {
        private readonly Dictionary<string, Action<TEntity, object>> _propertyMappers = new Dictionary<string, Action<TEntity, object>>();
        private Func<TEntity> _initializedEntity;
        private bool _ignoreUnMapped = false;
        private const int MaxIterationDepth = 100;
        private readonly ModelInstantiator _instantiator = new ModelInstantiator(MaxIterationDepth);

        public Mapper<TEntity> UsingEntity(Func<TEntity> entityFactory)
        {
            _initializedEntity = (() => (TEntity)_instantiator.InitializeModel(entityFactory()));
            return this;
        }

        public Mapper<TEntity> IgnoreUnMapped() { _ignoreUnMapped = true; return this; }

        public Mapper<TEntity> Map<TProperty>(Expression<Func<TEntity, TProperty>> memberExpression,
            string dictionaryFieldMap, Expression<Func<object, TProperty>> converter)
        {
            var converterInput = Expression.Parameter(typeof(object), "Convert");
            var invokeConverter = Expression.Invoke(converter, converterInput);
            var assign = Expression.Assign(memberExpression.Body, invokeConverter);
            var mapAction = Expression.Lambda<Action<TEntity, object>>(
                assign, memberExpression.Parameters.Single(), converterInput).Compile();
            _propertyMappers[dictionaryFieldMap] = mapAction;
            return this;
        }

        public TEntity MapSource(Dictionary<string, object> inputEntity)
        {
            //Get class instance ,which is initialized
            var instance = _initializedEntity();

            foreach (var dictEntry in inputEntity)
            {
                try
                {
                    if (!_ignoreUnMapped)
                        _propertyMappers[dictEntry.Key](instance, dictEntry.Value);
                    else
                        if (_propertyMappers.ContainsKey(dictEntry.Key)) _propertyMappers[dictEntry.Key](instance, dictEntry.Value);
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
            return instance;
        }
    }
}
