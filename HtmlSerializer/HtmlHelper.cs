using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlHelper
    {
        private static readonly HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public List<string> HtmlTags { get; set; }
        public List<string> HtmlVoidTags { get; set; }

        private HtmlHelper()
        {
            var tags = File.ReadAllText("jsonFiles/HtmlTags.json");
            HtmlTags = new List<string>();
            HtmlTags.AddRange(JsonSerializer.Deserialize<string[]>(tags)); 
            
            var voidTags = File.ReadAllText("jsonFiles/HtmlVoidTags.json");
            HtmlVoidTags = new List<string>();
            HtmlVoidTags.AddRange(JsonSerializer.Deserialize<string[]>(voidTags));
        }
    }
}
