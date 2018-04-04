using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
    public class Pickup : Interfaces.ITask
    {
        public MapCreature Actor { get; set; }
        public Action CompletedAction { get; set; }
        public Interfaces.TaskState State { get; set; }
        public int Priority { get; set; }
        public WorkTypes WorkType { get; set; }

        public string SuspendReason { get; set; }
        public string Description { get; set; }

        public void Initialize()
        {

        }

        public void Progress(float dT)
        {
            int X = (int)(Math.Round(Actor.Position.X));
            int Y= (int)(Math.Round(Actor.Position.Y));
            MapItem m = Actor.ParentMap.GetObjects<MapItem>(X, Y)[0];
            Actor.Carrying = m;
            foreach(Components.Stockpile p in Actor.ParentMap.Stockpiles)
            {
                if (p.HasItem(m))
                    p.GiveItem(m.X, m.Y);
            }
            Actor.ParentMap.Objects.Remove(m);
            this.State = Interfaces.TaskState.Complete;
        }
    }
}
