using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    partial class SorcererStreetScriptHolder
    {
        public class ScriptSpawnPlayer : SorcererStreetMapScript
        {
            private string _PlayerName;
            private int _PlayerTeam;
            private Point _SpawnPosition;
            private string[] _Deck;

            public ScriptSpawnPlayer()
                : this(null)
            {
            }

            public ScriptSpawnPlayer(SorcererStreetMap Map)
                : base(Map, 140, 70, "Sorcerer Street Spawn Player", new string[] { "Spawn player" }, new string[] { "Player spawned" })
            {
                _PlayerName = "";
                _PlayerTeam = 0;
                _SpawnPosition = new Point();
                _Deck = new string[0];
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                Card[] ArrayCardInDeck = new Card[_Deck.Length];
                for (int C = 0; C < _Deck.Length; C++)
                {
                    ArrayCardInDeck[C] = Card.LoadCard(_Deck[C], Map.Content, Map.SorcererStreetParams.DicRequirement, Map.SorcererStreetParams.DicEffect, Map.SorcererStreetParams.DicAutomaticSkillTarget);
                }

                Player NewPlayer = new Player("", PlayerName, "", true, _PlayerTeam, true, Color.White, ArrayCardInDeck);

                if (_PlayerTeam == 0)
                {
                    Map.ListPlayer.Clear();
                    Map.AddPlayer(NewPlayer);
                }
                else
                {
                    Map.AddPlayer(NewPlayer);
                }

                for (int C = 0; C < 4; ++C)
                {
                    NewPlayer.ListCardInHand.Add(ArrayCardInDeck[RandomHelper.Next(ArrayCardInDeck.Length)]);
                }

                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                PlayerName = BR.ReadString();
                _PlayerTeam = BR.ReadInt32();
                _SpawnPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
                Deck = new string[BR.ReadInt32()];
                for (int C = 0; C < Deck.Length; ++C)
                {
                    Deck[C] = BR.ReadString();
                }
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(PlayerName);
                BW.Write(_PlayerTeam);
                BW.Write(_SpawnPosition.X);
                BW.Write(_SpawnPosition.Y);
                BW.Write(Deck.Length);
                for(int C = 0; C < Deck.Length; ++C)
                {
                    BW.Write(Deck[C]);
                }
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnPlayer(Map);
            }

            #region Properties

            [CategoryAttribute("Spawn behavior"),
            DescriptionAttribute("Player name"),
            DefaultValueAttribute("")]
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

            [CategoryAttribute("Spawn behavior"),
            DescriptionAttribute("Player team"),
            DefaultValueAttribute("")]
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

            [CategoryAttribute("Spawn behavior"),
            DescriptionAttribute("Player spawn position"),
            DefaultValueAttribute("")]
            public Point SpawnPosition
            {
                get
                {
                    return _SpawnPosition;
                }
                set
                {
                    _SpawnPosition = value;
                }
            }

            [Editor(typeof(DeckSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawn behavior"),
            DescriptionAttribute("Player spawn position"),
            DefaultValueAttribute("")]
            public string[] Deck
            {
                get
                {
                    return _Deck;
                }
                set
                {
                    _Deck = value;
                }
            }

            #endregion
        }
    }
}
