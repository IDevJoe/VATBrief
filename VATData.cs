using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATBrief
{
    class VATData
    {
        private string rawtext = "";
        private List<VATDataCategory> categories = new List<VATDataCategory>();

        public VATData(string text)
        {
            // Put text in memory for now, throw away later
            rawtext = text;
            parse();
            // GOODBYE RAW!
            rawtext = null;
        }

        private void parse()
        {
            string[] lines = rawtext.Split('\n');
            List<string> brokendown = new List<string>();
            foreach (string line in lines)
            {
                if(!line.StartsWith(";") && !String.IsNullOrWhiteSpace(line)) brokendown.Add(line);
            }
            brokendown.Add("!END:");
            lines = brokendown.ToArray();
            VATDataCategory currentCategory = new VATDataCategory("DEFAULT");
            string catName = "";
            foreach (string line in lines)
            {
                if (line.StartsWith("!"))
                {
                    catName = line.Trim().Substring(1);
                    catName = catName.Substring(0, catName.Length - 1);
                    if (currentCategory != null)
                    {
                        categories.Add(currentCategory);
                    }
                    currentCategory = new VATDataCategory(catName);
                }
                else
                {
                    currentCategory.addData(line.Trim());
                }
            }
        }

        public VATDataCategory[] GetCategories()
        {
            return categories.ToArray();
        }
    }

    class VATDataCategory
    {
        private string name;
        private List<string> data = new List<string>();
        public VATDataCategory(string name)
        {
            this.name = name;
        }

        public void addData(string data)
        {
            this.data.Add(data);
        }

        public string getName()
        {
            return name;
        }

        public string[] getData()
        {
            return data.ToArray();
        }
    }
}
