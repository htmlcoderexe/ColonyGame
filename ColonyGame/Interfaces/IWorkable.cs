using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IWorkable
    {
        float WorkRequired { get; set; }
        float WorkComplete { get; set; }
        void WorkCompleted();
    }
}
