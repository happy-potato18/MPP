using AssemblyStructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Assert = NUnit.Framework.Assert;

namespace tests
{
    [TestFixture]
    public class Tests
    {
       
        [Test]
        public void TestGetNamespacesMethod_ReturnsNamespaceName()
        {
            var assembly = Assembly.LoadFrom( "class_library.dll");
            var receivedNamespaceName =AssemblyBrowser.GetNamespaces(assembly)[0].MemberName;
            var expectedNamespaceName = "classes_library";
            Assert.AreEqual(expectedNamespaceName,receivedNamespaceName,":=(");
        }

        [Test]
        public void TestGetTypesMethod_SearchingClerkType_ReturnsTypeName()
        {
            var assembly = Assembly.LoadFrom("class_library.dll");
            var namespaceName = AssemblyBrowser.GetNamespaces(assembly)[0].MemberName;
            var receivedTypesList = AssemblyBrowser.GetNamespaceTypes(assembly, namespaceName);
                                       
            Assert.IsTrue(receivedTypesList.Find(item => item.MemberName == "Clerk") != null);
        }

        [Test]
        public void TestGetTypeMembersMethod_SearchingStringProperty_ReturnsPropertyDescription()
        {
            var assembly = Assembly.LoadFrom("class_library.dll");
            var namespaceName = AssemblyBrowser.GetNamespaces(assembly)[0].MemberName;
            var clerkType = AssemblyBrowser.GetNamespaceTypes(assembly, namespaceName)
                                .Find(item => item.MemberName == "Clerk").MemberName;
            var receivedPropertyDescription = AssemblyBrowser.GetTypeMembers(assembly, clerkType)
                                    .Find(item => item.MemberName.Contains("PassNum")).MemberName;
            var expectedPropertyDescription = "PassNum: String";

            Assert.AreEqual(expectedPropertyDescription,receivedPropertyDescription,":-(");
        }

        [Test]
        public void TestGetTypeMembersMethod_SearchingMethod_ReturnsMethodSignature()
        {
            var assembly = Assembly.LoadFrom("class_library.dll");
            var namespaceName = AssemblyBrowser.GetNamespaces(assembly)[0].MemberName;
            var electricianType = AssemblyBrowser.GetNamespaceTypes(assembly, namespaceName)
                                .Find(item => item.MemberName == "Electrician").MemberName;
            var receivedMethodSignature = AssemblyBrowser.GetTypeMembers(assembly, electricianType)
                                    .Find(item => item.MemberName.Contains("AddWorker")).MemberName;
            var expectedMethodSignature = "public virtual String AddWorker()";

            Assert.AreEqual(expectedMethodSignature, receivedMethodSignature, ":'-(");
        }

        [Test]
        public void TestExtendedTask_SearchingExtensionMethod_ReturnsExtensionMethodSignature()
        {
            var assembly = Assembly.LoadFrom("class_library.dll");
            var namespaceName = AssemblyBrowser.GetNamespaces(assembly)[0].MemberName;
            var electricianType = AssemblyBrowser.GetNamespaceTypes(assembly, namespaceName)
                                .Find(item => item.MemberName == "Electrician").MemberName;
            var receivedMethodSignature = AssemblyBrowser.GetTypeMembers(assembly, electricianType)
                                    .Find(item => item.MemberName.Contains("CountTools")).MemberName;
            var expectedMethodSignature = "(extension) public static Int32 CountTools()";

            Assert.AreEqual(expectedMethodSignature, receivedMethodSignature, ":'-(");
        }
    }
}