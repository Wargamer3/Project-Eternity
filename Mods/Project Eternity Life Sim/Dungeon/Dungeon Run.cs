using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum DungeonPersonKind
    {
        Monster,
        Explorer,
        Captive,
    }

    public class DungeonSpawn
    {
        public int X;
        public int Y;
        public DungeonPersonKind Kind;
        public string Name;
        public string Model;
        public int HP = 12;
        public int Purse;
        public string Drop;
        public bool Paid;
        public bool Freed;
        public LifeSkill Trade;
        public PlayerCharacter Body;
    }

    public class PartyMember
    {
        public string Id;
        public string Name;
        public int Seat = -1;
        public bool InParty;
        public bool Resident;
        public LifeSkill Trade;
        public string Tool;
        public PlayerCharacter Body;
    }

    public enum JoinKind
    {
        PickNew,
        TakeExisting,
        Wait,
        Ready,
    }

    public class DungeonParty
    {
        public const int Cap = 4;

        public bool Started;
        public readonly List<PartyMember> Roster = new List<PartyMember>();

        public int PartyCount()
        {
            int Count = 0;
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].InParty)
                {
                    Count++;
                }
            }

            return Count;
        }

        public int Uncontrolled()
        {
            int Count = 0;
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].Seat < 0)
                {
                    Count++;
                }
            }

            return Count;
        }

        public PartyMember Lead()
        {
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].InParty)
                {
                    return Roster[i];
                }
            }

            return null;
        }

        public PartyMember Find(string Id)
        {
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].Id == Id)
                {
                    return Roster[i];
                }
            }

            return null;
        }

        public PartyMember BySeat(int Seat)
        {
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].Seat == Seat)
                {
                    return Roster[i];
                }
            }

            return null;
        }

        public static JoinKind Kind(bool Started, bool Seated, int Uncontrolled)
        {
            if (Seated)
            {
                return JoinKind.Ready;
            }

            if (!Started)
            {
                return JoinKind.PickNew;
            }

            if (Uncontrolled > 0)
            {
                return JoinKind.TakeExisting;
            }

            return JoinKind.Wait;
        }

        public void Note(int SeatsPresent)
        {
            int Picked = 0;
            for (int i = 0; i < Roster.Count; i++)
            {
                if (Roster[i].Seat >= 0)
                {
                    Picked++;
                }
            }

            if (!Started && Picked >= SeatsPresent && Picked > 0)
            {
                Started = true;
            }
        }

        public PartyMember Pick(int Seat, string Id, string Name)
        {
            if (Started || string.IsNullOrEmpty(Id) || BySeat(Seat) != null || Find(Id) != null || PartyCount() >= Cap)
            {
                return null;
            }

            PartyMember Who = new PartyMember();
            Who.Id = Id;
            Who.Name = string.IsNullOrEmpty(Name) ? Id : Name;
            Who.Seat = Seat;
            Who.InParty = true;
            Roster.Add(Who);
            return Who;
        }

        public bool LateJoin(int Seat, string Id)
        {
            if (!Started || BySeat(Seat) != null)
            {
                return false;
            }

            PartyMember Who = Find(Id);
            if (Who == null || Who.Seat >= 0)
            {
                return false;
            }

            Who.Seat = Seat;
            return true;
        }

        public PartyMember Leave(int Seat)
        {
            PartyMember Who = BySeat(Seat);
            if (Who == null)
            {
                return null;
            }

            Who.Seat = -1;
            return Who;
        }

        public bool Recruit(string Id, string Name, out bool SentHome)
        {
            return Recruit(Id, Name, LifeSkill.None, out SentHome);
        }

        public bool Recruit(string Id, string Name, LifeSkill Trade, out bool SentHome)
        {
            SentHome = false;
            if (string.IsNullOrEmpty(Id) || Find(Id) != null)
            {
                return false;
            }

            PartyMember Who = new PartyMember();
            Who.Id = Id;
            Who.Name = string.IsNullOrEmpty(Name) ? Id : Name;
            Who.Resident = true;
            Who.Trade = Trade;
            Who.Tool = Trades.Starter(Trade);
            if (PartyCount() < Cap)
            {
                Who.InParty = true;
                Roster.Add(Who);
                return true;
            }

            SentHome = true;
            Who.InParty = false;
            Roster.Add(Who);
            return false;
        }

        public bool InviteHome(string Id, string Name)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return false;
            }

            PartyMember Who = Find(Id);
            if (Who == null)
            {
                Who = new PartyMember();
                Who.Id = Id;
                Who.Name = string.IsNullOrEmpty(Name) ? Id : Name;
                Roster.Add(Who);
            }

            Who.Resident = true;
            return true;
        }
    }

    public static class DungeonView
    {
        public const float Cell = 32f;
        public const float Rise = 32f;

        public static Vector3 Spot(int X, int Y, int Height)
        {
            return new Vector3((X + 0.5f) * Cell, Height * Rise, (Y + 0.5f) * Cell);
        }

        public static void Frame(Camera3D Camera, DungeonRun Run, string Id)
        {
            if (Camera == null || Run == null || string.IsNullOrEmpty(Id))
            {
                return;
            }

            int X;
            int Y;
            if (!Run.CellX.TryGetValue(Id, out X) || !Run.CellY.TryGetValue(Id, out Y))
            {
                return;
            }

            Run.Settle(Id);
            float Xf;
            float Yf;
            Vector3 Focus = Spot(X, Y, 0);
            if (Run.PosX.TryGetValue(Id, out Xf) && Run.PosY.TryGetValue(Id, out Yf))
            {
                Focus = new Vector3(Xf, 0f, Yf);
            }

            Focus.Y = 20f;
            LifeSimFreeCamera.Frame(Camera, Focus, 160f);
        }
    }

    public class DungeonRun
    {
        public int Seed = 1;
        public int FloorCount = 8;
        public int Width = 32;
        public int Height = 32;
        public int Wing;
        public readonly Dictionary<int, DungeonFloor> Floors = new Dictionary<int, DungeonFloor>();
        public DungeonParty Party = new DungeonParty();
        public readonly Dictionary<string, int> FloorOf = new Dictionary<string, int>();
        public readonly Dictionary<string, int> CellX = new Dictionary<string, int>();
        public readonly Dictionary<string, int> CellY = new Dictionary<string, int>();
        public readonly Dictionary<string, float> PosX = new Dictionary<string, float>();
        public readonly Dictionary<string, float> PosY = new Dictionary<string, float>();
        public readonly Dictionary<string, float> Facing = new Dictionary<string, float>();
        public readonly Dictionary<string, float> Altitude = new Dictionary<string, float>();
        public readonly Dictionary<string, float> Lift = new Dictionary<string, float>();

        public DungeonFloor Visit(int Index)
        {
            if (Index < 0 || Index >= FloorCount)
            {
                return null;
            }

            DungeonFloor Floor;
            if (Floors.TryGetValue(Index, out Floor) && Floor.Cells != null)
            {
                return Floor;
            }

            int LinkX = -1;
            int LinkY = -1;
            if (Index > 0)
            {
                DungeonFloor Below = Visit(Index - 1);
                LinkX = Below.DownX;
                LinkY = Below.DownY;
            }

            Floor = DungeonGenerator.Build(Seed, Index, FloorCount, Width, Height, LinkX, LinkY);
            DungeonDress.Fill(Floor, Seed + Index * 13, Wing);
            Floors[Index] = Floor;
            return Floor;
        }

        public HashSet<int> AwakeFloors()
        {
            HashSet<int> Awake = new HashSet<int>();
            for (int i = 0; i < Party.Roster.Count; i++)
            {
                PartyMember Who = Party.Roster[i];
                int Floor;
                if (Who.Seat < 0 || !FloorOf.TryGetValue(Who.Id, out Floor))
                {
                    continue;
                }

                if (Floor >= 0)
                {
                    Awake.Add(Floor);
                }
            }

            return Awake;
        }

        public bool Enter(string Id)
        {
            PartyMember Who = Party.Find(Id);
            if (Who == null)
            {
                return false;
            }

            DungeonFloor Ground = Visit(0);
            int X = Ground.EntryX;
            int Y = Ground.EntryY;
            DungeonCell Beside = Landing(Ground, X, Y);
            if (Beside != null)
            {
                X = Beside.X;
                Y = Beside.Y;
            }

            Place(Id, 0, X, Y);
            if (Who == Party.Lead())
            {
                BringFollowers(Who.Id, -1, 0, X, Y);
            }

            return true;
        }

        public bool Climb(string Id)
        {
            int FloorIndex;
            int X;
            int Y;
            if (!FloorOf.TryGetValue(Id, out FloorIndex) || !CellX.TryGetValue(Id, out X) || !CellY.TryGetValue(Id, out Y))
            {
                return false;
            }

            DungeonFloor Floor = Visit(FloorIndex);
            DungeonCell Cell = Floor == null ? null : Floor.At(X, Y);
            if (Cell == null)
            {
                return false;
            }

            PartyMember Who = Party.Find(Id);
            bool Lead = Who != null && Who == Party.Lead();
            if (Cell.Role == DungeonRole.StairUp && Floor.Index == 0 && Floor.EntryLeaves)
            {
                LeaveDungeon(Id);
                if (Lead)
                {
                    BringFollowers(Id, FloorIndex, -1, 0, 0);
                }

                return true;
            }

            if (Cell.Role == DungeonRole.StairDown && Floor.DownX == X && Floor.DownY == Y)
            {
                DungeonFloor Next = Visit(Floor.Index + 1);
                if (Next == null)
                {
                    return false;
                }

                Place(Id, Next.Index, Next.UpX, Next.UpY);
                if (Lead)
                {
                    BringFollowers(Id, FloorIndex, Next.Index, Next.UpX, Next.UpY);
                }

                return true;
            }

            if (Cell.Role == DungeonRole.StairUp && Floor.Index > 0)
            {
                DungeonFloor Previous = Visit(Floor.Index - 1);
                Place(Id, Previous.Index, Previous.DownX, Previous.DownY);
                if (Lead)
                {
                    BringFollowers(Id, FloorIndex, Previous.Index, Previous.DownX, Previous.DownY);
                }

                return true;
            }

            return false;
        }

        public bool OrderMove(string Id, int X, int Y)
        {
            int FloorIndex;
            int FromX;
            int FromY;
            if (!FloorOf.TryGetValue(Id, out FloorIndex) || !CellX.TryGetValue(Id, out FromX) || !CellY.TryGetValue(Id, out FromY))
            {
                return false;
            }

            DungeonFloor Floor = Visit(FloorIndex);
            if (Floor == null || !Floor.Reaches(FromX, FromY, X, Y))
            {
                return false;
            }

            Place(Id, FloorIndex, X, Y);
            return true;
        }

        public void Nudge(string Id, float WishX, float WishY, float Seconds)
        {
            Settle(Id);
            int FloorIndex;
            if (!FloorOf.TryGetValue(Id, out FloorIndex) || !PosX.ContainsKey(Id))
            {
                return;
            }

            float Length = (float)Math.Sqrt(WishX * WishX + WishY * WishY);
            if (Length > 1f)
            {
                WishX /= Length;
                WishY /= Length;
            }

            if (Length > 0.01f)
            {
                Facing[Id] = (float)Math.Atan2(WishX, WishY);
            }

            if (Seconds < 0f)
            {
                Seconds = 0f;
            }

            DungeonFloor Floor = Visit(FloorIndex);
            float Speed = 90f;
            float NextX = PosX[Id] + WishX * Speed * Seconds;
            float NextY = PosY[Id] + WishY * Speed * Seconds;
            if (Fits(Floor, NextX, PosY[Id]))
            {
                PosX[Id] = NextX;
            }

            if (Fits(Floor, PosX[Id], NextY))
            {
                PosY[Id] = NextY;
            }

            CellX[Id] = (int)Math.Floor(PosX[Id] / DungeonView.Cell);
            CellY[Id] = (int)Math.Floor(PosY[Id] / DungeonView.Cell);
        }

        public void Stride(string Id, PlayerCharacter Who, float WishX, float WishY, float Strength, float Seconds, bool Jump)
        {
            if (Who == null)
            {
                Nudge(Id, WishX, WishY, Seconds);
                return;
            }

            Settle(Id);
            int FloorIndex;
            if (!FloorOf.TryGetValue(Id, out FloorIndex) || !PosX.ContainsKey(Id))
            {
                return;
            }

            if (Seconds < 0f)
            {
                Seconds = 0f;
            }

            float Length = (float)Math.Sqrt(WishX * WishX + WishY * WishY);
            Vector3 Wish = Vector3.Zero;
            if (Length > 1f)
            {
                Wish = new Vector3(WishX / Length, WishY / Length, 0f);
            }
            else if (Length > 0.01f)
            {
                Wish = new Vector3(WishX, WishY, 0f);
            }

            DungeonFloor Floor = Visit(FloorIndex);
            float Ground = GroundAt(Floor, PosX[Id], PosY[Id]);
            float Z;
            if (!Altitude.TryGetValue(Id, out Z))
            {
                Z = Ground;
            }

            Who.Position = new Vector3(PosX[Id], PosY[Id], Z);
            Who.IgnoreTownSolids = true;
            Who.Move(Wish, Wish == Vector3.Zero ? 0d : Strength, Seconds);
            Who.IgnoreTownSolids = false;

            float NextX = Who.Position.X;
            float NextY = Who.Position.Y;
            if (!Fits(Floor, NextX, PosY[Id]))
            {
                NextX = PosX[Id];
                Who.MovementSpeed = new Vector3(0f, Who.MovementSpeed.Y, 0f);
            }

            if (!Fits(Floor, NextX, NextY))
            {
                NextY = PosY[Id];
                Who.MovementSpeed = new Vector3(Who.MovementSpeed.X, 0f, 0f);
            }

            PosX[Id] = NextX;
            PosY[Id] = NextY;
            CellX[Id] = (int)Math.Floor(PosX[Id] / DungeonView.Cell);
            CellY[Id] = (int)Math.Floor(PosY[Id] / DungeonView.Cell);
            Facing[Id] = (float)Math.Atan2(-Who.RotationMatrix.M13, Who.RotationMatrix.M33);

            Ground = GroundAt(Floor, PosX[Id], PosY[Id]);
            float Up;
            if (!Lift.TryGetValue(Id, out Up))
            {
                Up = 0f;
            }

            bool Air = Who.CurrentPose == Pose.MidAir;
            bool Supported = Z <= Ground + 2f && Up <= 4f;
            if (Jump && (!Air || Supported))
            {
                Up = 110f;
                Air = true;
                Who.CurrentPose = Pose.MidAir;
            }

            if (!Air && Z > Ground + 10f)
            {
                Air = true;
                Up = 0f;
                Who.CurrentPose = Pose.MidAir;
            }

            if (Air)
            {
                Up -= 160f * Seconds;
                Z += Up * Seconds;
                if (Z <= Ground)
                {
                    Z = Ground;
                    Up = 0f;
                    Who.CurrentPose = Pose.Stand;
                }
            }
            else
            {
                Z = Ground;
                Up = 0f;
            }

            Altitude[Id] = Z;
            Lift[Id] = Up;
            Who.Position = new Vector3(PosX[Id], PosY[Id], Z);
            Who.DungeonPose = true;
            if (Who.BodyModel != null)
            {
                MapInfo Here = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
                if (Who.BodySprings != null)
                {
                    Who.BodySprings.World = Who.BodyDrawWorld(ChunkOrigin.Shift(Here));
                    Who.BodySprings.Seconds = Seconds;
                }

                Who.BodyModel.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(Seconds)));
                DungeonFloor Under = Floor;
                FootPlant.Apply(Who, ChunkOrigin.Shift(Here), (X, Y) => DungeonContact(Under, X, Y));
                if (Who.BodySprings != null)
                {
                    Who.BodySprings.Pose(Who.BodyModel);
                }
            }
        }

        static GroundContact DungeonContact(DungeonFloor Floor, float X, float Y)
        {
            const float Step = 0.25f;
            float Height = GroundAt(Floor, X, Y);
            float SlopeX = (GroundAt(Floor, X + Step, Y) - Height) / Step;
            float SlopeY = (GroundAt(Floor, X, Y + Step) - Height) / Step;
            return new GroundContact(Height, SlopeX, SlopeY);
        }

        public static float GroundAt(DungeonFloor Floor, float X, float Y)
        {
            if (Floor == null)
            {
                return 0f;
            }

            int CellX = (int)Math.Floor(X / DungeonView.Cell);
            int CellY = (int)Math.Floor(Y / DungeonView.Cell);
            DungeonCell Cell = Floor.At(CellX, CellY);
            if (Cell == null || !Cell.Walkable())
            {
                return 0f;
            }

            float Base = Cell.Height * DungeonView.Rise;
            int DX;
            int DY;
            float Rise;
            float Angle;
            if (Cell.Role != DungeonRole.Step || !StairAim(Floor, Cell, true, out DX, out DY, out Rise, out Angle) || Rise <= 0f)
            {
                return Base;
            }

            float LocalX = (X - CellX * DungeonView.Cell) / DungeonView.Cell;
            float LocalY = (Y - CellY * DungeonView.Cell) / DungeonView.Cell;
            float Along = DX != 0 ? LocalX : LocalY;
            if (DX < 0 || DY < 0)
            {
                Along = 1f - Along;
            }

            if (Along < 0f)
            {
                Along = 0f;
            }
            else if (Along > 1f)
            {
                Along = 1f;
            }

            return Base + Along * Rise;
        }

        public static bool StairAim(DungeonFloor Floor, DungeonCell Cell, bool Upward, out int DX, out int DY, out float Rise, out float Angle)
        {
            DX = 0;
            DY = 0;
            Rise = 0f;
            Angle = 0f;
            if (Floor == null || Cell == null)
            {
                return false;
            }

            int[] StepX = new int[] { 1, -1, 0, 0 };
            int[] StepY = new int[] { 0, 0, 1, -1 };
            float[] StepAngle = new float[] { -MathHelper.PiOver2, MathHelper.PiOver2, 0f, MathHelper.Pi };
            int Best = -1;
            int BestRise = Upward ? 0 : -1;
            for (int i = 0; i < 4; i++)
            {
                DungeonCell Next = Floor.At(Cell.X + StepX[i], Cell.Y + StepY[i]);
                if (Next == null || !Next.Walkable())
                {
                    continue;
                }

                int Delta = Next.Height - Cell.Height;
                if (Upward)
                {
                    if (Delta > BestRise)
                    {
                        Best = i;
                        BestRise = Delta;
                    }
                }
                else if (Best < 0)
                {
                    Best = i;
                    BestRise = Delta;
                }
            }

            if (Best < 0)
            {
                return false;
            }

            DX = StepX[Best];
            DY = StepY[Best];
            Rise = BestRise * DungeonView.Rise;
            Angle = StepAngle[Best];
            return true;
        }

        public void Settle(string Id)
        {
            if (string.IsNullOrEmpty(Id) || PosX.ContainsKey(Id))
            {
                return;
            }

            int X;
            int Y;
            if (!CellX.TryGetValue(Id, out X) || !CellY.TryGetValue(Id, out Y))
            {
                return;
            }

            PosX[Id] = (X + 0.5f) * DungeonView.Cell;
            PosY[Id] = (Y + 0.5f) * DungeonView.Cell;
        }

        static bool Fits(DungeonFloor Floor, float X, float Y)
        {
            float Reach = 7f;
            return Open(Floor, X - Reach, Y - Reach) && Open(Floor, X + Reach, Y - Reach) && Open(Floor, X - Reach, Y + Reach) && Open(Floor, X + Reach, Y + Reach);
        }

        static bool Open(DungeonFloor Floor, float X, float Y)
        {
            if (Floor == null)
            {
                return false;
            }

            DungeonCell Cell = Floor.At((int)Math.Floor(X / DungeonView.Cell), (int)Math.Floor(Y / DungeonView.Cell));
            return Cell != null && Cell.Walkable();
        }

        public void StepCompanions()
        {
            PartyMember Lead = Party.Lead();
            int LeadFloor;
            int LeadX;
            int LeadY;
            if (Lead == null || !FloorOf.TryGetValue(Lead.Id, out LeadFloor) || !CellX.TryGetValue(Lead.Id, out LeadX) || !CellY.TryGetValue(Lead.Id, out LeadY))
            {
                return;
            }

            for (int i = 0; i < Party.Roster.Count; i++)
            {
                PartyMember Who = Party.Roster[i];
                int Floor;
                if (!Who.InParty || Who.Seat >= 0 || Who == Lead || !FloorOf.TryGetValue(Who.Id, out Floor) || Floor != LeadFloor)
                {
                    continue;
                }

                DungeonFloor Map = Visit(Floor);
                DungeonSpawn Foe = NearestFoe(Map, CellX[Who.Id], CellY[Who.Id]);
                if (Foe != null && Who.Body != null && Foe.Body != null)
                {
                    Who.Body.Position = new Microsoft.Xna.Framework.Vector3(CellX[Who.Id] * 32f, CellY[Who.Id] * 32f, 0f);
                    DungeonCombat.BeginStrike(Who.Body, Foe.Body, 0.45);
                    Foe.HP = Foe.Body.CurrentHP;
                    DungeonPay.Claim(LifeSync.Home, Foe);
                    continue;
                }

                StepToward(Who.Id, Map, LeadX, LeadY);
            }
        }

        public static DungeonSpawn NearestFoe(DungeonFloor Floor, int X, int Y)
        {
            if (Floor == null)
            {
                return null;
            }

            DungeonSpawn Best = null;
            int BestDistance = 3;
            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                DungeonSpawn Spawn = Floor.Spawns[i];
                if (Spawn.Kind != DungeonPersonKind.Monster || Spawn.HP <= 0 || Spawn.Body == null)
                {
                    continue;
                }

                int Distance = Math.Abs(Spawn.X - X) + Math.Abs(Spawn.Y - Y);
                if (Distance < BestDistance)
                {
                    Best = Spawn;
                    BestDistance = Distance;
                }
            }

            return Best;
        }

        static DungeonCell Landing(DungeonFloor Floor, int X, int Y)
        {
            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DY = new int[] { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                DungeonCell Next = Floor.At(X + DX[i], Y + DY[i]);
                if (Next != null && Next.Walkable() && Next.Role != DungeonRole.StairUp && Next.Role != DungeonRole.StairDown)
                {
                    return Next;
                }
            }

            return null;
        }

        void StepToward(string Id, DungeonFloor Floor, int GoalX, int GoalY)
        {
            int X = CellX[Id];
            int Y = CellY[Id];
            if (X == GoalX && Y == GoalY)
            {
                return;
            }

            int BestX = X;
            int BestY = Y;
            int Best = Math.Abs(GoalX - X) + Math.Abs(GoalY - Y);
            int[] DX = new int[] { 1, -1, 0, 0 };
            int[] DY = new int[] { 0, 0, 1, -1 };
            for (int i = 0; i < 4; i++)
            {
                int NX = X + DX[i];
                int NY = Y + DY[i];
                if (!DungeonFloor.CanCross(Floor.At(X, Y), Floor.At(NX, NY)))
                {
                    continue;
                }

                int Distance = Math.Abs(GoalX - NX) + Math.Abs(GoalY - NY);
                if (Distance < Best)
                {
                    Best = Distance;
                    BestX = NX;
                    BestY = NY;
                }
            }

            Place(Id, Floor.Index, BestX, BestY);
        }

        void BringFollowers(string LeadId, int FromFloor, int ToFloor, int X, int Y)
        {
            for (int i = 0; i < Party.Roster.Count; i++)
            {
                PartyMember Who = Party.Roster[i];
                int Floor;
                if (!Who.InParty || Who.Seat >= 0 || Who.Id == LeadId || Who.Resident)
                {
                    continue;
                }

                if (FromFloor >= 0 && (!FloorOf.TryGetValue(Who.Id, out Floor) || Floor != FromFloor))
                {
                    continue;
                }

                if (ToFloor < 0)
                {
                    LeaveDungeon(Who.Id);
                }
                else
                {
                    Place(Who.Id, ToFloor, X, Y);
                }
            }
        }

        void Place(string Id, int Floor, int X, int Y)
        {
            FloorOf[Id] = Floor;
            CellX[Id] = X;
            CellY[Id] = Y;
            PosX[Id] = (X + 0.5f) * DungeonView.Cell;
            PosY[Id] = (Y + 0.5f) * DungeonView.Cell;
            Altitude.Remove(Id);
            Lift.Remove(Id);
        }

        void LeaveDungeon(string Id)
        {
            FloorOf.Remove(Id);
            CellX.Remove(Id);
            CellY.Remove(Id);
            PosX.Remove(Id);
            PosY.Remove(Id);
            Facing.Remove(Id);
            Altitude.Remove(Id);
            Lift.Remove(Id);
            PartyMember Body = Party.Find(Id);
            if (Body != null && Body.Body != null)
            {
                Body.Body.DungeonPose = false;
            }

            if (FloorOf.Count == 0)
            {
                DungeonPay.CashOut(LifeSync.Home);
            }
        }
    }

    public static class DungeonDress
    {
        public static void Fill(DungeonFloor Floor, int Seed)
        {
            Fill(Floor, Seed, 0);
        }

        public static void Fill(DungeonFloor Floor, int Seed, int Wing)
        {
            if (Floor == null || Floor.Spawns.Count > 0)
            {
                return;
            }

            Random Rng = new Random(Seed);
            List<DungeonCell> Open = new List<DungeonCell>();
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Walkable() && !(Cell.X == Floor.EntryX && Cell.Y == Floor.EntryY) && Cell.Role != DungeonRole.StairUp && Cell.Role != DungeonRole.StairDown)
                {
                    Open.Add(Cell);
                }
            }

            Place(Floor, Open, Rng, DungeonPersonKind.Monster, "Goblin", 3, Wing);
            Place(Floor, Open, Rng, DungeonPersonKind.Explorer, "Wanderer", 1, Wing);
            Place(Floor, Open, Rng, DungeonPersonKind.Captive, "Captive", 1, Wing);
        }

        static void Place(DungeonFloor Floor, List<DungeonCell> Open, Random Rng, DungeonPersonKind Kind, string Name, int Count, int Wing)
        {
            for (int n = 0; n < Count && Open.Count > 0; n++)
            {
                int Pick = Rng.Next(Open.Count);
                DungeonCell Cell = Open[Pick];
                Open.RemoveAt(Pick);
                DungeonSpawn Spawn = new DungeonSpawn();
                Spawn.X = Cell.X;
                Spawn.Y = Cell.Y;
                Spawn.Kind = Kind;
                Spawn.Name = Name + " " + Floor.Index + " " + n;
                if (Kind == DungeonPersonKind.Monster)
                {
                    Spawn.Model = n % 2 == 0 ? "Bad Guy 1.glb" : "Bad Guy 2.glb";
                }

                Spawn.Body = Spellcast.Unlisted(Spawn.Name);
                if (Kind == DungeonPersonKind.Monster)
                {
                    Spawn.Purse = 8 + Floor.Index + (Wing > 0 ? 16 : 0);
                    if ((Floor.Index + n) % 4 == 0)
                    {
                        Spawn.Drop = "Iron Ingot";
                    }

                    if (Wing > 0)
                    {
                        Spawn.Body.MaxHP += 10;
                        Spawn.Body.CurrentHP = Spawn.Body.MaxHP;
                    }
                }
                else if (Kind == DungeonPersonKind.Captive)
                {
                    Spawn.Trade = Trades.ByFloor(Floor.Index, n);
                }

                Spawn.HP = Spawn.Body.CurrentHP;
                Spawn.Body.Position = new Microsoft.Xna.Framework.Vector3(Cell.X * DungeonView.Cell, Cell.Y * DungeonView.Cell, Cell.Height * DungeonView.Rise);
                Floor.Spawns.Add(Spawn);
            }
        }
    }
}
