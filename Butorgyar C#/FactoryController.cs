using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butorgyar
{
    public class FactoryController
    {
        private FurnitureFactory furnitureFactory;

        public FactoryController(FurnitureFactory factory)
        {
            furnitureFactory = factory;
        }

        public void OperateFactory()
        {
            //5 darab artdeco nappali bútorkollekció létrehozása
            furnitureFactory.SetLivingRoomFactory(new ArtDecoLivingRoomFactory());
            furnitureFactory.CreateLivingRoomFurniture(5, new ArtDecoLivingRoomFactory());


            //3 darab classic előszoba bútorkollekció létrehozása
            furnitureFactory.SetHallwayFactory(new ClassicHallwayFactory());
            furnitureFactory.CreateHallwayFurniture(3, new ClassicHallwayFactory());

            //2 darab modern előszoba bútorkollekció létrehozása (saját példa)
            furnitureFactory.SetHallwayFactory(new ModernHallwayFactory());
            furnitureFactory.CreateHallwayFurniture(2, new ModernHallwayFactory());
            
        }
    }
}
