var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long result = 0;

foreach (var line in File.ReadLines(file))
{
    var middle = line.IndexOf(',');
    var splita = line.IndexOf('-');
    var splitb = line.IndexOf('-', middle);
    var aleft = int.Parse(line[..splita]);
    var aright = int.Parse(line[(splita + 1)..middle]);
    var bleft = int.Parse(line[(middle + 1)..splitb]);
    var bright = int.Parse(line[(splitb + 1)..]);
    if ((aleft <= bleft && aright >= bright) || (bleft <= aleft && bright >= aright))
    {
        result++;
    }
}

Console.WriteLine(new { result });