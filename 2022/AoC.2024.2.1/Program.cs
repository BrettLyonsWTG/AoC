var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long score = 0;

foreach (var line in File.ReadLines(file))
{
    score +=
        line[2] switch {
            'X' => 1,
            'Y' => 2,
            'Z' => 3,
            _ => 0
        } +
        (line[0], line[2]) switch
        {
            ('A', 'Z') => 0,
            ('B', 'X') => 0,
            ('C', 'Y') => 0,
            ('A', 'X') => 3,
            ('B', 'Y') => 3,
            ('C', 'Z') => 3,
            ('A', 'Y') => 6,
            ('B', 'Z') => 6,
            ('C', 'X') => 6,
            _ => 0
        };
}

Console.WriteLine(new { score });