var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var lines = File.ReadAllLines(file);
var middle = Array.IndexOf(lines, "");
var rules = lines[..middle].Select(l => (a: int.Parse(l[..2]), b: int.Parse(l[3..]))).ToHashSet();
var updates = lines[(middle + 1)..].Select(l => l.Split(',').Select(int.Parse).ToList()).ToList();

var result = updates.Where(u =>
{
	for (int a = 0; a < u.Count - 1; a++)
	{
		for (int b = a + 1; b < u.Count; b++)
        {
            if (rules.Contains((u[b], u[a])))
            {
                return false;
            }
        }
    }
	return true;
}).Sum(u => u[(u.Count - 1) / 2]);

Console.WriteLine(new { result });