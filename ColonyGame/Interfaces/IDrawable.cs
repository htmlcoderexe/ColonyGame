using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IDrawable
    {
        void Render(float dT, GraphicsDevice device, SpriteBatch batch,Rectangle ViewPort,float Scale);
    }
}
