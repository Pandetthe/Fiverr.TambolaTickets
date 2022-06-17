namespace TambolaTickets
{
    public class Program
    {
        public static void Main()
        {
            TambolaGenerator generator = new();
            List<TambolaTicket> tickets = generator.GenerateTickets();
            foreach (TambolaTicket ticket in tickets)
                ticket.Display();
            
        }
    }
}