using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FakerClass
{
   
    public class FakerConfig
    {
        public List<string> Properties = new List<string>();
        private List<Tuple<Delegate,Delegate>>_customGenerators = new List<Tuple<Delegate,Delegate>>();
        public void Add<TObject,TProperty,TGeneratorDelegate>(Expression<Func<TObject,TProperty>> lambda,
                                                              TGeneratorDelegate generator)
        {
            var propertyInfo = ((lambda.Body as MemberExpression).Member as PropertyInfo);
            var propertyName = propertyInfo.Name;
            ParameterExpression instanceParameter = Expression.Parameter(typeof(TObject), "instance");
            ParameterExpression propertyParameter = Expression.Parameter(typeof(TProperty), "property");
            MemberExpression property = Expression.Property(instanceParameter, propertyInfo.GetSetMethod());
            BinaryExpression assignment = Expression.Assign(property,propertyParameter);
            Properties.Add(propertyName.ToLower());
            var setter = Expression.Lambda<Action<TObject, TProperty>>
                       (assignment, instanceParameter, propertyParameter).Compile();
            _customGenerators.Add(new Tuple<Delegate,Delegate>(setter, generator as Delegate));
                    
        }

        public int IndexOfGeneratorIfExist(string propertyName)
        {
            return Properties.IndexOf(propertyName.ToLower());
         
        }

        public void ExecuteAssignment(object obj, int generatorIndex)
        {
            if (generatorIndex == -1)
                return;
            var customGenerator = _customGenerators[generatorIndex];
            var generator = customGenerator.Item2.Method;
            object declaredTypeObject = null;
            if(!generator.IsStatic)
            {
                declaredTypeObject = Activator.CreateInstance(generator.DeclaringType);
            }
            var generatorParameters = customGenerator.Item2.Method.GetParameters();
            var parameterValues = new List<object>();
            if(generatorParameters.Length != 0)
            {
                foreach (var parameter in generatorParameters)
                {
                    parameterValues.Add(Activator.CreateInstance(parameter.ParameterType));  

                }
            }
            var generatedvalue = customGenerator.Item2.Method.Invoke(declaredTypeObject, parameterValues.ToArray());
            int nop = 3;
            customGenerator.Item1.DynamicInvoke(new object[] {obj, generatedvalue });
            


        }

    }
}
