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
        // Brick
        //----------------------------------------------------------------------
        public class Brick : ScreenSprite, ICanCollide
        {
            public static Vector2 size = new Vector2(40f, 20f);
            public static Color unbreakable = Color.White;
            public static Color[] colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple, unbreakable };
            int color;
            public Brick(Vector2 position, int color) : base(ScreenSpriteAnchor.TopLeft, position, 0f, size, colors[color % colors.Length], "", "SquareSimple", TextAlignment.LEFT,SpriteType.TEXTURE)
            {
                this.color = color;
            }
            public bool CollidesWith(ICanCollide ball)
            {
                bool hit = false;
                ICanMove moveable = ball as ICanMove;
                if(ball.Position.X > Position.X && ball.Position.X < Position.X + size.X)
                {
                    // we're in the x range of the brick, check the y range
                    if (ball.Position.Y < Position.Y)
                    {
                        // check for intesection with top
                        if (ball.Position.Y + ball.Size.Y / 2f > Position.Y - size.Y / 2f)
                        {
                            hit = true;
                            ball.Position = new Vector2(ball.Position.X, Position.Y - size.Y / 2f - ball.Size.Y / 2f);
                            moveable.Velocity = new Vector2(moveable.Velocity.X, -moveable.Velocity.Y);
                        }
                    }
                    else
                    {
                        // check for intersection with bottom
                        if (ball.Position.Y - ball.Size.Y / 2f < Position.Y + size.Y / 2f)
                        {
                            hit = true;
                            ball.Position = new Vector2(ball.Position.X, Position.Y + size.Y / 2f + ball.Size.Y / 2f);
                            moveable.Velocity = new Vector2(moveable.Velocity.X, -moveable.Velocity.Y);
                        }
                    }
                }
                else if(ball.Position.Y > Position.Y - size.Y/2f && ball.Position.Y < Position.Y + size.Y/2f)
                {
                    // we're in the y range of the brick, check the x range
                    if (ball.Position.X < Position.X)
                    {
                        // check for intesection with left
                        if (ball.Position.X + ball.Size.X / 2f > Position.X - size.X / 2f)
                        {
                            hit = true;
                            ball.Position = new Vector2(Position.X - size.X / 2f - ball.Size.X / 2f, ball.Position.Y);
                            moveable.Velocity = new Vector2(-moveable.Velocity.X, moveable.Velocity.Y);
                        }
                    }
                    else
                    {
                        // check for intersection with right
                        if (ball.Position.X - ball.Size.X / 2f < Position.X + size.X / 2f)
                        {
                            hit = true;
                            ball.Position = new Vector2(Position.X + size.X / 2f + ball.Size.X / 2f, ball.Position.Y);
                            moveable.Velocity = new Vector2(-moveable.Velocity.X, moveable.Velocity.Y);
                        }
                    }
                }
                else
                {
                    // check for intersection with corners
                    if (Vector2.Distance(ball.Position, Position) < ball.Size.X / 2f)
                    {
                        hit = true;
                        //ball.Position = Position + new Vector2(size.X / 2f, size.Y / 2f) + new Vector2(ball.Size.X / 2f, ball.Size.Y / 2f);
                        //moveable.Velocity = new Vector2(-moveable.Velocity.X, -moveable.Velocity.Y);
                    }
                    else if (Vector2.Distance(ball.Position, Position + new Vector2(size.X, 0)) < ball.Size.X / 2f)
                    {
                        hit = true;
                        //ball.Position = Position + new Vector2(size.X / 2f, -size.Y / 2f) + new Vector2(ball.Size.X / 2f, -ball.Size.Y / 2f);
                        //moveable.Velocity = new Vector2(-moveable.Velocity.X, -moveable.Velocity.Y);
                    }
                    else if (Vector2.Distance(ball.Position, Position + new Vector2(0, size.Y)) < ball.Size.X / 2f)
                    {
                        hit = true;
                        //ball.Position = Position + new Vector2(-size.X / 2f, size.Y / 2f) + new Vector2(-ball.Size.X / 2f, ball.Size.Y / 2f);
                        //moveable.Velocity = new Vector2(-moveable.Velocity.X, -moveable.Velocity.Y);
                    }
                    else if (Vector2.Distance(ball.Position, Position + size) < ball.Size.X / 2f)
                    {
                        hit = true;
                        //ball.Position = Position + new Vector2(-size.X / 2f, -size.Y / 2f) + new Vector2(-ball.Size.X / 2f, -ball.Size.Y / 2f);
                        //moveable.Velocity = new Vector2(-moveable.Velocity.X, -moveable.Velocity.Y);
                    }
                }
                if(hit) Collide(ball);
                return hit;
            }
            public void Collide(ICanCollide ball)
            {
                BreakoutBoard.score++;
                if(Color != unbreakable) color--;
                else color = colors.Length-1;
                if (color < 0) { Visible = false; BreakoutBoard.score += 10; }
                else { Color = colors[color % colors.Length]; }
            }
        }
        //----------------------------------------------------------------------
    }
}
