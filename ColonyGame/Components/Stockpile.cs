using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Components
{
    public class Stockpile : Interfaces.IZone, Interfaces.IMapObject, Interfaces.ISelectable
    {
        public List<MapItem> Items;

        public int Height { get; set; }

        public string Name { get; set; }

        public Map ParentMap { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public List<Point> Reserved {get;set;}

        public string Description { get; set; }

        public Rectangle Bound
        {
            get
            {
                return new Rectangle(this.X*Map.Tile.TileStride, this.Y * Map.Tile.TileStride, this.Width * Map.Tile.TileStride, this.Height * Map.Tile.TileStride);
            }
            
        }

        public Stockpile()
        {
            this.Reserved = new List<Point>();
            this.Items = new List<MapItem>();
        }

        public void TakeItem(MapItem Item)
        {
            Items.Add(Item);
        }

        public bool HasItem(MapItem Item)
        {
            return Bound.Contains(new Vector2(Item.X*Components.Map.Tile.TileStride, Item.Y * Components.Map.Tile.TileStride));
        }
        public void GiveItem(int X, int Y)
        {
            this.Reserved.Remove(new Point(X, Y));
        }
        public void ForceCheck()
        {
            foreach(MapItem i in ParentMap.GetObjects<MapItem>())
            {
                if (this.Items.Contains(i) && !this.HasItem(i))
                    this.Items.Remove(i);
                if (!this.Items.Contains(i) && this.HasItem(i))
                    this.Items.Add(i);
                
            }
        }
        public MapItem FindItem(Interfaces.IItemFilter Filter = null)
        {
            if (this.Items.Count > 0)
            {
                MapItem i = this.Items[0];
                this.Items.Remove(i);
                i.Reserved = true;
                return i;
            }
            return null;
        }
        public List<Interfaces.ITask> GenerateTasks()
        {
            List<Interfaces.ITask> result = new List<Interfaces.ITask>();

            Point v;

            List<MapItem> items = ParentMap.GetObjects<MapItem>();
            foreach(MapItem Item in items)
            {
                if(!Item.Reserved&&ParentMap.FindStockpile(Item)==null)
                {
                    v = GetFreeSpot();
                    if (v == new Point(-1, -1))
                    {
                        return result;
                    }
                    Components.Tasks.Haul h = new Tasks.Haul(Item, v);
                    Item.Reserved = true;
                    Reserved.Add(v);
                    result.Add(h);
                }
            }

            return result;
        }

        public Point GetFreeSpot()
        {
            for(int x=X;x<X+Width;x++)
                for (int y = Y; y < Y + Height; y++)
                {
                    if(this.ParentMap.GetObjects<Components.MapItem>(x,y).Count<=0)
                    {
                        if(!Reserved.Contains(new Point(x, y)))
                        return new Point(x, y);
                    }
                    else
                    {

                    }
                }
            return new Point(-1,-1);
        }

        public void Update(float dT)
        {
            this.Description = string.Join(", ", this.Items.Select(s => s.Name).ToArray<string>());

        }
    }
}
