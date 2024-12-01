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
if (calories > 0)
    elves.Add(calories);

elves.Sort();
var top = elves[^3..^0].Sum();

Console.WriteLine(new { top });