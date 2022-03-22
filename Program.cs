using System;
using System.Linq;

namespace OrdersExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please, enter an order # (0 to exit): ");

                int orderNumber;

                if (!int.TryParse(Console.ReadLine(), out orderNumber))
                {
                    Console.WriteLine("Order # is invalid.");
                    continue;
                }

                if (orderNumber == 0) break;

                PrintOrder(orderNumber);
            }

            Console.WriteLine("Goodbye!");
        }

        private static void PrintOrder(int orderNumber)
        {
            using (var session = DocumentStoreHolder.Store.OpenSession())
            {
                var order = session
                    .Include<Order>(o => o.Company)
                    .Include(o => o.Employee)
                    .Include(o => o.Lines.Select(l => l.Product))
                    .Load(orderNumber);

                if (order == null)
                {
                    Console.WriteLine($"Order ${orderNumber} not found.");

                    return;
                }

                Console.WriteLine($"Order #{orderNumber}");

                var c = session.Load<Company>(order.Company);
                Console.WriteLine($"Company : {c.Id} - {c.Name}");

                var e = session.Load<Employee>(order.Employee);
                Console.WriteLine($"Employee : {e.Id} - {e.LastName}, {e.FirstName}");

                foreach (var orderLine in order.Lines)
                {
                    var p = session.Load<Product>(orderLine.Product);
                    Console.WriteLine($" - {orderLine.ProductName}, " +
                        $" {orderLine.Quantity} x {p.QuantityPerUnit}"); 
                }
            }
        }
    }
}
