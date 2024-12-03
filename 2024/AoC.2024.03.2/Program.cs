var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var input = File.ReadAllText(file);
bool enabled = true;
int result = 0;

foreach (Match m in Regex.Matches(input, @"mul\((\d+),(\d+)\)|do\(\)|don\'t\(\)"))
{
	switch (m.ValueSpan)
	{
		case "do()":
			enabled = true;
			break;
		case "don't()":
			enabled = false;
			break;
		default:
			if (enabled)
				result += int.Parse(m.Groups[1].ValueSpan) * int.Parse(m.Groups[2].ValueSpan);
			break;
	}
}

Console.WriteLine(new { result });