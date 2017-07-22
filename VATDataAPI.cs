using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VATBrief
{
    class VATDataAPI
    {
        public static List<string> urls = new List<string>();
        private static Random random = new Random();
        private static List<VATClient> clients = new List<VATClient>();

        public static void query()
        {
            string thisQuery = urls[random.Next(urls.Count)];
            Console.WriteLine("Querying data server "+thisQuery);
            VATData data = new VATData(VATRequestManager.queryURL(thisQuery));
            parse(data);
        }

        public static VATClient[] GetClients()
        {
            return clients.ToArray();
        }

        private static void parse(VATData data)
        {
            clients.Clear();
            Console.WriteLine("Sorting connected clients...");
            for (int i = 1; i < data.GetCategories().Length; i++)
            {
                VATDataCategory cat = data.GetCategories()[i];
                string[] catdata = cat.getData();
                Console.WriteLine("Category: "+cat.getName());
                if (cat.getName() == "CLIENTS" || cat.getName() == "PREFILE")
                {
                    foreach (string client in catdata)
                    {
                        var c = client.Split(':');
                        clients.Add(new VATClient(c[0], c[1], c[2], (c[3] == "ATC"), c[4], c[11], c[13], c[16], c[29], c[35]));
                    }
                }
            }
            Console.WriteLine("Sorted "+clients.Count+" connected clients");
        }

        public class VATClient
        {
            public string Callsign { private set; get; }
            public string Cid { private set; get; }
            public string Realname { private set; get; }
            public bool IsController { private set; get; }
            public string Frequency { private set; get; }
            public string DepartureAirport { private set; get; }
            public string DestinationAirport { private set; get; }
            public string Rating { private set; get; }
            public string Remarks { private set; get; }
            public string ATIS { private set; get; }

            public VATClient(string callsign, string cid, string realname, bool isController, string frequency, string depAP,
                string destAP, string rating, string remarks, string ATIS)
            {
                this.Callsign = callsign;
                this.Cid = cid;
                this.Realname = realname;
                this.IsController = isController;
                this.Frequency = frequency;
                this.DepartureAirport = depAP;
                this.DestinationAirport = destAP;
                this.Rating = rating;
                this.Remarks = remarks;
                this.ATIS = ATIS;
            }
        }
    }
}
