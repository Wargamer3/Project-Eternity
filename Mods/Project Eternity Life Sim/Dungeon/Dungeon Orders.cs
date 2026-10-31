using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class DungeonOrders
    {
        public static void Confirm(NavMapGameManager Manager, PlaySession Session, int X, int Y)
        {
            if (Manager == null || Session == null || string.IsNullOrEmpty(Session.PartyId))
            {
                return;
            }

            DungeonRun Run = Manager.Home.Run();
            int FloorIndex;
            if (!Run.FloorOf.TryGetValue(Session.PartyId, out FloorIndex))
            {
                return;
            }

            DungeonFloor Floor = Run.Visit(FloorIndex);
            DungeonCell Cell = Floor == null ? null : Floor.At(X, Y);
            if (Cell == null)
            {
                return;
            }

            DungeonSpawn Spawn = SpawnAt(Floor, X, Y);
            if (Spawn != null && Spawn.Kind == DungeonPersonKind.Monster && Spawn.HP > 0)
            {
                Strike(Run, Session.PartyId, Spawn);
                return;
            }

            if (Spawn != null && Spawn.Kind == DungeonPersonKind.Captive)
            {
                PartyMember Saver = Run.Party.Find(Session.PartyId);
                bool SentHome;
                Rescue.Free(Run.Party, Spawn, Saver == null ? null : Saver.Body, out SentHome);
                return;
            }

            if (Spawn != null && Spawn.Kind == DungeonPersonKind.Explorer && Spawn.Body != null)
            {
                PartyMember Saver = Run.Party.Find(Session.PartyId);
                if (Saver != null && Saver.Body != null)
                {
                    Social.Talk(Saver.Body.RelationshipWith(Spawn.Body), 15f);
                }

                return;
            }

            if (Cell.Role == DungeonRole.StairUp || Cell.Role == DungeonRole.StairDown)
            {
                if (Run.OrderMove(Session.PartyId, X, Y))
                {
                    Run.Climb(Session.PartyId);
                    Session.InDungeon = Run.FloorOf.ContainsKey(Session.PartyId);
                }

                return;
            }

            Run.OrderMove(Session.PartyId, X, Y);
        }

        public static void Step(NavMapGameManager Manager, PlaySession Session, float DX, float DY, float Seconds, bool Attack, bool Interact, PlayerCharacter Who, float Strength, bool Jump)
        {
            if (Manager == null || Session == null || string.IsNullOrEmpty(Session.PartyId))
            {
                return;
            }

            DungeonRun Run = Manager.Home.Run();
            Run.Stride(Session.PartyId, Who, DX, DY, Strength, Seconds, Jump);
            int FloorIndex;
            int X;
            int Y;
            if (!Run.FloorOf.TryGetValue(Session.PartyId, out FloorIndex) || !Run.CellX.TryGetValue(Session.PartyId, out X) || !Run.CellY.TryGetValue(Session.PartyId, out Y))
            {
                return;
            }

            DungeonFloor Floor = Run.Visit(FloorIndex);
            if (Attack)
            {
                DungeonSpawn Foe = DungeonRun.NearestFoe(Floor, X, Y);
                Strike(Run, Session.PartyId, Foe);
            }

            if (Interact)
            {
                Confirm(Manager, Session, X, Y);
            }
        }

        public static bool Buy(PlaySession Session, BuildingZone Zone)
        {
            if (Session == null)
            {
                return false;
            }

            int Funds = Session.Funds;
            bool Bought = PlotSale.TryBuy(Zone, Session.HouseholdId, ref Funds);
            if (Bought)
            {
                Session.Funds = Funds;
                Biomes.AfterBuy(Session.Home, Zone);
            }

            return Bought;
        }

        public static void WorkHere(PlayerCharacter Who, FurnitureItem Box, HouseholdState Home)
        {
            if (Who == null || Box == null || Home == null || Who.Life.Away.Busy)
            {
                return;
            }

            if (Box.Interaction == "Garden Plot")
            {
                Work.Tend(Who.Life, LifeSkill.Gardening, Home.Pantry);
            }
            else if (Box.Interaction == "Fishing Spot")
            {
                Work.Tend(Who.Life, LifeSkill.Fishing, Home.Pantry);
            }
            else if (Box.Interaction == "Forge")
            {
                Recipe Next = Cookbook.BestSmith(Who.Life, Home.Pantry);
                Work.Craft(Who.Life, Next ?? Cookbook.Blade, Home.Pantry, Home.Reserved);
            }
            else if (Box.Interaction == "Alchemy Bench")
            {
                Work.Craft(Who.Life, Cookbook.Tonic, Home.Pantry, Home.Reserved);
            }
            else if (Box.Interaction == "Ore Rock")
            {
                GatherNode Node = null;
                for (int i = 0; i < Home.Nodes.Count; i++)
                {
                    GatherNode Rock = Home.Nodes[i];
                    if (Rock != null && Rock.Skill == LifeSkill.Mining && Gather.CanMine(Who.Life, Rock, Home, Who.SimHour))
                    {
                        Node = Rock;
                        break;
                    }
                }

                Gather.Take(Who.Life, Node, Home, Who.SimHour);
            }
        }

        public static void SendAway(PlayerCharacter Who, LifeSkill Skill, HouseholdState Home, double Now)
        {
            if (Who == null || Home == null)
            {
                return;
            }

            if (Work.StartAway(Who.Life, Skill, Now))
            {
                Home.Track(Who.Life);
            }
        }

        static void Strike(DungeonRun Run, string Id, DungeonSpawn Foe)
        {
            PartyMember Who = Run.Party.Find(Id);
            if (Who == null || Who.Body == null || Foe == null || Foe.Body == null)
            {
                return;
            }

            int X;
            int Y;
            if (Run.CellX.TryGetValue(Id, out X) && Run.CellY.TryGetValue(Id, out Y))
            {
                Who.Body.Position = new Vector3(X * 32f, Y * 32f, 0f);
            }

            DungeonCombat.BeginStrike(Who.Body, Foe.Body, 0.45);
            Foe.HP = Foe.Body.CurrentHP;
            DungeonPay.Claim(LifeSync.Home, Foe);
        }

        static DungeonSpawn SpawnAt(DungeonFloor Floor, int X, int Y)
        {
            if (Floor == null)
            {
                return null;
            }

            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                DungeonSpawn Spawn = Floor.Spawns[i];
                if (Spawn.X == X && Spawn.Y == Y)
                {
                    return Spawn;
                }
            }

            return null;
        }
    }
}
