var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);
var maxx = grid.Keys.Max(g => g.x);
var maxy = grid.Keys.Max(g => g.y);
var start = grid.Single(g => g.Value == '^').Key;

var starting = GetPositions((int.MinValue, int.MinValue));
PrintPositions(starting.positions, (int.MinValue, int.MinValue));

var test = GetPositions((3, 6));
PrintPositions(test.positions, (3, 6));
Console.WriteLine(new { test.looping });

(Dictionary<(int x, int y), char> positions, bool looping) GetPositions((int x, int y) obstacle)
{
    var positions = new Dictionary<(int x, int y), char>();
    bool looping = false;
    var pos = start;
    var dir = '^';

    while (true)
    {
        if (positions.TryGetValue(pos, out char d) && d == dir) { looping = true; break; }
        var next = GetNext(pos, dir);
        var path = grid.TryGetValue(next, out char c) && c is '#' || next == obstacle ? '+' : dir;
        positions[pos] = path;
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

void PrintPositions(Dictionary<(int x, int y), char> positions, (int x, int y) obstacle)
{
    for (int y = 0; y <= maxy; y++)
    {
        for (int x = 0; x < maxx; x++)
        {
            Console.Write((x, y) == start ? '^' : (x, y) == obstacle ? 'O' : positions.TryGetValue((x, y), out char c) ? c is '<' or '>' ? '-' : c is '^' or 'v' ? '|' : c : grid[(x, y)]);
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
