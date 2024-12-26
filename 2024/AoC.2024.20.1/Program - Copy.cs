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

List<(int x, int y)> GetSteps((int x, int y) cut)
{
    var cutWalls = walls.Except([cut]).ToHashSet();
    int steps = 0;
    var current = new List<(int x, int y)> { start };
    var done = new List<(int x, int y)>();
    var moves = new List<((int x, int y) from, (int x, int y) to)>();

    while (current.Count > 0)
    {
        steps++;
        var nextMoves = current.SelectMany(c => new[] { (c.x, c.y - 1), (c.x, c.y + 1), (c.x - 1, c.y), (c.x + 1, c.y) }
            .Except(cutWalls)
            .Except(done)
            .Select(n => (from: c, to: n)))
            .ToArray();
        moves.AddRange(nextMoves);
        var nexts = nextMoves.Select(n => n.to).Distinct().ToList();
        if (nexts.Contains(end)) break;
        done.AddRange(nexts);
        current = nexts;
    }

    var path = new List<(int x, int y)> { end };
    while (path.Last() != start)
    {
        var last = path.Last();
        var move = moves.First(m => m.to == last);
        path.Add(move.from);
    }
    return path;
}

var full = GetSteps((-1, -1));
PrintMap(full, (-1, -1));
Console.WriteLine();

var cuts = map.Where(m => m.Value is '#' &&
    ((m.Key.x > 1 && m.Key.x < maxx - 1 && map[(m.Key.x - 1, m.Key.y)] == '.' && map[(m.Key.x + 1, m.Key.y)] == '.') ||
     (m.Key.y > 1 && m.Key.y < maxy - 1 && map[(m.Key.x, m.Key.y - 1)] == '.' && map[(m.Key.x, m.Key.y + 1)] == '.')))
    .Select(m => m.Key)
    .OrderBy(m => m.y)
    .ThenBy(m => m.x);

var savingCount = 0;
var savings = cuts.Select(cut => (cut, path: GetSteps(cut))).Where(c => full.Count - c.path.Count >= 100);
foreach (var (cut, path) in savings)
{
    savingCount++;
    //PrintMap(path, cut);
    Console.WriteLine(new { cut, savings = full.Count - path.Count });
    //Console.WriteLine();
    //Console.ReadLine();
}

//foreach (var cut in cuts)
//{
//    var steps = GetSteps(cut);
//    PrintMap(steps, cut);
//    Console.WriteLine(new { cut, steps = steps.Count - 1 });
//    Console.ReadLine();
//}
