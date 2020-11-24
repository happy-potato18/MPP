using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Text;
using System.Threading.Tasks;

namespace SerializatorClass
{
    public class SerializatorJSON<T> : ISerializator<T>
    {
        public string MySerialize(ConcurrentDictionary<int,T> threads)
        {
            string res = "";
            try
            {

                foreach (var thread in threads)
                {

                    var jsonString = JsonConvert.SerializeObject(thread, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }); 
                    res += jsonString;
                    res += "\n";
                       
                }

                //return "OK";
                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }

       
    }
}
