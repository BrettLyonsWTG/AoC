using System.Xml.Linq;

var file = Debugger.IsAttached ? "example.txt" : "input.txt";

XElement root = new XElement("root", new XAttribute("dir", true));
XDocument doc = new XDocument(root);
XElement ? current = null;

foreach (var line in File.ReadLines(file))
{
    if (line.StartsWith("$ cd"))
    {
        var path = line[5..];
        if (path == "/")
            current = root;
        else if (path == "..")
            current = current!.Parent!;
        else
            current = current!.Element(path)!;
    }
    else if (line is "$ ls") { continue; }
    else
    {
        var parts = line.Split(' ');
        var size = parts[0];
        var name = parts[1];
        if (size is "dir")
            current!.Add(new XElement(name, new XAttribute("dir", true)));
        else
            current!.Add(new XElement(name, new XAttribute("size", size)));
    }
}

var result = doc.Descendants().Where(e => e.Attribute("dir") != null)
    .Select(d => d.Descendants().Sum(e => e.Attribute("size")?.Value is string s ? int.Parse(s) : 0))
    .Where(s => s <= 100000)
    .Sum();

Console.WriteLine(new { result });