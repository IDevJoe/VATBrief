using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VATBrief
{
    class Program
    {

        public const string Seperator = "-------------------------------------------------------------------------------";

        static void Main(string[] args)
        {
            Console.Title = "VATBrief";
            Console.WriteLine("Downloading VATSIM data...");
            VATRequestManager.updateStatus();
            VATDataAPI.query();
            askAirport();
        }

        private static void askAirport()
        {
            Console.WriteLine(Seperator);
            Console.Write("Enter Airport (or leave it empty to update data): ");
            string ap = Console.ReadLine();
            ap = ap.ToUpper();
            Console.Clear();
            Regex r = new Regex("^[a-zA-Z]*$");
            if (ap == "")
            {
                VATDataAPI.query();
                askAirport();
            }
            if (!r.IsMatch(ap) || ap.Length != 4)
            {
                Console.WriteLine("Invalid airport code");
                askAirport();
            }
            // Provide with info
            Console.WriteLine(ap+" INFORMATION");
            Console.WriteLine(Seperator);
            var clients = new List<VATDataAPI.VATClient>();
            clients.AddRange(VATDataAPI.GetClients());
            var atis = clients.Find((e) => e.Callsign == ap + "_ATIS");
            Console.WriteLine(ap+" ATIS: "+(atis != null ? parseATIS(atis.ATIS) : "No ATIS Online"));
            Console.WriteLine(Seperator);
            Console.WriteLine("Controller List");
            Console.WriteLine("Callsign\tName\t\t\t\tFrequency");
            Console.WriteLine("--------\t----\t\t\t\t---------");
            var controllers = clients.Where((e) => (e.Callsign.StartsWith(ap+"_") || (ap.StartsWith("K") && e.Callsign.StartsWith(ap.Substring(1)+"_"))) && e.IsController).ToArray();
            foreach (VATDataAPI.VATClient client in controllers)
            {
                Console.WriteLine((client.Callsign.Length < 8 ? "_"+client.Callsign : client.Callsign)+"\t"+shortenName(client.Realname)+"\t\t"+(client.Realname.Length > 15 ? "" : "\t")+ client.Frequency);
            }
            Console.WriteLine(Seperator);
            Console.WriteLine("Departure/Arrival list");
            var departures = clients.Where((e) => e.DepartureAirport == ap).ToArray();
            var arrivals = clients.Where((e) => e.DestinationAirport == ap).ToArray();
            Console.WriteLine("Callsign\tName\t\t\t\tAirport\t\tDep/Arr");
            Console.WriteLine("--------\t----\t\t\t\t-------\t\t-------");
            foreach (VATDataAPI.VATClient client in departures)
            {
                Console.WriteLine((client.Callsign.Length > 7 ? client.Callsign.Substring(7) : client.Callsign) +"\t\t"+shortenName(client.Realname)+"\t\t"+ (client.Realname.Length > 15 ? "" : "\t") + client.DestinationAirport+"\t\tDep");
            }
            foreach (VATDataAPI.VATClient client in arrivals)
            {
                Console.WriteLine((client.Callsign.Length > 7 ? client.Callsign.Substring(7) : client.Callsign) + "\t\t" + shortenName(client.Realname) + "\t\t" + (client.Realname.Length > 15 ? "" : "\t") + client.DepartureAirport + "\t\tArr");
            }
            askAirport();
        }

        private static string shortenName(string name)
        {
            if (name.Length > 15)
            {
                name = name.Substring(0, name.Length - (name.Length - 15)) + "...";
            }
            return name;
        }

        private static string parseATIS(string toParse)
        {
            toParse = toParse.Substring(2);
            var tps = toParse.Split('^');
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < tps.Length; i++)
            {
                builder.Append(tps[i].Substring(1) + "\n");
            }
            return builder.ToString();
        }
    }
}
