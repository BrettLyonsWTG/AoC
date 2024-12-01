var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var input = File.ReadAllLines(file)[0];

for (int i = 4; i <= input.Length; i++)
{
    if (input[(i - 4)..i].SequenceEqual(input[(i - 4)..i].Distinct()))
    {
        Console.WriteLine(new { i });
        break;
    }
}