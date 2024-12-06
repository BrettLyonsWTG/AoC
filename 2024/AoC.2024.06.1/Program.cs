var file = Debugger.IsAttached ? "example.txt" : "input.txt";


var grid = File.ReadLines(file).SelectMany((l, y) => l.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);
var maxx = grid.Keys.Max(g => g.x);
var maxy = grid.Keys.Max(g => g.y);
var pos = grid.Single(g => g.Value == '^').Key;
var positions = new HashSet<(int x, int y)> { pos };
var dir = '^';

while (true)
{
	var next = GetNext(pos, dir);
	if (next.x < 0 || next.x > maxx || next.y < 0 || next.y > maxy)
		break;
	if (grid[next] is '#')
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
		if (next.x < 0 || next.x > maxx || next.y < 0 || next.y > maxy)
			break;
	}
	pos = next;
	positions.Add(pos);
}

Console.WriteLine(new { result = positions.Count });

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
