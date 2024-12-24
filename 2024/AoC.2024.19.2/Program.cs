using System.ComponentModel.DataAnnotations;

var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);

var towels = lines[0].Split(", ");
var designs = lines[2..].ToArray();

long possible = 0;

foreach (var design in designs)
{
    Console.WriteLine(new { design });
    Dictionary<int, long> combos = new() { [0] = 1 };
    while (combos.Count > 0)
    {
        var mincombo = combos.Keys.Min();
        var unmatched = design[mincombo..];
        var matches = towels.Where(unmatched.StartsWith).Select(t => t.Length).GroupBy(l => l).Select(g => (length: g.Key, count: g.Count())).ToList();

        foreach (var match in matches)
        {
            var next = mincombo + match.length;
            if (next == design.Length)
            {
                possible += combos[mincombo] * match.count;
            }
            else
            {
                if (!combos.ContainsKey(next))
                {
                    combos[next] = 0;
                }
                combos[next] += combos[mincombo] * match.count;
            }
        }
        combos.Remove(mincombo);
    }
}

Console.WriteLine(new { possible });