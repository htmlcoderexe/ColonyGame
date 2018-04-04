using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IZone
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }
    }
}
