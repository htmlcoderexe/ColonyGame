using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI.Controls;
using GUI;

namespace ColonyGame.Windows
{
    public class StatusWindow :GUI.Window
    {
        private Interfaces.ISelectable item;
        RichTextDisplay d;
        public StatusWindow(WindowManager WM,Interfaces.ISelectable Item)
        {
            this.item = Item;
            this.AnchorBottom = true;
            this.Width = 400;
            this.Height = 160;
            d= new RichTextDisplay(300, 32, WM);
            d.X = 2;
            d.Y = 2;
            this.Title = Item.Name;
            this.AddControl(d);
        }
        public override void Update(float dT)
        {
            base.Update(dT);
            if (item.Description != null)
                d.SetText(item.Description);

        }
    }
}
