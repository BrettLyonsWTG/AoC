var file = Debugger.IsAttached ? "example.txt" : "input.txt";

long score = 0;

foreach (var line in File.ReadLines(file))
{
    var play = (line[0], line[2]) switch
    {
        ('A', 'X') => 'Z',
        ('A', 'Y') => 'X',
        ('A', 'Z') => 'Y',
        ('B', 'X') => 'X',
        ('B', 'Y') => 'Y',
        ('B', 'Z') => 'Z',
        ('C', 'X') => 'Y',
        ('C', 'Y') => 'Z',
        ('C', 'Z') => 'X',
        _ => '\0'
    };
    score +=
        play switch {
            'X' => 1,
            'Y' => 2,
            'Z' => 3,
            _ => 0
        } +
        line[2] switch
        {
            'X' => 0,
            'Y' => 3,
            'Z' => 6,
            _ => 0
        };
}

Console.WriteLine(new { score });