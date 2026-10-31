using System;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelJoinParty : ActionPanelLifeSimPlayer
    {
        public ActionPanelJoinParty(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("JoinParty", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            DungeonParty Party = MapManager.Home.Party;
            JoinKind Kind = DungeonParty.Kind(Party.Started, false, Party.Uncontrolled());
            if (Kind == JoinKind.PickNew)
            {
                int Added = 0;
                try
                {
                    string[] Files = Directory.GetFiles("Content/Life Sim/Characters/", "*.pec");
                    for (int i = 0; i < Files.Length; i++)
                    {
                        string File = Path.GetFileNameWithoutExtension(Files[i]);
                        AddChoiceToCurrentPanel(new ActionPanelClaimCharacter(Owner, MapManager, ListActionMenuChoice, File, false));
                        Added++;
                    }
                }
                catch (Exception)
                {
                    Added = 0;
                }

                if (Added == 0)
                {
                    AddChoiceToCurrentPanel(new ActionPanelClaimCharacter(Owner, MapManager, ListActionMenuChoice, "Wanderer", false));
                }
            }
            else if (Kind == JoinKind.TakeExisting)
            {
                for (int i = 0; i < Party.Roster.Count; i++)
                {
                    PartyMember Who = Party.Roster[i];
                    if (Who.Seat < 0)
                    {
                        AddChoiceToCurrentPanel(new ActionPanelClaimCharacter(Owner, MapManager, ListActionMenuChoice, Who.Id, true));
                    }
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelJoinParty(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            string Line = MapManager.Home.Party.Started ? "Choose someone already here" : "Choose your character";
            if (MapManager.Home.Party.Started && MapManager.Home.Party.Uncontrolled() == 0)
            {
                Line = "Wait for a free character";
            }

            TextHelper.DrawText(g, Line, new Vector2(40, 48), Color.White);
            DrawNextChoice(g);
        }

        public override string ToString()
        {
            return "Join";
        }
    }

    public class ActionPanelClaimCharacter : ActionPanelLifeSimPlayer
    {
        readonly string Key;
        readonly bool Existing;

        public ActionPanelClaimCharacter(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Key, bool Existing)
            : base("ClaimCharacter", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Key = Key;
            this.Existing = Existing;
        }

        public override void OnSelect()
        {
            PlayerCharacter Who = Existing ? TakeExisting() : PickNew();
            MapManager.Home.Party.Note(MapManager.ListLocalPlayer.Count);
            RemoveAllSubActionPanels();
            if (Who != null)
            {
                Owner.ControlCharacter(Who);
            }
        }

        PlayerCharacter PickNew()
        {
            PlayerCharacter Who = null;
            MapInfo Here = Owner.InvisibleCharacterAsCursor.SharedMapContex.CurrentMapInfo;
            if (Key != "Wanderer" && MapManager.Content != null)
            {
                try
                {
                    Who = new PlayerCharacter(Key, MapManager.Content, MapManager, Here);
                }
                catch (Exception)
                {
                    Who = null;
                }
            }

            if (Who == null)
            {
                Who = Spellcast.Unlisted(Key);
                Who.Side = SkirmishSide.Hero;
                if (Who.SharedMapContex != null)
                {
                    Who.SharedMapContex.RootMapContainer = MapManager;
                    Who.SharedMapContex.CurrentMapInfo = Here;
                }
            }

            Wear(Who);
            string Id = Who.Name + "#" + Owner.Session.Seat;
            Who.PartyId = Id;
            LifeSimMap Map = Here == null || Here.MapMapContainer == null ? null : Here.MapMapContainer.ActiveMap as LifeSimMap;
            Vector3 Spot = CursorGround.From(Owner.InvisibleCharacterAsCursor.Position);
            if (Here != null && Here.Heights != null)
            {
                Spot.Z = HeightField.Stand(Here, Spot.X, Spot.Y);
            }
            else
            {
                Spot.Z += 1f;
            }

            if (Map != null)
            {
                Map.SpawnCharacter(Who, 0, Spot);
            }
            else if (Here != null)
            {
                Who.SetPosition(Spot);
                Who.Body.CanJump = true;
                Who.SeedBelief();
                Who.Init();
                Here.Adopt(Who);
            }

            TownLife.FileStanding(Who);

            Who.AIControlled = false;
            PartyMember Member = MapManager.Home.Party.Pick(Owner.Session.Seat, Id, Who.Name);
            if (Member != null)
            {
                Member.Body = Who;
                Owner.Session.PartyId = Id;
                Owner.Session.LifeSelection = Who;
            }

            return Who;
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

        PlayerCharacter TakeExisting()
        {
            if (!MapManager.Home.Party.LateJoin(Owner.Session.Seat, Key))
            {
                return null;
            }

            PartyMember Member = MapManager.Home.Party.Find(Key);
            Owner.Session.PartyId = Key;
            if (Member == null || Member.Body == null)
            {
                return null;
            }

            Member.Body.PartyId = Key;
            Owner.Session.LifeSelection = Member.Body;
            return Member.Body;
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelClaimCharacter(Owner, MapManager, ListActionMenuChoice, Key, Existing);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
