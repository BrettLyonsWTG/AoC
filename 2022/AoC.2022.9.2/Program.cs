var file = Debugger.IsAttached ? "example.txt" : "input.txt";

List<Pos> knots = [.. Enumerable.Repeat((0, 0), 10)];
HashSet<Pos> path = [(0, 0)];

foreach (var line in File.ReadLines(file))
{
    for (int i = 0; i < int.Parse(line[2..]); i++)
    {
        knots[0] = line[0] switch
        {
            'U' => (knots[0].x, knots[0].y - 1),
            'D' => (knots[0].x, knots[0].y + 1),
            'L' => (knots[0].x - 1, knots[0].y),
            'R' => (knots[0].x + 1, knots[0].y),
            _ => throw new InvalidOperationException()
        };

        for (int h = 0, t = 1; t < knots.Count; h++, t++)
        {
            var dx = knots[t].x - knots[h].x;
            var dy = knots[t].y - knots[h].y;

            if (!(dx is >= -1 and <= 1 && dy is >= -1 and <= 1))
            {
                knots[t] = (knots[t].x + (dx switch { < 0 => 1, > 0 => -1, _ => 0 }),
                            knots[t].y + (dy switch { < 0 => 1, > 0 => -1, _ => 0 }));
            }
        }

        path.Add(knots[9]);
    }
}

Console.WriteLine(new { positions = path.Count });

internal record struct Pos(int x, int y)
{
    public static implicit operator (int x, int y)(Pos value) => (value.x, value.y);
    public static implicit operator Pos((int x, int y) value) => new Pos(value.x, value.y);
}
