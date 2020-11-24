using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyStructure
{
    
    internal static class MethodInfoExtension
    {
        internal static string GetMethodSignature(this MethodInfo methodInfo, bool isExtensionMethod = false)
        {
            string signature = "";
            
            //add access modifiers 
            if(methodInfo.IsFamilyAndAssembly)
                signature += "private protected ";
            else if (methodInfo.IsFamilyOrAssembly)
                signature += "protected internal ";
            else if (methodInfo.IsAssembly)
                signature += "internal ";
            else if (methodInfo.IsFamily)
                signature += "protected ";
            else if (methodInfo.IsPrivate)
                signature += "private ";
            else if (methodInfo.IsPublic)
                signature += "public ";
            
            //add particular modifiers
            if (methodInfo.IsStatic)
                signature += "static ";
            else if (methodInfo.IsVirtual)
                signature += "virtual ";
            else if (methodInfo.IsAbstract)
                signature += "abstract ";
            else if (methodInfo.IsFinal)
                signature += "final ";

            //add returned type
            signature += methodInfo.ReturnType.Name+" ";

            //add method name
            signature += methodInfo.Name;

            //add generic parameters
            if(methodInfo.IsGenericMethod)
            {
                signature += "<";
                signature += String.Join(',',methodInfo.GetGenericArguments().Select(arg => arg.Name));
                signature += ">";
            }

            //add parameters
            signature += "(";
            var parameters = methodInfo.GetParameters();
            int start = 0;
            if (isExtensionMethod)
            {
                start = 1;
            }
            for(int i = start; i< parameters.Length; i++)
            {
                
                signature += String.Format("{0} {1},", parameters[i].ParameterType.Name, parameters[i].Name);

            }
            signature = signature.Trim(',');
            signature += ")";

            return signature;

        }
    }


    public static  class AssemblyBrowser 
    {
        private static Dictionary<string,List<MemberDescription>> memberDescriptionsforExtensions = new Dictionary<string, List<MemberDescription>>();
        public static List<MemberDescription> GetNamespaces(Assembly assembly)
        {
             var types = assembly.GetTypes();
            
            return types.Select(type => type.Namespace).Distinct()
                        .Where(namesp => (namesp != null))
                        .Select(namesp => new MemberDescription(namesp,MemberType.Namespace,assembly)).ToList();
        }

        public static List<MemberDescription> GetNamespaceTypes(Assembly assembly, string namespaceName)
        {
            var types = assembly.GetTypes().Where(type => !type.Attributes.HasFlag(TypeAttributes.NestedPrivate)).ToList();
            List<MemberDescription> namespaceTypes = new List<MemberDescription>();
            foreach (var type in types)
            {
                if (type.Namespace == namespaceName)
                {
                    if (type.IsAbstract && type.IsSealed)
                    {
                        var allDeclaredMethods = type.GetMethods()
                                               .Where(method => ((method.IsPublic || method.IsAssembly)&&(!method.Attributes.HasFlag(MethodAttributes.SpecialName)) && (method.Module == type.Module))).ToList();
                        var extensionMethodsCount = allDeclaredMethods.Where(method => method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false) == true).Count();
                        if ((extensionMethodsCount != 0) && (allDeclaredMethods.Count() == extensionMethodsCount))
                        {
                            var extendedType = allDeclaredMethods.First().GetParameters()[0].ParameterType.Name;
                            if (memberDescriptionsforExtensions.ContainsKey(extendedType))
                                memberDescriptionsforExtensions[extendedType].AddRange(allDeclaredMethods
                                                            .Select(method => new MemberDescription(method.GetMethodSignature(), MemberType.TypeMember, assembly)).ToList());
                            else
                                memberDescriptionsforExtensions.Add(extendedType, allDeclaredMethods
                                                            .Select(method => new MemberDescription("(extension) " + method.GetMethodSignature(true), MemberType.TypeMember, assembly)).ToList());

                        }

                    }
                    else
                        namespaceTypes.Add(new MemberDescription(type.Name, MemberType.Type, assembly));
                }

            }

            return namespaceTypes;

        }

        public static List<MemberDescription> GetTypeMembers(Assembly assembly, string typeName)
        {
            var type = assembly.GetTypes().First(type => type.Name == typeName );
            List<MemberDescription> typeMembers = new List<MemberDescription>();
            typeMembers.AddRange(type.GetFields()
                                .Select(field => new MemberDescription(field.Name + ": " + field.FieldType.Name, MemberType.TypeMember, assembly)));
            typeMembers.AddRange(type.GetProperties()
                        .Select(property => new MemberDescription(property.Name + ": " + property.PropertyType.Name, MemberType.TypeMember, assembly)));
            typeMembers.AddRange(type.GetMethods()
                                .Where(method => (!method.Attributes.HasFlag(MethodAttributes.SpecialName)) && (method.Module == type.Module))
                                .Select(method => new MemberDescription(method.GetMethodSignature(),MemberType.TypeMember,assembly )));
            
            if(memberDescriptionsforExtensions.ContainsKey(typeName))
            {
                typeMembers.AddRange(memberDescriptionsforExtensions[typeName]);
            }
            
            return typeMembers;
        }
               
    }
}
