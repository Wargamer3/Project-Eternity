using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelPracticeMenu : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeMenu(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("Practice", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            AddChoiceToCurrentPanel(new ActionPanelPracticeStride(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelPracticeStrike(Owner, MapManager, ListActionMenuChoice));
            string Versatile = VersatileChoice();
            if (!string.IsNullOrEmpty(Versatile))
            {
                AddChoiceToCurrentPanel(new ActionPanelPracticeTypedStrike(Owner, MapManager, ListActionMenuChoice, Versatile));
            }

            AddChoiceToCurrentPanel(new ActionPanelPracticeEndTurn(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelPracticeLog(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelPracticeLeave(Owner, MapManager, ListActionMenuChoice));
            if (ActionMenuCursor >= ListNextChoice.Count)
            {
                ActionMenuCursor = 0;
            }

            Watch();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.TakeToolKeys(this))
            {
                return;
            }

            if (Owner.Session == null || Owner.Session.Practice == null)
            {
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            Watch();
            if (NavigateThroughNextChoices(MapManager.sndSelection))
            {
            }
            else if (ConfirmNextChoices(MapManager.sndConfirm))
            {
            }

            Owner.EndKeys();
        }

        string VersatileChoice()
        {
            if (Owner.Session == null || Owner.Session.Practice == null || Owner.Session.Practice.Fighter == null)
            {
                return null;
            }

            return PracticeMath.VersatileOffer(Owner.Session.Practice.Fighter.EquippedWeapon());
        }

        void Watch()
        {
            if (Owner.Session != null && Owner.Session.Practice != null)
            {
                Owner.Session.WatchFight(Owner.Session.Practice.Fighter);
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
            return new ActionPanelPracticeMenu(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            PracticeEncounter Fight = Owner.Session == null ? null : Owner.Session.Practice;
            if (Fight == null)
            {
                return;
            }

            string Line = "Actions " + Fight.ActionsLeft + "    Dummy " + Fight.HP + "/" + PracticeEncounter.StartingHP;
            if (!string.IsNullOrEmpty(Fight.LastResult))
            {
                Line += "    " + Fight.LastResult;
            }

            TextHelper.DrawText(g, Line, new Vector2(40, 48), Color.LightGray);
            TextHelper.DrawText(g, "Up/Down choose    Enter confirm", new Vector2(40, 76), Color.LightGray);
            DrawNextChoice(g);
        }
    }

    public class ActionPanelPracticeStride : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeStride(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PracticeStride", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            PracticeEncounter Fight = Owner.Session == null ? null : Owner.Session.Practice;
            if (Fight == null)
            {
                RemoveFromPanelList(this);
                return;
            }

            if (Fight.ActionsLeft <= 0)
            {
                Fight.LastResult = "No actions left.";
                RemoveFromPanelList(this);
                return;
            }

            if (PracticeMath.Busy(Fight.Fighter))
            {
                Fight.LastResult = "Still moving.";
                RemoveFromPanelList(this);
                return;
            }

            Fight.LastResult = "Choose a place to stride.";
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Owner.TakeToolKeys(this))
            {
                return;
            }

            PracticeEncounter Fight = Owner.Session == null ? null : Owner.Session.Practice;
            ActionPanelLifeCursor Cursor = ListActionMenuChoice.GetMainPanel() as ActionPanelLifeCursor;
            if (Fight == null || Cursor == null)
            {
                RemoveFromPanelList(this);
                Owner.EndKeys();
                return;
            }

            Owner.Session.WatchFight(Fight.Fighter);
            Cursor.MoveAim(gameTime);
            if (ActiveInputManager.InputCancelPressed() || Owner.KeyPressed(Keys.Back))
            {
                RemoveFromPanelList(this);
            }
            else if (Owner.KeyPressed(Keys.Enter) || ActiveInputManager.InputConfirmPressed())
            {
                TryStride(Fight, Cursor);
                RemoveFromPanelList(this);
            }

            Owner.EndKeys();
        }

        void TryStride(PracticeEncounter Fight, ActionPanelLifeCursor Cursor)
        {
            if (Fight.ActionsLeft <= 0)
            {
                Fight.LastResult = "No actions left.";
                return;
            }

            if (PracticeMath.Busy(Fight.Fighter))
            {
                Fight.LastResult = "Still moving.";
                return;
            }

            LifeSimMap Map = Cursor.AimMap();
            float Tile = MapCursor.TileOf(Map);
            Vector3 Spot = BuildFloor.SnapCenter(Cursor.AimPoint, Tile, Cursor.AimGrid);
            Spot.Z = HeightField.View(Map == null ? null : Map.Chunk, Spot.X, Spot.Y, Cursor.AimFloor);
            if (!LifeOrder.Stride(Fight.Fighter, Spot, Map, Fight.Dummy))
            {
                Fight.LastResult = "Can't stride there.";
                return;
            }

            Fight.ActionsLeft--;
            Fight.LastResult = "Stride. " + Fight.ActionsLeft + " actions left.";
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPracticeStride(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Stride";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            PracticeEncounter Fight = Owner.Session == null ? null : Owner.Session.Practice;
            string Line = "Enter strides. Cancel returns.";
            if (Fight != null && !string.IsNullOrEmpty(Fight.LastResult))
            {
                Line += "  " + Fight.LastResult;
            }

            TextHelper.DrawText(g, Line, new Vector2(40, 48), Color.LightGray);
        }
    }

    public class ActionPanelPracticeStrike : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeStrike(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PracticeStrike", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            if (Owner.Session != null)
            {
                Owner.Session.StrikePractice();
            }

            if (Owner.Session == null || Owner.Session.Practice != null)
            {
                RemoveFromPanelList(this);
            }
        }

        public override void DoUpdate(GameTime gameTime)
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
            return new ActionPanelPracticeStrike(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Strike";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPracticeTypedStrike : ActionPanelLifeSimPlayer
    {
        readonly string RequestedType;

        public ActionPanelPracticeTypedStrike(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string RequestedType)
            : base("PracticeTypedStrike", Owner, MapManager, ListActionMenuChoice, false)
        {
            this.RequestedType = RequestedType;
        }

        public override void OnSelect()
        {
            if (Owner.Session != null)
            {
                Owner.Session.StrikePractice(RequestedType);
            }

            if (Owner.Session == null || Owner.Session.Practice != null)
            {
                RemoveFromPanelList(this);
            }
        }

        public override void DoUpdate(GameTime gameTime)
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
            return new ActionPanelPracticeTypedStrike(Owner, MapManager, ListActionMenuChoice, RequestedType);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(RequestedType))
            {
                return "Strike";
            }

            return char.ToUpper(RequestedType[0]) + RequestedType.Substring(1);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPracticeEndTurn : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeEndTurn(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PracticeEndTurn", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            if (Owner.Session != null)
            {
                Owner.Session.EndPracticeTurn();
            }

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
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
            return new ActionPanelPracticeEndTurn(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "End Turn";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPracticeLog : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeLog(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PracticeLog", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            PlayerCharacter Who = Owner.Session != null && Owner.Session.Practice != null ? Owner.Session.Practice.Fighter : null;
            if (Who != null)
            {
                AddToPanelListAndSelect(new ActionPanelViewCharacterLogs(Owner, Who, MapManager, ListActionMenuChoice));
            }

            RemoveFromPanelList(this);
        }

        public override void DoUpdate(GameTime gameTime)
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
            return new ActionPanelPracticeLog(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Log";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelPracticeLeave : ActionPanelLifeSimPlayer
    {
        public ActionPanelPracticeLeave(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("PracticeLeave", Owner, MapManager, ListActionMenuChoice, false)
        {
        }

        public override void OnSelect()
        {
            if (Owner.Session != null)
            {
                Owner.Session.EndPractice();
            }
        }

        public override void DoUpdate(GameTime gameTime)
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
            return new ActionPanelPracticeLeave(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Leave";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
