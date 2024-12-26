var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var map = File.ReadLines(file)
    .SelectMany((l, y) => l.Select((c, x) => (c, p: (x, y))))
    .ToDictionary(m => m.p, m => m.c);

var start = map.Where(m => m.Value == 'S').Single().Key;
var end = map.Where(m => m.Value == 'E').Single().Key;
map[start] = '.';
map[end] = '.';
var paths = map.Where(m => m.Value == '.').Select(m => m.Key).ToHashSet();
var walls = map.Where(m => m.Value == '#').Select(m => m.Key).ToHashSet();
var maxx = map.Keys.Max(p => p.x);
var maxy = map.Keys.Max(p => p.y);

var current = new List<(int x, int y)> { start };
var done = new List<(int x, int y)>();
var moves = new List<((int x, int y) from, (int x, int y) to)>();
var moves_lock = new Lock();
var print_lock = new Lock();

while (current.Count > 0)
{
    var nextMoves = current.SelectMany(c => new[] { (c.x, c.y - 1), (c.x, c.y + 1), (c.x - 1, c.y), (c.x + 1, c.y) }
        .Except(done)
        .Except(walls)
        .Select(n => (from: c, to: n)))
        .ToArray();
    moves.AddRange(nextMoves);
    var nexts = nextMoves.Select(n => n.to).Distinct().ToList();
    done.AddRange(nexts);
    current = nexts;
}

void PrintMap(List<(int, int)> path, (int, int) cut = default)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            Console.ForegroundColor = (x, y) == cut ? ConsoleColor.Green : path.Contains((x, y)) ? ConsoleColor.Red : ConsoleColor.White;
            Console.BackgroundColor = (x, y) == cut ? ConsoleColor.White : ConsoleColor.Black;
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : path.Contains((x, y)) ? 'O' : map[(x, y)]);
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

List<(int x, int y)> GetMoves((int x, int y) from, (int x, int y) to)
{
    var doneMoves = new List<((int x, int y) from, (int x, int y) to)>();
    var current = new List<(int x, int y)> { from };
    while (current.Count > 0)
    {
        ((int x, int y) from, (int x, int y) to)[] nextMoves = moves.Join(current, m => m.from, c => c, (m, _) => m)
            .Except(doneMoves)
            .ToArray();
        doneMoves.AddRange(nextMoves);
        if (nextMoves.Any(n => n.to == to))
        {
            break;
        }
        current = nextMoves.Select(n => n.to).Distinct().ToList();
    }
    if (current.Count == 0)
    {
        return [];
    }
    else
    {
        var pos = from;
        var path = new List<(int x, int y)>() { from };
        while (pos != to)
        {
            var next = doneMoves.First(m => m.from == pos).to;
            path.Add(next);
            pos = next;
        }
        return path;
    }
}

var fullPath = GetMoves(start, end);
Console.WriteLine(new { full = fullPath.Count - 1, path = paths.Count - 1 });
Console.WriteLine();

if (!fullPath.Order().SequenceEqual(paths.Order()))
{
    throw new InvalidOperationException("Full path is not the same as all paths");
}

var cuts = map
    .Where(m => m.Value is '#' && m.Key.x > 1 && m.Key.x < maxx - 1 && map[(m.Key.x - 1, m.Key.y)] == '.' && map[(m.Key.x + 1, m.Key.y)] == '.')
    .Select(m => (cut: m.Key, ends: new[] { (m.Key.x - 1, m.Key.y), (m.Key.x + 1, m.Key.y) }))
    .Concat(map
    .Where(m => m.Value is '#' && m.Key.y > 1 && m.Key.y < maxy - 1 && map[(m.Key.x, m.Key.y - 1)] == '.' && map[(m.Key.x, m.Key.y + 1)] == '.')
    .Select(m => (cut: m.Key, ends: new[] { (m.Key.x, m.Key.y - 1), (m.Key.x, m.Key.y + 1) })))
    .OrderBy(m => m.cut.y)
    .ThenBy(m => m.cut.x);

var result = 0;

foreach (var cut in cuts)
{
    var cutIn = fullPath.First(p => cut.ends.Contains(p));
    var cutInIdx = fullPath.IndexOf(cutIn);
    var cutOut = cut.ends.Except([cutIn]).Single();
    var cutOutIdx = fullPath.IndexOf(cutOut);
    var steps = cutInIdx + fullPath.Count - cutOutIdx + 1;
    var savings = fullPath.Count - 1 - steps;
    if (savings >= 100 || Debugger.IsAttached)
    {
        result++;
        PrintMap([.. fullPath[..(cutInIdx + 1)], cut.cut, .. fullPath[cutOutIdx..]], cut.cut);
        Console.WriteLine(new { cut.cut, result, savings });
        Console.WriteLine();
        Console.ReadLine();
    }
    else
    {
        Console.WriteLine(new { cut.cut, result });
    }
}

Console.WriteLine(new { result });
