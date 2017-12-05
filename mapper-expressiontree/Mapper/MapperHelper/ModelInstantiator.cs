using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MapperUsingExpressionTree.Mapper.MapperHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelInstantiator
    {
        private ExpressionTreeGetSet _expressionTree = null;
        private int _iterationPointer = 1;
        private readonly List<string> _assemblyList = new List<string>();
        public int MaxIterationDepth { get; set; }

        public ModelInstantiator(int maxIterationDepth)
        {
            this.MaxIterationDepth = maxIterationDepth;
        }

        public object InitializeModel<T>(T obj) where T : class, new()
        {
            Assembly assembly = Assembly.GetAssembly(obj.GetType());
            _assemblyList.Add(assembly.GetName().Name);
            GenerateCode(obj);
            return obj;
        }

        private void GenerateCode<T>(T obj) where T : new()
        {
            //Iterate through the list of Public Properties of the object instance 
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                _expressionTree = new ExpressionTreeGetSet(property);

                if (CheckAssembly(property))
                {
                    _iterationPointer++;
                    //Avoid infinite Loop situation 
                    if (_iterationPointer < this.MaxIterationDepth)
                    {
                        var propertyInstance = _expressionTree.Get(obj);
                        if (propertyInstance != null) continue;

                        //Dynamically create Property Instance
                        propertyInstance = property.PropertyType.DynamicInstance();

                        if (property.CanWrite)
                            _expressionTree.Set(obj, propertyInstance);

                        GenerateCode(propertyInstance);
                    }
                }
                else if (property.PropertyType.IsGenericType)
                {
                    if (!property.PropertyType.IsGenericTypeDefinition)
                    {
                        InitializeGenericType(property.PropertyType, obj);
                    }
                    else
                    {
                        // Get the generic type parameters or type arguments.
                        Type[] typeParameters = property.PropertyType.GetGenericArguments();
                        Type tType = property.PropertyType.MakeGenericType(typeParameters);
                        var newList = tType.DynamicInstance();
                        _expressionTree.Set(obj, newList);
                    }
                }
            }
        }

        private bool CheckAssembly(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            return _assemblyList.Any(assemblyName => property.PropertyType.Assembly.FullName.Contains(assemblyName));
        }

        private void InitializeGenericType(Type t, dynamic parentObject)
        {
            // Get the generic type parameters or type arguments.
            Type[] typeParameters = t.GetGenericArguments();

            //New instance creation using Expressiontree
            var newT = t.DynamicInstance();

            //Set property
            _expressionTree.Set(parentObject, newT);

            foreach (Type tParam in typeParameters)
            {
                if (tParam.IsGenericParameter)
                {
                    DisplayGenericParameter(tParam);
                }
                else if (tParam.IsClass && IsGenericListOfT(t) && tParam != typeof(string))
                {
                    var newParamT = tParam.DynamicInstance();
                    ((IList) newT).Add(newParamT);
                    GenerateCode(newParamT);
                }
            }
        }

        private static bool IsGenericListOfT(Type type)
        {
            return type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof (List<>);
        }

        private static bool IsIEnumerable(Type type)
        {
            return type.IsGenericType
                   && type.GetGenericTypeDefinition() == typeof (IEnumerable<>);
        }

        // The following method displays information about a generic
        // type parameter. Generic type parameters are represented by
        // instances of System.Type, just like ordinary types.
        private static void DisplayGenericParameter(Type tp)
        {
            Console.WriteLine("      Type parameter: {0} position {1}",
                tp.Name, tp.GenericParameterPosition);

            Type classConstraint = null;

            foreach (Type iConstraint in tp.GetGenericParameterConstraints().Where(iConstraint => iConstraint.IsInterface))
            {
                Console.WriteLine("         Interface constraint: {0}",
                    iConstraint);
            }

            Console.WriteLine("         Base type constraint: None");

            GenericParameterAttributes sConstraints =
                tp.GenericParameterAttributes &
                GenericParameterAttributes.SpecialConstraintMask;

            if (sConstraints == GenericParameterAttributes.None)
            {
                Console.WriteLine("         No special constraints.");
            }
            else
            {
                if (GenericParameterAttributes.None != (sConstraints &
                                                        GenericParameterAttributes.DefaultConstructorConstraint))
                {
                    Console.WriteLine("         Must have a parameterless constructor.");
                }
                if (GenericParameterAttributes.None != (sConstraints &
                                                        GenericParameterAttributes.ReferenceTypeConstraint))
                {
                    Console.WriteLine("         Must be a reference type.");
                }
                if (GenericParameterAttributes.None != (sConstraints &
                                                        GenericParameterAttributes.NotNullableValueTypeConstraint))
                {
                    Console.WriteLine("         Must be a non-nullable value type.");
                }
            }
        }
    }
}
