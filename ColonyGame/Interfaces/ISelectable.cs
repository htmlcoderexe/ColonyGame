using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface ISelectable
    {
        string Name { get; set; }
        string Description { get; set; }
        Rectangle Bound { get;  }
    }
}
