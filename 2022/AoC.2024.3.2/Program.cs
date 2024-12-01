var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long total = 0;
var lines = File.ReadAllLines(file);

for (int i = 0; i < lines.Length; i += 3)
{
    var badge = lines[i].Intersect(lines[i + 1]).Intersect(lines[i + 2]).SingleOrDefault();
    total += badge is >= 'a' and <= 'z' ? badge - 'a' + 1 : badge - 'A' + 27;
}

Console.WriteLine(new { total });