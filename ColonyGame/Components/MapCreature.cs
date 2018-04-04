using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyGame.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ColonyGame.Components.Map;

namespace ColonyGame.Components
{
    public class MapCreature : Interfaces.IDrawable, IMapObject, IPositionMoving,  ISelectable,IActor
    {
        //Selectable
        public string Description { get; set; }
        
        public string Name { get; set; }
        //MapObject
        public Map ParentMap { get; set; }
        //Moving object
        public Vector2 Movement { get; set; }

        public Vector2 Position { get; set; }
        //walker
        private Queue<Vector2> _WalkPath;

        public Queue<Vector2> WalkPath {
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


        public Vector2 WalkTarget { get; set; }
        //its sprite
        public int Sprite { get; set; }
        //task stuff
        public Queue<Interfaces.ITask> Tasks;

        public Interfaces.ITask CurrentTask;

        public MapItem Carrying;

        public Dictionary<Components.WorkTypes, bool> WorkProfile;
        public Dictionary<Components.WorkTypes, float> SkillMatrix;

        public Rectangle Bound { get; set; }

        public MapCreature()
        {
            this.Tasks = new Queue<ITask>();
            this.Name = "Colonist";
            this.SkillMatrix = new Dictionary<WorkTypes, float>();
            this.WorkProfile = new Dictionary<WorkTypes, bool>();
        }

        public void DoRandomShit(float dT)
        {
            System.Random r = new Random();
            Components.Tasks.WalkTo w = new Components.Tasks.WalkTo(r.Next(0, this.ParentMap.Width), r.Next(0, this.ParentMap.Height));
            w.Description = "Doing random shit";
            w.Actor = this;
            this.Tasks.Enqueue(w);
        }
        public void DoTasks(float dT)
        {
            if (CurrentTask == null)
            {
                if (Tasks.Count > 0)
                {
                    if(GameScenes.MainGame.DebugKeyDown)
                    {

                    }
                    StartTask(Tasks.Dequeue());
                }
                return;
            }
            if (this.CurrentTask.Description != null)
            {
                this.Description = this.CurrentTask.Description;
                if(this.CurrentTask as Components.Tasks.ComplexTask != null &&(this.CurrentTask as Components.Tasks.ComplexTask).CurrentTask!=null)
                {
                    this.Description += ": " + (this.CurrentTask as Tasks.ComplexTask).CurrentTask.Description ?? "unspecified work";
                }
            }

            switch (CurrentTask.State)
            {
                case TaskState.Complete:
                {
                    Console.Write(this.Name + " completed " + this.CurrentTask.Description + ".");
                        CurrentTask.CompletedAction?.Invoke();
                        CurrentTask = null;
                    if (Tasks.Count > 0)
                    {
                        StartTask(Tasks.Dequeue());
                    }
                    break;
                }
                case TaskState.InProgress:
                {
                    CurrentTask.Progress(dT);
                    break;
                }
                case TaskState.Suspended:
                    {
                        CurrentTask.State = TaskState.Enqueued;
                        //
                        CurrentTask.Actor = null;
                        Console.Write(this.Name + " cancels " + this.CurrentTask.Description + ": " + CurrentTask.SuspendReason);
                        //CurrentTask.SuspendReason = "";
                        ParentMap.AvailableTasks.Add(CurrentTask);
                        CurrentTask = null;
                        break;
                    }
            }
        }

        public void StartTask(ITask task)
        {
            //TODO: add code to actually check if a task can be done at all
            if(task.SuspendReason!=null)
            {

            }
            CurrentTask = task;
            task.State = TaskState.InProgress;

            task.Initialize();
            int a = 1;
        }

        public void Render(float dT, GraphicsDevice device, SpriteBatch batch, Rectangle ViewPort, float Scale)
        {
            Rectangle rekt = new Rectangle(0, 0, Tile.TileStride, Tile.TileStride);
            Vector2 drawspot = new Vector2(Position.X * Tile.TileStride * Scale, Position.Y * Tile.TileStride * Scale);
            drawspot.X -= ViewPort.X;
            drawspot.Y -= ViewPort.Y;
            
            rekt.X = this.Sprite * Tile.TileStride;

            batch.Draw(Assets.Textures["creatures"], drawspot, rekt, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
            this.Bound = new Rectangle((int)Math.Floor(this.Position.X * Tile.TileStride), (int)Math.Floor(this.Position.Y * Tile.TileStride), Tile.TileStride, Tile.TileStride);
        }

        public void DoStuff(float dT)
        {

            this.Description = "Idle";
            if (GameScenes.MainGame.DebugKeyDown)
                this.Description = "Idle";


            if (this.Tasks.Count > 0)
            {
                DoTasks(dT);
            }
            else
            {
                if(CurrentTask!=null)
                {
                    DoTasks(dT);
                    return;
                }
                ITask t = ParentMap.GetTask(this);
                if(t!=null)
                { 
                    t.Actor = this;
                    this.Tasks.Enqueue(t);
                }
                else
                {
                    Console.Write(this.Name + " is idling");
                    DoRandomShit(dT);
                }
            }
        }

        public virtual float GetSpeed()
        {
            return 5;
        }

        public bool CanDo(ITask Task)
        {
            if (Task.WorkType == WorkTypes.Generic)
                return true;
            bool result = false;
            WorkProfile.TryGetValue(Task.WorkType, out result);
            return result;
        }
        /*
public void Walk(float dT)
{
   Vector2 diff = Position - WalkTarget;
   if (diff.Length() <= 0.1f)
   {
       if (WalkPath.Count <= 0)
           return;
       WalkTarget = WalkPath.Dequeue();
   }
   else
   {
       diff.Normalize();
       this.Position += -diff * dT*5;
   }

}

public void WalkTo(int X, int Y)
{
   Vector2 Diff = new Vector2(X - this.Position.X, Y - this.Position.Y);

   this.WalkPath.Clear();

   float straight = Math.Abs(Diff.X) - Math.Abs(Diff.Y);
   float dX, dY, dZ;
   dX = Diff.X > 0 ? 1 : -1;
   dY = Diff.Y > 0 ? 1 : -1;
   dZ = dX == dY ? 1 : -1;
   //this.WalkPath.Enqueue(new Vector2(dX,dY));
   //*
   if (straight>0)
   {
       this.WalkPath.Enqueue(new Vector2(this.Position.X + Diff.Y*dZ, this.Position.Y+Diff.Y));
   }
   if (straight<0)
   {
       this.WalkPath.Enqueue(new Vector2(this.Position.X + Diff.X, this.Position.Y + Diff.X*dZ));
   }
   //* /
   this.WalkPath.Enqueue(new Vector2(X, Y));



}//*/
    }
}
