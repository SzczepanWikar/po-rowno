using System.Linq;
using System.Numerics;

namespace Core.Common.DataStructures
{
    public class Graph<T> : IndexedMatrix<T>
        where T : struct, INumber<T>
    {
        public Graph()
            : base(T.Zero) { }

        public bool HasCycle()
        {
            var visited = new HashSet<Guid>();
            var recursionStack = new HashSet<Guid>();

            foreach (var node in _matrix.Keys)
            {
                if (!visited.Contains(node))
                {
                    if (DFS(node, visited, recursionStack))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void MinimizeEdges()
        {
            var balances = _matrix.ToDictionary(e => e.Key, _ => T.Zero);

            foreach (var row in _matrix.Keys)
            {
                var dept = _matrix[row].Values.Aggregate(T.Zero, (acc, curr) => acc + curr);
                var credit = _matrix.Values.Aggregate(T.Zero, (acc, curr) => acc + curr[row]);
                balances[row] = credit - dept;
            }

            Zero();

            MinimizeEdges(balances);
        }

        public IEnumerable<(Guid Row, Guid Column, T Weight)> GetEdges()
        {
            List<(Guid, Guid, T)> res = new();

            foreach (var row in _matrix)
            {
                foreach (var cell in row.Value)
                {
                    if (cell.Value != T.Zero)
                    {
                        res.Add(new(row.Key, cell.Key, cell.Value));
                    }
                }
            }

            return res;
        }

        private void MinimizeEdges(Dictionary<Guid, T> balances)
        {
            var (maxDept, maxCredit) = GetMinAndMax(balances);

            if (balances[maxDept] == T.Zero && balances[maxCredit] == T.Zero)
            {
                return;
            }

            var minValue =
                T.Abs(balances[maxDept]) > balances[maxCredit]
                    ? balances[maxCredit]
                    : T.Abs(balances[maxDept]);

            balances[maxCredit] -= minValue;
            balances[maxDept] += minValue;

            _matrix[maxDept][maxCredit] = minValue;

            MinimizeEdges(balances);
        }

        private static (Guid MinKey, Guid MaxKey) GetMinAndMax(Dictionary<Guid, T> balances)
        {
            Guid max = Guid.Empty,
                min = Guid.Empty;

            foreach (var row in balances)
            {
                if (max == Guid.Empty)
                {
                    max = row.Key;
                }

                if (min == Guid.Empty)
                {
                    min = row.Key;
                }

                if (max == row.Key)
                {
                    continue;
                }

                if (balances[max] < row.Value)
                {
                    max = row.Key;
                    continue;
                }

                if (balances[min] > row.Value)
                {
                    min = row.Key;
                    continue;
                }
            }

            return (min, max);
        }

        private bool DFS(Guid node, HashSet<Guid> visited, HashSet<Guid> recursionStack)
        {
            visited.Add(node);
            recursionStack.Add(node);

            if (_matrix.ContainsKey(node))
            {
                foreach (var neighbor in _matrix[node].Keys)
                {
                    T edgeValue = _matrix[node][neighbor];
                    if (!edgeValue.Equals(T.Zero))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            if (DFS(neighbor, visited, recursionStack))
                            {
                                return true;
                            }
                        }
                        else if (recursionStack.Contains(neighbor))
                        {
                            return true;
                        }
                    }
                }
            }

            recursionStack.Remove(node);
            return false;
        }
    }
}
