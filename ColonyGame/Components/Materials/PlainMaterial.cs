using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ColonyGame.Components.Materials
{
    public class PlainMaterial : Interfaces.IMaterial
    {
        public string Description { get; set; }

        public Color MaterialColor { get; set; }

        public string Name { get; set; }

        public float PricePerUnit { get; set; }
    }
}
