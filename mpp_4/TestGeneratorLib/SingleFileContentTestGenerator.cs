﻿using System;
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
        public static List<string> GenerateTestFile(List<string> fileContent)
        {
            List<string> testFileContent = new List<string>();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(String.Join('\n', fileContent)); 
            var rootNode = syntaxTree.GetCompilationUnitRoot();
            //using statements will be added to text after others because every class needs such link
            var usingStatenments = rootNode.Usings.Select(usingDirective => "using " + usingDirective.Name + ";").ToList();
            usingStatenments.Add("using NUnit.Framework;");
            var namespaces = rootNode.Members;
            foreach(var NamespaceMemberDeclaration in namespaces)
            {
                var namespaceDeclaration = (NamespaceDeclarationSyntax)NamespaceMemberDeclaration;
                usingStatenments.Add("using " + namespaceDeclaration.Name + ";");
                testFileContent.Add("namespace " + namespaceDeclaration.Name + ".Tests \n{");
                var classes = namespaceDeclaration.Members;
                foreach(var classMemberDeclaration in classes)
                {
                    var classDeclaration = (ClassDeclarationSyntax)classMemberDeclaration;
                    testFileContent.Add("\t[TestFixture]");
                    testFileContent.Add("\tpublic class " + classDeclaration.Identifier + "Tests\n\t{");
                    var methods = classDeclaration.Members.Where(member => (member.Kind() == SyntaxKind.MethodDeclaration));
                    foreach(var methodMemberDeclaration in methods)
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)methodMemberDeclaration;
                        testFileContent.Add("\t\t[Test]");
                        testFileContent.Add("\t\tpublic void " + methodDeclaration.Identifier + "Test()\n\t\t{");
                        testFileContent.Add("\t\t\tAssert.Fail(\"autogenerated\");");
                        testFileContent.Add("\t\t}\n");
                    }

                    testFileContent.Add("\t}\n");
                }

                testFileContent.Add("}");

            }
            testFileContent.InsertRange(0, usingStatenments);
            return testFileContent;

        }
    }
}
