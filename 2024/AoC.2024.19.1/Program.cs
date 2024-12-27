var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);

var towels = lines[0].Split(", ");
var designs = lines[2..].ToArray();

var possible = 0;

foreach (var design in designs)
{
    List<string> patterns = [""];
    while (patterns.Count > 0)
    {
        patterns = patterns.SelectMany(p =>
            towels.Where(t => design.Substring(p.Length).StartsWith(t))
                .Select(t => p + t)).Distinct().ToList();

        if (patterns.Any(n => n == design))
        {
            possible++;
            patterns.Clear();
        }
    }
}

Console.WriteLine(new { possible });