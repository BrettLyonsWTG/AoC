var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var start = File.ReadAllText(file).Trim();

var disk = new List<long?>();

for (var i = 0; i < start.Length; i++)
{
    var qty = start[i] - '0';
    if (int.IsEvenInteger(i))
    {
        long? id = i / 2L;
        disk.AddRange(Enumerable.Repeat(id, qty));
    }
    else
    {
        disk.AddRange(Enumerable.Repeat((long?)null, qty));
    }
}

//for (int i = 0; i < disk.Count; i++)
//{
//    Console.Write(disk[i] is long id ? (char)(id % 10 + '0') : '.');
//}
//Console.WriteLine();

var free = 0;
for (var i = disk.Count - 1; i >= 0; i--)
{
    if (disk[i] is null)
        continue;
    free = disk.IndexOf(null, free, i - free);
    if (free == -1)
        break;
    disk[free] = disk[i];
    disk[i] = null;

    //for (int p = 0; p < disk.Count; p++)
    //{
    //    Console.Write(disk[p] is long d ? (char)(d % 10 + '0') : '.');
    //}
    //Console.WriteLine();
}

for (int p = 0; p < disk.Count; p++)
{
    Console.Write(disk[p] is long d ? (char)(d % 10 + '0') : '.');
}
Console.WriteLine();

double result = 0;
for (int i = 0; i < disk.Count; i++)
{
    if (disk[i] is null)
        break;
    result += disk[i]!.Value * i;
}

Console.WriteLine(new { result });