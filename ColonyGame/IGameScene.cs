using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame
{
    public interface IGameScene
    {
        KeyboardState pks { get; set; }
        MouseState pms { get; set; }
        void Update(float dT);
        void HandleInput(float dT);
        void Render(float dT, GraphicsDevice device, SpriteBatch batch);
        void Init(GraphicsDevice device);
        void ScreenResized(GraphicsDevice graphicsDevice);
    }
}
