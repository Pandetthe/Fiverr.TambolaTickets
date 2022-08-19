namespace TambolaTickets
{
    /// <summary>
    /// Tambola ticket class
    /// </summary>
    public class TambolaTicket
    {
        #region Entries
        /// <summary>
        /// Private two dimension array with entries data. The first array is a row and the second is a column.
        /// </summary>
        private int[][] Entries;

        /// <summary>
        /// Function to update the ticket values.
        /// </summary>
        /// <param name="rowIndex">Row index in the Entries array.</param>
        /// <param name="columnIndex">Column index in the Entries array.</param>
        /// <param name="value">Value to be entered.</param>
        public void UpdateEntry(int rowIndex, int columnIndex, int value)
        {
            if (columnIndex < 0 || columnIndex >= _columnAmount)
                throw new Exception("Column index out of range.");
            if (rowIndex < 0 || rowIndex >= 3)
                throw new Exception("Row index out of range.");
            Entries[rowIndex][columnIndex] = value;
        }

        /// <summary>
        /// Function that returns ticket values.
        /// </summary>
        public int[][] GetEntries() => Entries;

        /// <summary>
        /// Function that returns ticket column values.
        /// </summary>
        /// <param name="columnIndex">Column index in the Entries array.</param>
        public int[] GetColumnValues(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= _columnAmount)
                throw new Exception("Column index out of range.");
            return Entries.Select(row => row[columnIndex]).ToArray();
        }

        /// <summary>
        /// Function that returns ticket row values.
        /// </summary>
        /// <param name="rowIndex">Column index in the Entries array.</param>
        /// <returns></returns>
        public int[] GetRowValues(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= 3)
                throw new Exception("Row index out of range.");
            return Entries[rowIndex];
        }
        #endregion Entries

        /// <summary>
        /// Function to check whether a row is complete.
        /// </summary>
        /// <param name="rowIndex">Column index in the Entries array.</param>
        /// <returns>If the row contains 5 numbers it returns true, if not it returns false.</returns>
        private bool IsRowCompleted(int rowIndex) => GetRowValues(rowIndex).Count(x => x != 0) == 5;

        /// <summary>
        /// Field containing the ticket number.
        /// </summary>
        private readonly string _numberOfTicket;

        /// <summary>
        /// Field containing the number of columns of the ticket.
        /// </summary>
        private readonly int _columnAmount;

        /// <summary>
        /// Ticket constructor assigning default values.
        /// </summary>
        /// <param name="numberOfTicket">Ticket number.</param>
        /// <param name="columnAmount">Amount of columns in ticket</param>
        public TambolaTicket(int numberOfTicket, int columnAmount)
        {
            _numberOfTicket = numberOfTicket.ToString();
            _columnAmount = columnAmount;
            Entries = new int[3][];
            for (int i = 0; i < 3; i++)
                Entries[i] = new int[_columnAmount];
        }

        /// <summary>
        /// Function to check whether it is possible to insert a number to a given column and row of a ticket.
        /// </summary>
        /// <param name="rowIndex">Row index in the Entries array.</param>
        /// <param name="colIndex">Column index in the Entries array.</param>
        /// <returns></returns>
        public bool CanPlaceNumber(int rowIndex, int colIndex)
        {
            int count = 0;
            for (int i = colIndex - 1; i >= 0; i--)
            {
                if (Entries[rowIndex][i] == 0)
                    break;
                count++;
            }
            for (int i = colIndex + 1; i < _columnAmount; i++)
            {
                if (Entries[rowIndex][i] == 0)
                    break;
                count++;
            }
            return count < 2;
        }

        /// <summary>
        /// Function for inserting numbers into a ticket.
        /// </summary>
        /// <param name="sortedNumbers">Variable with all numbers sorted into tickets and corresponding columns.</param>
        /// <returns>Returns true if the numbers have been successfully entered into the ticket, if not returns false.</returns>
        private bool Insert(Dictionary<int, List<int>> sortedNumbers)
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
                        if (repeat >= 100) return false;
                        row = random.Next(0, 3);
                        repeat++;
                    }
                    while (IsRowCompleted(row) || Entries[row][numbers.Key] != 0 || !CanPlaceNumber(row, numbers.Key));
                    UpdateEntry(row, numbers.Key, numbers.Value[index]);
                }
            }
            foreach (KeyValuePair<int, List<int>> numbers in sortedNumbers.Where(col => col.Value.Count == 1))
            {
                int row, repeat = 0;
                do
                {
                    if (repeat >= 100) return false;
                    row = random.Next(0, 3);
                    repeat++;
                }
                while (IsRowCompleted(row) || Entries[row][numbers.Key] != 0 || !CanPlaceNumber(row, numbers.Key));
                UpdateEntry(row, numbers.Key, numbers.Value.First());
            }
            return true;
        }

        /// <summary>
        /// Function for inserting numbers into a ticket.
        /// </summary>
        /// <param name="sortedNumbers">Variable with all numbers sorted into tickets and corresponding columns.</param>
        /// <returns>Returns true if the numbers have been successfully entered into the ticket, if not returns false.</returns>
        public bool InsertData(Dictionary<int, List<int>> sortedNumbers)
        {
            int repeat = 0;
            do
            {
                if (repeat >= 100) return false;
                Entries = new int[3][];
                for (int i = 0; i < 3; i++)
                    Entries[i] = new int[_columnAmount];
                repeat++;
            } while (!Insert(sortedNumbers));
            Sort();
            return true;
        }

        /// <summary>
        /// Function to sort the numbers in each column from smallest to largest.
        /// </summary>
        private void Sort()
        {
            for (var col = 0; col < _columnAmount; col++)
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

        /// <summary>
        /// A function that displays a ticket in the console.
        /// </summary>
        public void Display()
        {
            Console.WriteLine(new string('-', 46));
            Console.WriteLine($"| Tambola Ticket No: {_numberOfTicket}                       |");
            Console.WriteLine(new string('-', 46));
            for (int i = 0; i < 3; i++)
            {
                int[] row = GetRowValues(i);
                string result = "|";
                for (int j = 0; j < row.Length; j++)
                    result += row[j] > _columnAmount ? $" {row[j]} |" : row[j] == 0 ? "    |" : $"  {row[j]} |";
                Console.WriteLine(result);
                Console.WriteLine(new string('-', 46));
            }
        }
    }
}
