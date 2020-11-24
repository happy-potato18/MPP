using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FakerClass
{
    class Generators
    {

        private static Random rnd = new Random();

        public static object SignedIntegerValuesGenerator(Type t)
        {
            var max = Convert.ToInt64(t.GetField("MaxValue").GetValue(t));
            var randomValue = rnd.NextDouble();
            return Convert.ChangeType(max * (2 * randomValue - 1) + randomValue - 1, t);

        }

        public static object UnsignedIntegerValuesGenerator(Type t)
        {
            var max = Convert.ToUInt64(t.GetField("MaxValue").GetValue(t));
            var randomValue = rnd.NextDouble();
            return Convert.ChangeType(randomValue * max, t);

        }
        public static object SingleValuesGenerator(Type t)
        {
            const int single_exp_limit = 45;
            var max = Convert.ToSingle(t.GetField("MaxValue").GetValue(t));
            double mantissa = Math.Pow(10, rnd.Next(0, single_exp_limit));
            var randomValue = rnd.NextDouble();
            return Convert.ChangeType(((max * (2 * randomValue - 1) + randomValue - 1) / mantissa), t);
        }

        public static object DoubleValuesGenerator(Type t)
        {
            const int double_exp_limit = 324;
            var max = Convert.ToDouble(t.GetField("MaxValue").GetValue(t));
            double mantissa = Math.Pow(10, rnd.Next(0, double_exp_limit));
            var randomValue = rnd.NextDouble();
            return Convert.ChangeType(((max * (2 * randomValue - 1) + randomValue - 1) / mantissa), t);
        }

        //public static object DecimalValuesGenerator(Type t)
        //{
        //    const int decimal_exp_limit = 28;
        //    var max = (decimal)t.GetField("MaxValue").GetValue(t);
        //    decimal mantissa = (decimal)Math.Pow(10, rnd.Next(0, decimal_exp_limit));
        //    var randomValue = (decimal)rnd.NextDouble();
        //    return Convert.ChangeType(((max * (2 * randomValue - 1) + randomValue - 1) / mantissa), t);
        //}

        //public static object CharValuesGenerator(Type t)
        //{
        //    return (char)rnd.Next(UInt16.MinValue, UInt16.MaxValue + 1);

        //}
        public static object BoolValuesGenerator(Type t)
        {
            return (DateTime.Now.Ticks % 2 == 0);
        }

        public static object StringValuesGenerator(Type t)
        {
            const int minStringLength = 1;
            const int maxStringLength = 100;
            const int capitalALetterCode = 65;
            const int smallZLetter = 90;
            var arrayLength = rnd.Next(minStringLength, maxStringLength + 1);
            byte[] bytesArray = new byte[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                bytesArray[i] = (byte)rnd.Next(capitalALetterCode, smallZLetter+1);
            }
            return Encoding.Default.GetString(bytesArray);

        }

        public static object DateTimeValuesGenerator(Type t)
        {
            var binaryTime = (long)Convert.ChangeType((rnd.NextDouble() * (DateTime.MaxValue.Ticks - DateTime.MinValue.Ticks) + DateTime.MinValue.Ticks), typeof(long));
            return DateTime.FromBinary(binaryTime);
        }

        public static object IEnumerableValuesGenerator(Type ienumerableType, Type[] argumentsTypes,
                                                        Func<Type, object>[] ArgumentsGenerators)
        {
            const int max_item_count = 20;
            string[] PossibleMethodsName = { "Add", "Enqueue", "Push" };

            var IenumerableObject = Activator.CreateInstance(ienumerableType);
            MethodInfo method = null;
            foreach (var methodName in PossibleMethodsName)
            {
                method = ienumerableType.GetMethod(methodName);
                if (method != null)
                    break;
            }

            var numOfItems = rnd.Next(1, max_item_count + 1);
            while (numOfItems > 0)
            {
                var argumentsValues = new object[argumentsTypes.Length];
                for (int i = 0; i < argumentsTypes.Length; i++)
                {
                    argumentsValues[i] = ArgumentsGenerators[i](argumentsTypes[i]);
                }
                method.Invoke(IenumerableObject, argumentsValues);
                numOfItems--;
            }
            return IenumerableObject;

        }




    }
}

