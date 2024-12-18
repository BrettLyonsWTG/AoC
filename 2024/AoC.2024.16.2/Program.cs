var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var track = File.ReadLines(file)
    .SelectMany((l, y) => l.Select((c, x) => (c, p: (x, y))))
    .GroupBy(t => t.c)
    .ToDictionary(g => g.Key, g => g.Select(t => t.p).ToList());

var walls = track['#'];
var paths = track['.'];
var start = track['S'].Single();
var end = track['E'].Single();
var max = walls.Max();

void PrintTrack(List<((int, int) p, char c)> path)
{
    var pathVals = path.ToDictionary(p => p.p, p => p.c);
    for (int y = 0; y <= max.y; y++)
    {
        for (int x = 0; x <= max.x; x++)
        {
            Console.ForegroundColor = walls.Contains((x, y)) ? ConsoleColor.Yellow : pathVals.ContainsKey((x, y)) ? ConsoleColor.DarkRed : ConsoleColor.White;
            Console.BackgroundColor = pathVals.ContainsKey((x, y)) ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : walls.Contains((x, y)) ? '#' : pathVals.TryGetValue((x, y), out char c) ? c : '.');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> startqueue = [((start, '>'), 0, [(start, '>')])];
List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> endqueue
    = new ((int x, int y) p, char d, char e, int c)[]
    {
        ((end.x + 1, end.y), 'v', '<', 1001),
        ((end.x + 1, end.y), '<', '<', 1),
        ((end.x + 1, end.y), '^', '<', 1001),
        ((end.x - 1, end.y), 'v', '>', 1001),
        ((end.x - 1, end.y), '>', '>', 1),
        ((end.x - 1, end.y), '^', '>', 1001),
        ((end.x, end.y + 1), '>', '^', 1001),
        ((end.x, end.y + 1), '^', '^', 1),
        ((end.x, end.y + 1), '<', '^', 1001),
        ((end.x, end.y - 1), '>', 'v', 1001),
        ((end.x, end.y - 1), 'v', 'v', 1),
        ((end.x, end.y - 1), '<', 'v', 1001),
    }.Where(e =>
        e.p.x >= 0 && e.p.y >= 0 &&
        e.p.x <= max.x && e.p.y <= max.y &&
        !walls.Contains(e.p) &&
        !walls.Contains(e.d switch
        {
            '<' => (e.p.x - 1, e.p.y),
            '>' => (e.p.x + 1, e.p.y),
            '^' => (e.p.x, e.p.y - 1),
            'v' => (e.p.x, e.p.y + 1),
            _ => e.p
        })).Select(e => (key: (e.p, e.d), e.c, path: new[] { (end, e.e), (e.p, e.d) }.ToList()))
        .ToList();

var starts = new Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)>();
var ends = new Dictionary<((int x, int y) p, char d), (int cost, List<List<((int x, int y) p, char d)>> paths)>()
{
    [(end, '>')] = (0, [[(end, '>')]]),
    [(end, '<')] = (0, [[(end, '<')]]),
    [(end, '^')] = (0, [[(end, '^')]]),
    [(end, 'v')] = (0, [[(end, 'v')]]),
};

while (startqueue.Count > 0 || endqueue.Count > 0)
{
    if (endqueue.Count > 0)
    {
        var current = endqueue.OrderBy(e => e.cost).First();
        endqueue.Remove(current);
        var nextpos = current.key.d switch
        {
            '>' => (current.key.p.x - 1, current.key.p.y),
            '<' => (current.key.p.x + 1, current.key.p.y),
            'v' => (current.key.p.x, current.key.p.y - 1),
            '^' => (current.key.p.x, current.key.p.y + 1),
        };
        char[] nextdirs = current.key.d switch
        {
            '>' => ['>', '^', 'v'],
            '<' => ['<', '^', 'v'],
            'v' => ['v', '<', '>'],
            '^' => ['^', '<', '>'],
            _ => throw new InvalidOperationException()
        };
        List<(((int x, int y) p, char d) key, int cost, List<((int x, int y) p, char d)> path)> nexts =
            nextdirs.Select(d => (p: nextpos, d, c: d == current.key.d ? 1 : 1001))
                .ExceptBy(walls, n => n.p)
                .Select(n => (key: (n.p, n.d), cost: current.cost + n.c, path: current.path.Append((n.p, n.d)).ToList()))
                .Where(n => !ends.TryGetValue(n.key, out var p) || p.cost >= n.cost)
                .ToList();

        foreach (var next in nexts)
        {
            PrintTrack(next.path);
            Console.ReadLine();
            if (ends.TryGetValue(next.key, out var last))
            {
                if (next.cost < last.cost)
                {
                    last.cost = next.cost;
                    last.paths.Clear();
                }
                last.paths.Add(next.path);
            }
            else
            {
                ends[next.key] = (next.cost, [next.path]);
            }
            if (!starts.ContainsKey(next.key))
            {
                endqueue.Add((next.key, next.cost, next.path));
            }
        }
    }
}