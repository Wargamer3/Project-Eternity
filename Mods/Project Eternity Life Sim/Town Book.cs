using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class TownRecord
    {
        public string Name = string.Empty;
        public float X;
        public float Z;
        public float Ground;
        public int ChunkX;
        public int ChunkZ;
        public List<int> Roads = new List<int>();
        public Dictionary<string, int> Stock = new Dictionary<string, int>();
    }

    public class TownPerson
    {
        public string Id = string.Empty;
        public string Name = string.Empty;
        public int Town = -1;
        public float X;
        public float Z;
        public bool Traveling;
        public bool Playable = true;
        public int Destination = -1;
        public int Way;
        public List<float> WayX = new List<float>();
        public List<float> WayZ = new List<float>();
        public Dictionary<string, int> Bag = new Dictionary<string, int>();
    }

    public class TownMail
    {
        public string Item = string.Empty;
        public int Count;
        public int Destination;
        public double Arrive;
    }

    public class TownBook
    {
        public const float MetersPerHour = 80f;

        public int WorldIndex;
        public List<TownRecord> Towns = new List<TownRecord>();
        public List<TownPerson> People = new List<TownPerson>();
        public List<TownMail> Mail = new List<TownMail>();
        public int NextId = 1;

        static readonly string[] TownNames = new string[]
        {
            "Millford", "Ashwell", "Rookbridge", "Highbarn",
            "Larkspur", "Dunwick", "Bramble", "Ostler",
            "Fernhold", "Graymere", "Saltwick", "Pebbleford",
        };

        static readonly string[] StarterNames = new string[]
        {
            "Wren", "Ada", "Bea", "Cora", "Nell", "Ida", "Mae", "Ruth", "Lark", "Fern", "Joy", "Tess",
        };

        static readonly string[] NeighborNames = new string[]
        {
            "Otto", "Ned", "Hugh", "Ivo", "Pia", "Sybil", "Edith", "Harold", "Marta", "Colin",
            "Agnes", "Tobin", "Vera", "Oscar", "Hilda", "Bram", "Nessa", "Ellis", "Greta", "Wilf",
            "Dora", "Leif", "Kit", "Rowan", "Ansel", "Maud", "Cedric", "Iris", "Hob", "Willa",
        };

        public static string NeighborName(int Index)
        {
            if (Index < 0)
            {
                Index = -Index;
            }

            return NeighborNames[Index % NeighborNames.Length];
        }

        public static string TownName(int Index)
        {
            if (Index < 0)
            {
                return string.Empty;
            }

            if (Index < TownNames.Length)
            {
                return TownNames[Index];
            }

            return TownNames[Index % TownNames.Length] + " " + ((Index / TownNames.Length) + 1).ToString();
        }

        public int AddTown(string Name, float X, float Z, float Ground, int ChunkX, int ChunkZ)
        {
            TownRecord Town = new TownRecord();
            Town.Name = string.IsNullOrEmpty(Name) ? TownName(Towns.Count) : Name;
            Town.X = X;
            Town.Z = Z;
            Town.Ground = Ground;
            Town.ChunkX = ChunkX;
            Town.ChunkZ = ChunkZ;
            Towns.Add(Town);
            return Towns.Count - 1;
        }

        public void Link(int A, int B)
        {
            if (A == B || A < 0 || B < 0 || A >= Towns.Count || B >= Towns.Count)
            {
                return;
            }

            if (!Towns[A].Roads.Contains(B))
            {
                Towns[A].Roads.Add(B);
            }

            if (!Towns[B].Roads.Contains(A))
            {
                Towns[B].Roads.Add(A);
            }
        }

        public void SeedStarters()
        {
            if (People.Count > 0 || Towns.Count == 0)
            {
                return;
            }

            for (int i = 0; i < Towns.Count; i++)
            {
                string Name = StarterNames[i % StarterNames.Length];
                if (i >= StarterNames.Length)
                {
                    Name = Name + " " + ((i / StarterNames.Length) + 1).ToString();
                }

                Admit(Name, i);
            }
        }

        public TownPerson Admit(string Name, int Town)
        {
            if (Town < 0 || Town >= Towns.Count)
            {
                return null;
            }

            return Admit(Name, Town, Towns[Town].X, Towns[Town].Z);
        }

        public TownPerson Admit(string Name, int Town, float X, float Z)
        {
            return Admit(Name, Town, X, Z, true);
        }

        public TownPerson Admit(string Name, int Town, float X, float Z, bool Playable)
        {
            if (Town < 0 || Town >= Towns.Count)
            {
                return null;
            }

            TownPerson Person = new TownPerson();
            Person.Id = WorldIndex.ToString() + ":" + NextId.ToString();
            NextId++;
            Person.Name = string.IsNullOrEmpty(Name) ? "Wanderer" : Name;
            Person.Town = Town;
            Person.X = X;
            Person.Z = Z;
            Person.Playable = Playable;
            People.Add(Person);
            return Person;
        }

        public bool MoveTo(string Id, int Town, float X, float Z)
        {
            TownPerson Person = Find(Id);
            if (Person == null || Person.Traveling || Town < 0 || Town >= Towns.Count)
            {
                return false;
            }

            Person.Town = Town;
            Person.X = X;
            Person.Z = Z;
            return true;
        }

        public TownPerson Find(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return null;
            }

            for (int i = 0; i < People.Count; i++)
            {
                if (People[i].Id == Id)
                {
                    return People[i];
                }
            }

            return null;
        }

        public int Nearest(float X, float Z, float Reach)
        {
            int Best = -1;
            float BestDistance = Reach * Reach;
            for (int i = 0; i < Towns.Count; i++)
            {
                float DX = Towns[i].X - X;
                float DZ = Towns[i].Z - Z;
                float Distance = DX * DX + DZ * DZ;
                if (Distance <= BestDistance)
                {
                    BestDistance = Distance;
                    Best = i;
                }
            }

            return Best;
        }

        public List<TownPerson> Group(int Town)
        {
            List<TownPerson> Found = new List<TownPerson>();
            for (int i = 0; i < People.Count; i++)
            {
                if (People[i].Town == Town && !People[i].Traveling)
                {
                    Found.Add(People[i]);
                }
            }

            return Found;
        }

        public List<TownPerson> OnTheRoad()
        {
            List<TownPerson> Found = new List<TownPerson>();
            for (int i = 0; i < People.Count; i++)
            {
                if (People[i].Traveling)
                {
                    Found.Add(People[i]);
                }
            }

            return Found;
        }

        public List<string> Lines()
        {
            List<string> Result = new List<string>();
            for (int t = 0; t < Towns.Count; t++)
            {
                List<TownPerson> Here = Group(t);
                for (int i = 0; i < Here.Count; i++)
                {
                    Result.Add(Towns[t].Name + ": " + Here[i].Name);
                }
            }

            List<TownPerson> Road = OnTheRoad();
            for (int i = 0; i < Road.Count; i++)
            {
                Result.Add("Traveling: " + Road[i].Name);
            }

            return Result;
        }

        public Dictionary<string, int> Pile(int Town, Dictionary<string, int> Home)
        {
            if (Town < 0 || Town >= Towns.Count)
            {
                return Home;
            }

            if (Town == 0 && Home != null)
            {
                return Home;
            }

            if (Towns[Town].Stock == null)
            {
                Towns[Town].Stock = new Dictionary<string, int>();
            }

            return Towns[Town].Stock;
        }

        public int CountOf(int Town, string Item, Dictionary<string, int> Home)
        {
            Dictionary<string, int> Pile = this.Pile(Town, Home);
            int Have;
            if (Pile == null || string.IsNullOrEmpty(Item) || !Pile.TryGetValue(Item, out Have))
            {
                return 0;
            }

            return Have;
        }

        public void Gather(int Town, LifeSheet Sheet, Dictionary<string, int> Home)
        {
            if (Sheet == null || Town < 0 || Town >= Towns.Count)
            {
                return;
            }

            Dictionary<string, int> Pile = this.Pile(Town, Home);
            Work.Tend(Sheet, LifeSkill.Gardening, Pile);
            Work.Tend(Sheet, LifeSkill.Fishing, Pile);
            DepositAway(Sheet, LifeSkill.Hunting, Pile);
        }

        public static void DepositAway(LifeSheet Sheet, LifeSkill Skill, Dictionary<string, int> Pile)
        {
            if (Sheet == null || (Skill != LifeSkill.Hunting && Skill != LifeSkill.Foraging))
            {
                return;
            }

            int Amount = 1 + Sheet.Rank(Skill) / 2 + Sheet.CraftNow(0);
            Work.Add(Pile, Skill == LifeSkill.Hunting ? "Meat" : "Herb", Amount);
            Sheet.AddRank(Skill);
        }

        public bool Carry(string Id, int Destination, string Item, int Count, Dictionary<string, int> Home)
        {
            TownPerson Person = Find(Id);
            if (Person == null || !Person.Playable || Person.Traveling || Count <= 0 || string.IsNullOrEmpty(Item))
            {
                return false;
            }

            int From = Person.Town;
            if (From < 0 || Destination < 0 || Destination >= Towns.Count || Destination == From)
            {
                return false;
            }

            List<int> Route = Path(From, Destination);
            if (Route == null)
            {
                return false;
            }

            Dictionary<string, int> Source = Pile(From, Home);
            if (!Take(Source, Item, Count))
            {
                return false;
            }

            int Held;
            Person.Bag.TryGetValue(Item, out Held);
            Person.Bag[Item] = Held + Count;
            Person.Traveling = true;
            Person.Town = -1;
            Person.Destination = Destination;
            Person.Way = 0;
            Person.WayX.Clear();
            Person.WayZ.Clear();
            for (int i = 0; i < Route.Count; i++)
            {
                TownRecord Stop = Towns[Route[i]];
                Person.WayX.Add(Stop.X);
                Person.WayZ.Add(Stop.Z);
            }

            return true;
        }

        public bool SendParcel(int From, int Destination, string Item, int Count, double Now, Dictionary<string, int> Home)
        {
            if (From < 0 || Destination < 0 || From >= Towns.Count || Destination >= Towns.Count || From == Destination || Count <= 0)
            {
                return false;
            }

            double Hours = TripHours(From, Destination);
            if (Hours < 0)
            {
                return false;
            }

            if (!Take(Pile(From, Home), Item, Count))
            {
                return false;
            }

            TownMail Parcel = new TownMail();
            Parcel.Item = Item;
            Parcel.Count = Count;
            Parcel.Destination = Destination;
            Parcel.Arrive = Now + Hours;
            Mail.Add(Parcel);
            return true;
        }

        public bool Step(double Hours, Dictionary<string, int> Home)
        {
            if (Hours <= 0)
            {
                return false;
            }

            float LeftBudget = (float)(Hours * MetersPerHour);
            bool Arrived = false;
            for (int i = 0; i < People.Count; i++)
            {
                TownPerson Person = People[i];
                if (!Person.Traveling)
                {
                    continue;
                }

                float Left = LeftBudget;
                while (Left > 0f && Person.Way < Person.WayX.Count)
                {
                    float DX = Person.WayX[Person.Way] - Person.X;
                    float DZ = Person.WayZ[Person.Way] - Person.Z;
                    float Distance = (float)Math.Sqrt(DX * DX + DZ * DZ);
                    if (Distance <= Left || Distance < 0.05f)
                    {
                        Person.X = Person.WayX[Person.Way];
                        Person.Z = Person.WayZ[Person.Way];
                        Left -= Distance;
                        Person.Way++;
                    }
                    else
                    {
                        Person.X += DX / Distance * Left;
                        Person.Z += DZ / Distance * Left;
                        Left = 0f;
                    }
                }

                if (Person.Way >= Person.WayX.Count)
                {
                    Arrive(Person, Home);
                    Arrived = true;
                }
            }

            return Arrived;
        }

        public bool Deliver(double Now, Dictionary<string, int> Home)
        {
            bool Any = false;
            for (int i = Mail.Count - 1; i >= 0; i--)
            {
                TownMail Parcel = Mail[i];
                if (Now < Parcel.Arrive)
                {
                    continue;
                }

                Work.Add(Pile(Parcel.Destination, Home), Parcel.Item, Parcel.Count);
                Mail.RemoveAt(i);
                Any = true;
            }

            return Any;
        }

        public double TripHours(int From, int Destination)
        {
            float Length = RouteLength(From, Destination);
            if (Length < 0f)
            {
                return -1;
            }

            double Hours = Length / MetersPerHour;
            if (Hours < 1)
            {
                return 1;
            }

            return Hours;
        }

        public float RouteLength(int From, int Destination)
        {
            List<int> Route = Path(From, Destination);
            if (Route == null || From < 0 || From >= Towns.Count)
            {
                return -1f;
            }

            float Length = 0f;
            float X = Towns[From].X;
            float Z = Towns[From].Z;
            for (int i = 0; i < Route.Count; i++)
            {
                TownRecord Stop = Towns[Route[i]];
                float DX = Stop.X - X;
                float DZ = Stop.Z - Z;
                Length += (float)Math.Sqrt(DX * DX + DZ * DZ);
                X = Stop.X;
                Z = Stop.Z;
            }

            return Length;
        }

        public void Save(string Path)
        {
            string Folder = System.IO.Path.GetDirectoryName(Path);
            if (!string.IsNullOrEmpty(Folder))
            {
                Directory.CreateDirectory(Folder);
            }

            using (FileStream Stream = new FileStream(Path, FileMode.Create, FileAccess.Write))
            {
                Save(Stream);
            }
        }

        public void Save(Stream Stream)
        {
            BinaryWriter Writer = new BinaryWriter(Stream);
            Writer.Write((byte)'T');
            Writer.Write((byte)'W');
            Writer.Write((byte)'N');
            Writer.Write((byte)'1');
            Writer.Write(2);
            Writer.Write(WorldIndex);
            Writer.Write(NextId);
            Writer.Write(Towns.Count);
            for (int i = 0; i < Towns.Count; i++)
            {
                TownRecord Town = Towns[i];
                Writer.Write(Town.Name ?? string.Empty);
                Writer.Write(Town.X);
                Writer.Write(Town.Z);
                Writer.Write(Town.Ground);
                Writer.Write(Town.ChunkX);
                Writer.Write(Town.ChunkZ);
                Writer.Write(Town.Roads.Count);
                for (int r = 0; r < Town.Roads.Count; r++)
                {
                    Writer.Write(Town.Roads[r]);
                }

                WritePile(Writer, i == 0 ? null : Town.Stock);
            }

            Writer.Write(People.Count);
            for (int i = 0; i < People.Count; i++)
            {
                TownPerson Person = People[i];
                Writer.Write(Person.Id ?? string.Empty);
                Writer.Write(Person.Name ?? string.Empty);
                Writer.Write(Person.Town);
                Writer.Write(Person.X);
                Writer.Write(Person.Z);
                Writer.Write(Person.Traveling);
                Writer.Write(Person.Destination);
                Writer.Write(Person.Way);
                Writer.Write(Person.WayX.Count);
                for (int w = 0; w < Person.WayX.Count; w++)
                {
                    Writer.Write(Person.WayX[w]);
                    Writer.Write(Person.WayZ[w]);
                }

                WritePile(Writer, Person.Bag);
                Writer.Write(Person.Playable);
            }

            Writer.Write(Mail.Count);
            for (int i = 0; i < Mail.Count; i++)
            {
                Writer.Write(Mail[i].Item ?? string.Empty);
                Writer.Write(Mail[i].Count);
                Writer.Write(Mail[i].Destination);
                Writer.Write(Mail[i].Arrive);
            }

            Writer.Flush();
        }

        public static TownBook Load(string Path)
        {
            if (string.IsNullOrEmpty(Path) || !File.Exists(Path))
            {
                return new TownBook();
            }

            using (FileStream Stream = new FileStream(Path, FileMode.Open, FileAccess.Read))
            {
                return Load(Stream);
            }
        }

        public static TownBook Load(Stream Stream)
        {
            TownBook Book = new TownBook();
            BinaryReader Reader = new BinaryReader(Stream);
            byte[] Magic = Reader.ReadBytes(4);
            if (Magic.Length < 4 || Magic[0] != (byte)'T' || Magic[1] != (byte)'W' || Magic[2] != (byte)'N' || Magic[3] != (byte)'1')
            {
                return Book;
            }

            int Version = Reader.ReadInt32();
            if (Version != 1 && Version != 2)
            {
                return new TownBook();
            }

            Book.WorldIndex = Reader.ReadInt32();
            Book.NextId = Reader.ReadInt32();
            int TownCount = Reader.ReadInt32();
            for (int i = 0; i < TownCount; i++)
            {
                TownRecord Town = new TownRecord();
                Town.Name = Reader.ReadString();
                Town.X = Reader.ReadSingle();
                Town.Z = Reader.ReadSingle();
                Town.Ground = Reader.ReadSingle();
                Town.ChunkX = Reader.ReadInt32();
                Town.ChunkZ = Reader.ReadInt32();
                int Roads = Reader.ReadInt32();
                for (int r = 0; r < Roads; r++)
                {
                    Town.Roads.Add(Reader.ReadInt32());
                }

                Town.Stock = ReadPile(Reader);
                Book.Towns.Add(Town);
            }

            int People = Reader.ReadInt32();
            for (int i = 0; i < People; i++)
            {
                TownPerson Person = new TownPerson();
                Person.Id = Reader.ReadString();
                Person.Name = Reader.ReadString();
                Person.Town = Reader.ReadInt32();
                Person.X = Reader.ReadSingle();
                Person.Z = Reader.ReadSingle();
                Person.Traveling = Reader.ReadBoolean();
                Person.Destination = Reader.ReadInt32();
                Person.Way = Reader.ReadInt32();
                int Stops = Reader.ReadInt32();
                for (int w = 0; w < Stops; w++)
                {
                    Person.WayX.Add(Reader.ReadSingle());
                    Person.WayZ.Add(Reader.ReadSingle());
                }

                Person.Bag = ReadPile(Reader);
                Person.Playable = Version < 2 || Reader.ReadBoolean();
                Book.People.Add(Person);
            }

            int Parcels = Reader.ReadInt32();
            for (int i = 0; i < Parcels; i++)
            {
                TownMail Parcel = new TownMail();
                Parcel.Item = Reader.ReadString();
                Parcel.Count = Reader.ReadInt32();
                Parcel.Destination = Reader.ReadInt32();
                Parcel.Arrive = Reader.ReadDouble();
                Book.Mail.Add(Parcel);
            }

            return Book;
        }

        void Arrive(TownPerson Person, Dictionary<string, int> Home)
        {
            int Destination = Person.Destination;
            if (Destination >= 0 && Destination < Towns.Count)
            {
                Dictionary<string, int> Pile = this.Pile(Destination, Home);
                foreach (KeyValuePair<string, int> Item in Person.Bag)
                {
                    Work.Add(Pile, Item.Key, Item.Value);
                }

                Person.X = Towns[Destination].X;
                Person.Z = Towns[Destination].Z;
                Person.Town = Destination;
            }

            Person.Bag.Clear();
            Person.Traveling = false;
            Person.Destination = -1;
            Person.Way = 0;
            Person.WayX.Clear();
            Person.WayZ.Clear();
        }

        List<int> Path(int From, int Destination)
        {
            if (From < 0 || Destination < 0 || From >= Towns.Count || Destination >= Towns.Count || From == Destination)
            {
                return null;
            }

            int[] Parent = new int[Towns.Count];
            for (int i = 0; i < Parent.Length; i++)
            {
                Parent[i] = -2;
            }

            List<int> Open = new List<int>();
            Parent[From] = -1;
            Open.Add(From);
            int Head = 0;
            while (Head < Open.Count)
            {
                int Current = Open[Head++];
                if (Current == Destination)
                {
                    break;
                }

                List<int> Roads = Towns[Current].Roads;
                for (int i = 0; i < Roads.Count; i++)
                {
                    int Next = Roads[i];
                    if (Next < 0 || Next >= Towns.Count || Parent[Next] != -2)
                    {
                        continue;
                    }

                    Parent[Next] = Current;
                    Open.Add(Next);
                }
            }

            if (Parent[Destination] == -2)
            {
                return null;
            }

            List<int> Route = new List<int>();
            int Step = Destination;
            while (Step != From && Step >= 0)
            {
                Route.Add(Step);
                Step = Parent[Step];
            }

            Route.Reverse();
            return Route;
        }

        static bool Take(Dictionary<string, int> Pile, string Item, int Count)
        {
            int Have;
            if (Pile == null || Count <= 0 || string.IsNullOrEmpty(Item) || !Pile.TryGetValue(Item, out Have) || Have < Count)
            {
                return false;
            }

            if (Have == Count)
            {
                Pile.Remove(Item);
            }
            else
            {
                Pile[Item] = Have - Count;
            }

            return true;
        }

        static void WritePile(BinaryWriter Writer, Dictionary<string, int> Pile)
        {
            if (Pile == null || Pile.Count == 0)
            {
                Writer.Write(0);
                return;
            }

            Writer.Write(Pile.Count);
            foreach (KeyValuePair<string, int> Item in Pile)
            {
                Writer.Write(Item.Key ?? string.Empty);
                Writer.Write(Item.Value);
            }
        }

        static Dictionary<string, int> ReadPile(BinaryReader Reader)
        {
            Dictionary<string, int> Pile = new Dictionary<string, int>();
            int Count = Reader.ReadInt32();
            for (int i = 0; i < Count; i++)
            {
                string Key = Reader.ReadString();
                int Amount = Reader.ReadInt32();
                if (!string.IsNullOrEmpty(Key) && Amount > 0)
                {
                    Pile[Key] = Amount;
                }
            }

            return Pile;
        }
    }
}
