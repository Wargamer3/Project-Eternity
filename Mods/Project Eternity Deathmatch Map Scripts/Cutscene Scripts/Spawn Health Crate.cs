using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptSpawnHealthCrate : DeathmatchMapScript
        {
            public ScriptSpawnHealthCrate()
                : this(null)
            {
            }

            public ScriptSpawnHealthCrate(DeathmatchMap Map)
                : base(Map, 100, 50, "Spawn Health Crate", new string[] { "Spawn" }, new string[0])
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(GameTime gameTime)
            {
                int PosX = RandomHelper.Random.Next(Map.MapSize.X);
                int PosY = RandomHelper.Random.Next(Map.MapSize.Y);
                HealthCrate NewCrate = new HealthCrate(new Vector3(5, 10, 0), Map);
                NewCrate.Load(Map.Content);
                Map.ListLayer[0].ListProp.Add(NewCrate);
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
            }

            public override void Save(BinaryWriter BW)
            {
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptSpawnHealthCrate(Map);
            }

            public class HealthCrate : InteractiveProp
            {
                private Texture2D sprCrate;

                private readonly DeathmatchMap Map;

                private bool ForceHealOnStop;

                public HealthCrate(Vector3 Position, DeathmatchMap Map)
                    : base("HP Crate", PropCategories.Interactive, new bool[,] { { true } }, false)
                {
                    this.Map = Map;
                    this.Position = Position;

                    ForceHealOnStop = false;
                }

                public override void Load(ContentManager Content)
                {
                    sprCrate = Content.Load<Texture2D>("Maps/Props/HP Crate");
                    Unit3D = new UnitMap3D(GameScreen.GraphicsDevice, Content.Load<Effect>("Shaders/Squad shader 3D"), sprCrate, 1);
                }

                public override void DoLoad(BinaryReader BR)
                {
                    throw new NotImplementedException();
                }

                public override void DoSave(BinaryWriter BW)
                {
                    throw new NotImplementedException();
                }

                public void HealSquad(Squad SquadToHeal)
                {
                    SquadToHeal.CurrentLeader.HealUnit(SquadToHeal.CurrentLeader.MaxHP / 2);
                    Map.ListLayer[0].ListProp.Remove(this);
                }

                public override void Update(GameTime gameTime)
                {
                }

                public override List<ActionPanel> OnUnitSelected(Squad SelectedUnit)
                {
                    List<ActionPanel> ListPanel = new List<ActionPanel>();

                    if (!ForceHealOnStop && SelectedUnit.X == Position.X && SelectedUnit.Y == Position.Y)
                    {
                        ListPanel.Add(new ActionPanelPickUpHealthCrate(Map, this, SelectedUnit));
                    }

                    return ListPanel;
                }

                public override List<ActionPanel> OnUnitBeforeStop(Squad StoppedUnit, Vector3 PositionToStopOn)
                {
                    List<ActionPanel> ListPanel = new List<ActionPanel>();

                    if (!ForceHealOnStop && PositionToStopOn.X == Position.X && PositionToStopOn.Y == Position.Y)
                    {
                        ListPanel.Add(new ActionPanelPickUpHealthCrate(Map, this, StoppedUnit));
                    }

                    return ListPanel;
                }

                public override void OnMovedOverBeforeStop(Squad SelectedUnit, Vector3 PositionMovedOn, Vector3 PositionStoppedOn)
                {
                }

                public override void OnUnitStop(Squad StoppedUnit)
                {
                    if (ForceHealOnStop && StoppedUnit.X == Position.X && StoppedUnit.Y == Position.Y)
                    {

                    }
                }

                public override void OnBattleEnd(Squad Attacker, Squad Defender)
                {
                }

                public override void OnTurnEnd(int PlayerIndex)
                {
                }

                public override void Draw(CustomSpriteBatch g)
                {
                    float PosX = (Position.X - Map.CameraPosition.X) * Map.TileSize.X;
                    float PosY = (Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y;

                    g.Draw(sprCrate, new Vector2(PosX, PosY), Color.White);
                }

                public override void Draw3D(GraphicsDevice GraphicsDevice)
                {
                    Unit3D.Draw(GraphicsDevice);
                }

                protected override InteractiveProp Copy()
                {
                    throw new NotImplementedException();
                }

                public class ActionPanelPickUpHealthCrate : BattleMapActionPanel
                {
                    private const string PanelName = "Use Health Crate";

                    private readonly BattleMap Map;
                    private readonly HealthCrate Owner;
                    private readonly Squad ActiveSquad;

                    public ActionPanelPickUpHealthCrate(BattleMap Map, HealthCrate Owner, Squad ActiveSquad)
                        : base(PanelName, Map.ListActionMenuChoice, null, false)
                    {
                        this.Map = Map;
                        this.Owner = Owner;
                        this.ActiveSquad = ActiveSquad;
                    }

                    public override void OnSelect()
                    {
                        Owner.HealSquad(ActiveSquad);
                        RemoveAllSubActionPanels();
                    }

                    public override void DoUpdate(GameTime gameTime)
                    {
                        throw new NotImplementedException();
                    }

                    protected override void OnCancelPanel()
                    {
                        throw new NotImplementedException();
                    }

                    public override void DoRead(ByteReader BR)
                    {
                        throw new NotImplementedException();
                    }

                    public override void DoWrite(ByteWriter BW)
                    {
                        throw new NotImplementedException();
                    }

                    protected override ActionPanel Copy()
                    {
                        throw new NotImplementedException();
                    }

                    public override void Draw(CustomSpriteBatch g)
                    {
                    }
                }
            }
        }
    }
}
