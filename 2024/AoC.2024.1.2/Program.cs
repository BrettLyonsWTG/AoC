var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var left = new List<int>();
var right = new List<int>();

foreach (var line in File.ReadAllLines(file))
{
    var parts = line.Split("   ");
    left.Add(int.Parse(parts[0]));
    right.Add(int.Parse(parts[1]));
}

var result = left.Sum(l => l * right.Count(r => r == l));

Console.WriteLine(new { result });