using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using FMOD;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptChangeUnitTheme : DeathmatchMapScript
        {
            private string _ThemePath;
            private uint _UnitToChangeID;
            private bool ThemeChanged;

            public ScriptChangeUnitTheme()
                : this(null)
            {
                _UnitToChangeID = 0;
            }

            public ScriptChangeUnitTheme(DeathmatchMap Map)
                : base(Map, 100, 50, "Change Unit Theme", new string[] { "Change Theme" }, new string[] { "Theme Changed" })
            {
                _UnitToChangeID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                ThemeChanged = false;
                for (int P = 0; P < Map.ListPlayer.Count && !ThemeChanged; P++)
                {
                    for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count && !ThemeChanged; U++)
                    {
                        if (Map.ListPlayer[P].ListSquad[U].ID != _UnitToChangeID)
                            continue;

                        if (!Character.DicBattleTheme.ContainsKey(_ThemePath))
                            Character.DicBattleTheme.Add(_ThemePath, new FMODSound(GameScreen.FMODSystem, "Content/Maps/BGM/" + _ThemePath));

                        Map.ListPlayer[P].ListSquad[U].CurrentLeader.Pilot.BattleThemeName = _ThemePath;
                        ThemeChanged = true;
                    }
                }

                ExecuteEvent(this, 0);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                UnitToChangeID = BR.ReadUInt32();
                ThemePath = BR.ReadString();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(UnitToChangeID);
                BW.Write(ThemePath);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptChangeUnitTheme(Map);
            }

            #region Properties

            [CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The Identification number of the targeted Unit.")]
            public UInt32 UnitToChangeID
            {
                get
                {
                    return _UnitToChangeID;
                }
                set
                {
                    _UnitToChangeID = value;
                }
            }

            [Editor(typeof(Selectors.BGMSelector), typeof(UITypeEditor)),
            CategoryAttribute("Target Attributes"),
            DescriptionAttribute("The theme path"),
            DefaultValueAttribute(0)]
            public string ThemePath
            {
                get
                {
                    return _ThemePath;
                }
                set
                {
                    _ThemePath = value;
                }
            }

            #endregion
        }
    }
}
