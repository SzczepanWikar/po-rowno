using System.Numerics;

namespace Core.Common.DataStructures
{
    public class IndexedMatrix<T>
        where T : struct, INumber<T>
    {
        protected Dictionary<Guid, Dictionary<Guid, T>> _matrix = new();
        private readonly T _default;

        public IndexedMatrix(T defaultValue)
        {
            _default = defaultValue;
        }

        public bool HasIndex(Guid index)
        {
            return _matrix.ContainsKey(index);
        }

        public void AddIndex(Guid index)
        {
            if (HasIndex(index))
            {
                return;
            }

            foreach (var row in _matrix)
            {
                row.Value.Add(index, _default);
            }

            var newRow = new Dictionary<Guid, T>();
            _matrix.Add(index, newRow);

            foreach (var key in _matrix.Keys)
            {
                newRow.Add(key, _default);
            }
        }

        public T? GetValue(Guid row, Guid column)
        {
            var matrixRow = _matrix.GetValueOrDefault(row);

            if (matrixRow == null || !matrixRow.ContainsKey(column))
            {
                return null;
            }

            return matrixRow.GetValueOrDefault(column);
        }

        public void SetValue(Guid row, Guid column, T value)
        {
            if (!HasIndex(row))
            {
                AddIndex(row);
            }

            if (!HasIndex(column))
            {
                AddIndex(column);
            }

            _matrix[row][column] = value;
        }

        public void Fill(T value)
        {
            foreach (var row in _matrix.Keys)
            {
                _matrix[row] = _matrix[row].ToDictionary(e => e.Key, _ => value);
            }
        }

        public void Zero()
        {
            Fill(T.Zero);
        }
    }
}
