var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var range = Debugger.IsAttached ? 6 : 70;
var fallen = Debugger.IsAttached ? 12 : 1024;
var start = (x: 0, y: 0);
var end = (x: range, y: range);
var bytes = File.ReadLines(file).Select(l => { var p = l.Split(',').Select(int.Parse).ToArray(); return (x: p[0], y: p[1]); }).ToList();

void PrintGrid(IEnumerable<(int x, int y)> bytes, IEnumerable<(int x, int y)> path, (int x, int y) mark = default)
{
    var bytesSet = new HashSet<(int x, int y)>(bytes ?? []);
    var pathSet = new HashSet<(int x, int y)>(path ?? []);
    for (var y = 0; y <= range; y++)
    {
        for (var x = 0; x <= range; x++)
        {
            Console.BackgroundColor = (mark != default && mark == (x, y)) ? ConsoleColor.Red : pathSet.Contains((x, y)) ? ConsoleColor.Gray : ConsoleColor.Black;
            Console.ForegroundColor = pathSet.Contains((x, y)) ? ConsoleColor.Black : ConsoleColor.White;
            Console.Write(bytesSet.Contains((x, y)) ? '#' : pathSet.Contains((x, y)) ? 'O' : '.');
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

List<(int x, int y)> GetPath(IEnumerable<(int x, int y)> bytes)
{
    var falls = bytes.ToHashSet();
    var prevs = new List<(int x, int y)> { start };
    var moves = new Dictionary<(int x, int y), (int x, int y)>();

    while (prevs.Count > 0 && !moves.ContainsKey(end))
    {
        var nexts = prevs.SelectMany(p => new (int x, int y)[] { (p.x + 1, p.y), (p.x - 1, p.y), (p.x, p.y + 1), (p.x, p.y - 1) }
            .Where(n => n.x >= 0 && n.y >= 0 && n.x <= range && n.y <= range)
            .Except(moves.Keys)
            .Except(falls)
            .Select(n => (n, p)))
            .DistinctBy(n => n.n)
            .ToList();

        foreach (var next in nexts)
        {
            moves[next.n] = next.p;
        }

        prevs = nexts.Select(n => n.n).ToList();
    }

    if (moves.ContainsKey(end))
    {
        var endpath = new List<(int x, int y)>();
        var current = end;
        while (current != start)
        {
            endpath.Add(current);
            current = moves[current];
        }
        return endpath.Append(start).ToList();
    }
    else
    {
        return [];
    }
}

int i = fallen;
while (true)
{
    var path = GetPath(bytes.Take(i + 1));
    if (path.Count == 0)
    {
        PrintGrid(bytes[..(i + 1)], GetPath(bytes.Take(i)), bytes[i]);
        Console.WriteLine(new { i, b = $"{bytes[i].x},{bytes[i].y}" });
        break;
    }
    else
    {
        i = bytes.FindIndex(i + 1, b => path.Contains(b));
    }
}