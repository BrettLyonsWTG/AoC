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

var cuts = map.Where(m => 
    (m.Key.x > 1 && m.Key.x < maxx - 1 && m.Key.y > 1 && m.Key.y < maxy - 1 && m.Value is '#')
    && ((map[(m.Key.x - 1, m.Key.y)] == '.' && map[(m.Key.x + 1, m.Key.y)] == '.')
        || (map[(m.Key.x, m.Key.y - 1)] == '.' && map[(m.Key.x, m.Key.y - 1)] == '.')))
    .Select(m => m.Key);

void PrintMap(List<(int, int)> path)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x <= maxx; x++)
        {
            Console.Write((x, y) == start ? 'S' : (x, y) == end ? 'E' : path.Contains((x, y)) ? 'O' : map[(x, y)]);
        }
        Console.WriteLine();
    }
}

//int steps = 0;
//var current = new List<(int x, int y)> { start };
//var done = new List<(int x, int y)>();
//var moves = new List<((int x, int y), (int x, int y))>();

//while (current.Count > 0)
//{
//    steps++;
//    var nextMoves = current.SelectMany(c => new[] { (c.x, c.y - 1), (c.x, c.y + 1), (c.x - 1, c.y), (c.x + 1, c.y) }
//        .Except(walls)
//        .Except(done)
//        .Select(n => (from: c, to: n)))
//        .ToArray();
//    moves.AddRange(nextMoves);
//    var nexts = nextMoves.Select(n => n.to).Distinct().ToList();
//    if (nexts.Contains(end)) break;
//    done.AddRange(nexts);
//    current = nexts;
//}

//PrintMap(current);
//Console.WriteLine(new { steps });
//Console.WriteLine();

int GetSteps((int x, int y) cut)
{
    var cutWalls = walls.Except([cut]).ToHashSet();
    int steps = 0;
    var current = new List<(int x, int y)> { start };
    var done = new List<(int x, int y)>();
    var moves = new List<((int x, int y), (int x, int y))>();

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

    return steps;
}

var full = GetSteps((0, 0));

var savings = cuts.Select(c => full - GetSteps(c)).GroupBy(c => c).Select(c => (saving: c.Key, count: c.Count())).OrderBy(c => c.saving).ToList();
savings.ForEach(s => Console.WriteLine(s));
