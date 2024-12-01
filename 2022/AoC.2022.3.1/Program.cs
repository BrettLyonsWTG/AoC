var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long total = 0;

foreach (var line in File.ReadLines(file))
{
    var half = line.Length / 2;
    var both = line[..half].Intersect(line[^half..]).SingleOrDefault();
    total += both is >= 'a' and <= 'z' ? both - 'a' + 1 : both - 'A' + 27;
}

Console.WriteLine(new { total });