using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butorgyar
{
    public class FurnitureFactory
    {
        private ILivingRoomFactory livingRoomFactory;
        private IHallwayFactory hallwayFactory;
        private List<FurnitureCollection> producedCollections = new List<FurnitureCollection>();

        public void SetLivingRoomFactory(ILivingRoomFactory factory)
        {
            livingRoomFactory = factory;
        }

        public void SetHallwayFactory(IHallwayFactory factory)
        {
            hallwayFactory = factory;
        }

        public FurnitureCollection CreateLivingRoomFurniture(int quantity, ILivingRoomFactory style)
        {
            FurnitureCollection collection = new FurnitureCollection();
            string styleName = style.GetStyle();

            for (int i = 1; i <= quantity; i++)
            {
                collection.Tables.Add(livingRoomFactory.CreateTable());
                Log($"Table created in style {styleName}");
                collection.Cabinets.Add(livingRoomFactory.CreateCabinet());
                Log($"Cabinet created in style {styleName}");
                collection.Chairs.Add(livingRoomFactory.CreateChair());
                Log($"Chair created in style {styleName}");
                Log($"Living room furniture {i}. created in {styleName} style");
                Produce(collection);
            }
            
            return collection;
        }

        public FurnitureCollection CreateHallwayFurniture(int quantity, IHallwayFactory style)
        {
            FurnitureCollection collection = new FurnitureCollection();

            string styleName = style.GetStyle();

            for (int i = 1; i <= quantity; i++)
            {
                collection.CoatRacks.Add(hallwayFactory.CreateTable());
                Log($"Coat rack created in style {styleName}");
                collection.Cabinets.Add(hallwayFactory.CreateCabinet());
                Log($"Cabinet created in style {styleName}");
                Log($"Hallway furniture {i}. created in {styleName} style");
                Produce(collection);
            }
            
            return collection;
        }

        private void Produce(FurnitureCollection collection)
        {
            collection = Package(collection);
            producedCollections.Add(collection);
        }

        private FurnitureCollection Package(FurnitureCollection inputCollection)
        {
            FurnitureCollection packagedCollection = new FurnitureCollection();
            Log("Packaging completed");

            return packagedCollection;
        }

        private void Log(string message)
        {
            // A naplózórendszer használata
            Logger.Instance.Log(message);
        }
    }
}
