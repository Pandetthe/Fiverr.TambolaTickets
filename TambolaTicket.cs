using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambolaTickets
{
    public class TambolaTicket
    {
        #region Entries
        private int[][] Entries;

        public void UpdateEntry(int row, int column, int value)
        {
            if (column < 0 || column >= 9) throw new Exception("Column index out of range");
            if (row < 0 || row >= 3) throw new Exception("Row index out of range");
            Entries[row][column] = value;
        }

        public int[][] GetEntries() => Entries;

        public int[] GetColumnValues(int column)
        {
            if (column < 0 || column >= 9) throw new Exception("Column index out of range");
            return Entries.Select(row => row[column]).ToArray();
        }

        public int[] GetRowValues(int row)
        {
            if (row < 0 || row >= 3) throw new Exception("Row index out of range");
            return Entries[row];
        }
        #endregion Entries

        private bool IsRowCompleted(int row) => GetRowValues(row).Count(x => x != 0) == 5;

        private readonly string NumberOfTicket;

        public TambolaTicket(int numberOfTicket)
        {
            NumberOfTicket = numberOfTicket.ToString();
            Entries = new int[3][];
            for (int i = 0; i < 3; i++)
                Entries[i] = new int[9];
        }

        public bool InsertData(Dictionary<int, List<int>> sortedNumbers)
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            foreach (KeyValuePair<int, List<int>> numbers in sortedNumbers.Where(col => col.Value.Count == 3))
                for (int row = 0; row < 3; row++)
                    UpdateEntry(row, numbers.Key, numbers.Value[row]);
            foreach (KeyValuePair<int, List<int>> numbers in sortedNumbers.Where(col => col.Value.Count == 2))
            {
                for (int index = 0; index < 2; index++)
                {
                    int row, repeat = 0;
                    do
                    {
                        if (repeat >= 1000) return false;
                        row = random.Next(0, 3);
                        repeat++;
                    }
                    while (IsRowCompleted(row) || Entries[row][numbers.Key] != 0);
                    UpdateEntry(row, numbers.Key, numbers.Value[index]);
                }
            }
            foreach (KeyValuePair<int, List<int>> numbers in sortedNumbers.Where(col => col.Value.Count == 1))
            {
                int row, repeat = 0;
                do
                {
                    if (repeat >= 1000) return false;
                    row = random.Next(0, 3);
                    repeat++;
                }
                while (IsRowCompleted(row) || Entries[row][numbers.Key] != 0);
                UpdateEntry(row, numbers.Key, numbers.Value.First());
            }
            Sort();
            return true;
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

        public void Display()
        {
            Console.WriteLine(new string('-', 46));
            Console.WriteLine($"| Tambola Ticket No: {NumberOfTicket}                       |");
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
