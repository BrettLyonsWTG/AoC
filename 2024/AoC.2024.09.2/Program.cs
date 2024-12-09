var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var start = File.ReadAllText(file).Trim();

var disk = new List<(long? id, int qty)>();

for (var i = 0; i < start.Length; i++)
{
    var qty = start[i] - '0';
    long? id = int.IsEvenInteger(i) ? i / 2L : null;
    disk.Add((id, qty));
}

PrintDisk();

long m = disk.Max(d => d.id.GetValueOrDefault());
for (; m >= 0; m--)
{
    var cur = disk.FindIndex(d => d.id == m);
    var free = disk.FindIndex(0, cur, d => d.id is null && d.qty >= disk[cur].qty);
    if (free == -1)
        continue;
    if (disk[cur].qty == disk[free].qty)
    {
        disk[free] = disk[cur];
        disk[cur] = (null, disk[cur].qty);
    }
    else
    {
        var spare = disk[free].qty - disk[cur].qty;
        disk.Insert(free, disk[cur]);
        disk[free + 1] = (null, spare);
        disk[cur + 1] = (null, disk[cur + 1].qty);
    }
}

PrintDisk();

double result = 0;
var max = disk.Sum(d => d.qty);
var pos = 0;
foreach (var f in disk)
{
    if (f.id is not null)
    {
        for (var i = 0; i < f.qty; i++)
        {
            result += f.id.Value * pos++;
        }
    }
    else
    {
        pos += f.qty;
    }
    if (pos >= max)
        break;
}

Console.WriteLine(new { result });

void PrintDisk()
{
    foreach (var file in disk)
    {
        Console.Write(new string(file.id.HasValue ? (char)(file.id.Value % 10 + '0') : '.', file.qty));
    }
    Console.WriteLine();
}