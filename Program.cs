namespace TambolaTickets
{
    public class Program
    {
        public static void Main()
        {
            for (int i = 0; i < 100; i++)
            {
                TambolaGenerator generator = new();
                List<TambolaTicket> tickets = generator.GenerateTickets();
                foreach (TambolaTicket ticket in tickets)
                    ticket.Display();
                Console.WriteLine("-------------------------");
            }
        }
    }
}