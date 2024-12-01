var file = Debugger.IsAttached ? "example.txt" : "input.txt";

string result = "";
string[] lines = File.ReadAllLines(file);
int middle = Array.IndexOf(lines, "");
var cols = lines[middle - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToDictionary(c => int.Parse(c), _ => new Stack<char>());

for (int l = middle - 2; l >= 0; l--)
{
    for (int c = 1, x = 1; c <= cols.Count; c++, x += 4)
    {
        if (lines[l][x] is char ch and not ' ')
            cols[c].Push(ch);
    }
}

for (int l = middle + 1; l < lines.Length; l++)
{
    var cmd = lines[l].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var moves = int.Parse(cmd[1]);
    var from = int.Parse(cmd[3]);
    var to = int.Parse(cmd[5]);
    for (int i = 0; i < moves; i++)
        cols[to].Push(cols[from].Pop());
}

result = new string(cols.Values.Select(c => c.Peek()).ToArray());

Console.WriteLine(new { result });