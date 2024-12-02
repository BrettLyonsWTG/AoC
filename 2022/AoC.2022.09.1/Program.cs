var file = Debugger.IsAttached ? "example.txt" : "input.txt";

Pos head = (0, 0);
Pos tail = (0, 0);
HashSet<Pos> path = new() { tail };

foreach (var line in File.ReadLines(file))
{
    for (int i = 0; i < int.Parse(line[2..]); i++)
    {
        head = line[0] switch
        {
            'U' => (head.x, head.y - 1),
            'D' => (head.x, head.y + 1),
            'L' => (head.x - 1, head.y),
            'R' => (head.x + 1, head.y),
            _ => throw new InvalidOperationException()
        };

        var dx = tail.x - head.x;
        var dy = tail.y - head.y;

        if (!(dx is >= -1 and <= 1 && dy is >= -1 and <= 1))
        {
            tail = (tail.x + (dx switch { < 0 => 1, > 0 => -1, _ => 0 }),
                    tail.y + (dy switch { < 0 => 1, > 0 => -1, _ => 0 }));
        }

        path.Add(tail);
        PrintPath(head, tail, path);
    }
}

PrintPath(head, tail, path);

Console.WriteLine(new { positions = path.Count });

static void PrintPath(Pos head, Pos tail, ICollection<Pos> path)
{
    var miny = path.Append(head).Min(p => p.y);
    var minx = path.Append(head).Min(p => p.x);
    var maxy = path.Append(head).Max(p => p.y);
    var maxx = path.Append(head).Max(p => p.x);

    if (Debugger.IsAttached)
    {
        for (int y = miny; y <= maxy; y++)
        {
            for (int x = minx; x <= maxx; x++)
            {
                var pos = (x, y);
                Console.Write(pos == head ? 'H' : pos == tail ? 'T' : pos is (0, 0) ? 's' : path.Contains(pos) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }
    Console.WriteLine();
}

internal record struct Pos(int x, int y)
{
    public static implicit operator (int x, int y)(Pos value) => (value.x, value.y);
    public static implicit operator Pos((int x, int y) value) => new Pos(value.x, value.y);
}
