using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

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
                List<Card> ListCardInDeck = new List<Card>(_Deck.Length);
                for (int C = 0; C < _Deck.Length; C++)
                {
                    ListCardInDeck.Add(Card.LoadCard(_Deck[C], Map.Content, Map.SorcererStreetParams.DicRequirement, Map.SorcererStreetParams.DicEffect, Map.SorcererStreetParams.DicAutomaticSkillTarget, Map.SorcererStreetParams.DicManualSkillTarget));
                }

                Player NewPlayer = new Player("", PlayerName, "", false, _PlayerTeam, true, Color.White, ListCardInDeck);
                TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(_SpawnPosition.X, _SpawnPosition.Y, 0);
                NewPlayer.GamePiece.SetPosition(new Vector3(ActiveTerrain.InternalPosition.X, ActiveTerrain.InternalPosition.Y, ActiveTerrain.LayerIndex));

                NewPlayer.LoadGamePieceModel();

                if (_PlayerTeam == 0)
                {
                    Map.ListPlayer.Clear();
                    Map.AddPlayer(NewPlayer);
                    NewPlayer.Color = Color.Red;
                }
                else
                {
                    Map.AddPlayer(NewPlayer);
                    NewPlayer.Color = Color.Blue;
                }
                NewPlayer.TotalMagic = NewPlayer.Gold = Map.MagicAtStart;

                for (int C = 0; C < 4; ++C)
                {
                    NewPlayer.ListCardInHand.Add(ListCardInDeck[RandomHelper.Next(ListCardInDeck.Count)]);
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
