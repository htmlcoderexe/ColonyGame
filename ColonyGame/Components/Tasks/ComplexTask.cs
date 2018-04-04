using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonyGame.Interfaces;

namespace ColonyGame.Components.Tasks
{
    public class ComplexTask : Interfaces.ITask
    {
        public Queue<ITask> Subtasks { get; set; }
        public ITask CurrentTask { get; set; }
        public string SuspendReason { get; set; }
        public string Description { get; set; }
        public WorkTypes WorkType { get; set; }

        private MapCreature _Actor;

        public ComplexTask()
        {
            this.Subtasks = new Queue<ITask>();
        }
        public  MapCreature Actor {
            get
            {
                return _Actor;
            }
            set
            {
                this._Actor = value;
                foreach (ITask t in this.Subtasks)
                    t.Actor = value;
            }
        }

        public TaskState State
        { get; set; }
        public int Priority { get; set; }

        public Action CompletedAction { get; set; }

        public virtual  void Initialize()
        {

            this.Subtasks.Clear();
            this.CurrentTask = null;
        }

        public virtual void Progress(float dT)
        {
            if (CurrentTask == null)
            {
                if (Subtasks.Count > 0)
                {
                    
                    CurrentTask=(Subtasks.Dequeue());
                    CurrentTask.Actor = this.Actor;
                    CurrentTask.State = TaskState.InProgress;
                    CurrentTask.Initialize();
                }
                return;
            }
            switch (CurrentTask.State)
            {
                case TaskState.Enqueued:
                    {
                        CurrentTask.State = TaskState.InProgress;
                        CurrentTask.Initialize();
                        break;
                    }
                case TaskState.Complete:
                    {
                        CurrentTask.CompletedAction?.Invoke();
                        CurrentTask = null;
                        if (Subtasks.Count > 0)
                        {
                            CurrentTask = (Subtasks.Dequeue());
                            CurrentTask.Actor = this.Actor;
                            CurrentTask.State = TaskState.InProgress;
                            CurrentTask.Initialize();
                        }
                        else
                        {
                            this.State = TaskState.Complete;
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
                        
                        this.State = TaskState.Suspended;
                        this.SuspendReason = this.CurrentTask.SuspendReason;
                        Console.Write(this.Actor.Name + " cancels " +this.CurrentTask.Description+ ": " + CurrentTask.SuspendReason);
                        break;
                    }
            }
        }
    }
}
