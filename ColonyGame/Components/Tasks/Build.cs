using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components.Tasks
{
     public class Build : ComplexTask
    {
        Interfaces.IBuildable b;
        public Build(Interfaces.IBuildable b)
        {
            this.b = b;
        }
        public override void Initialize()
        {
            base.Initialize();
            this.WorkType = WorkTypes.Construction;
            ObtainItem o = new ObtainItem();
            o.CompletedAction = new Action(() => b.MainMaterial = new Materials.PlainMaterial {MaterialColor=o.GetItem().MaterialColor });
            WalkTo w = new WalkTo(b.X, b.Y);
            Drop d = new Drop();
            d.Destroy = true;
            DoWork dw = new DoWork();
            dw.Description = "constructing";
            dw.WorkLeft = (b as Interfaces.IWorkable).WorkRequired;
            dw.Target = b as Interfaces.IWorkable;
            this.Subtasks.Enqueue(o);
            this.Subtasks.Enqueue(w);
            this.Subtasks.Enqueue(d);
            this.Subtasks.Enqueue(dw);
        }
    }
}
