using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using TracerClass;
using System.Reflection;

namespace SerializatorClass
{
   
    public class SerializatorXML<T> : ISerializator<T>
    {
        private string SerializeMethodsInfo(object infoStruct, PropertyInfo property, int repeatingTimes)
        {
            var methods = (List<TraceResult>)property.GetValue(infoStruct);
            if (methods.Count == 0)
                return "";
            string res = ">\n" + new string('\t', repeatingTimes) + "<" + property.Name + ">\n";
            repeatingTimes++;
            foreach (var method in methods)
            {
                var MethodPropertiesInfo = method.GetType().GetProperties();
                res += new string('\t', repeatingTimes) + "<" + method.GetType().Name + " ";
                repeatingTimes++;
                PropertyInfo tree = null;
                foreach (var methodproperty in MethodPropertiesInfo)
                {
                    if ((!methodproperty.PropertyType.IsPrimitive) && (methodproperty.PropertyType != typeof(String)))
                        tree = methodproperty;
                    else if (methodproperty.PropertyType != typeof(Boolean))
                        res += methodproperty.Name + "=\"" + methodproperty.GetValue(method).ToString() + "\" ";
                }
                string ret = SerializeMethodsInfo(method, tree, repeatingTimes);
                repeatingTimes--;
                res += ret;
                if (ret != "")
                    res += "\n" + new string('\t', repeatingTimes) + "</" + method.GetType().Name + ">\n";
                else
                    res += "/>\n";
            }

            repeatingTimes--;
            res += new string('\t', repeatingTimes) + "</" + property.Name + ">\n";
            return res;
        }



        public string MySerialize(ConcurrentDictionary<int, T> threads)
        {
            string res = "<root>\n";
            int repeatingTimes= 0;
            repeatingTimes++;
            foreach (var thread in threads)
            {
               
                res += new string('\t',repeatingTimes) +"<"+ thread.Value.GetType().Name+ " ";
                var PropertiesInfo = thread.Value.GetType().GetProperties();
                repeatingTimes++;
                res +="id=\"" + thread.Key.ToString() + "\" ";
                
                foreach (var property in PropertiesInfo)
                {
                    if (property.PropertyType.IsPrimitive)
                    {
                       res += property.Name + "=\"" + property.GetValue(thread.Value).ToString() + "\" ";
                       
                    }
                    else
                    {
                        res += SerializeMethodsInfo(thread.Value, property, repeatingTimes);
                    }
                }
                repeatingTimes--;
                res += new string('\t', repeatingTimes) + "</" + thread.Value.GetType().Name + ">\n";

            }
           
           res += "</root>";
                return res;
     

        }
    }
}
