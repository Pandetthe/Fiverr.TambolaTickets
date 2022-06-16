using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambolaTickets
{
    public class TambolaTicket
    {
        private int[][] Entries { get; set; }

        private int NumberOfEntries { get => Entries.Sum(row => row.Count(x => x != 0)); }

        private bool IsCompleted { get => NumberOfEntries == 15; }

        private int Number { get; set; }

        public TambolaTicket(int number)
        {
            Number = number;
            Entries = new int[3][];
            for (int i = 0; i < 3; i++)
                Entries[i] = new int[9];
        }

        private int[] GetColumnValues(int column)
        {
            if (column < 0 || column >= 9) throw new Exception("Column index out of range");
            return Entries.Select(row => row[column]).ToArray();
        }

        private int[] GetRowValues(int row)
        {
            if (row < 0 || row >= 3) throw new Exception("Row index out of range");
            return Entries[row];
        }

        private bool IsRowCompleted(int row) => GetRowValues(row).Count(x => x != 0) == 5;

        private bool IsColumnCompleted(int column) => GetColumnValues(column).Count(x => x != 0) == 3;

        private void UpdateEntry(int row, int column, int value)
        {
            if (column < 0 || column >= 9) throw new Exception("Column index out of range");
            if (row < 0 || row >= 3) throw new Exception("Row index out of range");
            Entries[row][column] = value;
        }

        private void Sort()
        {
            for (var col = 0; col < 9; col++)
            {
                List<int> data = new();
                for (int row = 0; row < 3; row++)
                    data.Add(Entries[row][col]);
                data = data.Where(x => x != 0).OrderBy(x => x).ToList();
                for (int row = 0; row < 3 && data.Count != 0; row++)
                {
                    if (Entries[row][col] == 0) continue;
                    int num = data.First();
                    data.Remove(num);
                    UpdateEntry(row, col, num);
                }
            }
        }

        public void Generate()
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            Dictionary<int, List<int>> numbers = new();
            for (int i = 0; i < 9; i++)
            {
                numbers.Add(i, new List<int>());
                for (int j = 0; j < 10; j++)
                    numbers[i].Add(i * 10 + j + 1);
            }
            for (int col = 0; col < 9; col++)
            {
                int row;
                do row = random.Next(0, 3);
                while (!IsRowCompleted(row) && Entries[row][col] != 0);
                int index = random.Next(0, numbers[col].Count);
                UpdateEntry(row, col, numbers[col][index]);
                numbers[col].RemoveAt(index);
            }
            void FillRecursively()
            {
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (random.NextDouble() <= 0.5 || IsCompleted || IsRowCompleted(row)
                            || IsColumnCompleted(col) || Entries[row][col] != 0) continue;
                        if (numbers[col].Count == 0) continue;
                        int index = random.Next(0, numbers[col].Count);
                        UpdateEntry(row, col, numbers[col][index]);
                        numbers[col].RemoveAt(index);
                    }
                }
                if (IsCompleted)
                {
                    Sort();
                    return;
                }
                FillRecursively();
            }
            FillRecursively();
        }

        public void Display()
        {
            Console.WriteLine(new string('-', 46));
            Console.WriteLine($"| Tambola Ticket No: {Number}                       |");
            Console.WriteLine(new string('-', 46));
            for (int i = 0; i < 3; i++)
            {
                int[] row = GetRowValues(i);
                string result = "|";
                for (int j = 0; j < row.Length; j++)
                    result += row[j] > 9 ? $" {row[j]} |" : row[j] == 0 ? "    |" : $"  {row[j]} |";
                Console.WriteLine(result);
                Console.WriteLine(new string('-', 46));
            }
        }
    }
}
