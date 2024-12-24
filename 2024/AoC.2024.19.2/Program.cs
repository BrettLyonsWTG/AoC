var file = Debugger.IsAttached ? "input.txt" : "input.txt";

var lines = File.ReadAllLines(file);

var towels = lines[0].Split(", ");
var designs = lines[2..].ToArray();

var possible = 0;

foreach (var design in designs)
{
    Console.WriteLine(new { design });
    List<string> patterns = [""];
    while (patterns.Count > 0)
    {
        patterns = patterns.SelectMany(p => 
            towels.Where(t => design.Substring(p.Length).StartsWith(t))
                .Select(t => p + t)).ToList();

        possible += patterns.Count(n => n == design);
        patterns.RemoveAll(n => n == design);
    }
}

Console.WriteLine(new { possible });