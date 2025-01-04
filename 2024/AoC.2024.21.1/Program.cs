var file = Debugger.IsAttached ? "example.txt" : "input.txt";

var codes = File.ReadAllLines(file).Select(c => (code: c, num: int.Parse(c[..3]))).ToList();

IEnumerable<List<char>> GetNumCombos(IEnumerable<char> input, IEnumerable<char> done, (int x, int y) pos)
{
    if (input.Any())
    {
        foreach (var presses in GetNumPresses(input.First(), ref pos))
        {
            foreach (var combo in GetNumCombos(input.Skip(1), done.Concat(presses).Append('A'), pos))
            {
                yield return combo;
            }
        }
    }
    else
    {
        yield return done.ToList();
    }
}

List<List<char>> GetNumPresses(char button, ref (int x, int y) pos)
{
    var presses = new List<List<string>>();

    (int x, int y) next = button switch
    {
        'A' => (2, 3),
        '0' => (1, 3),
        '1' => (0, 2),
        '2' => (1, 2),
        '3' => (2, 2),
        '4' => (0, 1),
        '5' => (1, 1),
        '6' => (2, 1),
        '7' => (0, 0),
        '8' => (1, 0),
        '9' => (2, 0),
        _ => throw new InvalidOperationException()
    };
    var presses1 = new List<char>();
    if (next.x > pos.x) presses1.AddRange(Enumerable.Repeat('>', next.x - pos.x));
    if (next.x < pos.x) presses1.AddRange(Enumerable.Repeat('<', pos.x - next.x));
    if (next.y > pos.y) presses1.AddRange(Enumerable.Repeat('v', next.y - pos.y));
    if (next.y < pos.y) presses1.AddRange(Enumerable.Repeat('^', pos.y - next.y));

    var combos = GetCombos(presses1).ToList();

    if (pos.x == 0 && next.y == 3)
    {
        var down = 3 - pos.y;
        combos.RemoveAll(c => c.Take(down).All(d => d == 'v'));
    }
    else if (pos.y == 3 && next.x == 0)
    {
        var left = 2 - pos.x;
        combos.RemoveAll(c => c.First() == '<');
    }

    pos = next;
    return combos;
}

IEnumerable<List<char>> GetDirCombos(IEnumerable<char> input, IEnumerable<char> done, (int x, int y) pos)
{
    if (input.Any())
    {
        foreach (var presses in GetDirPresses(input.First(), ref pos))
        {
            foreach (var combo in GetDirCombos(input.Skip(1), done.Concat(presses).Append('A'), pos))
            {
                yield return combo;
            }
        }
    }
    else
    {
        yield return done.ToList();
    }
}

List<List<char>> GetDirPresses(char button, ref (int x, int y) pos)
{
    var presses = new List<List<string>>();

    (int x, int y) next = button switch
    {
        'A' => (2, 0),
        '^' => (1, 0),
        '<' => (0, 1),
        'v' => (1, 1),
        '>' => (2, 1),
        _ => throw new InvalidOperationException()
    };
    var presses1 = new List<char>();
    if (next.x > pos.x) presses1.AddRange(Enumerable.Repeat('>', next.x - pos.x));
    if (next.x < pos.x) presses1.AddRange(Enumerable.Repeat('<', pos.x - next.x));
    if (next.y > pos.y) presses1.AddRange(Enumerable.Repeat('v', next.y - pos.y));
    if (next.y < pos.y) presses1.AddRange(Enumerable.Repeat('^', pos.y - next.y));

    var combos = GetCombos(presses1).ToList();

    if (button == '<' && pos.y == 0)
    {
        combos.RemoveAll(c => c.Last() == 'v');
    }
    else if (pos == (0, 1) && next.y == 0)
    {
        combos.RemoveAll(c => c.First() == '^');
    }

    pos = next;
    return combos;
}

List<List<char>> GetCombos(List<char> input)
{
    var comboIds = GetComboIds(Enumerable.Range(0, input.Count), []).ToList();
    var combos = comboIds.Select(c => c.Select(d => input[d]).ToList()).DistinctBy(c => new string([.. c])).ToList();
    return combos;
}

IEnumerable<List<int>> GetComboIds(IEnumerable<int> input, IEnumerable<int> done)
{
    bool noInput = true;

    foreach (var i in input)
    {
        noInput = false;
        foreach (var combo in GetComboIds(input.Except([i]), done.Append(i)))
        {
            yield return combo;
        }
    }

    if (noInput)
    {
        yield return done.ToList();
    }
}

string GetPresses(string code)
{
    string bestPresses = "";
    var best1 = int.MaxValue;
    var best2 = int.MaxValue;

    var numCombos = GetNumCombos(code, [], (2, 3)).ToList();
    foreach (var combo in numCombos)
    {
        var combos1 = GetDirCombos(combo, [], (2, 0)).ToList();
        var min1 = combos1.Min(c => c.Count);
        if (min1 <= best1)
        {
            best1 = min1;
            foreach (var combo1 in combos1.Where(c => c.Count == min1))
            {
                var combos2 = GetDirCombos(combo1, [], (2, 0)).ToList();
                var min2 = combos2.Min(c => c.Count);
                if (min2 <= best2)
                {
                    best2 = min2;
                    bestPresses = new([.. combos2.First(c => c.Count == min2)]);
                }
            }
        }
    }

    return bestPresses;
}

long total = 0;
foreach (var line in File.ReadLines(file))
{
    var presses = GetPresses(line);
    var num = int.Parse(line[..3]);
    Console.WriteLine($"{line}: {presses}");
    total += presses.Length * num;
}

Console.WriteLine(total);

/*
826A: <vA<AA>>^AvA<^A>AAAvA^A<vA<A>>^AA<A>vA^A<v<A>>^A<vA>A^A<A>A<vA<A>>^AA<A>vA^A
341A: <v<A>>^AvA^A<vA<AA>>^AAvA<^A>AvA^A<vA<A>>^A<A>vA^A<vA>^A<v<A>>^AvA^A<A>A
582A: <vA<AA>>^AvA<^A>AAvA^A<v<A>>^AvA^A<vA<A>>^AA<A>vA^A<vA<A>>^AvA^A<A>A
983A: <v<A>>^AAAvA^A<vA<AA>>^AvAA<^A>A<vA<A>>^AAvA^A<A>A<vA<A>>^A<A>vA^A
670A: <v<A>>^AAvA^A<vA<AA>>^AAvA<^A>AvA^A<vA>^A<v<A>>^AAA<A>vA^A<vA>^A<A>A
237342
*/