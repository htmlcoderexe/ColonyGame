using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IActor
    {
        void DoStuff(float dT);
        float GetSpeed();
        bool CanDo(ITask Task);
    }
}
