using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
    public class ObtainItem : ComplexTask
    {
        Interfaces.IItemFilter Filter;
        MapItem item;
        public MapItem GetItem()
        {
            return item;
        }
        public ObtainItem(Interfaces.IItemFilter Filter = null)
        {
            this.Filter = Filter;
            this.Description = "Get item";

        }

        public override void Initialize()
        {
            base.Initialize();
            foreach(Stockpile pile in Actor.ParentMap.Stockpiles)
            {
                item = pile.FindItem(this.Filter);
                if (item != null)
                    break;
            }
            if(item==null)
            {
                this.State = Interfaces.TaskState.Suspended;
                SuspendReason = "Needs material.";
                return;
            }
            WalkTo w = new WalkTo(item.X, item.Y);
            this.Subtasks.Enqueue(w);
            Pickup p = new Pickup();
            this.Subtasks.Enqueue(p);
        }
    }
}
