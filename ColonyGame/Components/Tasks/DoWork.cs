using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyGame.Interfaces;

namespace ColonyGame.Components.Tasks
{
    public class DoWork : Interfaces.ITask
    {
        public MapCreature Actor { get; set; }

        public Action CompletedAction { get; set; }

        public string Description { get; set; }

        public TaskState State { get; set; }
        public int Priority { get; set; }
        public WorkTypes WorkType { get; set; }

        public string SuspendReason { get; set; }

        public float WorkLeft { get; set; }

        public Interfaces.IWorkable Target { get; set; }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void Progress(float dT)
        {
            float WorkMultiplier = 10;
            this.WorkLeft -= dT * WorkMultiplier;
            this.Target.WorkComplete += dT * WorkMultiplier;
            if (this.WorkLeft <= 0)
                this.State = TaskState.Complete;
        }
    }
}
