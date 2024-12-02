var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var left = new List<int>();
var right = new List<int>();

foreach (var line in File.ReadLines(file))
{
    var parts = line.Split("   ");
    left.Add(int.Parse(parts[0]));
    right.Add(int.Parse(parts[1]));
}

left.Sort();
right.Sort();
var result = left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();

Console.WriteLine(new { result });