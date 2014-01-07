using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExpectedObjects;
using ExpectedObjects.Strategies;
using Outercurve.SQLiteCreateTree;

namespace SQLiteParseTreeTest
{
    /// <summary>
    /// 
    /// </summary>
    /// <from>https://github.com/derekgreer/expectedObjects/blob/master/src/ExpectedObjects/Strategies/ClassComparisonStrategy.cs</from>
    /// <license>MIT</license>
    public class TreeNodeComparisonStrategy : IComparisonStrategy
    {
        public bool AreEqual(object expected, object actual, IComparisonContext comparisonContext)
        {
            const bool equal = true;
            bool areEqual = comparisonContext.CompareProperties(expected, actual,
                (pi, actualPropertyInfo) =>
                {
                    //we ignore context, it's not relevant
                    if (pi.Name == "Context")
                        return true;
                    return CompareProperty(pi, actualPropertyInfo, expected, actual,
                        comparisonContext, equal);
                });

            areEqual = comparisonContext.CompareFields(expected, actual,
                                                               (fi, actualFieldInfo) =>
                                                               CompareField(fi, actualFieldInfo, expected, actual,
                                                                               comparisonContext, equal)) && areEqual;

            return areEqual;


        }

        public bool CanCompare(Type type)
        {
            return type.IsSubclassOf(typeof (SQLiteParseTreeNode));
        }


        static bool CompareField(FieldInfo expectedFieldInfo, FieldInfo actualFieldInfo, object expected, object actual, IComparisonContext comparisonContext, bool equal)
        {
            object value1 = expectedFieldInfo.GetValue(expected);

            if (actualFieldInfo == null)
            {
                return comparisonContext
                    .AreEqual(value1, Activator.CreateInstance(typeof(MissingMember<>)
                                                                   .MakeGenericType(expectedFieldInfo.FieldType)), expectedFieldInfo.Name);
            }

            object value2 = actualFieldInfo.GetValue(actual);
            return comparisonContext.AreEqual(value1, value2, expectedFieldInfo.Name);
        }

        static bool CompareProperty(PropertyInfo expectedPropertyInfo, PropertyInfo actualPropertyInfo, object expected, object actual,
                             IComparisonContext comparisonContext, bool areEqual)
        {
            ParameterInfo[] indexes = expectedPropertyInfo.GetIndexParameters();

            if (indexes.Length == 0)
            {
                areEqual = CompareStandardProperty(expectedPropertyInfo, actualPropertyInfo, expected, actual, comparisonContext) &&
                           areEqual;
            }
            else
            {
                areEqual = CompareIndexedProperty(expectedPropertyInfo, expected, actual, indexes, comparisonContext) && areEqual;
            }

            return areEqual;
        }

        static bool CompareIndexedProperty(PropertyInfo pi, object expected, object actual, ParameterInfo[] indexes,
                                           IComparisonContext comparisonContext)
        {
            bool areEqual = true;

            foreach (ParameterInfo index in indexes)
            {
                if (index.ParameterType == typeof (int))
                {
                    PropertyInfo expectedCountPropertyInfo = expected.GetType().GetProperty("Count");

                    PropertyInfo actualCountPropertyInfo = actual.GetType().GetProperty("Count");

                    if (expectedCountPropertyInfo != null)
                    {
                        var expectedCount = (int) expectedCountPropertyInfo.GetValue(expected, null);
                        var actualCount = (int) actualCountPropertyInfo.GetValue(actual, null);

                        if (expectedCount != actualCount)
                        {
                            areEqual = false;
                            break;
                        }

                        for (int i = 0; i < expectedCount; i++)
                        {
                            object[] indexValues = {i};
                            object value1 = pi.GetValue(expected, indexValues);
                            object value2 = pi.GetValue(actual, indexValues);

                            if (!comparisonContext.AreEqual(value1, value2, pi.Name + "[" + i + "]"))
                            {
                                areEqual = false;
                            }
                        }
                    }
                }
            }

            return areEqual;
        }

        static bool CompareStandardProperty(PropertyInfo pi1, PropertyInfo pi2, object expected, object actual,
                                            IComparisonContext comparisonContext)
        {
            object value1 = pi1.GetValue(expected, null);

            if (pi2 == null)
            {
                return comparisonContext
                    .AreEqual(value1, Activator.CreateInstance(typeof (MissingMember<>)
                                                                   .MakeGenericType(pi1.PropertyType)), pi1.Name);
            }

            object value2 = pi2.GetValue(actual, null);
            return comparisonContext.AreEqual(value1, value2, pi1.Name);
        }
    }


    public static class ExpectedObjectExtensions
    {
        public static ExpectedObject AddTreeNode( this ExpectedObject eo)
        {
            return eo.Configure(context => context.PushStrategy<TreeNodeComparisonStrategy>());
        }
    }
}
