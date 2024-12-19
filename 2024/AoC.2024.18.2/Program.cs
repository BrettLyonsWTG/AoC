var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var range = Debugger.IsAttached ? 6 : 70;
var fallen = Debugger.IsAttached ? 12 : 1024;
var start = (x: 0, y: 0);
var end = (x: range, y: range);
var bytes = File.ReadLines(file).Select(l => { var p = l.Split(',').Select(int.Parse).ToArray(); return (x: p[0], y: p[1]); }).ToArray();
var falls = bytes.Take(fallen).ToList();

void PrintGrid(IEnumerable<(int x, int y)> bytes, IEnumerable<(int x, int y)> path)
{
    var bytesSet = new HashSet<(int x, int y)>(bytes ?? []);
    var pathSet = new HashSet<(int x, int y)>(path ?? []);
    for (var y = 0; y <= range; y++)
    {
        for (var x = 0; x <= range; x++)
        {
            Console.Write(bytesSet.Contains((x, y)) ? '#' : pathSet.Contains((x, y)) ? 'O' : '.');
        }
        Console.WriteLine();
    }
}

var steps = 0;
var prevs = new List<(int x, int y)> { start };
var moves = new Dictionary<(int x, int y), (int x, int y)>();

while (!moves.ContainsKey(end))
{
    var nexts = prevs.SelectMany(p => new (int x, int y)[] { (p.x + 1, p.y), (p.x - 1, p.y), (p.x, p.y + 1), (p.x, p.y - 1) }
        .Where(n => n.x >= 0 && n.y >= 0 && n.x <= range && n.y <= range)
        .Except(falls)
        .Except(moves.Keys)
        .Select(n => (n, p)))
        .DistinctBy(n => n.n)
        .ToList();

    foreach (var next in nexts)
    {
        moves[next.n] = next.p;
    }

    prevs = nexts.Select(n => n.n).ToList();
    steps++;
}

var endpath = new List<(int x, int y)>();
var current = end;
while (current != start)
{
    endpath.Add(current);
    current = moves[current];
}

PrintGrid(falls, endpath.Append(start));
Console.WriteLine(new { steps });
