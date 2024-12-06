var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var result = File.ReadLines(file).Select(l => l.Where(c => char.IsDigit(c)).Select(c => c - '0')).Sum(d => d.First() * 10 + d.Last());

Console.WriteLine(new { result });