using static System.Net.Mime.MediaTypeNames;

var file = Debugger.IsAttached ? "input.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);
var maxx = grid.Keys.Max(g => g.x);
var maxy = grid.Keys.Max(g => g.y);
var start = grid.Single(g => g.Value == '^').Key;

var starting = GetPositions((int.MinValue, int.MinValue));

int result = 0;

//foreach (var pos in starting.positions.Where(p => p.Key.x < maxx && p.Value.Contains('>') && grid.Any(g => g.Key.x == p.Key.x && g.Key.y > p.Key.y && g.Value is '#')))
//{
//    var obstacle = (pos.Key.x + 1, pos.Key.y);
//    var test = GetPositions(obstacle);
//    //Console.WriteLine(new { pos.Key, dir = '>', test.looping });
//    if (test.looping)
//    {
//        result++;
//        //if (Debugger.IsAttached) PrintPositions(test.positions, obstacle);
//    }
//}

//foreach (var pos in starting.positions.Where(p => p.Key.x > 0 && p.Value.Contains('<') && grid.Any(g => g.Key.x == p.Key.x && g.Key.y < p.Key.y && g.Value is '#')))
//{
//    var obstacle = (pos.Key.x - 1, pos.Key.y);
//    var test = GetPositions(obstacle);
//    //Console.WriteLine(new { pos.Key, dir = '<', test.looping });
//    if (test.looping)
//    {
//        result++;
//        //if (Debugger.IsAttached) PrintPositions(test.positions, obstacle);
//    }
//}

//foreach (var pos in starting.positions.Where(p => p.Key.y < maxy && p.Value.Contains('v') && grid.Any(g => g.Key.x < p.Key.x && g.Key.y == p.Key.y && g.Value is '#')))
//{
//    var obstacle = (pos.Key.x, pos.Key.y + 1);
//    var test = GetPositions(obstacle);
//    //Console.WriteLine(new { pos.Key, dir = 'v', test.looping });
//    if (test.looping)
//    {
//        result++;
//        //if (Debugger.IsAttached) PrintPositions(test.positions, obstacle);
//    }
//}

//foreach (var pos in starting.positions.Where(p => p.Key.y > 0 && p.Value.Contains('^') && grid.Any(g => g.Key.x > p.Key.x && g.Key.y == p.Key.y && g.Value is '#')))
//{
//    var obstacle = (pos.Key.x, pos.Key.y - 1);
//    Console.WriteLine(new { pos.Key, dir = 'v'});
//    var test = GetPositions(obstacle);
//    Console.WriteLine(new { pos.Key, dir = '^', test.looping });
//    if (test.looping)
//    {
//        result++;
//        //if (Debugger.IsAttached) PrintPositions(test.positions, obstacle);
//    }
//}

var obstacle = (11, 69);
var test = GetPositions(obstacle);
PrintPositions(test.positions, obstacle);
//Console.WriteLine(new { pos.Key, dir = '^', test.looping });

Console.WriteLine(new { result });

(Dictionary<(int x, int y), List<char>> positions, bool looping) GetPositions((int x, int y) obstacle)
{
    var positions = new Dictionary<(int x, int y), List<char>>();
    bool looping = false;
    var pos = start;
    var dir = '^';

    while (true)
    {
        if (positions.TryGetValue(pos, out var d) && d.Contains(dir)) { looping = true; break; }
        var next = GetNext(pos, dir);
        var path = grid.TryGetValue(next, out char c) && c is '#' || next == obstacle ? '+' : dir;
        if (positions.TryGetValue(pos, out var p))
        {
            if (!p.Contains(path)) p.Add(path);
        }
        else
        {
            positions[pos] = [path];
        }
        if (next.x < 0 || next.x > maxx || next.y < 0 || next.y > maxy) break;

        if (grid[next] is '#' || next == obstacle)
        {
            dir = dir switch
            {
                '^' => '>',
                '>' => 'v',
                'v' => '<',
                '<' => '^',
                _ => throw new InvalidOperationException()
            };
            next = GetNext(pos, dir);
        }
        pos = next;
    }
    return (positions, looping);
}

void PrintPositions(Dictionary<(int x, int y), List<char>> positions, (int x, int y) obstacle)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x < maxx; x++)
        {
            Console.Write((x, y) == start ? '^' : (x, y) == obstacle ? 'O' : positions.TryGetValue((x, y), out var c) ? c.Count > 1 ? '+' 
                : c[0] is '<' or '>' ? '-' : c[0] is '^' or 'v' ? '|' : c[0] : grid[(x, y)]);
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

static (int x, int y) GetNext((int x, int y) pos, char dir)
{
    return dir switch
    {
        '^' => (pos.x, pos.y - 1),
        '>' => (pos.x + 1, pos.y),
        'v' => (pos.x, pos.y + 1),
        '<' => (pos.x - 1, pos.y),
        _ => throw new InvalidOperationException()
    };
}
