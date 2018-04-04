using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public enum TaskState
    {
        Enqueued,InProgress,Suspended,Paused,Complete
    }
    public interface ITask
    {
        Components.MapCreature Actor { get; set; }
        TaskState State { get; set; }
        void Progress(float dT);
        void Initialize();
        string SuspendReason { get; set; }
        string Description { get; set; }
        Action CompletedAction { get; set; }
        int Priority { get; set; }
        Components.WorkTypes WorkType { get; set; }
    }
}
