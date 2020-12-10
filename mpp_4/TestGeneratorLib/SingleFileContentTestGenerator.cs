using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading.Tasks;

namespace TestGeneratorLib
{
    public static class SingleFileContentTestGenerator
    {
        
        private static Tuple<bool, List<string>> CreateSetupFunction(ClassDeclarationSyntax classDeclaration)
        {
            if(classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                return new Tuple<bool, List<string>>(false, null);
            var constructors = classDeclaration.Members
                                               .Where(member => (member.Kind() == SyntaxKind.ConstructorDeclaration));
            var classVariableName = "_" + classDeclaration.Identifier + "UnderTest";
            List<string> fieldsForSetupStrings = new List<string>();
            List<string> setupStrings = new List<string>();
            List<string> dependenciesName = new List<string>();
            setupStrings.Add("\n\t\t[Setup]\n\t\tpublic void Setup()\n\t\t{");
            fieldsForSetupStrings.Add(String.Format("\t\t private {0} {1};", classDeclaration.Identifier, classVariableName));
            if (constructors.Count() != 0)
            {
                var constructor = constructors
                                .OrderByDescending(member => ((ConstructorDeclarationSyntax)member).ParameterList.Parameters.Count)
                                .First();
                var constructorParameteters = ((ConstructorDeclarationSyntax)constructor).ParameterList.Parameters;
                if (constructorParameteters.Count == 0)
                {
                    return new Tuple<bool, List<string>>(false, null);
                }

               
                foreach (var parameter in constructorParameteters)
                {
                    var argumentType = parameter.Type;

                    if (argumentType.ToString().StartsWith("I"))
                    {
                        var argumentName = "_" + parameter.Identifier;
                        dependenciesName.Add(argumentName + ".Object");
                        fieldsForSetupStrings.Add(String.Format("\t\t private Mock<{0}> {1};", argumentType.ToString(), argumentName));
                        setupStrings.Add(String.Format("\t\t\t{0} = new Mock<{1}>();", argumentName, argumentType.ToString()));

                    }
                    else
                    {
                        var argumentName = "_" + parameter.Identifier;
                        dependenciesName.Add(argumentName);
                        fieldsForSetupStrings.Add(String.Format("\t\t private {0} {1};", argumentType.ToString(), argumentName));
                        setupStrings.Add(String.Format("\t\t\t{0} = {1};", argumentName, GetDefaultTypeValue(argumentType.ToString())));
                    }

                }
            }


            setupStrings.Add(String.Format("\t\t\t{0} = new {1}({2});", classVariableName, classDeclaration.Identifier, String.Join(',', dependenciesName.ToArray())));
            setupStrings.Add("\t\t}");
            fieldsForSetupStrings.AddRange(setupStrings);
            fieldsForSetupStrings.Add("\n");
            return new Tuple<bool, List<string>>(true, fieldsForSetupStrings);
        }

        private static string GetDefaultTypeValue(string typeName)
        {
            if ((typeName == "int") ||
                (typeName == "byte") ||
                (typeName == "double") ||
                (typeName == "decimal") ||
                (typeName == "sbyte") ||
                (typeName == "short") ||
                (typeName == "ushort") ||
                (typeName == "uint") ||
                (typeName == "long") ||
                (typeName == "ulong") ||
                (typeName == "float")
                )
                return "0";
            else if (typeName == "char")
                return "'A'";
            else if (typeName == "string")
                return "\"\"";
            else if (typeName == "bool")
                return "false";
            else
                return "null";
        }
        private static List<string> AddTestMethodBody(MethodDeclarationSyntax methodDeclaration, string classVarName)
        {
            List<string> methodBodyStrings = new List<string>();
            List<string> argumentsNames = new List<string>();
            if (methodDeclaration.ParameterList.Parameters.Count != 0)
            {
                foreach(var param in methodDeclaration.ParameterList.Parameters)
                {
                    var parameterTypeName= param.Type.ToString();
                    var parameterName = param.Identifier;
                    var defaultTypeValue = GetDefaultTypeValue(parameterTypeName);
                    methodBodyStrings.Add(String.Format("\t\t\t{0} {1} = {2};", parameterTypeName, parameterName.Text, defaultTypeValue));
                    argumentsNames.Add(parameterName.Text);
                }
            }
            var returnTypeName = methodDeclaration.ReturnType.ToString();
            if(returnTypeName != "void")
            {
                methodBodyStrings.Add(String.Format("\t\t\t{0} actual = {1}.{2}({3});", returnTypeName, classVarName, methodDeclaration.Identifier, String.Join(',', argumentsNames.ToArray())));
                methodBodyStrings.Add(String.Format("\t\t\t{0} expected = {1};", returnTypeName, GetDefaultTypeValue(returnTypeName)));
                methodBodyStrings.Add("\t\t\tAssert.That(actual, Is.EqualTo(expected));");
            }
            else
            {
                methodBodyStrings.Add(String.Format("\t\t\t{0}.{1}({2});", classVarName,methodDeclaration.Identifier, String.Join(',', argumentsNames.ToArray())));
            }
           
            methodBodyStrings.Add("\t\t\tAssert.Fail(\"autogenerated\");");
            return methodBodyStrings;
        }
        public static List<string> GenerateTestFile(Tuple<string, List<string>> fileContent)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(String.Join('\n', fileContent.Item2));
            var rootNode = syntaxTree.GetCompilationUnitRoot();
            List<string> testFileContent = new List<string>();
            //using statements will be added to text after others because every class needs such link
            var usingStatenments = rootNode.Usings.Select(usingDirective => "using " + usingDirective.Name + ";").ToList();
            usingStatenments.Add("using NUnit.Framework;");
            usingStatenments.Add("using Moq;");
            var namespaces = rootNode.Members;
            foreach (var NamespaceMemberDeclaration in namespaces)
            {
                var namespaceDeclaration = (NamespaceDeclarationSyntax)NamespaceMemberDeclaration;
                usingStatenments.Add("using " + namespaceDeclaration.Name + ";");
                testFileContent.Add("namespace " + namespaceDeclaration.Name + ".Tests \n{");
                var classes = namespaceDeclaration.Members;
                foreach (var classMemberDeclaration in classes)
                {
                    var classDeclaration = (ClassDeclarationSyntax)classMemberDeclaration;
                    testFileContent.Add("\t[TestFixture]");
                    testFileContent.Add("\tpublic class " + classDeclaration.Identifier + "Tests\n\t{");
                    var setupStringIfExist = CreateSetupFunction(classDeclaration);
                    if (setupStringIfExist.Item1 )
                        testFileContent.AddRange(setupStringIfExist.Item2);
                    string classVarName;
                    if (classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword))
                        classVarName = classDeclaration.Identifier.ToString();
                    else
                        classVarName = "_" + classDeclaration.Identifier + "UnderTest";
                    var methods = classDeclaration.Members.Where(member => (member.Kind() == SyntaxKind.MethodDeclaration));
                    foreach (var methodMemberDeclaration in methods)
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)methodMemberDeclaration;
                        testFileContent.Add("\t\t[Test]");
                        testFileContent.Add("\t\tpublic void " + methodDeclaration.Identifier + "Test()\n\t\t{");
                        testFileContent.AddRange(AddTestMethodBody(methodDeclaration,classVarName));
                        testFileContent.Add("\t\t}\n");
                    }

                    testFileContent.Add("\t}\n");
                }

                testFileContent.Add("}");

            }

            testFileContent.InsertRange(0, usingStatenments);

            return testFileContent;

        }

        public static async Task<Tuple<string,List<string>>> GenerateTestFileAsync(Tuple<string,List<string>> fileContent)
        {
            List<string> generatedcontent = await Task.Run(() =>
                GenerateTestFile(fileContent)
            );

            return new Tuple<string,List<string>>(fileContent.Item1,generatedcontent);
           
        }
    }
}
