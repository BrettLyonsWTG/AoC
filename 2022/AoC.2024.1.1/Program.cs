var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var elves = new List<int>();

string? line;
var calories = 0;
using var reader = new StreamReader(file);
while ((line = reader.ReadLine()) is not null)
{
    if (line is "")
    {
        elves.Add(calories);
        calories = 0;
    }
    else
    {
        calories += int.Parse(line);
    }
}

var max = elves.Max();
var pos = elves.IndexOf(max) + 1;

Console.WriteLine(new { pos, max });