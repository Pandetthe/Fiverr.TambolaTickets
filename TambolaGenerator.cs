
namespace TambolaTickets
{
    public class TambolaGenerator
    {
        private Dictionary<int, List<int>> AvailableNumbers { get; set; }

        private Dictionary<int, Dictionary<int, List<int>>> SortedNumbers { get; set; }

        public bool IsCompleted(int ticketIndex) => NumberOfEntries(ticketIndex) == 15;

        public int NumberOfEntries(int ticketIndex)
        {
            if (ticketIndex < 0 || ticketIndex > 6) throw new Exception("Invalid ticket index");
            return SortedNumbers[ticketIndex].Sum(row => row.Value.Count);
        }

        public TambolaGenerator()
        {
            AvailableNumbers = new();
            SortedNumbers = new();
            for (int i = 0; i < 9; i++)
            {
                AvailableNumbers.Add(i, new List<int>());
                for (int j = 0; j < 10; j++)
                    AvailableNumbers[i].Add(i * 10 + j + 1);
            }
        }

        private void GenerateFirst(Random random)
        {
            for (int ticket = 0; ticket < 6; ticket++)
            {
                for (int column = 0; column < 9; column++)
                {
                    int number = AvailableNumbers[column][random.Next(0, AvailableNumbers[column].Count)];
                    AvailableNumbers[column].Remove(number);
                    SortedNumbers[ticket][column].Add(number);
                }
            }
        }
        private void GenerateSecond(Random random)
        {
            for (int pass = 0; pass < 4; pass++)
            {
                for (int column = 0; column < 9; column++)
                {
                    int number = AvailableNumbers[column][random.Next(0, AvailableNumbers[column].Count)];
                    AvailableNumbers[column].Remove(number);
                    int ticketIndex;
                    do ticketIndex = random.Next(0, 6);
                    while (SortedNumbers[ticketIndex].Sum(row => row.Value.Count) == 15 || SortedNumbers[ticketIndex][column].Count == 3);
                    SortedNumbers[ticketIndex][column].Add(number);
                }
            }
        }

        public List<TambolaTicket> GenerateTickets()
        {
            Random random = new(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 6; i++)
            {
                SortedNumbers.Add(i, new());
                for (int j = 0; j < 9; j++)
                    SortedNumbers[i].Add(j, new());
            }
            GenerateFirst(random);
            GenerateSecond(random);
            List<TambolaTicket> list = new();
            for (int i = 0; i < 6; i++)
                list.Add(new TambolaTicket(i + 1, SortedNumbers[i]));
            return list;
        }
    }
}
