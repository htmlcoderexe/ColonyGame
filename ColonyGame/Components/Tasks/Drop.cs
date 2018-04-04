using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
    public class Drop :Interfaces.ITask
    {
        public MapCreature Actor { get; set; }
        public Action CompletedAction { get; set; }
        public Interfaces.TaskState State  { get; set; }
        public int Priority { get; set; }
        public WorkTypes WorkType { get; set; }

        public string SuspendReason { get; set; }
        public string Description { get; set; }
        public bool Destroy = false;
        public void Initialize()
        {

        }

        public void Progress(float dT)
        {
            if(Actor.Carrying==null)
            {
                this.State = Interfaces.TaskState.Suspended;
                Console.Write("Unable to drop item: not carrying anything.");
                return;
            }
            if(!Destroy)
            { 
                Actor.Carrying.X = (int)(Math.Round(Actor.Position.X));
                Actor.Carrying.Y = (int)(Math.Round(Actor.Position.Y));
                Actor.ParentMap.Objects.Add(Actor.Carrying);
                Actor.Carrying.Reserved = false;
                foreach (Components.Stockpile p in Actor.ParentMap.Stockpiles)
                {
                    if(p.HasItem(Actor.Carrying))
                    p.TakeItem(Actor.Carrying);

                }
            }
        Actor.Carrying = null;
            this.State = Interfaces.TaskState.Complete;
        }
    }
}
