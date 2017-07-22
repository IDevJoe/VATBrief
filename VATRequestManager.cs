using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VATBrief
{
    public class VATRequestManager
    {
        private static String status = "";
        public static string metar = "";

        public static void updateStatus()
        {
            status = queryURL("http://status.vatsim.net/status.txt");
            VATData data = new VATData(status);
            parseServers(data);
        }

        private static void parseServers(VATData data)
        {
            VATDataCategory c1 = data.GetCategories()[0];
            Console.WriteLine("Parsing servers...");
            Console.WriteLine("Category: "+c1.getName());
            foreach (string server in c1.getData())
            {
                if (server.StartsWith("msg0"))
                {
                    Console.WriteLine("MESSAGE FROM VATSIM:");
                    Console.WriteLine(server.Substring(5));
                } else if (server.StartsWith("url0"))
                {
                    VATDataAPI.urls.Add(server.Substring(5));
                } else if (server.StartsWith("metar0"))
                {
                    metar = server.Substring(7);
                }
            }
            Console.WriteLine("Loaded "+VATDataAPI.urls.Count+" data servers (Primary: "+VATDataAPI.urls[0]+")");
            Console.WriteLine("Metar Server: "+metar);
        }

        public static string queryURL(string url)
        {
            string thisQuery = url;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(thisQuery);
            webRequest.AutomaticDecompression = DecompressionMethods.GZip;
            string data = "";
            using (HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse())
            using (Stream stream = resp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

    }
}
