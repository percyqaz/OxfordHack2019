﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace Game
{
    class World
    {
        //world variables
        TileSet.TileType[,] Tiles;
        int[,] Fluids;
        float[] SurfaceNoise;
        int Camera = 0;
        int Depth_Dug = 0;
        Particle[] Particles;

        //used in world gen
        Random Rand = new Random();

        //player variables
        int Player_X = 10;
        float Player_Y = 5;
        float PlayerVel = 0;
        int MiningPower = 50;
        int Money = 0;
        int Score = 0;
        int Dynamite = 3;
        int MiningPrice = 150;
        int DynamitePrice = 200;
        int Health = 100;
        int InvFrames = 0;
        string DamageSource = "IMMORTALITY";
        Dictionary<TileSet.TileType, int> BlockTracker = new Dictionary<TileSet.TileType, int>();

        public bool Game_Over => Health == 0 && InvFrames == 0;

        //variables for mining blocks
        int miningProgress;
        float percentProgress;
        bool mining;
        int miningTargetX;
        int miningTargetY;

        //dynamite variables
        int Dynamite_X;
        int Dynamite_Y;
        int Dynamite_Fuse;

        int CaveIn = -40;


        public World(bool dev)
        {
            var r = new Random();
            if (dev) //make everything OP
            {
                Money = int.MaxValue; Dynamite = int.MaxValue; MiningPower = int.MaxValue; InvFrames = int.MaxValue;
            }
            else //use a random seed so the world isnt consistent every time
            {
                long seed = DateTime.Now.ToBinary();
                r = new Random((int)seed);
                SimplexNoise.Noise.Seed = (int)seed;
            }
            SurfaceNoise = SimplexNoise.Noise.Calc1D(Screen.WIDTH, 0.2f);
            Tiles = new TileSet.TileType[Screen.WIDTH, 100];
            Fluids = new int[Screen.WIDTH, 100];
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = GenTile(x, y);
                    if (!BlockFlow(x, y)) Fluids[x, y] = WorldGenerator.GenFluid(x, y);
                }
            }

            Particles = new Particle[80];
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i] = new Particle();
            }
        }

        public void Update()
        {
            PhysicsPass();
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
                PlayerVel = Math.Min(0.9f, PlayerVel + 0.08f);
            }
            if (Player_Y > 15 + Camera) Camera += 1;

            //handle mining blocks
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
                    BlockMinedStat(Tiles[miningTargetX, miningTargetY]);
                    Tiles[miningTargetX, miningTargetY] = TileSet.TileType.AIR;
                }
            }

            //dynamite logic
            if (Dynamite_Fuse > 0)
            {
                Dynamite_Fuse -= 3;
                if (Dynamite_Fuse % 5 == 0)
                {
                    GetParticle()?.LinearParticle(15, Dynamite_X, Dynamite_Y, 0, -0.5f, Color.Red, Color.White);
                }
                if (Dynamite_Fuse <= 0)
                {
                    Explosion(Dynamite_X, Dynamite_Y);
                }
            }
            else if (Dynamite > 0 && Keyboard.IsKeyDown(Key.Z))
            {
                Dynamite -= 1;
                Dynamite_Fuse = 255;
                Dynamite_X = Player_X;
                Dynamite_Y = Round(Player_Y + 1);
            }

            //hp stuff
            if (InvFrames > 0) InvFrames -= 1;
            if (!BlockFlow(Player_X, Bottom - 1) && Fluids[Player_X, Bottom - 1] < -3) Hurt(25, "LAVA");
            if (CanMine(Player_X, Top) && Tiles[Player_X, Top] == TileSet.TileType.GRAVEL) Hurt(25, "FALLING GRAVEL");
            if (Rand.Next(100) == 5) Health = Math.Min(100, Health + 1);

            if (InvFrames > 500 && Keyboard.IsKeyDown(Key.D0)) //only possible in dev mode
            {
                Player_Y += 40;
                Camera += 40;
            }

            if (Rand.Next(100) < 8) CaveIn = Math.Max(CaveIn + 1, Round(Player_Y) + Depth_Dug - 40);
            if (Rand.Next(100) < 16) GetParticle()?.LinearParticle(20, Rand.Next(Screen.WIDTH), Math.Min(Camera, CaveIn), 0, 1f, Color.White, Color.Gray);
            if (CaveIn >= Player_Y + Depth_Dug)
            {
                Cave_In();
                CaveIn -= 20;
            }

            //infinite world depth simulator
            if (Camera > 50) ExtendWorld();
        }

        public void Draw(Screen s)
        {
            if (Keyboard.IsKeyDown(Key.Tab)) { Shop(s); }

            //renders world
            for (int y = 0; y < Screen.HEIGHT; y++)
            {
                if (y + Camera >= 100) continue;
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    var t = TileSet.Tile(Tiles[x, y + Camera]);
                    s.WritePixel(x, y, t.c, t.col, FluidColor(Fluids[x, y + Camera]));
                }
            }
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i].lifespan > 0)
                {
                    s.WritePixel((int)Particles[i].x(Particles[i].age), (int)Particles[i].y(Particles[i].age) - Camera, '*', Particles[i].col(Particles[i].age), Color.Black);
                    Particles[i].lifespan -= 1;
                    Particles[i].age += 1;
                }
            }
            //renders player
            s.WritePixel(Player_X, Round(Player_Y) - Camera, '@', Color.White, Color.DarkGreen);
            s.WritePixel(Player_X, Round(Player_Y) + 1 - Camera, '@', Color.White, Color.DarkGreen);
            //renders "mining cursor"
            if (mining) s.WritePixel(miningTargetX, miningTargetY - Camera, ' ', Color.White, Color.Gray);
            //renders dynamite
            if (Dynamite_Fuse > 0) s.WritePixel(Dynamite_X, Dynamite_Y - Camera, '%', Color.FromArgb(Dynamite_Fuse, 0, 0), Color.FromArgb(255 - Dynamite_Fuse, 0, 0));

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

            s.WriteAlignText(Screen.WIDTH - 3, 2, " Health: " + Health.ToString() + " ", InvFrames > 0 ? Color.Yellow : Color.Red, Color.Black);
            if (InvFrames > 0) s.WriteAlignText(Screen.WIDTH - 3, 3, " HURT BY " + DamageSource + " ", Color.Red, Color.Black);
            s.WriteAlignText(Screen.WIDTH - 3, 4, " Dynamite: " + Dynamite.ToString() + " ", Color.Red, Color.Black);
            if (CaveIn + 10 >= Player_Y + Depth_Dug)
            {
                s.WriteText(Screen.WIDTH / 2 - 11, Screen.HEIGHT - 3, "YOU HEAR RUMBLING. RUN.", Color.White, Color.Maroon);
            }
            else if (CaveIn + 20 >= Player_Y + Depth_Dug)
            {
                s.WriteText(Screen.WIDTH / 2 - 9, Screen.HEIGHT - 3, "You hear rumbling.", Color.White, Color.Black);
            }

            while (Health == 0 && InvFrames > 0)
            {
                s.WriteText(Rand.Next(Screen.WIDTH), Rand.Next(Screen.HEIGHT), "GAME OVER", Color.Black, Color.Fuchsia);
                InvFrames -= 1;
                if (InvFrames == 0)
                {
                    UI.StatsScreen(Score, (int)(Depth_Dug + Player_Y - 30), BlockTracker, s);
                }
            }
        }

        Particle GetParticle()
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                if (Particles[i].lifespan == 0) return Particles[i];
            }
            return null;
        }

        void Explosion(int x, int y)
        {
            for (int i = 0; i < 36; i++)
            {
                double a = i * Math.PI / 18;
                ExplosionRay(x, y, (float)Math.Sin(a), (float)Math.Cos(a), 20, 10);
            }
            if (Math.Pow(Player_X - x, 2) + Math.Pow(Player_Y - y, 2) <= 144) Hurt(50, "EXPLOSION");
            CaveIn += 15;
        }

        void ExplosionRay(float x, float y, float vx, float vy, int r, int gr)
        {
            GetParticle()?.LinearParticle(gr/2, (int)x, (int)y, vx*2, vy*2, Color.FromArgb(255, Rand.Next(255), 0), Color.FromArgb(Rand.Next(127), 0, 0));
            GetParticle()?.LinearParticle(gr/2, (int)x, (int)y, vx, vy, Color.White, Color.Black);
            for (int i = 0; i < r; i++)
            {
                int tx = Round(x); int ty = Round(y);
                if (CanMine(tx, ty))
                {
                    if (gr > 0) //only reward player explosives, not cave-ins
                    {
                        int v = TileSet.Tile(Tiles[tx, ty]).value / 2;
                        Money += v; Score += v;
                        BlockMinedStat(Tiles[tx, ty]);
                    }
                    Tiles[tx, ty] = i < gr ? TileSet.TileType.AIR : TileSet.TileType.GRAVEL;
                }
                x += vx;
                y += vy;
            }
        }

        void Cave_In()
        {
            for (int i = 0; i < 18; i++)
            {
                double a = i * Math.PI / 18;
                ExplosionRay(Player_X, Round(Player_Y), (float)Math.Cos(a), -(float)Math.Sin(a), 50, 0);
            }
        }

        void PhysicsPass()
        {
            void Liquid(int x, int y)
            {
                if (Fluids[x, y] < -5 && Rand.Next(150) == 5) GetParticle()?.FireParticle(x, y); 
                if (!BlockFlow(x, y + 1))
                {
                    MixFluids(x, y, x, y + 1, true);
                }
                if (!BlockFlow(x - 1, y))
                {
                    MixFluids(x - 1, y, x, y, false);
                }
                if (!BlockFlow(x + 1, y))
                {
                    MixFluids(x + 1, y, x, y, false);
                }
            }
            void Gravel(int x, int y)
            {
                if (!Collision(x, y + 1)) { Tiles[x, y + 1] = TileSet.TileType.GRAVEL; Tiles[x, y] = TileSet.TileType.AIR; }
                else if (!Collision(x - 1, y + 1)) { Tiles[x - 1, y + 1] = TileSet.TileType.GRAVEL; Tiles[x, y] = TileSet.TileType.AIR; }
                else if (!Collision(x + 2, y + 1)) { Tiles[x + 2, y + 1] = TileSet.TileType.GRAVEL; Tiles[x, y] = TileSet.TileType.AIR; }
            }
            for (int y = Camera + Screen.HEIGHT - 1; y >= Camera; y--)
            {
                if (y >= 100) continue;
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    if (Tiles[x, y] == TileSet.TileType.GRAVEL) Gravel(x, y);
                    if (Fluids[x, y] != 0) Liquid(x, y);
                }
            }
        }

        bool Collision(int x, int y)
        {
            if (x >= Screen.WIDTH || x < 0 || y < 0 || y >= 100) return true;
            return Tiles[x, y] != TileSet.TileType.AIR;
        }

        bool BlockFlow(int x, int y)
        {
            if (x >= Screen.WIDTH || x < 0 || y < 0 || y >= 100) return true;
            return !TileSet.Tile(Tiles[x, y]).porous;
        }

        bool CanMine(int x, int y)
        {
            if (x >= Screen.WIDTH || x < 0 || y < 0 || y >= 100) return false;
            return Tiles[x, y] != TileSet.TileType.AIR;
        }

        void Hurt(int amount, string reason)
        {
            if (InvFrames == 0)
            {
                DamageSource = reason;
                Health -= amount;
                InvFrames = 40;
                if (Health <= 0)
                {
                    Health = 0;
                    InvFrames = 100;
                    Console.Beep(100, 1000);
                }
                else
                {
                    Console.Beep(200, 500);
                }
            }
        }

        void MixFluids(int x1, int y1, int x2, int y2, bool downwards)
        {
            int totalVal = Fluids[x1, y1] + Fluids[x2, y2];
            if (Math.Abs(Fluids[x1, y1]) > Math.Abs(totalVal))
            {
                Tiles[x1, y1] = TileSet.TileType.OBSIDIAN;
                Tiles[x2, y2] = TileSet.TileType.OBSIDIAN;
                Fluids[x1, y1] = 0;
                Fluids[x2, y2] = 0;
                GetParticle()?.LinearParticle(20, x1, y1, 0, -0.75f, Color.Purple, Color.White);
                GetParticle()?.LinearParticle(20, x2, y2, 0, -0.75f, Color.Purple, Color.White);
            }
            else
            {
                if (downwards)
                {
                    int abs = Math.Abs(totalVal);
                    int sgn = totalVal / abs;
                    Fluids[x1, y1] = sgn * Math.Max(0, abs - 10);
                    Fluids[x2, y2] = sgn * Math.Min(10, abs);
                }
                else
                {
                    int split = totalVal - (int)Math.Floor(totalVal / 2f); //flooring makes lava and water slightly different
                    Fluids[x1, y1] = split;
                    Fluids[x2, y2] = totalVal - split;
                }
            }
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
                    Fluids[x, y] = Fluids[x, y + 40];
                }
            }
            Player_Y -= 40;
            Camera -= 40;
            Dynamite_Y -= 40;
            Depth_Dug += 40;
            for (int y = 60; y < 100; y++)
            {
                for (int x = 0; x < Screen.WIDTH; x++)
                {
                    Tiles[x, y] = GenTile(x, y + Depth_Dug);
                    if (!BlockFlow(x, y)) Fluids[x, y] = WorldGenerator.GenFluid(x, y + Depth_Dug);
                }
            }
        }

        Color FluidColor(int x)
        {
            if (x == 0) return Color.Black;
            int a = Math.Abs(x) * 25;
            return x > 0 ? Color.FromArgb(0, 0, a) : Color.FromArgb(a, 0, 0);
        }

        void Shop(Screen s)
        {
            UI.Menu("Item Shop", new List<Tuple<Func<string>, Action>>() {
                new Tuple<Func<string>, Action>(() => "Your Money:  "+Money.ToString(), () => { }),
                new Tuple<Func<string>, Action>(() => "Upgrade Mining Power ("+MiningPower.ToString()+") - Costs "+MiningPrice.ToString(), () => {
                    if (Money >= MiningPrice)
                    {
                        MiningPower *= 2;
                        Money -= MiningPrice;
                        MiningPrice  = (int)(MiningPrice * 1.5f);
                    }
                }),
                new Tuple<Func<string>, Action>(() => "Buy Dynamite ("+Dynamite.ToString()+") - Costs "+DynamitePrice.ToString(), () => {
                    if (Money >= DynamitePrice)
                    {
                        Dynamite += 1;
                        Money -= DynamitePrice;
                        DynamitePrice += 200;
                    }
                })
            }, s);
        }

        void BlockMinedStat(TileSet.TileType t)
        {
            if (t == TileSet.TileType.AIR) return; //no way im gonna fix this bug properly bro its 3am
            if (!BlockTracker.ContainsKey(t))
            {
                BlockTracker.Add(t, 0);
            }
            BlockTracker[t] += 1;
        }
    }
}
