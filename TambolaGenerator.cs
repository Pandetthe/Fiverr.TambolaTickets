
namespace TambolaTickets
{
    /// <summary>
    /// Generator for tambola tickets. With one generation it returns 6 tickets of size 3x9.
    /// </summary>
    public class TambolaGenerator
    {
        /// <summary>
        /// A field containing a number indicating the number of tickets generated from a single pool of numbers.
        /// </summary>
        private const int TICKET_AMOUNT = 6;

        /// <summary>
        /// A field containing the number of columns of the ticket.
        /// </summary>
        private const int COLUMN_AMOUNT = 9;

        /// <summary>
        /// Property with all numbers sorted into tickets and corresponding columns.
        /// </summary>
        private Dictionary<int, Dictionary<int, List<int>>> SortedNumbers { get; set; } = null!;

        /// <summary>
        /// Variable with all numbers before sorting to appropriate tickets.
        /// </summary>
        private Dictionary<int, List<int>> AvailableNumbers { get; set; } = null!;

        /// <summary>
        /// A function for checking that a ticket contains the correct number of numbers to be filled in.
        /// </summary>
        /// <param name="ticketIndex">Ticket index in the SortedNumbers dictionary.</param>
        /// <returns>If the ticket contains 15 numbers it returns true, if not it returns false.</returns>
        private bool IsTicketDataFilled(int ticketIndex)
        {
            if (ticketIndex < 0 || ticketIndex > TICKET_AMOUNT - 1)
                throw new Exception("Ticket index out of range!");
            int amount = GetAmountOfEntries(ticketIndex);
            return GetAmountOfEntries(ticketIndex) == 15;
        }

        /// <summary>
        /// Function to check the number of entries allocated to a ticket at a given time.
        /// </summary>
        /// <param name="ticketIndex">Ticket index in the SortedNumbers dictionary.</param>
        /// <returns>If the ticket contains 15 numbers it returns true, if not it returns false.</returns>
        private int GetAmountOfEntries(int ticketIndex)
        {
            return ticketIndex < 0 || ticketIndex > TICKET_AMOUNT - 1
                ? throw new Exception("Ticket index out of range!")
                : SortedNumbers[ticketIndex].Sum(col => col.Value.Count());
        }

        /// <summary>
        /// Function to check whether it is theoretically possible to allocate a number to a given column of a ticket.
        /// </summary>
        /// <param name="ticketIndex">Ticket index in the SortedNumbers dictionary.</param>
        /// <param name="columnIndex">Column index in the SortedNumbers[tickedIndex] list.</param>
        /// <returns>Returns true if theoretically possible, if not returns false.</returns>
        private bool CanAddNumberToColumn(int ticketIndex, int columnIndex)
        {
            if (ticketIndex < 0 || ticketIndex > TICKET_AMOUNT - 1)
                throw new Exception("Ticket index out of range!");
            if (columnIndex < 0 || columnIndex > COLUMN_AMOUNT - 1)
                throw new Exception("Column index out of range!");
            Dictionary<int, List<int>> ticket = SortedNumbers[ticketIndex];
            int count = 0;
            switch (ticket[columnIndex].Count)
            {
                case 3:
                    return false;
                case 2:
                    if (columnIndex == 0)
                        return false;
                    if (ticket.Any(x => x.Value.Count == 3))
                        return false;
                    if (columnIndex > 0 && ticket[columnIndex - 1].Count == 3)
                        return false;
                    if (columnIndex < COLUMN_AMOUNT - 1 && ticket[columnIndex + 1].Count == 3)
                        return false;
                    for (int i = columnIndex - 1; i >= 0; i--)
                    {
                        if (ticket[i].Count < 2)
                            break;
                        count++;
                    }
                    for (int i = columnIndex + 1; i < COLUMN_AMOUNT; i++)
                    {
                        if (ticket[i].Count < 2)
                            break;
                        count++;
                    }
                    if (count >= 2)
                        return false;
                    return true;
                case 1:
                    for (int i = columnIndex - 1; i >= 0; i--)
                    {
                        if (ticket[i].Count < 2)
                            break;
                        count++;
                    }
                    for (int i = columnIndex + 1; i < COLUMN_AMOUNT; i++)
                    {
                        if (ticket[i].Count < 2)
                            break;
                        count++;
                    }
                    if (count >= 2)
                        return false;
                    return true;
                case 0:
                    if (columnIndex > 0 && columnIndex < COLUMN_AMOUNT - 1 && ticket[columnIndex - 1].Count == 3
                        && ticket[columnIndex - 1].Count == 3)
                        return false;
                    return true;
            }
            throw new Exception("Amount of values in column is out of range.");
        }

        /// <summary>
        /// Function to distribute one number to each column of each ticket.
        /// </summary>
        /// <param name="random">Pseudo-random generator</param>
        private void GenerateFirst(Random random)
        {
            for (int ticket = 0; ticket < TICKET_AMOUNT; ticket++)
            {
                for (int column = 0; column < COLUMN_AMOUNT; column++)
                {
                    int number = AvailableNumbers[column][random.Next(0, AvailableNumbers[column].Count)];
                    AvailableNumbers[column].Remove(number);
                    SortedNumbers[ticket][column].Add(number);
                }
            }
        }

        /// <summary>
        /// Function to distribute the remaining numbers to the tickets.
        /// </summary>
        /// <param name="random">Pseudo-random generator</param>
        private bool GenerateSecond(Random random)
        {
            for (int pass = 0; pass < 4; pass++)
            {
                for (int column = 0; column < COLUMN_AMOUNT; column++)
                {
                    for (int i = 0; i < (column == COLUMN_AMOUNT - 1 ? 2 : 1); i++)
                    {
                        if (AvailableNumbers[column].Count == 0) continue;
                        int number = AvailableNumbers[column][random.Next(0, AvailableNumbers[column].Count)];
                        AvailableNumbers[column].Remove(number);
                        int ticketIndex;
                        int repeat = 0;
                        do
                        {
                            if (repeat >= 100) return false;
                            ticketIndex = random.Next(0, TICKET_AMOUNT);
                            repeat++;
                        }
                        while (!CanAddNumberToColumn(ticketIndex, column) || IsTicketDataFilled(ticketIndex));
                        SortedNumbers[ticketIndex][column].Add(number);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Function to generate tickets.
        /// </summary>
        /// <returns>Returns the list of tickets.</returns>
        public List<TambolaTicket> GenerateTickets()
        {
            List<TambolaTicket> list;
            bool generated;
            do
            {
                list = new();
                AvailableNumbers = new();
                SortedNumbers = new();
                for (int i = 0; i < 9; i++)
                {
                    AvailableNumbers.Add(i, new());
                    for (int j = 0; j < 10; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        AvailableNumbers[i].Add((i * 10) + j);
                    }
                    if (i == 8) AvailableNumbers[i].Add(90);
                }
                Random random = new Random(Guid.NewGuid().GetHashCode());
                for (int i = 0; i < 6; i++)
                {
                    SortedNumbers.Add(i, new());
                    for (int j = 0; j < 9; j++)
                        SortedNumbers[i].Add(j, new());
                }
                GenerateFirst(random);
                generated = GenerateSecond(random);
                for (int i = 0; i < TICKET_AMOUNT && generated; i++)
                {
                    TambolaTicket ticket = new(i + 1, COLUMN_AMOUNT);
                    generated = ticket.InsertData(SortedNumbers[i]);
                    list.Add(ticket);
                }
            } while (!generated);
            return list;
        }
    }
}
