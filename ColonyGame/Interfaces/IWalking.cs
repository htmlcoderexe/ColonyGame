using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonyGame.Interfaces
{
    public interface IWalking //here! I WALKING HERE!
    {
        Queue<Vector2> WalkPath { get; set; }

        void Walk(float dT);
    }
}
