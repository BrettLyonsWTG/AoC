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
    if (cut == default) cut = (-1, -1);

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

int maxcut = Debugger.IsAttached ? 20 : 20;
int minsav = Debugger.IsAttached ? 50 : 100;

var savings = fullPath.SelectMany((to, toCutIdx) =>
{
    Console.WriteLine($"{toCutIdx}/{fullPath.Count}");
    return Enumerable.Range(to.y - maxcut, maxcut * 2 + 1)
        .Where(y => y > 0 && y < maxy)
        .SelectMany(y =>
        {
            var dify = maxcut - Math.Abs(to.y - y);
            return Enumerable.Range(to.x - dify, dify * 2 + 1)
                .Where(x => x > 0 && x < maxx)
                .Select(x => (x, y))
                .Select(from => new { from, fromCutIdx = fullPath.IndexOf(from) })
                .Select(from => new { from.from, from.fromCutIdx, saving = from.fromCutIdx - toCutIdx - Math.Abs(from.from.x - to.x) - Math.Abs(from.from.y - to.y) })
                .Where(from => from.saving >= minsav);
        })
        .Select(from => (to, toCutIdx, from.from, from.fromCutIdx, from.saving))
        .OrderByDescending(s => s.saving);
}).ToList();

var grouped = savings.GroupBy(s => s.saving).Select(g => (g.Key, Count: g.Count())).OrderBy(g => g.Key).ToList();

grouped.ForEach(g => Console.WriteLine($"There are {g.Count} cheats that save {g.Key} picoseconds."));

Console.WriteLine(new { savings.Count });

var b = savings.First();
PrintMap(fullPath[..b.toCutIdx].Concat(fullPath[b.fromCutIdx..]).ToList(), b.to);
