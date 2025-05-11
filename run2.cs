namespace Tochka
{
    class Program
    {
        private static readonly int[][] Directions =
        [
            [0, 1],
            [0, -1],
            [1, 0],
            [-1, 0]
        ];

        class State((int x, int y)[] positions, int keys)
        {
            public readonly (int x, int y)[] Positions = positions.ToArray();
            public readonly int CollectedKeys = keys;

            public override bool Equals(object? obj)
            {
                if (obj is not State other) return false;
                for (int i = 0; i < Positions.Length; i++)
                    if (!Positions[i].Equals(other.Positions[i]))
                        return false;
                return CollectedKeys == other.CollectedKeys;
            }

            public override int GetHashCode()
            {
                int hash = CollectedKeys;
                foreach (var pos in Positions)
                {
                    hash ^= pos.x * 73856093 ^ pos.y * 19349663;
                }
                return hash;
            }
        }

        static List<List<char>> GetInput()
        {
            var data = new List<List<char>>();
            while (Console.ReadLine() is { } line && line != "")
            {
                data.Add(line.ToCharArray().ToList());
            }
            return data;
        }

        static int Solve(List<List<char>> grid)
        {
            var h = grid.Count;
            var w = grid[0].Count;
            var starts = new List<(int x, int y)>();
            var totalKeys = 0;

            var foundKeys = 0;
            const int maxKeys = 26;
            for (var i = 0; i < h; i++)
            {
                for (var j = 0; j < w; j++)
                {
                    var cell = grid[i][j];
                    if (cell == '@')
                    {
                        starts.Add((i, j));
                    }
                    else if (char.IsLower(cell))
                    {
                        var keyBit = 1 << (cell - 'a');
                        if ((totalKeys & keyBit) != 0) continue;
                        totalKeys |= keyBit;
                        foundKeys++;
                        if (foundKeys == maxKeys)
                            goto Done;
                    }
                }
            }
            Done:;
            
            var visited = new HashSet<State>();
            var queue = new Queue<(State state, int steps)>();

            var startState = new State(starts.ToArray(), 0);
            queue.Enqueue((startState, 0));
            visited.Add(startState);

            while (queue.Count > 0)
            {
                var (current, steps) = queue.Dequeue();

                if (current.CollectedKeys == totalKeys)
                    return steps;

                for (int i = 0; i < 4; i++) 
                {
                    var (x, y) = current.Positions[i];

                    foreach (var dir in Directions)
                    {
                        int nx = x + dir[0];
                        int ny = y + dir[1];

                        if (nx < 0 || ny < 0 || nx >= h || ny >= w)
                            continue;

                        char cell = grid[nx][ny];
                        if (cell == '#') continue;
                        
                        if (char.IsUpper(cell) && ((current.CollectedKeys >> (cell - 'A')) & 1) == 0)
                            continue;

                        int newKeys = current.CollectedKeys;
                        if (char.IsLower(cell))
                            newKeys |= 1 << (cell - 'a');

                        var newPositions = (current.Positions.ToArray());
                        newPositions[i] = (nx, ny);
                        var newState = new State(newPositions, newKeys);

                        if (visited.Add(newState))
                        {
                            queue.Enqueue((newState, steps + 1));
                        }
                    }
                }
            }

            return -1;
        }

        static void Main()
        {
            var data = GetInput();
            int result = Solve(data);

            if (result == -1)
            {
                Console.WriteLine("No solution found");
            }
            else
            {
                Console.WriteLine(result);
            }
        }
    }
}
