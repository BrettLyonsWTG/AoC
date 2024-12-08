var file = Debugger.IsAttached ? "example.txt" : "input.txt";

List<(Queue<int> items, char oprtr, int oprnd, int test, int iftrue, int iffalse)> monkeys = new();

using var reader = new StreamReader(file);
string? line;
while((line = reader.ReadLine()) != null)
{
    if (!line.StartsWith("Monkey")) break;
    line = reader.ReadLine()!;
    var items = new Queue<int>(line[18..].Split(", ").Select(int.Parse));
    line = reader.ReadLine()!;
    var oprtr = line[25..] is "old" && line[23] is '*' ? '^' : line[23];
    var oprnd = line[25..] is "old" && oprtr is '^' ? 2 : int.Parse(line[25..]);
    line = reader.ReadLine()!;
    var test = int.Parse(line[21..]);
    line = reader.ReadLine()!;
    var iftrue = int.Parse(line[29..]);
    line = reader.ReadLine()!;
    var iffalse = int.Parse(line[30..]);
    line = reader.ReadLine()!;
    monkeys.Add((items, oprtr, oprnd, test, iftrue, iffalse));
}

var inspections = new int[monkeys.Count];

for (int i = 1; i <= 20; i++)
{
    Console.WriteLine($"Round {i}:");
    for (int j = 0; j < monkeys.Count; j++)
    {
        //if (Debugger.IsAttached) Console.WriteLine($"Monkey {j}:");
        while (monkeys[j].items.Count > 0)
        {
            var item = monkeys[j].items.Dequeue();
            //if (Debugger.IsAttached) Console.WriteLine($"  Monkey inspects an item with a worry level of {item}.");
            inspections[j]++;
            var worry = monkeys[j].oprtr switch
            {
                '+' => item + monkeys[j].oprnd,
                '*' => item * monkeys[j].oprnd,
                '^' => (int)Math.Pow(item, monkeys[j].oprnd),
                _ => throw new NotImplementedException()
            };
            //if (Debugger.IsAttached) Console.WriteLine($"    Worry level {(monkeys[j].oprtr is '+' ? "increases by" : "is multiplied by")} {(monkeys[j].oprtr is '^' ? "itself" : monkeys[j].oprnd.ToString())} to {worry}.");
            worry /= 3;
            //if (Debugger.IsAttached) Console.WriteLine($"    Monkey gets bored with item. Worry level is divided by 3 to {worry}.");
            var divisible = worry % monkeys[j].test == 0;
            //if (Debugger.IsAttached) Console.WriteLine($"    Current worry level is{(result ? " " : " not ")}divisible by {monkeys[j].test}.");
            var recipient = divisible ? monkeys[j].iftrue : monkeys[j].iffalse;
            //if (Debugger.IsAttached) Console.WriteLine($"    Item with worry level 500 is thrown to monkey 3.");
            monkeys[recipient].items.Enqueue(worry);
        }
    }

    for (int m = 0; m < monkeys.Count; m++)
    {
        Console.WriteLine($"  Monkey {m}: {string.Join(", ", monkeys[m].items)}");
    }
}

for (int i = 0; i < inspections.Length; i++)
{
    Console.WriteLine($"Monkey {i} inspected items {inspections[i]} times.");
}

var results = inspections.OrderDescending().ToArray();
var result = results[0] * results[1];

Console.WriteLine(new { result });
