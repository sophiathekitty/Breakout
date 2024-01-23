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
        public class BreakoutBoard : Screen
        {
            GameInput input;
            public static int score = 0;
            public static int lives = 3;
            public static bool inGame = false;
            public static bool gameOver = false;    
            Paddle paddle;
            Ball ball;
            List<Brick> bricks = new List<Brick>();
            ScreenSprite scoreSprite;
            List<ScreenSprite> livesSprites = new List<ScreenSprite>();
            ScreenSprite gameOverSprite;
            ScreenSprite title;
            int level = 0;
            string[] brickLayouts = {
                "00000000000" +
                "00000000000" +
                "66666666666" +
                "55555555555" +
                "44444444444" +
                "33333333333",
                "66600000666" +
                "66600000666" +
                "00005550000" +
                "00005550000" +
                "33300000333" +
                "33300000333" +
                "00004440000" +
                "00004440000",
                "66006660066" +
                "66006660066" +
                "00000000000" +
                "00664446600" +
                "00664446600" +
                "00000000000" +
                "66006660066" +
                "66006660066" +
                "00000000000" +
                "00664446600" +
                "00664446600",
                "66066066066" +
                "66066066066" +
                "00113331100" +
                "55455055455" +
                "55455055455" +
                "00770007700" +
                "33443334433",
                "06606660660" +
                "06606660660" +
                "06606760660" +
                "23456665432" +
                "23456765432" +
                "23756665732" +
                "65432723456" +
                "65732123756" +
                "55566666555",
                "67676767676" +
                "66666666666" +
                "67676767676" +
                "66666666666" +
                "67676767676" +
                "66666666666" +
                "67676767676" +
                "66666666666" +
                "67676767676" +
                "66666666666" +
                "67676767676" +
                "66666666666" +
                "66666666666"};

            public BreakoutBoard() : base(GridBlocks.GetTextSurface("main display"))
            {
                BackgroundColor = Color.Black;
                input = new GameInput(GridBlocks.GetPlayer());
                paddle = new Paddle(new Vector2(Size.X/2f, Size.Y - 20), new Vector2(50f, 5f), Color.White, input);
                AddSprite(paddle);
                ball = new Ball(new Vector2(Size.X / 2f, Size.Y / 2f), Color.White,paddle);
                AddSprite(ball);
                scoreSprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopLeft, new Vector2(10, 10), 2f, new Vector2(100, 20), Color.White, "Monospace", "0", TextAlignment.LEFT, SpriteType.TEXT);
                AddSprite(scoreSprite);
                for (int i = 0; i < lives; i++)
                {
                    ScreenSprite life = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.TopRight, new Vector2(-30 - i * 30, 10), 1.5f, new Vector2(20, 20), Color.White, "Monospace", "♥", TextAlignment.LEFT, SpriteType.TEXT);
                    AddSprite(life);
                    livesSprites.Add(life);
                }
                gameOverSprite = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.Center, Vector2.Zero, 2.5f, Vector2.Zero, Color.White, "Monospace", "GAME OVER", TextAlignment.CENTER, SpriteType.TEXT);
                AddSprite(gameOverSprite);
                gameOverSprite.Visible = false;
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        Brick brick = new Brick(new Vector2(10 + j * (Brick.size.X + 5), 100 + i * (Brick.size.Y + 5)), i % 6);
                        bricks.Add(brick);
                        AddSprite(brick);
                    }
                }
                title = new ScreenSprite(ScreenSprite.ScreenSpriteAnchor.Center, Vector2.Zero, 2.5f, Vector2.Zero, Color.White, "Monospace", "BREAKOUT", TextAlignment.CENTER, SpriteType.TEXT);
                AddSprite(title);
                title.Visible = false;
                ResetBricks();
            }
            void ResetBricks()
            {
                for(int i = 0; i < bricks.Count; i++)
                {
                    if(i >= brickLayouts[level].Length) bricks[i].Visible = false;
                    else
                    {
                        if (brickLayouts[level][i] == '0') bricks[i].Visible = false;
                        else
                        {
                            bricks[i].Visible = true;
                            bricks[i].Color = Brick.colors[brickLayouts[level][i] - '1'];
                        }
                    }
                }
                RemoveSprite(ball);
                AddSprite(ball);
                RemoveSprite(paddle);
                AddSprite(paddle);
                RemoveSprite(scoreSprite);
                AddSprite(scoreSprite);
                RemoveSprite(gameOverSprite);
                AddSprite(gameOverSprite);
                gameOver = false;
                gameOverSprite.Visible = false;
            }
            Random rnd = new Random();
            void RunGame()
            {
                title.Visible = false;
                scoreSprite.Visible = true;
                int blocksLeft = 0;
                ball.CollidesWith(paddle);
                if (ball.Velocity.Length() > 5) ball.Velocity *= 0.99f;
                foreach (Brick brick in bricks)
                {
                    if (brick.Visible)
                    {
                        if (brick.Color != Brick.unbreakable) blocksLeft++;
                        ball.CollidesWith(brick);
                    }
                }
                scoreSprite.Data = score.ToString();
                if (blocksLeft == 0)
                {
                    level++;
                    if (level >= brickLayouts.Length) level = 0;
                    ResetBricks();
                    lives++;
                    inGame = false;
                    if (lives > 3) lives = 3;
                }
                for (int i = 0; i < livesSprites.Count; i++)
                {
                    if (i < lives) livesSprites[i].Visible = true;
                    else livesSprites[i].Visible = false;
                }
                if (gameOver)
                {
                    ball.Visible = false;
                    paddle.Visible = false;
                    gameOverSprite.Visible = true;
                    if (input.C)
                    {
                        gameOver = false;
                        score = 0;
                        lives = 3;
                        level = 0;
                        ResetBricks();
                        ball.Visible = true;
                        paddle.Visible = true;
                        gameOverSprite.Visible = false;
                        inGame = false;
                    }
                }
            }
            bool playerPresent = false;
            public override void Draw()
            {
                if (input.PlayerPresent)
                {
                    if (!playerPresent)
                    {
                        playerPresent = true;
                        inGame = false;
                        ball.Visible = true;
                        paddle.Visible = true;

                    }
                    RunGame();
                }
                else
                {
                    if (playerPresent)
                    {
                        level = 0;
                        score = 0;
                        lives = 3;
                        ResetBricks();
                    }
                    playerPresent = false;
                    title.Visible = true;
                    scoreSprite.Visible = false;
                    ball.Visible = false;
                    paddle.Visible = false;
                    gameOverSprite.Visible = false;
                    for (int i = 0; i < livesSprites.Count; i++)
                    {
                        livesSprites[i].Visible = false;
                    }
                }
                base.Draw();
            }
        }
    }
}
