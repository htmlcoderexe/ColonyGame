using ColonyGame.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
    public class Haul : ComplexTask
    {
        MapItem item;
        Point dest;
        public Haul(MapItem i, Point Destination)
        {
            item = i;
            dest = Destination;
            this.WorkType = WorkTypes.Hauling;
        }
        public override void Initialize()
        {
            this.Description="Haul "+item.Name;
            base.Initialize();
            WalkTo w = new WalkTo(item.X, item.Y);
            this.Subtasks.Enqueue(w);
            Pickup p = new Pickup();
            this.Subtasks.Enqueue(p);
            WalkTo a = new WalkTo(dest.X, dest.Y);
            this.Subtasks.Enqueue(a);
            Drop d = new Drop();
            this.Subtasks.Enqueue(d);
        }

        public override void Progress(float dT)
        {
           
            base.Progress(dT);
        }
    }
}
