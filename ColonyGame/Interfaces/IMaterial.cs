using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonyGame.Interfaces
{
    public interface IMaterial
    {
        Color MaterialColor { get; set; }
        float PricePerUnit { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
