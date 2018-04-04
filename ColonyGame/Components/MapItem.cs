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
    public class MapItem : Interfaces.IDrawable, IMapObject,IPositionFixed, IMaterial,ISelectable
    {
        public Color MaterialColor { get; set; }

        public string Name { get; set; }

        public Map ParentMap { get; set; }

        public float PricePerUnit { get; set; }

        public int Sprite { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Reserved { get; set; }

        public string Description { get; set; }

        public Rectangle Bound { get; set; }

        public void Render(float dT, GraphicsDevice device, SpriteBatch batch, Rectangle ViewPort, float Scale)
        {
            Rectangle rekt = new Rectangle(0, 0, Tile.TileStride, Tile.TileStride);
            Vector2 drawspot = new Vector2(X * Tile.TileStride * Scale, Y * Tile.TileStride * Scale);
            drawspot.X -= ViewPort.X;
            drawspot.Y -= ViewPort.Y;

            rekt.X = this.Sprite * Tile.TileStride;

            batch.Draw(Assets.Textures["items"], drawspot, rekt, MaterialColor, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 0.0f);
            int s = Components.Map.Tile.TileStride;
            this.Bound= new Rectangle(this.X*s,this.Y*s, s, s);
        }
    }
}
