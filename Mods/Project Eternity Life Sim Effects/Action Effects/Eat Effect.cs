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
    public class EatActionEffect : ActionEffect
    {
        private Weapon ReferenceWeapon;

        public LifeSimManualSkillTargetType TargetType;
        public List<BaseEffect> ListEffect;

        public EatActionEffect()
            : base("Eat")
        {
        }

        public EatActionEffect(BinaryReader BR)
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
            Params.Owner.ListAutomatedAction.Add(new AutomatedActionEat(Params));
        }

        public override ActionEffect LoadCopy(BinaryReader BR)
        {
            return new AttackActionEffect(BR);
        }
    }

    public class AutomatedActionEat : AutomatedAction
    {
        private const string PanelName = "Eat";

        private readonly LifeSimCharacterParams Params;

        private double Timer;
        public DiceRoll DiceRoll;

        public AutomatedActionEat(LifeSimCharacterParams Params)
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

            TextHelper.DrawText(g, "E", new Vector2(Position.X + 5, Position.Y + 5), Color.White);
        }
    }
}
