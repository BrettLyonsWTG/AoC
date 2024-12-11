var file = Debugger.IsAttached ? "example.txt" : "input.txt";
var cycles = Debugger.IsAttached ? 6 : 75;

var stones = File.ReadAllText(file).Trim().Split().Select(long.Parse).ToList();

Console.WriteLine($"Initial arrangement:");
Console.WriteLine(string.Join(" ", stones));
Console.WriteLine();

var splits = new Dictionary<long, long[]> { [0] = [1] };
var nextKeys = stones.Except(splits.Keys).Distinct().ToList();

for (int b = 1; b <= cycles && nextKeys.Count > 0; b++)
{
    var newKeys = new List<long>();
    foreach (var s in nextKeys)
    {
        var digits = s.ToString().Length;
        if (digits % 2 == 0)
        {
            var div = (long)Math.Pow(10, digits / 2);
            var left = s / div;
            var right = s % div;
            splits[s] = [left, right];
        }
        else
        {
            splits[s] = [s * 2024];
        }
        newKeys.AddRange(splits[s].Except(splits.Keys));
    }
    nextKeys = newKeys;
}

var splitCounts = stones.GroupBy(s => s).ToFrozenDictionary(s => s.Key, s => s.LongCount());

for (int c = 1; c <= cycles; c++)
{
    splitCounts = splitCounts.SelectMany(s => splits[s.Key].Select(x => new { x, s.Value })).GroupBy(n => n.x).ToFrozenDictionary(n => n.Key, n => n.Sum(x => x.Value));
    Console.WriteLine(new { c, sum = splitCounts.Values.Sum() });
}

Console.WriteLine(new { sum = splitCounts.Values.Sum() });
