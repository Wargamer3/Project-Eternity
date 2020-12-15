using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptAddPlayer : DeathmatchMapScript
        {
            private string _PlayerName;
            private int _PlayerTeam;
            private bool _IsHuman;
            private Color _PlayerColor;

            public ScriptAddPlayer()
                : this(null)
            {
            }

            public ScriptAddPlayer(DeathmatchMap Map)
                : base(Map, 100, 50, "Add Player", new string[] { "Add" }, new string[] { "Player created" })
            {
                _PlayerName = string.Empty;
                _PlayerTeam = 0;
                _IsHuman = false;
                _PlayerColor = Color.Red;
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                Player NewPlayer = new Player(_PlayerName, "CPU", _IsHuman, false, _PlayerTeam, _PlayerColor);
                Map.ListPlayer.Add(NewPlayer);
                IsEnded = true;
                ExecuteEvent(this, 0);
            }

            public override void Draw(CustomSpriteBatch g)
            {
            }

            public override void Load(BinaryReader BR)
            {
                _PlayerName = BR.ReadString();
                _PlayerTeam = BR.ReadInt32();
                _IsHuman = BR.ReadBoolean();
                _PlayerColor = Color.FromNonPremultiplied(BR.ReadByte(), BR.ReadByte(), BR.ReadByte(), 255);
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_PlayerName);
                BW.Write(_PlayerTeam);
                BW.Write(_IsHuman);
                BW.Write(_PlayerColor.R);
                BW.Write(_PlayerColor.G);
                BW.Write(_PlayerColor.B);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptAddPlayer(Map);
            }

            #region Properties

            [CategoryAttribute("Player Attributes"),
            DescriptionAttribute(".")]
            public string PlayerName
            {
                get
                {
                    return _PlayerName;
                }
                set
                {
                    _PlayerName = value;
                }
            }


            [CategoryAttribute("Player Attributes"),
            DescriptionAttribute(".")]
            public int PlayerTeam
            {
                get
                {
                    return _PlayerTeam;
                }
                set
                {
                    _PlayerTeam = value;
                }
            }


            [CategoryAttribute("Player Attributes"),
            DescriptionAttribute(".")]
            public bool IsHuman
            {
                get
                {
                    return _IsHuman;
                }
                set
                {
                    _IsHuman = value;
                }
            }


            [CategoryAttribute("Player Attributes"),
            DescriptionAttribute(".")]
            public Color PlayerColor
            {
                get
                {
                    return _PlayerColor;
                }
                set
                {
                    _PlayerColor = value;
                }
            }

            #endregion
        }
    }
}
