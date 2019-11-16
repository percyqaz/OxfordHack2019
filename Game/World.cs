using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace Game
{
    class World
    {
        //world variables
        TileSet.TileType[,] Tiles;
        float[] SurfaceNoise;
        int Camera = 0;
        int Depth_Dug = 0;

        //used in world gen
        Random Rand = new Random();

        //player variables
        int Player_X = 10;
        float Player_Y = 1;
        float PlayerVel = 0;
        int MiningPower = 10;
        int Money = 0;
        int Score = 0;

        //variables for mining blocks
        int miningProgress;
        float percentProgress;
        bool mining;
        int miningTargetX;
        int miningTargetY;


        public World()
        {
            SurfaceNoise = SimplexNoise.Noise.Calc1D(Screen.WIDTH, 0.2f);
            var r = new Random();
            Tiles = new TileSet.TileType[Screen.WIDTH, 100];
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = GenTile(x, y);
                }
            }
        }

        public void Update()
        {
            //gravity
            Player_Y += PlayerVel;

            int Left = Player_X - 1;
            int Top = Round(Player_Y - 0.5f);
            int Bottom = Round(Player_Y + 1f);
            int Right = Player_X + 1;

            //horizontal movement
            if (!(Collision(Left, Top) || Collision(Left, Top + 1) ||
                Collision(Left, Bottom - 1) || Collision(Left, Bottom))
                && Keyboard.IsKeyDown(Key.Left))
                Player_X -= 1;
            else if (!(Collision(Right, Top) || Collision(Right, Top + 1) ||
                Collision(Right, Bottom - 1) || Collision(Right, Bottom))
                && Keyboard.IsKeyDown(Key.Right))
                Player_X += 1;

            //vertical movement
            if (Collision(Player_X, Top)) { PlayerVel = 0; Player_Y += 0.5f; }
            if (Collision(Player_X, Bottom))
            {
                PlayerVel = 0; Player_Y -= 0.5f;
                if (Keyboard.IsKeyDown(Key.Up))
                    PlayerVel = -0.8f;
            }
            else
            {
                PlayerVel += 0.08f;
            }
            if (Player_Y > 15 + Camera) Camera += 1;
            
            mining = Keyboard.IsKeyDown(Key.Right) ?
                (CanMine(Player_X + 1, Top + 1) ? SetTarget(Player_X + 1, Top + 1) :
                (CanMine(Player_X + 1, Bottom - 1) ? SetTarget(Player_X + 1, Bottom - 1) : false))
            : (

            Keyboard.IsKeyDown(Key.Left) ?
                (CanMine(Player_X - 1, Top + 1) ? SetTarget(Player_X - 1, Top + 1) :
                (CanMine(Player_X - 1, Bottom - 1) ? SetTarget(Player_X - 1, Bottom - 1) : false))
                : (CanMine(Player_X, Bottom) ? SetTarget(Player_X, Bottom) : false));

            if (Keyboard.IsKeyDown(Key.Space))
            {
                int needed = TileSet.Tile(Tiles[miningTargetX, miningTargetY]).hitsNeeded(miningTargetY + Depth_Dug);
                miningProgress += MiningPower;
                percentProgress = (float)miningProgress / needed;
                if (percentProgress >= 1)
                {
                    Money += TileSet.Tile(Tiles[miningTargetX, miningTargetY]).value;
                    Score += TileSet.Tile(Tiles[miningTargetX, miningTargetY]).value;
                    Tiles[miningTargetX, miningTargetY] = TileSet.TileType.AIR;
                }
            }

            if (Camera > 50) ExtendWorld();
        }

        public void Draw(Screen s)
        {
            //renders world
            for (int y = 0; y < Screen.HEIGHT; y++)
            {
                if (y + Camera >= 100) continue;
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    var t = TileSet.Tile(Tiles[x, y + Camera]);
                    s.WritePixel(x, y, t.c, t.col, Color.Black); //fluids replace color.black
                }
            }
            //renders player
            s.WritePixel(Player_X, Round(Player_Y) - Camera, '@', Color.White, Color.LightGreen);
            s.WritePixel(Player_X, Round(Player_Y) + 1 - Camera, '@', Color.White, Color.LightGreen);
            //renders "mining cursor"
            if (mining) s.WritePixel(miningTargetX, miningTargetY - Camera, ' ', Color.White, Color.Gray);

            //renders mining progress bar
            if (miningProgress > 0)
            {
                for (int i = 3; i < Screen.WIDTH - 3; i++)
                {
                    s.WritePixel(i, Screen.HEIGHT - 2, ((i - 3f) / (Screen.WIDTH - 6f) <= percentProgress ? '=' : '-'), Color.White, Color.Black);
                }
            }

            s.WriteText(2, 2, " Depth: " + ((int)(Depth_Dug + Player_Y - 30)).ToString() + " ", Color.LightBlue, Color.Black);
            s.WriteText(2, 3, " Money: " + Money.ToString() + " ", Color.Gold, Color.Black);
            s.WriteText(2, 4, " Score: " + Score.ToString() + " ", Color.Lime, Color.Black);
        }

        bool Collision(int x, int y)
        {
            if (x >= Screen.WIDTH || x < 0 || y < 0 || y >= 100) return true;
            return Tiles[x, y] != TileSet.TileType.AIR;
        }

        bool CanMine(int x, int y)
        {
            if (x >= Screen.WIDTH || x < 0 || y < 0 || y >= 100) return false;
            return Tiles[x, y] != TileSet.TileType.AIR;
        }

        int Round(float x)
        {
            return (int)Math.Round(x - 0.5f);
        }

        bool SetTarget(int x, int y)
        {
            if (miningTargetX != x || miningTargetY != y)
            {
                miningTargetX = x; miningTargetY = y;
                miningProgress = 0;
            }
            return true;
        }

        TileSet.TileType GenTile(int x, int y)
        {
            if (y < 15 + 0.04 * SurfaceNoise[x]) return TileSet.TileType.AIR;
            return WorldGenerator.GenTile(x, y);
        }

        void ExtendWorld()
        {
            //cut top 40 blocks off world and generate 40 new ones
            for (int y = 0; y < 60; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = Tiles[x, y + 40];
                }
            }
            Player_Y -= 40;
            Camera -= 40;
            Depth_Dug += 40;
            for (int y = 60; y < 100; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = GenTile(x, y + Depth_Dug);
                }
            }
        }
    }
}
