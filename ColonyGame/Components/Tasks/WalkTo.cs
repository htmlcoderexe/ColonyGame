using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyGame.Interfaces;
using Microsoft.Xna.Framework;

namespace ColonyGame.Components.Tasks
{
    public class WalkTo : ITask
    {
        public MapCreature Actor { get; set; }
        public Action CompletedAction { get; set; }
        public TaskState State { get; set; }
        public int Priority { get; set; }
        public WorkTypes WorkType { get; set; }


        public string SuspendReason { get; set; }
        public string Description { get; set; }
        private Queue<Vector2> _WalkPath;

        public Queue<Vector2> WalkPath
        {
            get
            {
                if (this._WalkPath == null)
                    this._WalkPath = new Queue<Vector2>();
                return this._WalkPath;
            }
            set
            {
                this._WalkPath = value;
            }
        }

        public Vector2 WalkTarget { get; private set; }


        private int X;
        private int Y;

        public WalkTo(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            //*/
           
        }

        public void Initialize()
        {
            Vector2 Diff = new Vector2(X - this.Actor.Position.X, Y - this.Actor.Position.Y);

            this.WalkPath.Clear();

            float straight = Math.Abs(Diff.X) - Math.Abs(Diff.Y);
            float dX, dY, dZ;
            dX = Diff.X > 0 ? 1 : -1;
            dY = Diff.Y > 0 ? 1 : -1;
            dZ = dX == dY ? 1 : -1;
            //this.WalkPath.Enqueue(new Vector2(dX,dY));
            //*
            if (straight > 0)
            {
                this.WalkTarget = new Vector2(this.Actor.Position.X + Diff.Y * dZ, this.Actor.Position.Y + Diff.Y);
                this.WalkPath.Enqueue(new Vector2(X, Y));
            }
            if (straight < 0)
            {
                this.WalkTarget = new Vector2(this.Actor.Position.X + Diff.X, this.Actor.Position.Y + Diff.X * dZ);
                this.WalkPath.Enqueue(new Vector2(X, Y));
            }
            if (straight == 0)
            {
                this.WalkTarget = new Vector2(X, Y);
            }
            this.Description = "moving";
        }

        public void Progress(float dT)
        {
            Vector2 diff = Actor.Position - WalkTarget;
            if (diff.Length() <= 0.1f)
            {
                if (WalkPath.Count <= 0)
                {
                    this.State = TaskState.Complete;
                    return;
                }
                WalkTarget = WalkPath.Dequeue();
            }
            else
            {
                diff.Normalize();
                this.Actor.Position += -diff * dT * this.Actor.GetSpeed();
            }
        }
    }
}
