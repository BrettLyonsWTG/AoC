var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var grid = File.ReadLines(file).SelectMany((line, y) => line.Select((c, x) => (x, y, c))).ToDictionary(g => (g.x, g.y), g => g.c);

var maxx = grid.Max(g => g.Key.x);
var maxy = grid.Max(g => g.Key.y);

var result = 
	grid.Where(g => g.Value is 'X' && g.Key.x < maxx - 3 
		&& grid[(g.Key.x + 1, g.Key.y)] is 'M' && grid[(g.Key.x + 2, g.Key.y)] is 'A' && grid[(g.Key.x + 3, g.Key.y)] is 'S').Concat(
	grid.Where(g => g.Value is 'X' && g.Key.x > 2 
		&& grid[(g.Key.x - 1, g.Key.y)] is 'M' && grid[(g.Key.x - 2, g.Key.y)] is 'A' && grid[(g.Key.x - 3, g.Key.y)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.y < maxy - 3 
		&& grid[(g.Key.x, g.Key.y + 1)] is 'M' && grid[(g.Key.x, g.Key.y + 2)] is 'A' && grid[(g.Key.x, g.Key.y + 3)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.y > 2 
		&& grid[(g.Key.x, g.Key.y - 1)] is 'M' && grid[(g.Key.x, g.Key.y - 2)] is 'A' && grid[(g.Key.x, g.Key.y - 3)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.x < maxx - 3 && g.Key.y < maxy - 3
		&& grid[(g.Key.x + 1, g.Key.y + 1)] is 'M' && grid[(g.Key.x + 2, g.Key.y + 2)] is 'A' && grid[(g.Key.x + 3, g.Key.y + 3)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.x > 2 && g.Key.y < maxy - 3
		&& grid[(g.Key.x - 1, g.Key.y + 1)] is 'M' && grid[(g.Key.x - 2, g.Key.y + 2)] is 'A' && grid[(g.Key.x - 3, g.Key.y + 3)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.x < maxx - 3 && g.Key.y > 2
		&& grid[(g.Key.x + 1, g.Key.y - 1)] is 'M' && grid[(g.Key.x + 2, g.Key.y - 2)] is 'A' && grid[(g.Key.x + 3, g.Key.y - 3)] is 'S')).Concat(
	grid.Where(g => g.Value is 'X' && g.Key.x > 2 && g.Key.y > 2
		&& grid[(g.Key.x - 1, g.Key.y - 1)] is 'M' && grid[(g.Key.x - 2, g.Key.y - 2)] is 'A' && grid[(g.Key.x - 3, g.Key.y - 3)] is 'S')).Count();

Console.WriteLine(new { result });

// 2466 is too low