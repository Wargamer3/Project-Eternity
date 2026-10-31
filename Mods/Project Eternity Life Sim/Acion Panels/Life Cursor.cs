using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelLifeCursor : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "LifeCursor";

        int Floor;
        bool Grid = true;
        bool HasWorld;
        float WorldX;
        float WorldY;
        Vector3 Aim;
        FurnitureItem Aimed;
        PlayerCharacter AimedPerson;
        string Hint = "Arrows move";
        string LandNote;
        bool Follow;
        float FollowHeight = -1f;
        MouseState PreviousMouse;
        bool HasMouse;
        readonly List<InteractTarget> Reachable = new List<InteractTarget>();
        readonly List<FurnitureItem> Marked = new List<FurnitureItem>();

        public ActionPanelLifeCursor(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base(PanelName, Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            PlayerCharacter Who = Owner.Session == null ? null : Owner.Session.LifeSelection;
            if (Who != null && Who.SharedMapContex != null)
            {
                MapInfo Home = Who.SharedMapContex.CurrentMapInfo;
                Vector3 Shift = ChunkOrigin.Shift(Home);
                WorldX = Who.Position.X + Shift.X;
                WorldY = Who.Position.Y + Shift.Z;
                HasWorld = true;
                Floor = FloorUnder(Home, Who.Position);
            }

            EnsureCamera();
            if (!HasWorld)
            {
                SeedWorld();
            }

            SettleWorld();
            if (Owner.Session != null && Owner.Session.Practice != null)
            {
                Owner.OpenPracticeMenu();
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.TakeToolKeys(this))
            {
                return;
            }

            if (Owner.Session != null && Owner.Session.Practice != null)
            {
                if (!HasWorld)
                {
                    SeedWorld();
                }

                SettleWorld();
                Owner.Session.WatchFight(Owner.Session.Practice.Fighter);
                Owner.OpenPracticeMenu();
                Owner.EndKeys();
                return;
            }

            if (Owner.Session != null && Owner.Session.InDungeon)
            {
                DungeonMove(gameTime);
                Owner.EndKeys();
                return;
            }

            if (Owner.KeyPressed(Keys.B))
            {
                BuildingZone Zone = BuildingZones.At(CurrentInfo(), Aim.X, Aim.Y);
                int Funds = Owner.Session == null ? 0 : Owner.Session.Funds;
                string Buyer = Owner.Session == null ? null : Owner.Session.HouseholdId;
                LandNote = DungeonOrders.Buy(Owner.Session, Zone) ? "Bought the land" : PlotSale.Offer(Zone, Buyer, Funds);
            }

            if (Owner.KeyPressed(Keys.I))
            {
                InviteHome(Owner.Session.LifeSelection);
            }

            MoveAim(gameTime);
            if (Owner.KeyPressed(Keys.Enter) || ActiveInputManager.InputConfirmPressed())
            {
                Confirm(Aim);
            }

            Owner.EndKeys();
        }

        public void MoveAim(GameTime gameTime)
        {
            if (!HasWorld)
            {
                SeedWorld();
            }

            if (Owner.KeyPressed(Keys.F))
            {
                Follow = !Follow;
            }

            bool Nudge = ActiveInputManager.InputUpHold() || ActiveInputManager.InputDownHold() || ActiveInputManager.InputLeftHold() || ActiveInputManager.InputRightHold();
            if (Nudge)
            {
                Follow = false;
            }

            float Step = MapCursor.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float Along = 0f;
            float Across = 0f;
            if (ActiveInputManager.InputUpHold())
            {
                Along += 1f;
            }
            else if (ActiveInputManager.InputDownHold())
            {
                Along -= 1f;
            }

            if (ActiveInputManager.InputLeftHold())
            {
                Across -= 1f;
            }
            else if (ActiveInputManager.InputRightHold())
            {
                Across += 1f;
            }

            if (Along != 0f || Across != 0f)
            {
                LifeSimFreeCamera Life = Owner.Camera as LifeSimFreeCamera;
                float Yaw = Life == null ? 0f : Life.Yaw;
                float ForwardX = -(float)System.Math.Sin(Yaw);
                float ForwardZ = -(float)System.Math.Cos(Yaw);
                float RightX = -ForwardZ;
                float RightZ = ForwardX;
                float Scale = (float)System.Math.Sqrt(Along * Along + Across * Across);
                WorldX += (ForwardX * Along + RightX * Across) / Scale * Step;
                WorldY += (ForwardZ * Along + RightZ * Across) / Scale * Step;
            }

            if (Owner.KeyPressed(Keys.G))
            {
                Grid = !Grid;
            }
            else if (Owner.PreviousPressed())
            {
                Floor = BuildFloor.Shift(Floor, -1);
            }
            else if (Owner.NextPressed())
            {
                Floor = BuildFloor.Shift(Floor, 1);
            }

            SettleWorld();
        }

        public Vector3 AimPoint
        {
            get { return Aim; }
        }

        public int AimFloor
        {
            get { return Floor; }
        }

        public bool AimGrid
        {
            get { return Grid; }
        }

        public LifeSimMap AimMap()
        {
            return CurrentMap();
        }

        void EnsureCamera()
        {
            if (GameScreen.GraphicsDevice == null || Owner.Camera is LifeSimFreeCamera)
            {
                return;
            }

            Owner.Camera = new LifeSimFreeCamera(GameScreen.GraphicsDevice, Owner);
        }

        static int FloorUnder(MapInfo Home, Vector3 Local)
        {
            int Story = BuildFloor.Index(Local.Z);
            if (Home == null || Home.Heights == null)
            {
                return Story;
            }

            float Feet = HeightField.Feet(Home, Local.X, Local.Y);
            float StoryHeight = BuildFloor.Surface(Story);
            float FromGround = Local.Z - Feet;
            if (FromGround < 0f)
            {
                FromGround = -FromGround;
            }

            float FromStory = Local.Z - StoryHeight;
            if (FromStory < 0f)
            {
                FromStory = -FromStory;
            }

            if (FromGround <= FromStory)
            {
                return 0;
            }

            return Story;
        }

        void SeedWorld()
        {
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            MapInfo Here = Cursor.SharedMapContex == null ? null : Cursor.SharedMapContex.CurrentMapInfo;
            Vector3 Shift = ChunkOrigin.Shift(Here);
            WorldX = Cursor.Position.X + Shift.X;
            WorldY = Cursor.Position.Z + Shift.Z;
            HasWorld = true;
        }

        void SettleWorld()
        {
            EnsureCamera();
            TrackFollow();
            PlayerCharacter Cursor = Owner.InvisibleCharacterAsCursor;
            MapInfo Next = ChunkCross.At(WorldX, WorldY);
            if (Next != null)
            {
                MapManager.AwakeMap(Next);
            }
            else
            {
                HoldInChunk(Cursor.SharedMapContex == null ? null : Cursor.SharedMapContex.CurrentMapInfo);
            }

            bool Placed = ChunkCross.MoveCursor(Cursor, WorldX, WorldY, BuildFloor.Surface(Floor));
            LifeSimMap Map = CurrentMap();
            float Tile = MapCursor.TileOf(Map);
            Vector3 Ground;
            if (Placed)
            {
                Ground = new Vector3(Cursor.Position.X, Cursor.Position.Z, Cursor.Position.Y);
            }
            else
            {
                MapInfo Here = Cursor.SharedMapContex == null ? null : Cursor.SharedMapContex.CurrentMapInfo;
                float OriginX = Here == null || Here.MapMapContainer == null ? 0f : Here.MapMapContainer.OffsetX;
                float OriginY = Here == null || Here.MapMapContainer == null ? 0f : Here.MapMapContainer.OffsetY;
                Ground = MapCursor.Clamp(new Vector3(WorldX - OriginX, WorldY - OriginY, 0f), Map, Tile);
            }

            Aim = Ground;
            MapInfo Landed = Cursor.SharedMapContex == null ? null : Cursor.SharedMapContex.CurrentMapInfo;
            float Height = FollowHeight >= 0f ? FollowHeight : HeightField.View(Landed, Ground.X, Ground.Y, Floor);
            Vector3 Render = new Vector3(Ground.X, Height, Ground.Y);
            Cursor.SetPosition(Render);
            MapCursor.ShowTile(Map, Ground, Tile);
            if (Map != null)
            {
                Map.HideFloorsAbove = true;
                Map.ShownFloor = Floor;
                Map.ShowCursor = true;
            }

            TurnCamera();
            Vector3 Target = new Vector3(WorldX, Height, WorldY);
            LifeSimFreeCamera Life = Owner.Camera as LifeSimFreeCamera;
            if (Life != null)
            {
                Life.Aim(Target);
            }
            else
            {
                LifeSimFreeCamera.Frame(Owner.Camera, Target);
            }
            AimedPerson = PersonAt(Map, Ground, Tile);
            Aimed = BoxPlace.Under(CurrentBoxes(), Ground);
            if (Aimed != null && Aimed.Floor != Floor)
            {
                Aimed = null;
            }

            RefreshReach();
            Hint = Describe();
        }

        void TrackFollow()
        {
            FollowHeight = -1f;
            if (!Follow || Owner.Session == null)
            {
                return;
            }

            PlayerCharacter Who = Owner.Session.LifeSelection ?? AimedPerson;
            if (Who == null || Who.SharedMapContex == null)
            {
                Follow = false;
                return;
            }

            MapInfo Home = Who.SharedMapContex.CurrentMapInfo;
            Vector3 Shift = ChunkOrigin.Shift(Home);
            WorldX = Who.Position.X + Shift.X;
            WorldY = Who.Position.Y + Shift.Z;
            FollowHeight = Who.Position.Z + 18f;
            HasWorld = true;
        }

        void TurnCamera()
        {
            EnsureCamera();
            LifeSimFreeCamera Life = Owner.Camera as LifeSimFreeCamera;
            MouseState Now = Mouse.GetState();
            if (Life != null && HasMouse && Now.RightButton == ButtonState.Pressed)
            {
                Life.Orbit(Now.X - PreviousMouse.X, Now.Y - PreviousMouse.Y);
            }

            PreviousMouse = Now;
            HasMouse = true;
        }

        void HoldInChunk(MapInfo Here)
        {
            if (Here == null || Here.MapMapContainer == null)
            {
                return;
            }

            float X = Here.MapMapContainer.OffsetX;
            float Y = Here.MapMapContainer.OffsetY;
            float SpanX = Here.MapMapContainer.SpanX;
            float SpanY = Here.MapMapContainer.SpanY;
            if (SpanX <= 0f || SpanY <= 0f)
            {
                return;
            }

            float Limit = 0.01f;
            if (WorldX < X)
            {
                WorldX = X;
            }
            else if (WorldX > X + SpanX - Limit)
            {
                WorldX = X + SpanX - Limit;
            }

            if (WorldY < Y)
            {
                WorldY = Y;
            }
            else if (WorldY > Y + SpanY - Limit)
            {
                WorldY = Y + SpanY - Limit;
            }
        }

        void DungeonMove(GameTime gameTime)
        {
            DungeonRun Run = MapManager.Home.Run();
            int FloorIndex;
            if (!Run.FloorOf.TryGetValue(Owner.Session.PartyId, out FloorIndex))
            {
                Owner.Session.InDungeon = false;
                return;
            }

            Run.Visit(FloorIndex);
            KeyboardState Held = Keyboard.GetState();
            float WishX = 0f;
            float WishY = 0f;
            if (ActiveInputManager.InputLeftHold() || Held.IsKeyDown(Keys.Left) || Held.IsKeyDown(Keys.A))
            {
                WishX -= 1f;
            }
            else if (ActiveInputManager.InputRightHold() || Held.IsKeyDown(Keys.Right) || Held.IsKeyDown(Keys.D))
            {
                WishX += 1f;
            }

            if (ActiveInputManager.InputUpHold() || Held.IsKeyDown(Keys.Up) || Held.IsKeyDown(Keys.W))
            {
                WishY -= 1f;
            }
            else if (ActiveInputManager.InputDownHold() || Held.IsKeyDown(Keys.Down) || Held.IsKeyDown(Keys.S))
            {
                WishY += 1f;
            }

            PartyMember Member = Run.Party.Find(Owner.Session.PartyId);
            PlayerCharacter Who = Member != null && Member.Body != null ? Member.Body : Owner.Session.LifeSelection;
            bool Crouching = Held.IsKeyDown(Keys.LeftControl) || Held.IsKeyDown(Keys.RightControl);
            float Strength = 1f;
            if (Who != null)
            {
                if (Crouching)
                {
                    Who.CurrentPose = Pose.Crouch;
                    Strength = 0.5f;
                }
                else if (Who.CurrentPose == Pose.Crouch)
                {
                    Who.CurrentPose = Pose.Stand;
                }
            }

            if (!Crouching && (Held.IsKeyDown(Keys.LeftShift) || Held.IsKeyDown(Keys.RightShift)))
            {
                Strength = 1.75f;
            }

            if (Owner.JumpPressed())
            {
                Owner.Session.Hop = true;
            }

            bool Hop = Owner.Session.Hop;
            Owner.Session.Hop = false;
            Run.Stride(Owner.Session.PartyId, Who, WishX, WishY, Strength, (float)gameTime.ElapsedGameTime.TotalSeconds, Hop);
            int X = 0;
            int Y = 0;
            if (Run.CellX.ContainsKey(Owner.Session.PartyId))
            {
                X = Run.CellX[Owner.Session.PartyId];
                Y = Run.CellY[Owner.Session.PartyId];
            }

            Owner.Session.DungeonAimX = X;
            Owner.Session.DungeonAimY = Y;
            if (Owner.KeyPressed(Keys.Enter) || ActiveInputManager.InputConfirmPressed() || Owner.KeyPressed(Keys.F))
            {
                DungeonOrders.Confirm(MapManager, Owner.Session, X, Y);
            }

            EnsureCamera();
            DungeonView.Frame(Owner.Camera, Run, Owner.Session.PartyId);
        }

        void InviteHome(PlayerCharacter Who)
        {
            PartyMember Asker = MapManager.Home.Party.Find(Owner.Session.PartyId);
            if (Asker == null || Asker.Body == null || Who == null || Who == Asker.Body)
            {
                return;
            }

            int Residents = 0;
            for (int i = 0; i < MapManager.Home.Party.Roster.Count; i++)
            {
                PartyMember Member = MapManager.Home.Party.Roster[i];
                if (Member.Resident || Member.InParty)
                {
                    Residents++;
                }
            }

            CharacterRelationship Bond = Asker.Body.RelationshipWith(Who);
            if (!Social.CanInvite(Bond.Status, MapManager.Home.Stations.Beds, Residents))
            {
                return;
            }

            MapManager.Home.Party.InviteHome(Who.PartyId.Length > 0 ? Who.PartyId : Who.Name, Who.Name);
        }

        void EnterDungeon()
        {
            PlayerCharacter Who = Owner.Session.LifeSelection;
            if (Who != null && string.IsNullOrEmpty(Owner.Session.PartyId))
            {
                Owner.Session.PartyId = string.IsNullOrEmpty(Who.PartyId) ? Who.Name : Who.PartyId;
                Who.PartyId = Owner.Session.PartyId;
            }

            if (!MapManager.Home.Party.Started && MapManager.Home.Party.Find(Owner.Session.PartyId) == null)
            {
                PartyMember Member = MapManager.Home.Party.Pick(Owner.Session.Seat, Owner.Session.PartyId, Who == null ? Owner.Session.PartyId : Who.Name);
                if (Member != null)
                {
                    Member.Body = Who;
                }
            }

            DungeonRun Run = MapManager.Home.Run();
            if (Run.Floors.Count == 0)
            {
                Run.Wing = Biomes.Wing(CurrentInfo(), MapManager.Home);
            }

            if (Run.Enter(Owner.Session.PartyId))
            {
                Owner.Session.InDungeon = true;
                Owner.Session.DungeonAimX = MapManager.Home.Dungeon.CellX[Owner.Session.PartyId];
                Owner.Session.DungeonAimY = MapManager.Home.Dungeon.CellY[Owner.Session.PartyId];
            }
        }

        void Confirm(Vector3 Ground)
        {
            LifeSimMap Map = CurrentMap();
            float Tile = MapCursor.TileOf(Map);
            Ground = MapCursor.Clamp(Ground, Map, Tile);
            PlayerCharacter Person = PersonAt(Map, Ground, Tile);
            if (Owner.Session.LifeSelection == null)
            {
                if (Person != null)
                {
                    Owner.Session.LifeSelection = Person;
                }

                return;
            }

            Owner.Session.PracticeNote = "";
            RefreshReach();
            if (Reachable.Count == 1)
            {
                AddToPanelListAndSelect(new ActionPanelInteract(Owner, MapManager, ListActionMenuChoice, Reachable[0]));
                return;
            }

            if (Reachable.Count > 1)
            {
                AddToPanelListAndSelect(new ActionPanelInteract(Owner, MapManager, ListActionMenuChoice, new List<InteractTarget>(Reachable)));
                return;
            }

            Vector3 Spot = BuildFloor.SnapCenter(Ground, Tile, Grid);
            Spot.Z = HeightField.View(CurrentInfo(), Spot.X, Spot.Y, Floor);
            LifeOrder.Walk(Owner.Session.LifeSelection, Spot, Map);
        }

        void RefreshReach()
        {
            Reachable.Clear();
            Marked.Clear();
            PlayerCharacter Actor = Owner.Session == null ? null : Owner.Session.LifeSelection;
            InteractionContext Shared = new InteractionContext();
            Shared.Map = CurrentMap();
            Shared.Home = MapManager.Home;
            Shared.Session = Owner.Session;
            Shared.EnterDungeon = EnterDungeon;
            MapInfo Stood = CurrentInfo();
            IList<PlayerCharacter> NearPeople = Stood != null && Stood.People != null ? Stood.People : (Shared.Map == null ? null : Shared.Map.ListCharacter);
            List<InteractTarget> Found = LifeOrder.Nearby(Actor, Aim, Floor, CurrentBoxes(), NearPeople, Shared);
            for (int i = 0; i < Found.Count; i++)
            {
                Reachable.Add(Found[i]);
                if (Found[i].Box != null)
                {
                    Marked.Add(Found[i].Box);
                }
            }
        }

        PlayerCharacter PersonAt(LifeSimMap Map, Vector3 Ground, float Tile)
        {
            if (Map == null || Map.ListCharacter == null || Tile <= 0f)
            {
                return null;
            }

            int X = (int)System.Math.Floor(Ground.X / Tile);
            int Y = (int)System.Math.Floor(Ground.Y / Tile);
            PlayerCharacter Best = null;
            float BestDistance = float.MaxValue;
            for (int i = 0; i < Map.ListCharacter.Count; i++)
            {
                PlayerCharacter Who = Map.ListCharacter[i];
                if (Who == null || BuildFloor.Index(Who.Position.Z) != Floor)
                {
                    continue;
                }

                if ((int)System.Math.Floor(Who.Position.X / Tile) != X || (int)System.Math.Floor(Who.Position.Y / Tile) != Y)
                {
                    continue;
                }

                float DX = Who.Position.X - Ground.X;
                float DY = Who.Position.Y - Ground.Y;
                float Distance = DX * DX + DY * DY;
                if (Distance < BestDistance)
                {
                    Best = Who;
                    BestDistance = Distance;
                }
            }

            return Best;
        }

        string Describe()
        {
            PlayerCharacter Chosen = Owner.Session == null ? null : Owner.Session.LifeSelection;
            if (Chosen == null)
            {
                if (AimedPerson != null)
                {
                    return "Enter selects " + AimedPerson.Name;
                }

                return "Aim at a character";
            }

            if (Owner.Session != null && !string.IsNullOrEmpty(Owner.Session.PracticeNote))
            {
                return Owner.Session.PracticeNote;
            }

            if (Reachable.Count > 1)
            {
                return "Enter chooses what " + Chosen.Name + " uses";
            }

            if (Reachable.Count == 1)
            {
                return "Enter opens " + Reachable[0].Label;
            }

            return "Enter sends " + Chosen.Name + " here";
        }

        IList<FurnitureItem> CurrentBoxes()
        {
            MapInfo Here = CurrentInfo();
            return Here == null ? null : Here.Furniture;
        }

        MapInfo CurrentInfo()
        {
            if (Owner.InvisibleCharacterAsCursor.SharedMapContex == null)
            {
                return null;
            }

            return Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
        }

        LifeSimMap CurrentMap()
        {
            MapInfo Here = CurrentInfo();
            if (Here == null || Here.MapMapContainer == null)
            {
                return null;
            }

            return Here.MapMapContainer.ActiveMap;
        }

        protected override void OnCancelPanel()
        {
            LifeSimMap Map = CurrentMap();
            if (Map != null)
            {
                Map.HideFloorsAbove = false;
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelLifeCursor(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            bool Aiming = Owner.ActionHolder.HasSubPanels && Owner.ActionHolder.Last() is ActionPanelPracticeStride;
            bool Active = !Owner.ActionHolder.HasSubPanels || Aiming;
            if (Active)
            {
                EnsureCamera();
            }
            bool Fighting = Owner.Session != null && Owner.Session.Practice != null;
            if (Active && !HasWorld)
            {
                SeedWorld();
                SettleWorld();
            }

            MapInfo Here = CurrentInfo();
            LifeSimMap HereMap = CurrentMap();
            if (Active && HereMap != null)
            {
                MapCursor.ShowTile(HereMap, Aim, MapCursor.TileOf(HereMap));
            }

            MapManager.DrawChunks(g, Owner.Camera, delegate (LifeSimMap Map)
            {
                if (Active)
                {
                    Map.HideFloorsAbove = true;
                    Map.ShownFloor = Floor;
                }

                Map.ShowCursor = Here != null && (object.ReferenceEquals(Map.Chunk, Here) || (Here.MapMapContainer != null && object.ReferenceEquals(Here.MapMapContainer.ActiveMap, Map)));
            }, Owner.ZoneDraft, Owner.Session);

            if (Owner.Camera != null && Marked.Count > 0 && (Owner.Session == null || !Owner.Session.InDungeon))
            {
                g.End();
                PlacedBoxDraw.Highlight(g.GraphicsDevice, Marked, Owner.Camera.View, Owner.Camera.Projection, ChunkOrigin.Shift(Here));
                g.Begin();
            }

            if (Owner.Camera != null && Aimed != null && (Owner.Session == null || !Owner.Session.InDungeon))
            {
                g.End();
                PlacedBoxDraw.Ghost(g.GraphicsDevice, Aimed, Owner.Session != null && Owner.Session.LifeSelection != null, Owner.Camera.View, Owner.Camera.Projection, ChunkOrigin.Shift(Here));
                g.Begin();
            }

            MapManager.BeginOverlay(g);
            string Who = Owner.Session != null && Owner.Session.LifeSelection != null ? Owner.Session.LifeSelection.Name : "Nobody";
            if (Owner.Session != null && Owner.Session.InDungeon && MapManager.Home != null && MapManager.Home.Dungeon != null)
            {
                int Depth = 0;
                MapManager.Home.Dungeon.FloorOf.TryGetValue(Owner.Session.PartyId, out Depth);
                TextHelper.DrawText(g, "Player " + Owner.SeatIndex + "   " + Who + "   Dungeon " + (Depth + 1), new Vector2(40, 20), Color.White);
                TextHelper.DrawText(g, "Arrows walk    Shift runs    Space jumps    Ctrl crouches    Enter uses the stair    F attacks", new Vector2(40, 48), Color.LightGray);
                return;
            }

            string GridLabel = Grid ? "Grid On" : "Grid Off";
            TextHelper.DrawText(g, "Player " + Owner.SeatIndex + "   " + Who + "   Floor " + (Floor + 1) + "   " + GridLabel, new Vector2(40, 20), Color.White);
            if (Fighting)
            {
                return;
            }

            TextHelper.DrawText(g, Hint, new Vector2(40, 48), Color.LightGray);
            TextHelper.DrawText(g, LandLine(), new Vector2(40, 76), Color.Gold);
            TextHelper.DrawText(g, "Arrows move    G grid    Q/E floor    V view    B buy land", new Vector2(40, 100), Color.LightGray);
            DrawOrders(g);
            DrawRequests(g);
        }

        void DrawRequests(CustomSpriteBatch g)
        {
            if (MapManager.Home == null || MapManager.Home.Requests.Count == 0)
            {
                return;
            }

            float Y = 360f;
            TextHelper.DrawText(g, "Requests", new Vector2(40, Y), Color.White);
            Y += 22f;
            for (int i = 0; i < MapManager.Home.Requests.Count; i++)
            {
                HouseholdRequest Note = MapManager.Home.Requests[i];
                if (Note == null || !Note.Open)
                {
                    continue;
                }

                TextHelper.DrawText(g, Note.Who + " wants " + Note.Item + ". " + Note.Why, new Vector2(40, Y), Color.Gold);
                Y += 22f;
            }
        }

        string LandLine()
        {
            if (!string.IsNullOrEmpty(LandNote))
            {
                return LandNote;
            }

            BuildingZone Zone = BuildingZones.At(CurrentInfo(), Aim.X, Aim.Y);
            if (Zone == null)
            {
                return "B buys the plot under the cursor";
            }

            int Funds = Owner.Session == null ? 0 : Owner.Session.Funds;
            string Buyer = Owner.Session == null ? null : Owner.Session.HouseholdId;
            return PlotSale.Offer(Zone, Buyer, Funds);
        }

        void DrawOrders(CustomSpriteBatch g)
        {
            PlayerCharacter Who = Owner.Session == null ? null : Owner.Session.LifeSelection;
            if (Who == null)
            {
                return;
            }

            float Y = 132f;
            TextHelper.DrawText(g, "Orders", new Vector2(40, Y), Color.White);
            Y += 22f;
            if (Who.ListAIObjective.Count == 0)
            {
                TextHelper.DrawText(g, "none", new Vector2(40, Y), Color.Gray);
                return;
            }

            for (int n = 0; n < Who.ListAIObjective.Count; n++)
            {
                AIObjective Order = Who.ListAIObjective[Who.ListAIObjective.Count - 1 - n];
                bool Current = n == 0;
                string Line = (Current ? "> " : "  ") + (n + 1) + "  " + Order.GoalName + "  " + StepNames(Order.ListRemainingStep);
                TextHelper.DrawText(g, Line, new Vector2(40, Y), Current ? Color.White : Color.Gray);
                Y += 22f;
            }

            if (Who.ListCurrentAIObjectiveStep == null)
            {
                return;
            }

            for (int A = 0; A < Who.ListCurrentAIObjectiveStep.Count; A++)
            {
                Who.ListCurrentAIObjectiveStep[A].DrawIcon(g, new Vector2(40 + A * 40, Y));
            }
        }

        static string StepNames(List<AutomatedAction> Steps)
        {
            if (Steps == null || Steps.Count == 0)
            {
                return "(empty)";
            }

            string Names = Steps[0].ActionName;
            for (int i = 1; i < Steps.Count; i++)
            {
                Names += ", " + Steps[i].ActionName;
            }

            return Names;
        }
    }
}
