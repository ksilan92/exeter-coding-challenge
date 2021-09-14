using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Replace
{
    public class WordsCsv
    {
        [Name("English word")]
        public string English { get; set; }

        [Name("French word")]
        public string French { get; set; }

        [Name("Frequency")]
        public int Count { get; set; }
    }
}
