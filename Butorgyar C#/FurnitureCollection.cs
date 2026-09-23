using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butorgyar
{
    public class FurnitureCollection
    {
        public List<Table> Tables { get; set; } = new List<Table>();
        public List<Cabinet> Cabinets { get; set; } = new List<Cabinet>();
        public List<Chair> Chairs { get; set; } = new List<Chair>();
        public List<CoatRack> CoatRacks { get; set; } = new List<CoatRack>();
    }
}
