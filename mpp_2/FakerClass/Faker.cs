using System;
using System.Linq;
using System.Collections.Generic;
using static System.Math;
using System.Reflection;
using PluginBase;
using System.IO;
using System.Diagnostics;

namespace FakerClass
{
    public class Faker
    {
        private delegate object ValueGenerator(Type t);
        private static  Dictionary<Type, ValueGenerator> _generators = new Dictionary<Type, ValueGenerator>();
        private static Stack<Type> _objectsTypeStack = new Stack<Type>();
        private static FakerConfig _config;
        public Faker()
        {
            if(_generators.Count == 0)
                FillDictionary();
        }

        public Faker(FakerConfig config)
        {
            if (_generators.Count == 0)
                FillDictionary();

            _config = config;
        }

        static Assembly LoadPlugin(string relativePath)
        {
            
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Faker).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading generators from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<IPlugin> CreateGenerators(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    IPlugin result = Activator.CreateInstance(type) as IPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

        }

        private IEnumerable<IPlugin> LoadPlugins()
        {
            string[] pluginPaths =
            {
                @"Plugins\CharPlugin\bin\Debug\netcoreapp3.1\CharPlugin.dll",
                @"Plugins\DecimalPlugin\bin\Debug\netcoreapp3.1\DecimalPlugin.dll"
            };

            IEnumerable<IPlugin> plugins = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreateGenerators(pluginAssembly);
            }).ToList();
            return plugins;
        }
        private void FillDictionary()
        {
            var plugins = LoadPlugins();
            foreach (var plugin in plugins)
            {
                _generators.Add(plugin.GeneratedType, plugin.GenerateValue);
            }
            _generators.Add(typeof(Byte), Generators.UnsignedIntegerValuesGenerator);
            _generators.Add(typeof(SByte), Generators.SignedIntegerValuesGenerator);
            _generators.Add(typeof(Int16), Generators.SignedIntegerValuesGenerator);
            _generators.Add(typeof(UInt16), Generators.UnsignedIntegerValuesGenerator);
            _generators.Add(typeof(Int32), Generators.SignedIntegerValuesGenerator);
            _generators.Add(typeof(UInt32), Generators.UnsignedIntegerValuesGenerator);
            _generators.Add(typeof(Int64), Generators.SignedIntegerValuesGenerator);
            _generators.Add(typeof(UInt64), Generators.UnsignedIntegerValuesGenerator);
            _generators.Add(typeof(Single), Generators.SingleValuesGenerator);
            _generators.Add(typeof(Double), Generators.DoubleValuesGenerator);
            //generators.Add(typeof(Decimal), Generators.DecimalValuesGenerator);
            //generators.Add(typeof(Char), Generators.CharValuesGenerator);
            _generators.Add(typeof(Boolean), Generators.BoolValuesGenerator);
            _generators.Add(typeof(String), Generators.StringValuesGenerator);
            _generators.Add(typeof(DateTime), Generators.DateTimeValuesGenerator);

        }

        private object FillObject(Type classType)
        {
            ConstructorInfo constructor = null;
            try
            {
                constructor = classType.GetConstructors().OrderByDescending(constr => constr.GetParameters().Length).First();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            var declaredParameters = constructor.GetParameters();
            object[] parameters = new object[declaredParameters.Length];
            foreach (var parameter in declaredParameters)
            {
                
                var parameterType = parameter.ParameterType;
                try
                {
                    parameters[parameter.Position] = _generators[parameterType](parameterType);
                }
                catch (KeyNotFoundException)
                {
                    if (parameterType.GetInterfaces().Any(t => t.IsGenericType
                                                                     && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        var argumentsTypes = parameterType.GetGenericArguments();
                        Func<Type, object>[] argumentsGenerators = new Func<Type, object>[argumentsTypes.Length];
                        for (int i = 0; i < argumentsTypes.Length; i++)
                        {
                            argumentsGenerators[i] = new Func<Type, object>(_generators[argumentsTypes[i]]);
                        }
                        parameters[parameter.Position] = Generators.IEnumerableValuesGenerator(parameterType, argumentsTypes,
                                                          argumentsGenerators);
                    }
                    else if (!(parameter.ParameterType.Namespace.Contains("System")))
                    {
                        if (_objectsTypeStack.Contains(parameterType))
                        {
                            parameters[parameter.Position] = default;
                        }
                        else
                        {
                            var faker = new Faker();
                            parameters[parameter.Position] = typeof(Faker).GetMethod("Create")
                                                            .MakeGenericMethod(parameterType).Invoke(faker, null);
                        }
                                                
                    }
                    else
                    {
                        parameters[parameter.Position] = null;
                    }
                }

            }

            var obj = constructor.Invoke(parameters);
            foreach(var parameter in declaredParameters)
            {
                if ( _config.IndexOfGeneratorIfExist(parameter.Name) != -1)
                {
                    _config.ExecuteAssignment(obj, _config.IndexOfGeneratorIfExist(parameter.Name));
                }
            }
            
            var properties = classType.GetProperties().Where(property => property.CanWrite);
            foreach (var property in properties)
            {
                if (_config.IndexOfGeneratorIfExist(property.Name) != -1)
                {
                    _config.ExecuteAssignment(obj, _config.IndexOfGeneratorIfExist(property.Name));
                    continue;
                }
                var propertyType = property.PropertyType;
                object value = new object();
                try
                {
                    value = _generators[propertyType](propertyType);
                }
                catch (KeyNotFoundException)
                {
                    if (propertyType.GetInterfaces().Any(t => t.IsGenericType
                                                                 && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        var argumentsTypes = propertyType.GetGenericArguments();
                        Func<Type, object>[] argumentsGenerators = new Func<Type, object>[argumentsTypes.Length];
                        for (int i = 0; i < argumentsTypes.Length; i++)
                        {
                            argumentsGenerators[i] = new Func<Type, object>(_generators[argumentsTypes[i]]);
                        }
                        value = Generators.IEnumerableValuesGenerator(propertyType, argumentsTypes,
                                                                      argumentsGenerators);
                    }
                    else if (!(property.PropertyType.Namespace.Contains("System")))
                    {
                        if(_objectsTypeStack.Contains(propertyType))
                        {
                            value = default;
                        }
                        else
                        {
                            var faker = new Faker();
                            value = typeof(Faker).GetMethod("Create").MakeGenericMethod(propertyType).Invoke(faker, null);
                        }
                                                                      
                    }
                    else
                    {
                        value = null;
                    }

                }
                property.SetValue(obj, value);


            }
            _objectsTypeStack.Pop();
            return obj;

        }
        public object Create<T>()
        {
            _objectsTypeStack.Push(typeof(T));
            return FillObject(typeof(T));

        }
    }
}

