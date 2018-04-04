using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
    public class PlaceItem : ComplexTask
    {
        int MapX, MapY;
        public PlaceItem(int X, int Y)
        {
            this.MapX = X;
            this.MapY = Y;
        }
        public override void Initialize()
        {
            base.Initialize();
            Components.Tasks.ObtainItem t = new Components.Tasks.ObtainItem();
            Components.Tasks.WalkTo w = new Components.Tasks.WalkTo(MapX, MapY);
            Components.Tasks.Drop d = new Components.Tasks.Drop();
            this.Subtasks.Enqueue(t);
            this.Subtasks.Enqueue(w);
            this.Subtasks.Enqueue(d);
            this.Description = "Place item";
        }
    }
}
