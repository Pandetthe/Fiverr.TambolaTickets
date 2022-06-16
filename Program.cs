namespace TambolaTickets
{
    public class Program
    {
        public static void Main()
        {
            for (int i = 0; i < 6; i++)
            {
                TambolaTicket ticket = new(i+1);
                ticket.Generate();
                ticket.Display();
            }
        }
    }
}