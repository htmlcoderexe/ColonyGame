using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ColonyGame.Components
{
    public class Map : Interfaces.IDrawable
    {
        public class Tile
        {
            public int FloorType;
            public int WallType;
            public float Light;
            public float Temperature;
            public int WallAutoTileCode;
            public Interfaces.IMaterial WallMaterial;
            public const int TileStride = 64;
           
        }

        public struct Selectable : Interfaces.ISelectable
        {
            public Rectangle Bound { get; }

            public string Description { get; set; }

            public string Name { get; set; }
            
            public Selectable(int X, int Y)
            {
                Bound = new Rectangle(X, Y, Tile.TileStride, Tile.TileStride);
                
                Description = "";
                Name = "";
            }
            
        }

        public Selectable MakeSelectable(int MapX, int MapY)
        {
            Selectable result = new Selectable(MapX*Tile.TileStride,MapY*Tile.TileStride);
            Tile t = this.GetTile(MapX, MapY);
            result.Description = "Terrain";
            result.Name = "Floor " + t.FloorType;
            if (t.WallType != 0)
            {
                result.Name = "Wall " + t.WallType;
                if (t.WallMaterial != null)
                    result.Name = t.WallMaterial.Name + " wall";
            }
            return result;
        }


        public static Dictionary<int, Vector2> BlobIndices=new Dictionary<int, Vector2>()
        {
            #region BlobMappings
            {0, new Vector2(5,1)},
            {2, new Vector2(0,2)},
            {8, new Vector2(3,3)},
            {10, new Vector2(6,2)},
            {11, new Vector2(3,2)},
            {16, new Vector2(1,3)},
            {18, new Vector2(4,2)},
            {22, new Vector2(1,2)},
            {24, new Vector2(2,3)},
            {26, new Vector2(5,2)},
            {27, new Vector2(6,4)},
            {30, new Vector2(5,4)},
            {31, new Vector2(2,2)},
            {64, new Vector2(0,0)},
            {66, new Vector2(0,1)},
            {72, new Vector2(6,0)},
            {74, new Vector2(6,1)},
            {75, new Vector2(5,3)},
            {80, new Vector2(4,0)},
            {82, new Vector2(4,1)},
            {86, new Vector2(6,3)},
            {88, new Vector2(5,0)},
            {90, new Vector2(8,1)},
            {91, new Vector2(8,3)},
            {94, new Vector2(9,3)},
            {95, new Vector2(8,0)},
            {104, new Vector2(3,0)},
            {106, new Vector2(7,4)},
            {107, new Vector2(3,1)},
            {120, new Vector2(4,3)},
            {122, new Vector2(8,4)},
            {123, new Vector2(7,1)},
            {126, new Vector2(3,4)},
            {127, new Vector2(7,0)},
            {208, new Vector2(1,0)},
            {210, new Vector2(4,4)},
            {214, new Vector2(1,1)},
            {216, new Vector2(7,3)},
            {218, new Vector2(9,4)},
            {219, new Vector2(2,4)},
            {222, new Vector2(9,1)},
            {223, new Vector2(9,0)},
            {248, new Vector2(2,0)},
            {250, new Vector2(8,2)},
            {251, new Vector2(7,2)},
            {254, new Vector2(9,2)},
            {255, new Vector2(2,1)}
#endregion
        };

        public Tile BoundaryTile = new Tile { WallType=255 };

        public Tile[,] Tiles;

        public List<Interfaces.IMapObject> Objects;

        public List<Components.Stockpile> Stockpiles;
        public List<Interfaces.ITask> AvailableTasks;

        public Interfaces.ITask GetTask(Interfaces.IActor actor)
        {
            Interfaces.ITask result = null;
            foreach(Interfaces.ITask task in AvailableTasks)
            {
                if(actor.CanDo(task))
                {
                    if (result == null || result.Priority < task.Priority)
                        result=task;
                }
            }
            AvailableTasks.Remove(result);
            return result;
        }

        public Stockpile FindStockpile(MapItem Item)
        {
            foreach(Stockpile p in this.Stockpiles)
            {
                if (p.HasItem(Item))
                    return p;
            }
            return null;
        }

        public Map(int Width,int Height)
        {
            this.Tiles = new Tile[Width, Height];
            this.Stockpiles = new List<Stockpile>();
            this.AvailableTasks = new List<Interfaces.ITask>();
            Random rnd = new Random();
            Components.Materials.PlainMaterial m = new Materials.PlainMaterial
            {
                MaterialColor = new Color(rnd.Next(50, 255), rnd.Next(50, 255), rnd.Next(50, 255)),
                Name = "Stone",
                Description="Plain stone"
        };
            Tile t = new Tile
            {
                WallAutoTileCode = -1,
                WallType=1,
                FloorType=0,
                
            };
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    SetTile(x, y, new Tile(),true);
                }
            }
            for (int i=0;i<30;i++)
            {
                int rx = rnd.Next(0, Width);
                int ry = rnd.Next(0, Height);
                t = new Tile
                {
                    WallAutoTileCode = -1,
                    WallType = 1,
                    FloorType = 0,
                    WallMaterial = m
                };
                SetTile(rx, ry, t);
            }

            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    DoAutoTile(x, y);
                }
            }


                    this.Objects = new List<Interfaces.IMapObject>();
            Components.MapItem mi;
            for (int i = 0; i < 120; i++)
            {
                mi = new Components.MapItem();
                mi.X = rnd.Next(0, this.Width);
                mi.Y = rnd.Next(0, this.Height);
                mi.MaterialColor = new Color(rnd.Next(50, 255), rnd.Next(50, 255), rnd.Next(50, 255));
                mi.ParentMap = this;
                mi.Name = "" + GUI.Renderer.ColourToCode(mi.MaterialColor) + " Wood logs ^FFFFFF ";
                this.Objects.Add(mi);
            }

            Stockpile s = new Stockpile();
            s.ParentMap = this;
            s.X = 10;
            s.Y = 10;
            s.Width = 5;
            s.Height = 5;
            s.Name = "Stockpile 1";
            this.Stockpiles.Add(s);
            this.Objects.Add(s);
            s.ForceCheck();
        }


     
        public int Width { get { return this.Tiles.GetLength(0); } }
        public int Height { get { return this.Tiles.GetLength(1); } }

        public List<Interfaces.IMapObject> GetObjects(int X, int Y)
        {
            List<Interfaces.IMapObject> result = new List<Interfaces.IMapObject>();
            Interfaces.IPositionFixed p;
            foreach (Interfaces.IMapObject o in this.Objects)
            {
                p = o as Interfaces.IPositionFixed;
                if (p != null && p.X == X && p.Y == Y)
                    result.Add(o);
            }
            return result;
        }

        public List<T> GetObjects<T>()
        {
            List<T> result = new List<T>();
            foreach (T o in Objects.Where(a => a is T))
            {
                result.Add((T)o);
            }


            return result;
        }
        public List<T> GetObjects<T>(int X, int Y)
        {
            List<T> result = new List<T>();
            Interfaces.IPositionFixed p;
            foreach (T o in Objects.Where(a => a is T ))
            {
                p = o as Interfaces.IPositionFixed;
                if (p != null && p.X == X && p.Y == Y)
                   // result.Add(o);
                result.Add((T)o);
            }


            return result;
        }

        public Tile GetTile(int X, int Y)
        {
            if (X >= this.Width || X < 0 || Y >= this.Height || Y < 0)
                return BoundaryTile;
            else
                return Tiles[X, Y];
        }

        public void SetTile(int X, int Y,Tile Tile, bool NoUpdate=false)
        {
            if (X > this.Width || X < 0 || Y > this.Height || Y < 0)
                return ;
             Tiles[X, Y]=Tile;
            if(!NoUpdate)
            DoAutoTile(X, Y);
        }
        public int GetWall(int X,int Y)
        {
            return GetTile(X, Y).WallType == 0 ? 0 : 1;
        }

        public void Render(float dT, GraphicsDevice device, SpriteBatch batch, Rectangle ViewPort, float Scale)
        {
            if (Tiles == null)
                return;

            Rectangle rekt = new Rectangle(0, 0, Tile.TileStride , Tile.TileStride );
            Rectangle rekt2;
            RasterizerState rs = new RasterizerState { MultiSampleAntiAlias = true };
            rs = null;
            batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, rs, null, Matrix.Identity);
            Tile c;
            Vector2 autov;
            Vector2 drawspot;
            Color tc;
            tc = new Color(178,75,0);
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    c = this.Tiles[x, y];
                    c.FloorType = 2;
                    drawspot = new Vector2(x * Tile.TileStride * Scale, y * Tile.TileStride * Scale);
                    drawspot.X -= ViewPort.X;
                    drawspot.Y -= ViewPort.Y;
                    float offset = 0;
                    offset = -Tile.TileStride * Scale;
                    if (drawspot.X < offset || drawspot.X > ViewPort.Width || drawspot.Y < offset || drawspot.Y > ViewPort.Height)
                        continue;
                    rekt.X =c.FloorType * Tile.TileStride ;
                    if (Scale < 0.1f)
                        Scale = 0.1f;
                    batch.Draw(Assets.Textures["tiles1"], drawspot, rekt, tc, 0.0f, Vector2.Zero, Scale , SpriteEffects.None, 0.0f);
                    Color wallcolour = Color.Wheat;
                    if (c.WallType!=0)
                    {
                        autov = BlobIndices[c.WallAutoTileCode] * Tile.TileStride;
                        rekt2 = new Rectangle((int)autov.X, (int)autov.Y, Tile.TileStride, Tile.TileStride);
                        if (c.WallMaterial != null)
                            wallcolour = c.WallMaterial.MaterialColor;
                        batch.Draw(Assets.Textures["walls"], drawspot,rekt2, wallcolour, 0.0f, Vector2.Zero, Scale , SpriteEffects.None, 0.0f);

                    }

                }

            }
            batch.End();
            Interfaces.IDrawable d;
            batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            for (int i=0;i<Objects.Count();i++)
            {
                d=(Objects[i] as Interfaces.IDrawable);
                if(d!=null)
                d.Render(dT, device, batch, ViewPort,Scale);

            }
            batch.End();
        }

        public Vector2 TileCodeToOffset(int TileCode)
        {
            return BlobIndices[TileCode]*Tile.TileStride;
        }
        public void UpdateTile(int X,int Y)
        {
            for (int x = -1; x < 2; x++)
                for (int y = -1; y < 2; y++)
                    DoAutoTile(X+x, Y+y);
        }
        

        public void DoAutoTile(int X,int Y)
        {
            if (X >= this.Width || X < 0 || Y >= this.Height || Y < 0)
                return;
                int code = 0;

            int left, right, top, bottom, tl, tr, bl, br;
            left = GetWall(X - 1, Y);
            right = GetWall(X + 1, Y);
            top = GetWall(X, Y-1);
            bottom = GetWall(X,Y+1);

            tl=GetWall(X-1,Y-1);
            tr = GetWall(X + 1, Y-1);
            bl = GetWall(X - 1, Y+1);
            br = GetWall(X + 1, Y+1);

            if (top == 0)
            {
                tl = 0; tr = 0;
            }
            if (bottom == 0)
            {
                bl = 0; br = 0;
            }
            if (right == 0)
            {
                br = 0; tr = 0;
            }
            if (left == 0)
            {
                tl = 0; bl = 0;
            }

            code = tl + top * 2 + tr * 4 + left *8 + right * 16 + bl * 32 + bottom * 64 + br * 128;
            Tiles[X, Y].WallAutoTileCode = code;

        }
    }
}
