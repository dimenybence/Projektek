using System.Runtime.CompilerServices;

namespace Butorgyar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FurnitureFactory furnitureFactory = new FurnitureFactory();
            FactoryController factoryController = new FactoryController(furnitureFactory);

            // Gyártósor üzemeltetése
            factoryController.OperateFactory();

            // Naplózások kiíratása
            Console.WriteLine("Log Entries:\n");
            foreach (var entry in Logger.Instance.GetLogEntries())
            {
                Console.WriteLine(entry);
            }
        }
    }
}
