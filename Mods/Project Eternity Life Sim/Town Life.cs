using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class TownLife
    {
        public const float TownReach = 220f;

        public static TownBook Book;
        public static WorldSlot Slot;
        public static NavMapGameManager Game;

        static readonly Dictionary<string, PlayerCharacter> Bodies = new Dictionary<string, PlayerCharacter>();
        static readonly Dictionary<string, PlayerCharacter> Parked = new Dictionary<string, PlayerCharacter>();

        public static void Open(WorldSlot World, NavMapGameManager Manager)
        {
            Slot = World;
            Game = Manager;
            Bodies.Clear();
            Parked.Clear();
            Book = World == null ? new TownBook() : TownBook.Load(World.CitiesPath);
        }

        public static void Save()
        {
            if (Book == null || Slot == null || string.IsNullOrEmpty(Slot.Folder))
            {
                return;
            }

            Book.Save(Slot.CitiesPath);
        }

        public static void Deliver(double Now)
        {
            if (Book == null)
            {
                return;
            }

            Dictionary<string, int> Home = Pantry();
            if (Book.Deliver(Now, Home))
            {
                Save();
            }
        }

        public static void Advance(NavMapGameManager Manager, double Hours)
        {
            if (Book == null || Manager == null || Hours <= 0)
            {
                return;
            }

            bool[] Was = new bool[Book.People.Count];
            for (int i = 0; i < Book.People.Count; i++)
            {
                Was[i] = Book.People[i].Traveling;
            }

            bool Arrived = Book.Step(Hours, Pantry());
            for (int i = 0; i < Book.People.Count; i++)
            {
                if (!Was[i])
                {
                    continue;
                }

                TownPerson Person = Book.People[i];
                PlayerCharacter Who = Place(Manager, Person);
                if (Who != null)
                {
                    Who.CityId = Person.Traveling ? -1 : Person.Town;
                }
            }

            if (Arrived)
            {
                Save();
            }
        }

        public static void AddEyes(IList<Vector3> Eyes)
        {
            if (Book == null || Eyes == null)
            {
                return;
            }

            for (int i = 0; i < Book.People.Count; i++)
            {
                TownPerson Person = Book.People[i];
                if (!Person.Traveling)
                {
                    continue;
                }

                Eyes.Add(new Vector3(Person.X, 8f, Person.Z));
            }
        }

        public static void Wake(NavMapGameManager Manager, MapInfo Map)
        {
            if (Book == null || Manager == null || Map == null)
            {
                return;
            }

            for (int i = 0; i < Book.People.Count; i++)
            {
                TownPerson Person = Book.People[i];
                if (!InChunk(Person, Map))
                {
                    continue;
                }

                if (Bodies.ContainsKey(Person.Id))
                {
                    continue;
                }

                PlayerCharacter Who = Attach(Manager, Map, Person);
                if (Who != null)
                {
                    Settle(Who, Map, Person);
                    Who.CityId = Person.Traveling ? -1 : Person.Town;
                }
            }
        }

        public static void Stow(NavMapGameManager Manager, MapInfo Map)
        {
            if (Book == null || Map == null || Map.MapMapContainer == null)
            {
                return;
            }

            if (Map.People == null)
            {
                return;
            }

            for (int i = Map.People.Count - 1; i >= 0; i--)
            {
                PlayerCharacter Who = Map.People[i];
                if (Who == null || string.IsNullOrEmpty(Who.TownPersonId) || Held(Manager, Who))
                {
                    continue;
                }

                Remember(Who);
                Map.Drop(Who);
                Bodies.Remove(Who.TownPersonId);
                Parked[Who.TownPersonId] = Who;
            }
        }

        public static void Capture()
        {
            if (Book == null)
            {
                return;
            }

            foreach (PlayerCharacter Who in Bodies.Values)
            {
                Remember(Who);
            }

            Save();
        }

        public static bool PlaceSaved(NavMapGameManager Manager, PlayerCharacter Who)
        {
            if (Book == null || Manager == null || Who == null || string.IsNullOrEmpty(Who.TownPersonId))
            {
                return false;
            }

            TownPerson Person = Book.Find(Who.TownPersonId);
            if (Person == null)
            {
                Who.TownPersonId = string.Empty;
                Who.CityId = -1;
                return false;
            }

            Bodies[Person.Id] = Who;
            Parked.Remove(Person.Id);
            Who.CityId = Person.Traveling ? -1 : Person.Town;
            if (Who.SharedMapContex != null)
            {
                Who.SharedMapContex.RootMapContainer = Manager;
            }

            Place(Manager, Person);
            return true;
        }

        public static void Collect(NavMapGameManager Manager)
        {
            if (Book == null || Manager == null || Manager.UpdatedMaps == null || Book.Towns.Count == 0)
            {
                return;
            }

            for (int m = 0; m < Manager.UpdatedMaps.Count; m++)
            {
                MapInfo Info = Manager.UpdatedMaps[m];
                if (Info == null || Info.People == null)
                {
                    continue;
                }

                for (int i = 0; i < Info.People.Count; i++)
                {
                    PlayerCharacter Who = Info.People[i];
                    if (Who == null || IsCursor(Manager, Who))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(Who.TownPersonId) && Book.Find(Who.TownPersonId) != null)
                    {
                        continue;
                    }

                    FileStanding(Who);
                }
            }
        }

        public static void FileStanding(PlayerCharacter Who)
        {
            if (Book == null || Who == null || Book.Towns.Count == 0)
            {
                return;
            }

            float X;
            float Z;
            if (!WorldOf(Who, out X, out Z))
            {
                return;
            }

            int Town = Book.Nearest(X, Z, TownReach);
            if (Town < 0)
            {
                return;
            }

            if (!string.IsNullOrEmpty(Who.TownPersonId) && Book.Find(Who.TownPersonId) != null)
            {
                if (!Book.MoveTo(Who.TownPersonId, Town, X, Z))
                {
                    return;
                }
            }
            else
            {
                TownPerson Person = Book.Admit(Who.Name, Town, X, Z);
                if (Person == null)
                {
                    return;
                }

                Who.TownPersonId = Person.Id;
                Bodies[Person.Id] = Who;
            }

            Who.CityId = Town;
            Save();
        }

        public static PlayerCharacter Possess(NavMapGameManager Manager, string Id)
        {
            if (Book == null || Manager == null)
            {
                return null;
            }

            TownPerson Person = Book.Find(Id);
            if (Person == null || !Person.Playable)
            {
                return null;
            }

            return Place(Manager, Person);
        }

        public static void Gather(PlayerCharacter Who)
        {
            if (Book == null || Who == null || Who.CityId < 0)
            {
                return;
            }

            Book.Gather(Who.CityId, Who.Life, Pantry());
            Save();
        }

        public static bool Send(string Id, int Destination, string Item, int Count)
        {
            if (Book == null || !Book.Carry(Id, Destination, Item, Count, Pantry()))
            {
                return false;
            }

            Sync(Id);
            Save();
            return true;
        }

        public static bool Post(int From, int Destination, string Item, int Count, double Now)
        {
            if (Book == null || !Book.SendParcel(From, Destination, Item, Count, Now, Pantry()))
            {
                return false;
            }

            Save();
            return true;
        }

        public static Dictionary<string, int> Pile(PlayerCharacter Who, Dictionary<string, int> Home)
        {
            if (Book == null || Who == null || Who.CityId < 0)
            {
                return Home;
            }

            return Book.Pile(Who.CityId, Home);
        }

        public static bool Riding(PlayerCharacter Who)
        {
            if (Book == null || Who == null || string.IsNullOrEmpty(Who.TownPersonId))
            {
                return false;
            }

            TownPerson Person = Book.Find(Who.TownPersonId);
            return Person != null && Person.Traveling;
        }

        public static double Now(NavMapGameManager Manager)
        {
            if (Manager == null || Manager.Clock == null)
            {
                return 0;
            }

            return Manager.Clock.Day * Manager.Clock.HoursInDay + Manager.Clock.Hour;
        }

        static PlayerCharacter Place(NavMapGameManager Manager, TownPerson Person)
        {
            if (Person == null || Slot == null)
            {
                return null;
            }

            float Size = Slot.ChunkSize > 1f ? Slot.ChunkSize : WorldSlot.DefaultChunkSize;
            int ChunkX = (int)Math.Floor(Person.X / Size);
            int ChunkZ = (int)Math.Floor(Person.Z / Size);
            MapInfo Info = WorldStream.ChunkAt(ChunkX, ChunkZ, true);
            if (Info == null)
            {
                return null;
            }

            Manager.AwakeMap(Info);
            PlayerCharacter Who;
            if (!Bodies.TryGetValue(Person.Id, out Who))
            {
                Who = Attach(Manager, Info, Person);
            }

            if (Who == null)
            {
                return null;
            }

            Settle(Who, Info, Person);
            Who.CityId = Person.Traveling ? -1 : Person.Town;
            return Who;
        }

        static PlayerCharacter Attach(NavMapGameManager Manager, MapInfo Info, TownPerson Person)
        {
            LifeSimMap Map = Info.MapMapContainer == null ? null : Info.MapMapContainer.ActiveMap as LifeSimMap;
            PlayerCharacter Who;
            if (!Parked.TryGetValue(Person.Id, out Who))
            {
                Who = Spellcast.Unlisted(Person.Name);
                Who.TownPersonId = Person.Id;
                if (Who.SharedMapContex != null)
                {
                    Who.SharedMapContex.RootMapContainer = Manager;
                    Who.SharedMapContex.CurrentMapInfo = Info;
                }

                Wear(Who);
                float LocalX = Person.X - OffsetX(Info);
                float LocalY = Person.Z - OffsetY(Info);
                Vector3 FeetAt = new Vector3(LocalX, LocalY, Feet(Info, Person, LocalX, LocalY));
                if (Map != null)
                {
                    Map.SpawnCharacter(Who, 0, FeetAt);
                }
                else
                {
                    Who.SetPosition(FeetAt);
                    Who.AIControlled = true;
                    Who.Body.CanJump = true;
                    Who.SeedBelief();
                    Who.Init();
                    Info.Adopt(Who);
                }
            }
            else
            {
                Parked.Remove(Person.Id);
                if (Who.SharedMapContex != null)
                {
                    Who.SharedMapContex.RootMapContainer = Manager;
                    Who.SharedMapContex.CurrentMapInfo = Info;
                }

                Info.Adopt(Who);
            }

            Bodies[Person.Id] = Who;
            return Who;
        }

        static void Settle(PlayerCharacter Who, MapInfo Info, TownPerson Person)
        {
            if (Who == null || Info == null || Person == null)
            {
                return;
            }

            float LocalX = Person.X - OffsetX(Info);
            float LocalY = Person.Z - OffsetY(Info);
            float Height = Feet(Info, Person, LocalX, LocalY);
            ChunkCross.Settle(Who, Person.X, Person.Z, Height);
            Who.SetPosition(Who.Position);
        }

        static float Feet(MapInfo Info, TownPerson Person, float LocalX, float LocalY)
        {
            if (Info != null && Info.Heights != null)
            {
                return HeightField.Stand(Info, LocalX, LocalY);
            }

            if (Person.Town >= 0 && Book != null && Person.Town < Book.Towns.Count)
            {
                return Book.Towns[Person.Town].Ground;
            }

            return 8f;
        }

        static void Sync(string Id)
        {
            PlayerCharacter Who;
            if (!Bodies.TryGetValue(Id, out Who) && !Parked.TryGetValue(Id, out Who))
            {
                return;
            }

            TownPerson Person = Book.Find(Id);
            if (Person == null || Who == null)
            {
                return;
            }

            Who.CityId = Person.Traveling ? -1 : Person.Town;
        }

        static void Remember(PlayerCharacter Who)
        {
            TownPerson Person = Book == null ? null : Book.Find(Who.TownPersonId);
            if (Person == null || Person.Traveling)
            {
                return;
            }

            float X;
            float Z;
            if (WorldOf(Who, out X, out Z))
            {
                Person.X = X;
                Person.Z = Z;
            }
        }

        static bool WorldOf(PlayerCharacter Who, out float X, out float Z)
        {
            X = 0f;
            Z = 0f;
            if (Who == null || Who.SharedMapContex == null || Who.SharedMapContex.CurrentMapInfo == null)
            {
                return false;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            X = Who.Position.X + OffsetX(Here);
            Z = Who.Position.Y + OffsetY(Here);
            return true;
        }

        static float OffsetX(MapInfo Info)
        {
            if (Info == null || Info.MapMapContainer == null)
            {
                return 0f;
            }

            return Info.MapMapContainer.OffsetX;
        }

        static float OffsetY(MapInfo Info)
        {
            if (Info == null || Info.MapMapContainer == null)
            {
                return 0f;
            }

            return Info.MapMapContainer.OffsetY;
        }

        static bool InChunk(TownPerson Person, MapInfo Map)
        {
            if (Slot == null || Person == null || Map == null)
            {
                return false;
            }

            float Size = Slot.ChunkSize > 1f ? Slot.ChunkSize : WorldSlot.DefaultChunkSize;
            int ChunkX = (int)Math.Floor(Person.X / Size);
            int ChunkZ = (int)Math.Floor(Person.Z / Size);
            return Map.ChunkX == ChunkX && Map.ChunkZ == ChunkZ;
        }

        static bool IsCursor(NavMapGameManager Manager, PlayerCharacter Who)
        {
            if (Manager == null || Manager.ListLocalPlayer == null)
            {
                return false;
            }

            for (int i = 0; i < Manager.ListLocalPlayer.Count; i++)
            {
                PlayerOverseer Player = Manager.ListLocalPlayer[i];
                if (Player != null && Player.InvisibleCharacterAsCursor == Who)
                {
                    return true;
                }
            }

            return false;
        }

        static bool Held(NavMapGameManager Manager, PlayerCharacter Who)
        {
            if (Manager == null || Manager.ListLocalPlayer == null)
            {
                return false;
            }

            for (int i = 0; i < Manager.ListLocalPlayer.Count; i++)
            {
                PlayerOverseer Player = Manager.ListLocalPlayer[i];
                if (Player == null)
                {
                    continue;
                }

                if (Player.ActiveControlledCharacter == Who)
                {
                    return true;
                }

                if (Player.Session != null && Player.Session.Controlled == Who)
                {
                    return true;
                }
            }

            return false;
        }

        static Dictionary<string, int> Pantry()
        {
            if (Game == null || Game.Home == null)
            {
                return null;
            }

            return Game.Home.Pantry;
        }

        static void Wear(PlayerCharacter Who)
        {
            if (Who == null || Who.BodyModel != null || Who.SpriteMap != null || GameScreen.GraphicsDevice == null)
            {
                return;
            }

            string Glb = Path.Combine("Content", "Life Sim", "Characters", "Models", "Default.glb");
            if (File.Exists(Glb))
            {
                Who.WearBody(GameScreen.GraphicsDevice, Glb);
            }
        }
    }
}
