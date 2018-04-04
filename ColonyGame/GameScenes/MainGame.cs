using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ColonyGame.GameScenes
{
    public class MainGame : IGameScene
    {
        public KeyboardState pks { get; set; }

        public MouseState pms { get; set; }

        Components.Map Map { get; set; }

        Rectangle Viewport { get; set; }

        float ZoomLevel { get; set; }

        public Components.MapCreature player;

        public Windows.StatusWindow StatusWindow;

        GUI.WindowManager WM;

        public static bool DebugKeyDown;
        Keys DebugKey= Keys.LeftShift;

        Interfaces.ISelectable Selection = null;

        enum GameState
        {
            Map,Inspect,Designate,Build
        }

        public struct DesignationState
        {
            public  bool IsActive;
            public  bool Complete;
            public  Point Start;
            public  Point End;
         
            public DesignationState Normalize()
            {
                int X0, X1, Y0, Y1;
                X0 =Math.Min(Start.X, End.X);
                X1 =Math.Max(Start.X, End.X);
                Y0 =Math.Min(Start.Y, End.Y);
                Y1 =Math.Max(Start.Y, End.Y);
                DesignationState result = new DesignationState
                {
                    IsActive = this.IsActive,
                    Complete = this.Complete,
                    Start = new Point(X0, Y0),
                    End = new Point(X1, Y1)
                };
                return result;
            }

            public List<Point> GetCells()
            {
                List<Point> result = new List<Point>();
                int X0, X1, Y0, Y1;
                X0 = Math.Min(Start.X, End.X);
                X1 = Math.Max(Start.X, End.X);
                Y0 = Math.Min(Start.Y, End.Y);
                Y1 = Math.Max(Start.Y, End.Y);
                for (int x = X0; x <= X1; x++)
                    for (int y = Y0; y <= Y1; y++)
                    {
                        result.Add(new Point(x, y));
                    }
                return result;
            }

        }

        DesignationState dragState;
        public MainGame()
        {
            this.ZoomLevel = 1.0f;
        }

        GameState State;

        Point LastClick;
        int SelectionIndex;

        public void HandleInput(float dT)
        {
            KeyboardState cks = Keyboard.GetState();
            MouseState cms = Mouse.GetState();

            DebugKeyDown = cks.IsKeyDown(DebugKey);

            WM.MouseX = pms.X;
            WM.MouseY = pms.Y;
            int X = cms.Position.X;
            int Y = cms.Position.Y;
            bool MouseHandled = WM.HandleMouse(cms, dT);

            if (!MouseHandled)
            {
                if (cms.LeftButton == ButtonState.Released && pms.LeftButton == ButtonState.Pressed)
                    Click(cms.Position.X, cms.Position.Y);
                if (cms.LeftButton == ButtonState.Released && pms.LeftButton == ButtonState.Pressed)
                    MouseUp(cms.Position.X, cms.Position.Y);
                if (cms.LeftButton == ButtonState.Pressed && pms.LeftButton == ButtonState.Released)
                    MouseDown(cms.Position.X, cms.Position.Y);
                int MapX = (int)(Math.Floor((X + Viewport.X) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
                int MapY = (int)(Math.Floor((Y + Viewport.Y) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));

                if (this.State == GameState.Designate && this.dragState.IsActive && cms.LeftButton == ButtonState.Pressed && !this.dragState.Complete)
                {
                   // this.dragState.Complete = true;
                    this.dragState.End = new Point(MapX, MapY);
                }

                float ZoomF =(float)Math.Pow(2,1.0/3.0);
                Rectangle xViewport = Viewport;
                int newX, newY;
                float CenterX, CenterY, HalfWidth, HalfHeight;
                HalfWidth = (float)xViewport.Width / 2f;
                HalfHeight = (float)xViewport.Height / 2f;
                CenterX = xViewport.X + HalfWidth;
                CenterY = xViewport.Y + HalfHeight;
                //*/

                float zprev = ZoomLevel;
                if (cms.ScrollWheelValue > pms.ScrollWheelValue)
                {
                    ZoomLevel *= ZoomF;
                    ZoomF = 1 / ZoomF;
                }
                if (cms.ScrollWheelValue < pms.ScrollWheelValue)
                {
                    ZoomLevel /= ZoomF;
                }
                 ZoomLevel = MathHelper.Clamp(ZoomLevel, 0.5f, 2f);
                if (zprev != ZoomLevel)
                {
                    if (DebugKeyDown)
                        Console.Write(zprev + ">" + ZoomLevel);
                    newX = (int)Math.Round(CenterX / ZoomF - HalfWidth);
                    newY = (int)Math.Round(CenterY / ZoomF - HalfHeight);
                    Viewport = new Rectangle(newX, newY, xViewport.Width, xViewport.Height);
                }
            }

             //wasd
            int delta = 5;

            if (cks.IsKeyDown(Keys.W))
                this.Viewport = new Rectangle(this.Viewport.X, this.Viewport.Y - delta, this.Viewport.Width, this.Viewport.Height);
            if (cks.IsKeyDown(Keys.S))                                          
                this.Viewport = new Rectangle(this.Viewport.X, this.Viewport.Y + delta, this.Viewport.Width, this.Viewport.Height);
            if (cks.IsKeyDown(Keys.A))                                          
                this.Viewport = new Rectangle(this.Viewport.X - delta, this.Viewport.Y, this.Viewport.Width, this.Viewport.Height);
            if (cks.IsKeyDown(Keys.D))                                           
                this.Viewport = new Rectangle(this.Viewport.X + delta, this.Viewport.Y, this.Viewport.Width, this.Viewport.Height);
            //end wasd

            //modes

            if (cks.IsKeyDown(Keys.F1))
                this.State = GameState.Map;
            if (cks.IsKeyDown(Keys.F2))
                this.State = GameState.Designate;
            if (cks.IsKeyDown(Keys.F3))
                this.State = GameState.Build;
            if (cks.IsKeyDown(Keys.Escape))
                this.State = GameState.Map;

            //end modes

            pms = cms;

        }

        public void Click(int X,int Y)
        {
            int MapX, MapY,RealMapX,RealMapY;

            RealMapX = (int)(Math.Floor((X + Viewport.X)  / this.ZoomLevel));
            RealMapY = (int)(Math.Floor((Y + Viewport.Y)  / this.ZoomLevel));
            MapX = (int)(Math.Floor((X + Viewport.X) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
            MapY = (int)(Math.Floor((Y + Viewport.Y) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
           // Console.Write(GUI.Renderer.ColourToCode(Color.Blue) + " " + MapX + "," + MapY);
           Interfaces.ISelectable m;
            Point p,mp;
            bool sel = false;
            p = new Point(RealMapX, RealMapY);
            mp = new Point(MapX, MapY);
            List<Interfaces.ISelectable> selection = new List<Interfaces.ISelectable>();
            foreach (Interfaces.IMapObject o in this.Map.Objects)
            {
                m = o as Interfaces.ISelectable;
                if (m != null && m.Bound.Contains(p))
                {
                    //m.MaterialColor = Color.Red;
                    /*
                    Components.Tasks.WalkTo t = new Components.Tasks.WalkTo((o as Components.MapItem).X, (o as Components.MapItem).Y);
                    t.Actor = player;
                    player.Tasks.Enqueue(t);
                    //*/
                    selection.Add(m);
                    sel = true;
                }

            }
            if(sel)
            {
                if(LastClick.Equals(mp))
                {
                    SelectionIndex++;
                    if (SelectionIndex >= selection.Count)
                        SelectionIndex = 0;
                }
                else
                {
                    SelectionIndex = 0;
                }
                LastClick = mp;
                Select(selection[SelectionIndex]);
            }
            else
            {
                if (State != GameState.Build)
                {
                    Select(Map.MakeSelectable(MapX, MapY));
                }
                else
                {
                    Components.WallTest wt = new Components.WallTest();
                    wt.X = MapX;
                    wt.Y = MapY;
                    wt.WorkRequired = 34;
                    wt.ParentMap = this.Map;
                    Components.Tasks.Build b = new Components.Tasks.Build(wt);
                    b.Description = "Build wall";
                    this.Map.AvailableTasks.Add(b);
                }
            }
        }

        public void MouseDown(int X, int Y)
        {
           int RealMapX = (int)(Math.Floor((X + Viewport.X) / this.ZoomLevel));
           int RealMapY = (int)(Math.Floor((Y + Viewport.Y) / this.ZoomLevel));
           int MapX = (int)(Math.Floor((X + Viewport.X) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
           int MapY = (int)(Math.Floor((Y + Viewport.Y) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));

            if (this.State== GameState.Designate && this.dragState.IsActive==false)
            {
                this.dragState.IsActive = true;
                this.dragState.Complete = false;
                this.dragState.Start = new Point(MapX, MapY);
                //Console.Write("^FF0000  Drag start: " + this.dragState.Start.ToString());
            }
        }
        public void MouseUp(int X, int Y)
        {
            int RealMapX = (int)(Math.Floor((X + Viewport.X) / this.ZoomLevel));
            int RealMapY = (int)(Math.Floor((Y + Viewport.Y) / this.ZoomLevel));
            int MapX = (int)(Math.Floor((X + Viewport.X) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
            int MapY = (int)(Math.Floor((Y + Viewport.Y) / (float)Components.Map.Tile.TileStride / this.ZoomLevel));
            if (this.State == GameState.Designate && this.dragState.IsActive)
            {
                this.dragState.Complete = true;
                this.dragState.End = new Point(MapX, MapY);
                //Console.Write("^FF0000  Drag end: " + this.dragState.End.ToString());

            }
        }

        public void Select(Interfaces.ISelectable Selection)
        {

            Console.Write("Clicked on: " + Selection.Name);
            WM.Windows.Remove(StatusWindow);
            Windows.StatusWindow w = new Windows.StatusWindow(WM, Selection);
            if (StatusWindow != null)
            {
                w.X = StatusWindow.X;
                w.Y = StatusWindow.Y;
            }
            WM.Add(w);
            StatusWindow = w;
            this.Selection = Selection;
        }

        public void ConsoleWrite(string Text)
        {
            ConsoleWriteEx(Text);
        }
        public void ConsoleWriteEx(string Text, List<System.Action> Links = null)
        {
            if (WM == null)
                return;
            Windows.ConsoleWindow console = null;
            foreach (GUI.Window w in WM.Windows)
            {
                if ((w as Windows.ConsoleWindow) != null)
                {
                    console = w as Windows.ConsoleWindow;
                    break;
                }
            }
            if (console == null)
                return;
            console.AppendMessage(Text, Links);
        }

        public void Init(GraphicsDevice device)
        {
            Assets.Textures["tiles1"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\tiles2.png", System.IO.FileMode.Open));
            Assets.Textures["walls"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\walls2.png", System.IO.FileMode.Open));
            Assets.Textures["items"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\itemsprites.png", System.IO.FileMode.Open));
            Assets.Textures["creatures"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\creatures.png", System.IO.FileMode.Open));
            Assets.Textures["winskin"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\winskin.png", System.IO.FileMode.Open));
           // Assets.Textures["creatures"] = Texture2D.FromStream(device, new System.IO.FileStream("graphics\\creatures.png", System.IO.FileMode.Open));
            this.Map = new Components.Map(40, 40);
            //*
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 1";
            player.WorkProfile[Components.WorkTypes.Hauling] = true;
            this.Map.Objects.Add(player);
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 2";
            player.WorkProfile[Components.WorkTypes.Construction] = true;
            this.Map.Objects.Add(player);
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 3";
            player.WorkProfile[Components.WorkTypes.Hauling] = true;
            this.Map.Objects.Add(player);
            //*/
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 4";
            player.WorkProfile[Components.WorkTypes.Construction] = true;
            this.Map.Objects.Add(player);
            /*/
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 5";
            this.Map.Objects.Add(player);
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 6";
            this.Map.Objects.Add(player);
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 7";
            this.Map.Objects.Add(player);
            player = new Components.MapCreature();
            player.ParentMap = this.Map;
            player.Name = "Colonist 8";
            this.Map.Objects.Add(player);
            //*/
            WM = new GUI.WindowManager();
            WM.Renderer = new GUI.Renderer(device);
            WM.Renderer.GUIEffect = Assets.Effects["GUI"];
            WM.Renderer.UIFont = Assets.SpriteFonts["UIFontO"];
            WM.Renderer.WindowSkin = Assets.Textures["winskin"];
            GUI.ToolTipWindow t = new GUI.ToolTipWindow(WM, "GUI successfully installed!", 0, 0,false);
            Console.WriteCallback = new Action<string>(ConsoleWrite);
            Console.WriteCallbackEx = new Action<string, List<Action>>(ConsoleWriteEx);
            t.AnchorBottom = true;
            t.AnchorRight = true;
            WM.Add(t);
            WM.Add(new Windows.ConsoleWindow(WM));
            WM.Screen.X = 0;
            WM.Screen.Y = 0;

            ScreenResized(device);

            
        }

        public void Render(float dT, GraphicsDevice device, SpriteBatch batch)
        {
            
            this.Map.Render(dT, device, batch, this.Viewport,this.ZoomLevel);
            this.WM.Render(device);
            if(this.Selection!=null)
            {
                GUI.Renderer.Rect r1 = new GUI.Renderer.Rect(48, 32, 16, 16);
                WM.Renderer.SetColour(Color.Red);

                float dX=-Viewport.X/ZoomLevel;
                float dY = -Viewport.Y/ZoomLevel;
                float X0, X1, Y0, Y1, Stride;
                X0 = Selection.Bound.X + dX;
                X1 = Selection.Bound.X + dX + Selection.Bound.Width - 16;
                Y0 = Selection.Bound.Y + dY;
                Y1 = Selection.Bound.Y + Selection.Bound.Height - 16 + dY;

                X0 *= ZoomLevel;
                X1 *= ZoomLevel;
                Y0 *= ZoomLevel;
                Y1 *= ZoomLevel;
                Stride = 16 * ZoomLevel;
                WM.Renderer.RenderQuad(device, X0, Y0, Stride, Stride, r1);
                r1 = new GUI.Renderer.Rect(48+16, 32, 16, 16);
                WM.Renderer.RenderQuad(device, X1, Y0, Stride, Stride, r1);
                r1 = new GUI.Renderer.Rect(48+16, 32+16, 16, 16);
                WM.Renderer.RenderQuad(device, X1, Y1, Stride, Stride, r1);
                r1 = new GUI.Renderer.Rect(48, 32+16, 16, 16);
                WM.Renderer.RenderQuad(device, X0, Y1, Stride, Stride, r1);
            }

            if(this.State== GameState.Designate && this.dragState.IsActive)
            {
                GUI.Renderer.Rect r1 = new GUI.Renderer.Rect(80, 48, 16, 16);
                WM.Renderer.SetColour(Color.Red);
                DesignationState Selection = this.dragState.Normalize();
                float dX = -Viewport.X / ZoomLevel;
                float dY = -Viewport.Y / ZoomLevel;
                int X0, X1, Y0, Y1;
                float Stride,RealX,RealY;
                X0 = Selection.Start.X;
                Y0 = Selection.Start.Y;
                X1 = Selection.End.X;
                Y1 = Selection.End.Y;

                RealX=(dX+X0 * Components.Map.Tile.TileStride) * ZoomLevel;
                RealY=(dY+Y0 * Components.Map.Tile.TileStride) * ZoomLevel;
                Stride = 64 * ZoomLevel;
                for (int x = X0; x <=X1; x++)
                    for (int y = Y0; y <=Y1; y++)
                    {
                        WM.Renderer.RenderQuad(device, RealX + (x - X0) * Stride, RealY + (y - Y0) * Stride, Stride, Stride, r1);
                    }
            }
        }

        public void Update(float dT)
        {
            WM.Update(dT);
            foreach(Components.Stockpile p in this.Map.Stockpiles)
            {
                this.Map.AvailableTasks.AddRange(p.GenerateTasks());
                p.Update(dT);
            }

            for(int i=0;i<Map.Objects.Count;i++)
            {
                Interfaces.IActor o = Map.Objects[i] as Interfaces.IActor;
                if(o!=null)
                o.DoStuff(dT);
            }

            if(this.State== GameState.Designate && this.dragState.IsActive && this.dragState.Complete)
            {
                this.dragState.IsActive=false;
                foreach(Point p in this.dragState.GetCells())
                {
                    Components.WallTest wt = new Components.WallTest();
                    wt.X = p.X;
                    wt.Y = p.Y;
                    wt.WorkRequired = 34;
                    wt.ParentMap = this.Map;
                    Components.Tasks.Build b = new Components.Tasks.Build(wt);
                    b.Description = "Build wall";
                    this.Map.Objects.Add(wt);
                    this.Map.AvailableTasks.Add(b);
                }
            }

        }

        public void BatchDesignate(DesignationState Selection)
        {

        }

        public void ScreenResized(GraphicsDevice device)
        {
            int ScreenWidth = device.PresentationParameters.BackBufferWidth;
            int ScreenHeight = device.PresentationParameters.BackBufferHeight;
            //Screen = new RenderTarget2D(device, ScreenWidth, ScreenHeight, false, device.PresentationParameters.BackBufferFormat, device.PresentationParameters.DepthStencilFormat);
            //b = new SpriteBatch(device);
            WM.ScreenResized(ScreenWidth, ScreenHeight);
            this.Viewport = new Rectangle(this.Viewport.X, this.Viewport.Y, ScreenWidth, ScreenHeight);
        }
    }
}
