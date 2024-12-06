var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var result = File.ReadLines(file).Select(ParseDigits).Sum(d => d.First() * 10 + d.Last());

Console.WriteLine(new { result });

IEnumerable<int> ParseDigits(string line)
{
    for (var i = 0; i < line.Length; i++)
    {
        if (char.IsDigit(line[i])) yield return line[i] - '0';
        var sub = line.Substring(i);
        if (sub.StartsWith("one")) yield return 1;
        if (sub.StartsWith("two")) yield return 2;
        if (sub.StartsWith("three")) yield return 3;
        if (sub.StartsWith("four")) yield return 4;
        if (sub.StartsWith("five")) yield return 5;
        if (sub.StartsWith("six")) yield return 6;
        if (sub.StartsWith("seven")) yield return 7;
        if (sub.StartsWith("eight")) yield return 8;
        if (sub.StartsWith("nine")) yield return 9;
    }
}