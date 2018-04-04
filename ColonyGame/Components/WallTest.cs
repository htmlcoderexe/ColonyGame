using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static ColonyGame.Components.Map;

namespace ColonyGame.Components
{
    public class WallTest : Interfaces.IBuildable, Interfaces.IMapObject, Interfaces.IWorkable,Interfaces.IDrawable,Interfaces.ISelectable
    {
        public string Name { get; set; }

        public Map ParentMap { get; set; }

        public float WorkComplete
        {
            get
            {
                return _doneWork;
            }
            set
            {
                _doneWork = value;
                if (_doneWork >= WorkRequired)
                    WorkCompleted();
            }
        }

        float _doneWork;

        public float WorkRequired { get; set; }

        public Components.Materials.PlainMaterial MainMaterial { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string Description { get; set; }

        public Rectangle Bound { get; private set; }
        public int Sprite { get; private set; }

        public WallTest()
        {
            Sprite = 1;
        }

        public void WorkCompleted()
        {
            Components.Map.Tile t = this.ParentMap.GetTile(this.X, this.Y);
            t.WallType = 1;
            t.WallMaterial = MainMaterial;
            this.ParentMap.UpdateTile(this.X, this.Y);
            this.ParentMap.Objects.Remove(this);
        }

        public void Render(float dT, GraphicsDevice device, SpriteBatch batch, Rectangle ViewPort, float Scale)
        {
            Rectangle rekt = new Rectangle(0, 0, Tile.TileStride, Tile.TileStride);
            Vector2 drawspot = new Vector2(X * Tile.TileStride * Scale, Y * Tile.TileStride * Scale);
            drawspot.X -= ViewPort.X;
            drawspot.Y -= ViewPort.Y;

            rekt.X = this.Sprite * Tile.TileStride;

            batch.Draw(Assets.Textures["creatures"], drawspot, rekt, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
            this.Bound = new Rectangle(X * Tile.TileStride, Y * Tile.TileStride, Tile.TileStride, Tile.TileStride);

        }
    }
}
