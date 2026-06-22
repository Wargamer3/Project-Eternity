using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Online;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class AttackActionEffect : ActionEffect
    {
        private Weapon ReferenceWeapon;

        public LifeSimManualSkillTargetType TargetType;
        public List<BaseEffect> ListEffect;

        public AttackActionEffect()
            : base("Attack")
        {
        }

        public AttackActionEffect(BinaryReader BR)
            : this()
        {
        }

        public override void DoWrite(BinaryWriter BW)
        {
        }

        public override void OnEquip()
        {/*
            foreach (Weapon ActiveWeapon in Params.Owner.ListWeapon)
            {
                if (ActiveWeapon.Name == Params.Owner.ActiveWeaponName)
                {
                    ReferenceWeapon = ActiveWeapon;
                    break;
                }
            }*/
        }

        public override void ActivateFromMenu(CharacterAction ActionToExecute)
        {
            //Pretend TargetType would launch this menu
            Params.User.ActionHolder.AddToPanelListAndSelect(new ActionPanelAttackFreeCam(Params, ActionToExecute));
            //TargetType.AddAndExecuteEffect(null, null, null);
        }

        public override void ActivateAutomatedAction()
        {
            Params.Owner.ListAutomatedAction.Add(new AutomatedActionAttack(Params));
        }

        public override ActionEffect LoadCopy(BinaryReader BR)
        {
            return new AttackActionEffect(BR);
        }
    }

    public class ActionPanelAttackFreeCam : ActionPanelLifeSimPlayer
    {
        private const string PanelName = "AttackFreeCam";

        protected readonly LifeSimCharacterParams Params;
        private readonly CharacterAction ActionToExecute;

        private Camera3D Camera;
        private PlayerCharacter HoverCharacter;

        public ActionPanelAttackFreeCam(LifeSimCharacterParams Params, CharacterAction ActionToExecute)
            : base(PanelName, Params.User, Params.RootMapContainer, Params.User.ActionHolder, true)
        {
            this.Params = Params;
            this.ActionToExecute = ActionToExecute;

            Camera = new LifeSimFreeCamera(GameScreen.GraphicsDevice, Owner);
        }

        public override void OnSelect()
        {
            Owner.Camera = Camera;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            PlayerCharacter FoundCharacter = Owner.InvisibleCharacterAsCursor.SharedMapContex.ActiveMap.MoveCursorAndGetCharacterUnderMouse();

            if (FoundCharacter != null)
            {
                HoverCharacter = FoundCharacter;
            }

            if (HoverCharacter != null && HoverCharacter != Params.Owner && ActiveInputManager.InputConfirmPressed())
            {
                Params.ActivationContext.ActivatedAction = ActionToExecute;
                Params.ActivationContext.User = Params.Owner;
                Params.ActivationContext.Target = HoverCharacter;

                Params.Owner.ListAutomatedAction.Add(new AutomatedActionMove(Params));

                foreach (ActionEffect ActiveAction in ActionToExecute.ListActionEffect)
                {
                    ActiveAction.ActivateAutomatedAction();
                }

                RemoveAllSubActionPanels();
            }

            Camera.Update(gameTime);
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
            return new ActionPanelFreeCam(Owner, MapManager, ListActionMenuChoice);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (HoverCharacter != null)
            {
                ActionPanelViewCharacter.DrawPreview(g, HoverCharacter);
            }
        }
    }

    public class AutomatedActionAttack : AutomatedAction
    {
        private const string PanelName = "Attack";

        private readonly LifeSimCharacterParams Params;

        private double Timer;
        public DiceRoll DiceRoll;

        public AutomatedActionAttack(LifeSimCharacterParams Params)
            : base(PanelName, Params.Owner)
        {
            this.Params = Params;
            DiceRoll = new DiceRoll("2d20");
        }

        public override void OnStarted()
        {
            Timer = 60;
        }

        public override void Update(GameTime gameTime)
        {
            if (Timer <= 0)
            {
                int Damage = DiceRoll.Roll(Params.ActivationContext.User.STR);
                string DiceRollText = Params.ActivationContext.ActivatedAction + "\n\r" + DiceRoll.LastRollText;
                DiceRollText += "{{DiceResult:" + Damage + "}}";
                DiceRollText += Params.ActivationContext.User.Name + " did " + Damage + " damage to " + Params.ActivationContext.Target.Name;

                Params.ActivationContext.Target.CurrentHP -= Damage;
                Timer = 60;
                Params.ActivationContext.User.Logs.AddLog(DiceRollText);
                Params.ActivationContext.Target.Logs.AddLog(DiceRollText);
                Params.ActivationContext.User.ListAutomatedAction.Remove(this);
            }
        }

        public override void OnCancel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new AutomatedActionAttack(Params);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
            GameScreen.DrawBox(g, new Vector2(Position.X, Position.Y), 30, 30, Color.White);

            TextHelper.DrawText(g, "A", new Vector2(Position.X + 5, Position.Y + 5), Color.White);
        }
    }

    public class AutomatedActionMove : AutomatedAction
    {
        private const string PanelName = "Move";

        private readonly LifeSimCharacterParams Params;

        private double Timer;

        public AutomatedActionMove(LifeSimCharacterParams Params)
            : base(PanelName, Params.Owner)
        {
            this.Params = Params;
        }

        public override void OnStarted()
        {
            Timer = 60;
        }

        public override void Update(GameTime gameTime)
        {
            if (Timer <= 0)
            {
                Timer = 60;
                Params.ActivationContext.Target.Logs.AddLog(Params.ActivationContext.User.Name + " moved next to " + Params.ActivationContext.Target.Name);
                Params.ActivationContext.User.Logs.AddLog(Params.ActivationContext.User.Name + " moved next to " + Params.ActivationContext.Target.Name);
                Params.ActivationContext.User.ListAutomatedAction.Remove(this);
            }
        }

        public override void OnCancel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override AutomatedAction Copy()
        {
            return new AutomatedActionMove(Params);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public override void DrawIcon(CustomSpriteBatch g, Vector2 Position)
        {
            GameScreen.DrawBox(g, new Vector2(Position.X, Position.Y), 30, 30, Color.White);

            TextHelper.DrawText(g, "M", new Vector2(Position.X + 5, Position.Y + 5), Color.White);
        }
    }
}
