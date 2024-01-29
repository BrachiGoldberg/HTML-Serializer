// See https://aka.ms/new-console-template for more information
using HtmlSerializer;
using System.Text.RegularExpressions;


//var html =await Load("https://ultracode.education/");
var html = File.ReadAllText(@"C:\\Users\\user1\\Documents\\ברכי\\תיכנות\\שנה א\\web\\html\\עוז והדר\\index.html");
var cleanHtml = new Regex("\\n").Replace(html, "");
cleanHtml = new Regex("\\r").Replace(cleanHtml, "");
cleanHtml = new Regex("\\t").Replace(cleanHtml, "");
var elementsSplit = new Regex("<([^<]*?)>").Split(cleanHtml).Where(e=>e.Length>0).ToArray();

int i = 0;
string elementH;
for (int j = 0; j < elementsSplit.Length; j++)
{
    elementH = elementsSplit[j];
    for(int k = 0; k < elementsSplit[j].Length; )
    {
        if (elementsSplit[j][k] != cleanHtml[i])
        {
            if (cleanHtml[i] == '<')
                elementH = "<" + elementH;
        }
        else
            k++;
        i++;
    }
    elementsSplit[j] = elementH;
}

List<string> htmlElements = new List<string>();
foreach (var element in elementsSplit)
    if (new Regex("\\S").Match(element).Success)
        htmlElements.Add(element);


HtmlElement root = Serialize(htmlElements);


var htmlQuery = Selector.ConvertQueryToObject("nav label.md-icon path");
htmlQuery = Selector.ConvertQueryToObject("#contents div#element");
var result = root.MatchElementToQuery(htmlQuery);

Console.WriteLine();
async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

HtmlElement Serialize(List<string> htmlElements)
{
    HtmlElement root = new HtmlElement();
    HtmlElement current = new HtmlElement();
    int currentTag = 0;
    foreach (var element in htmlElements)
    {
        if (element == "</html")
            Console.WriteLine("The page scanned successfully");
        else if (element.Length >= 2 && element.Substring(0, 2) == "</")
            current = current.Parent;

        else
        {
            var firstWord = element.Split(" ")[0];

            if (firstWord.Length > 0 && firstWord[0] == '<')
            {
                firstWord = firstWord.Substring(1);
                if (HtmlHelper.Instance.HtmlTags.Any(t => t == firstWord))
                {
                    var newElement = new HtmlElement();
                    newElement.Name = firstWord;
                    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(element);
                    var idAttribute = attributes.FirstOrDefault(a => a.ToString().Split("=")[0] == "id");
                    if (idAttribute != null)
                        newElement.Id = idAttribute.ToString().Split("=")[1].Replace("\"", "");
                    newElement.Attributes.AddRange(attributes.Select(a => a.ToString()));
                    var classAttribute = attributes.FirstOrDefault(a => a.ToString().Split("=")[0] == "class");
                    if (classAttribute != null)
                        newElement.Classes.AddRange(classAttribute.ToString().Split('=')[1].ToString().Replace("\"", "").Split(" "));
                    if (firstWord == "html")
                    {
                        root = new HtmlElement();
                        current = root = newElement;
                    }
                    else
                    {
                        current.Children.Add(newElement);
                        newElement.Parent = current;
                        if (!HtmlHelper.Instance.HtmlVoidTags.Any(t => t == firstWord))
                            current = newElement;
                    }
                }
            }
            else
                current.InnerHtml = element;
        }
    }
    return root;
}
