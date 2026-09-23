using Butorgyar.ArtDecoElements;
using Butorgyar.ClassicElements;
using Butorgyar.ModernElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butorgyar
{
    public interface ILivingRoomFactory
    {
        Table CreateTable();
        Cabinet CreateCabinet();
        Chair CreateChair();
        public string GetStyle();
    }

    public class ArtDecoLivingRoomFactory : ILivingRoomFactory
    {
        public Table CreateTable()
        {
            return new ArtDecoTable();
        }

        public Cabinet CreateCabinet()
        {
            return new ArtDecoCabinet();
        }

        public Chair CreateChair()
        {
            return new ArtDecoChair();
        }

        public string GetStyle()
        {
            return "ArtDeco";
        }
    }

    public class ClassicLivingRoomFactory : ILivingRoomFactory
    {
        public Table CreateTable()
        {
            return new ClassicTable();
        }

        public Cabinet CreateCabinet()
        {
            return new ClassicCabinet();
        }

        public Chair CreateChair()
        {
            return new ClassicChair();
        }

        public string GetStyle()
        {
            return "Classic";
        }
    }

    public interface IHallwayFactory
    {
        CoatRack CreateTable();
        Cabinet CreateCabinet();
        public string GetStyle();
    }

    public class ArtDecoHallwayFactory : IHallwayFactory
    {
        public CoatRack CreateTable()
        {
            return new ArtDecoCoatRack();
        }

        public Cabinet CreateCabinet()
        {
            return new ArtDecoCabinet();
        }

        public string GetStyle()
        {
            return "ArtDeco";
        }
    }

    public class ClassicHallwayFactory : IHallwayFactory
    {
        public CoatRack CreateTable()
        {
            return new ClassicCoatRack();
        }

        public Cabinet CreateCabinet()
        {
            return new ClassicCabinet();
        }
        public string GetStyle()
        {
            return "Classic";
        }
    }

    public class ModernLivingRoomFactory : ILivingRoomFactory
    {
        public Table CreateTable()
        {
            return new ModernTable();
        }

        public Cabinet CreateCabinet()
        {
            return new ModernCabinet();
        }

        public Chair CreateChair()
        {
            return new ModernChair();
        }

        public string GetStyle()
        {
            return "Modern";
        }
    }

    public class ModernHallwayFactory : IHallwayFactory
    {
        public CoatRack CreateTable()
        {
            return new ModernCoatRack();
        }

        public Cabinet CreateCabinet()
        {
            return new ModernCabinet();
        }

        public string GetStyle()
        {
            return "Modern";
        }
    }
}
