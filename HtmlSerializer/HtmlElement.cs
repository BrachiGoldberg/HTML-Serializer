using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlElement
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; } = new List<string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();


        public IEnumerable<HtmlElement> Descendants()
        {
            var head = this;
            Queue<HtmlElement> q = new Queue<HtmlElement>();
            q.Enqueue(head);
            var element = q.Dequeue();
            foreach (var child in element.Children)
                q.Enqueue(child);
            while (q.Count > 0)
            {
                element = q.Dequeue();
                foreach (var child in element.Children)
                    q.Enqueue(child);
                yield return element;
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            var leaf = this;
            while (leaf != null)
            {
                yield return leaf;
                leaf = leaf.Parent;
            }
        }

        public IEnumerable<HtmlElement> MatchElementToQuery(Selector query)
        {
            var setResult = new HashSet<HtmlElement>();
            FindMatchElement(this.Descendants(), query, setResult);
            return setResult;
        }

        private static void FindMatchElement(IEnumerable<HtmlElement> descendants, Selector query, HashSet<HtmlElement> elementsMatched)
        {
            if (query == null)
                return;

            foreach (var descendant in descendants)
            {
                if ((query.TagName == null || descendant.Name == query.TagName)
                && (query.Id == null || query.Id == descendant.Id)
                && (query.Classes.All(c => descendant.Classes.Any(cr => c == cr))))
                {
                    if (query.Child == null)
                        elementsMatched.Add(descendant);
                    FindMatchElement(descendant.Descendants(), query.Child, elementsMatched);
                }
            }
        }
    }
}
