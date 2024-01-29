using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        public static Selector ConvertQueryToObject(string query = "div.container #my-div.my-class")
        {
            if (query == null || query == "")
                return null;
            var levels = query.Split(' ');

            var root = new Selector();
            var current = new Selector();
            root = current;
           
            for (int i = 0; i < levels.Length; i++)
            {
                var newSelector = new Selector();
                var level = levels[i];
                var elementsQuery = levels[i].Split('.', '#').Where(s=>s.Length>0);
                int indexElement = 0, j = 0;
                if (level[0] != '.' && level[0] != '#')
                {
                    if (HtmlHelper.Instance.HtmlTags.Any(t => t == elementsQuery.ElementAt(indexElement)))
                    {
                        newSelector.TagName = elementsQuery.ElementAt(indexElement++);
                        j = 1;
                    }
                }
                for (; j < level.Length; j++)
                {
                    if (level[j] == '.')
                        newSelector.Classes.Add(elementsQuery.ElementAt(indexElement));
                    else if (level[j] == '#')
                        newSelector.Id = elementsQuery.ElementAt(indexElement++);
                }
                current.Child = newSelector;
                newSelector.Parent = current;
                current = newSelector;
            }

            root = root.Child;

            return root;
        }
    }
}
