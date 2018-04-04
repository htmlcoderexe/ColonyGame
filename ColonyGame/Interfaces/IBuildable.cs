using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IBuildable
    {
        int X { get; set; }
        int Y { get; set; }
        Components.Materials.PlainMaterial MainMaterial { get; set; }
        //Components.Tasks.Build GenerateCurrentTask();
    }
}
