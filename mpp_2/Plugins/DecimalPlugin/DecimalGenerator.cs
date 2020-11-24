using PluginBase;
using System;

namespace DecimalPlugin
{
    public class DecimalGenerator : IPlugin
    {
        public Type GeneratedType => typeof(decimal);

        public object GenerateValue(Type t)
        {
            var rnd = new Random();
            const int decimal_exp_limit = 28;
            var max = (decimal)t.GetField("MaxValue").GetValue(t);
            decimal mantissa = (decimal)Math.Pow(10, rnd.Next(0, decimal_exp_limit));
            var randomValue = (decimal)rnd.NextDouble();
            return Convert.ChangeType(((max * (2 * randomValue - 1) + randomValue - 1) / mantissa), t);
        }
    }
}
