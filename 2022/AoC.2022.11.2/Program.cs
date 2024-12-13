var file = Debugger.IsAttached ? "example.txt" : "input.txt";

List<(Queue<ulong> items, char oprtr, ulong oprnd, ulong test, int iftrue, int iffalse)> monkeys = new();

using var reader = new StreamReader(file);
string? line;
while ((line = reader.ReadLine()) != null)
{
    if (!line.StartsWith("Monkey")) break;
    line = reader.ReadLine()!;
    var items = new Queue<ulong>(line[18..].Split(", ").Select(ulong.Parse));
    line = reader.ReadLine()!;
    var oprtr = line[25..] is "old" && line[23] is '*' ? '^' : line[23];
    var oprnd = line[25..] is "old" && oprtr is '^' ? 2 : ulong.Parse(line[25..]);
    line = reader.ReadLine()!;
    var test = ulong.Parse(line[21..]);
    line = reader.ReadLine()!;
    var iftrue = int.Parse(line[29..]);
    line = reader.ReadLine()!;
    var iffalse = int.Parse(line[30..]);
    line = reader.ReadLine()!;
    monkeys.Add((items, oprtr, oprnd, test, iftrue, iffalse));
}

var divisor = lcm(monkeys.Select(m => m.test));
static ulong lcm(IEnumerable<ulong> numbers) => numbers.Aggregate((a, s) => a * s / gcd(a, s));
static ulong gcd(ulong n1, ulong n2) => n2 == 0 ? n1 : gcd(n2, n1 % n2);

var inspections = new ulong[monkeys.Count];

for (int i = 1; i <= 10000; i++)
{
    for (int j = 0; j < monkeys.Count; j++)
    {
        while (monkeys[j].items.Count > 0)
        {
            var item = monkeys[j].items.Dequeue();
            inspections[j]++;
            var worry = monkeys[j].oprtr switch
            {
                '+' => item + monkeys[j].oprnd,
                '*' => item * monkeys[j].oprnd,
                '^' => (ulong)Math.Pow(item, monkeys[j].oprnd),
                _ => throw new NotImplementedException()
            };
            var divisible = worry % monkeys[j].test == 0;
            var recipient = divisible ? monkeys[j].iftrue : monkeys[j].iffalse;
            worry %= divisor;
            monkeys[recipient].items.Enqueue(worry);
        }
    }

    if (i is 1 or 20 || i % 1000 == 0)
    {
        Console.WriteLine($"== After round {i} ==");
        for (int m = 0; m < monkeys.Count; m++)
        {
            Console.WriteLine($"Monkey {m} inspected items {inspections[m]} times.");
        }
    }
}

var results = inspections.OrderDescending().ToArray();
var result = results[0] * results[1];

Console.WriteLine(new { result });
