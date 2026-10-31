using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelCities : ActionPanelLifeSimPlayer
    {
        public ActionPanelCities(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("Cities", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            TownLife.Collect(MapManager);
            TownBook Book = TownLife.Book;
            if (Book == null || Book.Towns.Count == 0)
            {
                AddChoiceToCurrentPanel(new ActionPanelCityLine(Owner, MapManager, ListActionMenuChoice, "No towns"));
                return;
            }

            for (int t = 0; t < Book.Towns.Count; t++)
            {
                List<TownPerson> Here = Book.Group(t);
                for (int i = 0; i < Here.Count; i++)
                {
                    if (!Here[i].Playable)
                    {
                        continue;
                    }

                    string Label = Book.Towns[t].Name + ": " + Here[i].Name;
                    AddChoiceToCurrentPanel(new ActionPanelCityPerson(Owner, MapManager, ListActionMenuChoice, Here[i].Id, Label));
                }
            }

            List<TownPerson> Road = Book.OnTheRoad();
            for (int i = 0; i < Road.Count; i++)
            {
                if (!Road[i].Playable)
                {
                    continue;
                }

                AddChoiceToCurrentPanel(new ActionPanelCityPerson(Owner, MapManager, ListActionMenuChoice, Road[i].Id, "Traveling: " + Road[i].Name));
            }

            PlayerCharacter Who = Owner.Session == null ? null : Owner.Session.Controlled;
            if (Who == null || Who.CityId < 0 || Who.CityId >= Book.Towns.Count || TownLife.Riding(Who))
            {
                return;
            }

            AddChoiceToCurrentPanel(new ActionPanelCityGather(Owner, MapManager, ListActionMenuChoice));
            AddChoiceToCurrentPanel(new ActionPanelCityCarryPeople(Owner, MapManager, ListActionMenuChoice, Who.CityId));
            AddChoiceToCurrentPanel(new ActionPanelCityMailTowns(Owner, MapManager, ListActionMenuChoice, Who.CityId));
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
            return new ActionPanelCities(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityLine : ActionPanelLifeSimPlayer
    {
        readonly string Label;

        public ActionPanelCityLine(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Label)
            : base("CityLine", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Label = Label;
        }

        public override void OnSelect()
        {
            RemoveAllSubActionPanels();
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
            return new ActionPanelCityLine(Owner, MapManager, ListActionMenuChoice, Label);
        }

        public override string ToString()
        {
            return Label;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelCityPerson : ActionPanelLifeSimPlayer
    {
        readonly string Id;
        readonly string Label;

        public ActionPanelCityPerson(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Id, string Label)
            : base("CityPerson", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Id = Id;
            this.Label = Label;
        }

        public override void OnSelect()
        {
            PlayerCharacter Who = TownLife.Possess(MapManager, Id);
            RemoveAllSubActionPanels();
            if (Who != null)
            {
                Owner.ControlCharacter(Who);
            }
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
            return new ActionPanelCityPerson(Owner, MapManager, ListActionMenuChoice, Id, Label);
        }

        public override string ToString()
        {
            return Label;
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelCityGather : ActionPanelLifeSimPlayer
    {
        public ActionPanelCityGather(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice)
            : base("CityGather", Owner, MapManager, ListActionMenuChoice, true)
        {
        }

        public override void OnSelect()
        {
            PlayerCharacter Who = Owner.Session == null ? null : Owner.Session.Controlled;
            TownLife.Gather(Who);
            RemoveAllSubActionPanels();
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
            return new ActionPanelCityGather(Owner, MapManager, ListActionMenuChoice);
        }

        public override string ToString()
        {
            return "Gather";
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelCityCarryPeople : ActionPanelLifeSimPlayer
    {
        readonly int Town;

        public ActionPanelCityCarryPeople(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, int Town)
            : base("CityCarryPeople", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Town = Town;
        }

        public override void OnSelect()
        {
            TownBook Book = TownLife.Book;
            if (Book == null)
            {
                return;
            }

            List<TownPerson> Here = Book.Group(Town);
            for (int i = 0; i < Here.Count; i++)
            {
                if (!Here[i].Playable)
                {
                    continue;
                }

                AddChoiceToCurrentPanel(new ActionPanelCityCarryTowns(Owner, MapManager, ListActionMenuChoice, Here[i].Id, Town));
            }

            if (Here.Count == 0)
            {
                AddChoiceToCurrentPanel(new ActionPanelCityLine(Owner, MapManager, ListActionMenuChoice, "Nobody here"));
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
            return new ActionPanelCityCarryPeople(Owner, MapManager, ListActionMenuChoice, Town);
        }

        public override string ToString()
        {
            return "Carry";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityCarryTowns : ActionPanelLifeSimPlayer
    {
        readonly string Id;
        readonly int From;

        public ActionPanelCityCarryTowns(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Id, int From)
            : base("CityCarryTowns", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Id = Id;
            this.From = From;
        }

        public override void OnSelect()
        {
            TownBook Book = TownLife.Book;
            if (Book == null)
            {
                return;
            }

            for (int i = 0; i < Book.Towns.Count; i++)
            {
                if (i == From)
                {
                    continue;
                }

                AddChoiceToCurrentPanel(new ActionPanelCityCarryGoods(Owner, MapManager, ListActionMenuChoice, Id, From, i));
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
            return new ActionPanelCityCarryTowns(Owner, MapManager, ListActionMenuChoice, Id, From);
        }

        public override string ToString()
        {
            TownPerson Person = TownLife.Book == null ? null : TownLife.Book.Find(Id);
            return Person == null ? "Carry" : Person.Name;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityCarryGoods : ActionPanelLifeSimPlayer
    {
        readonly string Id;
        readonly int From;
        readonly int Destination;

        public ActionPanelCityCarryGoods(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Id, int From, int Destination)
            : base("CityCarryGoods", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Id = Id;
            this.From = From;
            this.Destination = Destination;
        }

        public override void OnSelect()
        {
            TownBook Book = TownLife.Book;
            if (Book == null)
            {
                return;
            }

            Dictionary<string, int> Pile = Book.Pile(From, MapManager.Home.Pantry);
            bool Any = false;
            if (Pile != null)
            {
                foreach (KeyValuePair<string, int> Item in Pile)
                {
                    if (Item.Value <= 0)
                    {
                        continue;
                    }

                    Any = true;
                    AddChoiceToCurrentPanel(new ActionPanelCityCarrySend(Owner, MapManager, ListActionMenuChoice, Id, Destination, Item.Key, Item.Value));
                }
            }

            if (!Any)
            {
                AddChoiceToCurrentPanel(new ActionPanelCityLine(Owner, MapManager, ListActionMenuChoice, "Nothing stored"));
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
            return new ActionPanelCityCarryGoods(Owner, MapManager, ListActionMenuChoice, Id, From, Destination);
        }

        public override string ToString()
        {
            TownBook Book = TownLife.Book;
            if (Book == null || Destination < 0 || Destination >= Book.Towns.Count)
            {
                return "To";
            }

            return Book.Towns[Destination].Name;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityCarrySend : ActionPanelLifeSimPlayer
    {
        readonly string Id;
        readonly int Destination;
        readonly string Item;
        readonly int Count;

        public ActionPanelCityCarrySend(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Id, int Destination, string Item, int Count)
            : base("CityCarrySend", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Id = Id;
            this.Destination = Destination;
            this.Item = Item;
            this.Count = Count;
        }

        public override void OnSelect()
        {
            TownLife.Send(Id, Destination, Item, Count);
            RemoveAllSubActionPanels();
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
            return new ActionPanelCityCarrySend(Owner, MapManager, ListActionMenuChoice, Id, Destination, Item, Count);
        }

        public override string ToString()
        {
            return Item + " " + Count.ToString();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelCityMailTowns : ActionPanelLifeSimPlayer
    {
        readonly int From;

        public ActionPanelCityMailTowns(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, int From)
            : base("CityMailTowns", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.From = From;
        }

        public override void OnSelect()
        {
            TownBook Book = TownLife.Book;
            if (Book == null)
            {
                return;
            }

            for (int i = 0; i < Book.Towns.Count; i++)
            {
                if (i == From)
                {
                    continue;
                }

                AddChoiceToCurrentPanel(new ActionPanelCityMailGoods(Owner, MapManager, ListActionMenuChoice, From, i));
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
            return new ActionPanelCityMailTowns(Owner, MapManager, ListActionMenuChoice, From);
        }

        public override string ToString()
        {
            return "Mail";
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityMailGoods : ActionPanelLifeSimPlayer
    {
        readonly int From;
        readonly int Destination;

        public ActionPanelCityMailGoods(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, int From, int Destination)
            : base("CityMailGoods", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.From = From;
            this.Destination = Destination;
        }

        public override void OnSelect()
        {
            TownBook Book = TownLife.Book;
            if (Book == null)
            {
                return;
            }

            Dictionary<string, int> Pile = Book.Pile(From, MapManager.Home.Pantry);
            bool Any = false;
            if (Pile != null)
            {
                foreach (KeyValuePair<string, int> Item in Pile)
                {
                    if (Item.Value <= 0)
                    {
                        continue;
                    }

                    Any = true;
                    AddChoiceToCurrentPanel(new ActionPanelCityMailSend(Owner, MapManager, ListActionMenuChoice, From, Destination, Item.Key, Item.Value));
                }
            }

            if (!Any)
            {
                AddChoiceToCurrentPanel(new ActionPanelCityLine(Owner, MapManager, ListActionMenuChoice, "Nothing stored"));
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
            return new ActionPanelCityMailGoods(Owner, MapManager, ListActionMenuChoice, From, Destination);
        }

        public override string ToString()
        {
            TownBook Book = TownLife.Book;
            if (Book == null || Destination < 0 || Destination >= Book.Towns.Count)
            {
                return "To";
            }

            return Book.Towns[Destination].Name;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelCityMailSend : ActionPanelLifeSimPlayer
    {
        readonly int From;
        readonly int Destination;
        readonly string Item;
        readonly int Count;

        public ActionPanelCityMailSend(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, int From, int Destination, string Item, int Count)
            : base("CityMailSend", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.From = From;
            this.Destination = Destination;
            this.Item = Item;
            this.Count = Count;
        }

        public override void OnSelect()
        {
            TownLife.Post(From, Destination, Item, Count, TownLife.Now(MapManager));
            RemoveAllSubActionPanels();
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
            return new ActionPanelCityMailSend(Owner, MapManager, ListActionMenuChoice, From, Destination, Item, Count);
        }

        public override string ToString()
        {
            return Item + " " + Count.ToString();
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
