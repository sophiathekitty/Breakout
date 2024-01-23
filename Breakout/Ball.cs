using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        //----------------------------------------------------------------------
        // Ball
        //----------------------------------------------------------------------
        public class Ball : ScreenSprite, ICanCollide, ICanMove
        {
            Vector2 velocity = Vector2.One;
            public Vector2 Velocity { get { return velocity; } set { velocity = value; } }
            public static float radius = 5f;
            Paddle paddle;
            public Ball(Vector2 position, Color color, Paddle paddle) : base(ScreenSpriteAnchor.TopLeft, position, 0f, new Vector2(radius * 2), color, "", "Circle", TextAlignment.CENTER, SpriteType.TEXTURE)
            {
                this.paddle = paddle;
            }
            Random rand = new Random();
            public void Collide(ICanCollide box)
            {
                // if we hit a paddle, add the paddle's velocity to the ball's velocity
                ICanMove moveable = box as ICanMove;
                if (moveable != null) velocity += moveable.Velocity;
                // swap a random amount of x and y velocity
                float delta = ((float)rand.NextDouble() - 0.5f)/100;
                velocity = new Vector2(velocity.X + delta, velocity.Y - delta);
            }

            public bool CollidesWith(ICanCollide box)
            {
                bool hit = box.CollidesWith(this);
                if (hit) Collide(box);
                return false;
            }
            override public MySprite ToMySprite(RectangleF _viewport)
            {
                //Visible = BreakoutBoard.inGame;
                if (BreakoutBoard.inGame) Position += velocity;
                else
                {
                    Position = paddle.Position + new Vector2(paddle.Size.X/2f, -paddle.Size.Y / 2f - radius);
                    velocity = new Vector2(paddle.Velocity.X, -1);
                    if (Math.Abs(velocity.X) < 1) velocity = new Vector2(2, -1);
                }
                // bounce off the walls
                if (Position.X < _viewport.X + radius || Position.X > _viewport.Right - radius)
                {
                    velocity = new Vector2(-velocity.X, velocity.Y);
                    if (Position.X < _viewport.X + radius) Position = new Vector2(_viewport.X + radius, Position.Y);
                    else Position = new Vector2(_viewport.Right - radius, Position.Y);
                    // increase x velocity by a random amount
                    velocity = new Vector2(velocity.X * (((float)rand.NextDouble()*0.01f)+1f), velocity.Y);
                }
                // bounce off the ceiling
                if (Position.Y < _viewport.Y + radius)
                {
                    velocity = new Vector2(velocity.X, -velocity.Y);
                    Position = new Vector2(Position.X, _viewport.Y + radius);
                    // increase y velocity by a random amount
                    velocity = new Vector2(velocity.X, velocity.Y * (((float)rand.NextDouble() * 0.01f)+1f));
                }
                // if we hit the floor, we lose
                if (Position.Y > _viewport.Bottom - radius && BreakoutBoard.inGame) 
                { 
                    BreakoutBoard.inGame = false;
                    BreakoutBoard.lives--;
                    if (BreakoutBoard.lives <= 0) BreakoutBoard.gameOver = true;
                }
                return base.ToMySprite(_viewport);
            }
        }
        //----------------------------------------------------------------------
    }
}
