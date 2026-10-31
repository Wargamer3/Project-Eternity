using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ActionPanelInteract : ActionPanelLifeSimPlayer
    {
        readonly List<InteractTarget> Targets;
        readonly InteractTarget Single;

        public ActionPanelInteract(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, List<InteractTarget> Targets)
            : base("Interact", Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Targets = Targets ?? new List<InteractTarget>();
            Place();
        }

        public ActionPanelInteract(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, InteractTarget Single)
            : base(Single == null || string.IsNullOrEmpty(Single.Label) ? "Interact" : Single.Label, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Single = Single;
            Place();
        }

        void Place()
        {
            BaseMenuX = 360;
            BaseMenuY = 140;
            UpdateFinalMenuPosition();
        }

        public override void OnSelect()
        {
            ListNextChoice.Clear();
            ActionMenuCursor = 0;
            if (Single != null)
            {
                FillActions(Single);
            }
            else if (Targets != null)
            {
                for (int i = 0; i < Targets.Count; i++)
                {
                    AddChoiceToCurrentPanel(new ActionPanelInteractTarget(Owner, MapManager, ListActionMenuChoice, Targets[i]));
                }
            }

            UpdateFinalMenuPosition();
        }

        void FillActions(InteractTarget Target)
        {
            if (Target == null || Target.Choices == null)
            {
                return;
            }

            for (int i = 0; i < Target.Choices.Count; i++)
            {
                InteractionChoice Choice = Target.Choices[i];
                AddChoiceToCurrentPanel(new ActionPanelInteractRun(Owner, MapManager, ListActionMenuChoice, Choice.Label, Choice.Run));
            }

            if (Target.Person != null && (Owner.Session == null || Target.Person != Owner.Session.LifeSelection))
            {
                PlayerCharacter Who = Target.Person;
                AddChoiceToCurrentPanel(new ActionPanelInteractRun(Owner, MapManager, ListActionMenuChoice, "Control", () =>
                {
                    if (Owner.Session != null)
                    {
                        Owner.Session.LifeSelection = Who;
                    }
                }));
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

            Owner.EndKeys();
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
            if (Single != null)
            {
                return new ActionPanelInteract(Owner, MapManager, ListActionMenuChoice, Single);
            }

            return new ActionPanelInteract(Owner, MapManager, ListActionMenuChoice, Targets);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawNextChoice(g);
        }
    }

    public class ActionPanelInteractTarget : ActionPanelLifeSimPlayer
    {
        readonly InteractTarget Target;

        public ActionPanelInteractTarget(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, InteractTarget Target)
            : base(Target == null || string.IsNullOrEmpty(Target.Label) ? "Something" : Target.Label, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Target = Target;
        }

        public override void OnSelect()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelInteract(Owner, MapManager, ListActionMenuChoice, Target));
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
            return new ActionPanelInteractTarget(Owner, MapManager, ListActionMenuChoice, Target);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }

    public class ActionPanelInteractRun : ActionPanelLifeSimPlayer
    {
        readonly Action Run;

        public ActionPanelInteractRun(PlayerOverseer Owner, NavMapGameManager MapManager, ActionPanelHolder ListActionMenuChoice, string Label, Action Run)
            : base(string.IsNullOrEmpty(Label) ? "Use" : Label, Owner, MapManager, ListActionMenuChoice, true)
        {
            this.Run = Run;
        }

        public override void OnSelect()
        {
            if (Run != null)
            {
                Run();
            }

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
            return new ActionPanelInteractRun(Owner, MapManager, ListActionMenuChoice, Name, Run);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
