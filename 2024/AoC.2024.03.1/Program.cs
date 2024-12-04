var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var input = File.ReadAllText(file);
var result = Regex.Matches(input, @"mul\((\d+),(\d+)\)").Sum(m => int.Parse(m.Groups[1].ValueSpan) * int.Parse(m.Groups[2].ValueSpan));

Console.WriteLine(new { result });