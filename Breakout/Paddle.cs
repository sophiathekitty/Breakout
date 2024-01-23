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
        // Paddle
        //----------------------------------------------------------------------
        public class Paddle : ScreenSprite, ICanCollide, ICanMove
        {
            GameInput input;
            public static float speed = 1.5f;
            Vector2 lastPos;
            public Vector2 Velocity { get { return (Position - lastPos); } set { return; } }
            public Paddle(Vector2 position, Vector2 size, Color color, GameInput input) : base(ScreenSpriteAnchor.TopLeft, position, 0f, size, color, "", "SquareSimple", TextAlignment.LEFT, SpriteType.TEXTURE)
            {
                this.input = input;
                lastPos = Position;
            }
            public override MySprite ToMySprite(RectangleF _viewport)
            {
                // move the paddle by the mouse (velocity not position) input along the x axis and clamp to the _viewport
                if (input.PlayerPresent && !BreakoutBoard.gameOver)
                {
                    Position = new Vector2(MathHelper.Clamp(Position.X + (input.Mouse.X * speed), _viewport.X, _viewport.Right - Size.X), Position.Y);
                    if(!BreakoutBoard.inGame && input.Space) BreakoutBoard.inGame = true;
                }
                if(lastPos != Position) lastPos = Position;
                return base.ToMySprite(_viewport);
            }

            public bool CollidesWith(ICanCollide ball)
            {
                // if the ball's center is between the left and right edges of the paddle and the ball's bottom edge is above the paddle's top edge (paddle origin is topleft, ball's is center)
                bool hit = false;
                ICanMove moveable = ball as ICanMove;
                if (ball.Position.X > Position.X && ball.Position.X < Position.X + Size.X && ball.Position.Y + ball.Size.Y / 2f > Position.Y - Size.Y / 2f)
                {
                    hit = true;
                    ball.Position = new Vector2(ball.Position.X, Position.Y - Size.Y / 2f - ball.Size.Y / 2f)+Velocity;
                    moveable.Velocity = new Vector2(moveable.Velocity.X, -moveable.Velocity.Y);
                }
                if (Vector2.Distance(ball.Position, Position) < ball.Size.X / 2f)
                {
                    hit = true;
                    ball.Position = new Vector2(ball.Position.X, Position.Y);
                    moveable.Velocity = new Vector2(-moveable.Velocity.X, -Math.Abs(moveable.Velocity.Y))+ Velocity;
                }
                if (Vector2.Distance(ball.Position, Position + new Vector2(Size.X, 0)) < ball.Size.X / 2f)
                {
                    hit = true;
                    ball.Position = new Vector2(ball.Position.X, Position.Y);
                    moveable.Velocity = new Vector2(-moveable.Velocity.X, -Math.Abs(moveable.Velocity.Y)) + Velocity;
                }
                if (hit) Collide(ball);
                return hit;
            }

            public void Collide(ICanCollide ball)
            {
            }
        }
        //----------------------------------------------------------------------
    }
}
