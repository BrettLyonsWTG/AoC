var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var input = File.ReadAllLines(file)[0];

for (int i = 14; i <= input.Length; i++)
{
    if (input[(i - 14)..i].SequenceEqual(input[(i - 14)..i].Distinct()))
    {
        Console.WriteLine(new { i });
        break;
    }
}