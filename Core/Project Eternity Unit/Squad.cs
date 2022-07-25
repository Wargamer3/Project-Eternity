using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Core.Units
{
    public class Squad : UnitMapComponent
    {
        private Unit[] ArrayUnit;
        public Unit CurrentLeader { get { return CurrentLeaderIndex >= 0 ? ArrayUnit[CurrentLeaderIndex] : null; } }

        public Unit CurrentWingmanA { get { return CurrentWingmanAIndex >= 0 ? ArrayUnit[CurrentWingmanAIndex] : null; } }

        public Unit CurrentWingmanB { get { return CurrentWingmanBIndex >= 0 ? ArrayUnit[CurrentWingmanBIndex] : null; } }

        public int CurrentLeaderIndex;
        public int CurrentWingmanAIndex;
        public int CurrentWingmanBIndex;

        public string SquadName;
        public AIContainer SquadAI;
        public string SquadDefenseBattleBehavior;
        
        public bool IsEventSquad;
        public bool IsNameLocked;
        public bool IsLeaderLocked;
        public bool IsWingmanALocked;
        public bool IsWingmanBLocked;
        public bool IsDead;

        public override int Width { get { return CurrentLeader.SpriteMap.Width; } }
        public override int Height { get { return CurrentLeader.SpriteMap.Height; } }
        public override bool[,] ArrayMapSize { get { return CurrentLeader.UnitStat.ArrayMapSize; } }
        public override bool IsActive { get { return (CurrentLeader != null || IsEventSquad) && !IsDead; } }

        /// <summary>
        /// Returns CurrentLeader(0), CurrentWingmanA(1) or CurrentWingmanB(2) depending of the index.
        /// Since the SelectedUnit change which Unit is returned, ArrayUnit would not return the right unit if acceded directly.
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public Unit this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return CurrentLeader;

                    case 1:
                        return CurrentWingmanA;

                    case 2:
                        return CurrentWingmanB;

                    default:
                        return null;
                }
            }
        }

        public int UnitsInSquad
        {
            get
            {
                return ArrayUnit.Length;
            }
        }

        public int UnitsAliveInSquad
        {
            get
            {
                int ActiveUnit = 0;
                if (ArrayUnit[0].HP > 0)
                    ++ActiveUnit;
                if (ArrayUnit.Length >= 2 && ArrayUnit[1].HP > 0)
                    ++ActiveUnit;
                if (ArrayUnit.Length >= 3 && ArrayUnit[2].HP > 0)
                    ++ActiveUnit;

                return ActiveUnit;
            }
        }

        public Squad(string SquadName, Unit Leader = null, Unit WingmanA = null, Unit WingmanB = null)
        {
            IsDead = false;
            StartTurn();
            this.SquadName = SquadName;

            CurrentLeaderIndex = 0;
            CurrentWingmanAIndex = -1;
            CurrentWingmanBIndex = -1;

            SquadDefenseBattleBehavior = "";
            ListAttackedTeam = new List<int>();
            ListParthDrop = new List<string>();
            TeamTags = new TagSystem();

            if (WingmanB != null)
            {
                ArrayUnit = new Unit[3];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;
                ArrayUnit[2] = WingmanB;

                CurrentWingmanAIndex = 1;
                CurrentWingmanBIndex = 2;
            }
            else if (WingmanA != null)
            {
                ArrayUnit = new Unit[2];
                ArrayUnit[0] = Leader;
                ArrayUnit[1] = WingmanA;

                CurrentWingmanAIndex = 1;
            }
            else
            {
                ArrayUnit = new Unit[1];
                ArrayUnit[0] = Leader;
            }
        }

        public void Init(UnitEffectContext Context)
        {
            Context.SetContext(this, ArrayUnit[0], null, null, null, null, null);
            
            ArrayUnit[0].Init();
            CurrentLeaderIndex = 0;
            if (ArrayUnit.Length >= 2)
            {
                Context.SetContext(this, ArrayUnit[1], null, null, null, null, null);
                ArrayUnit[1].Init();
                CurrentWingmanAIndex = 1;
            }
            if (ArrayUnit.Length >= 3)
            {
                Context.SetContext(this, ArrayUnit[2], null, null, null, null, null);
                ArrayUnit[2].Init();
                CurrentWingmanBIndex = 2;
            }

            for (int U = 0; U < ArrayUnit.Length; U++)
            {
                for (int C = 0; C < this[U].ArrayCharacterActive.Length; C++)
                {
                    ArrayUnit[U].ArrayCharacterActive[C].SP = ArrayUnit[U].ArrayCharacterActive[C].MaxSP;
                }
                
                //Load the Battle Themes.
                for (int C = ArrayUnit[U].ArrayCharacterActive.Length - 1; C >= 0; --C)
                    if (!string.IsNullOrEmpty(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName))
                        if (!Character.DicBattleTheme.ContainsKey(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName))
                            Character.DicBattleTheme.Add(ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName, new FMOD.FMODSound(GameScreens.GameScreen.FMODSystem, "Content/Maps/BGM/" + ArrayUnit[U].ArrayCharacterActive[C].BattleThemeName + ".mp3"));
            }
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(CurrentLeader.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
            if (CurrentWingmanA != null)
                g.Draw(CurrentWingmanA.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, CurrentWingmanA.SpriteMap.Width / 2, CurrentWingmanA.SpriteMap.Height / 2), Color.White);
            if (CurrentWingmanB != null)
                g.Draw(CurrentWingmanB.SpriteMap, new Rectangle((int)Position.X + CurrentLeader.SpriteMap.Width / 2, (int)Position.Y, CurrentWingmanB.SpriteMap.Width / 2, CurrentWingmanB.SpriteMap.Height / 2), Color.White);
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
            if (Constants.ShowHealthBar)
            {
                DrawHealthBar(g, Position, CurrentLeader.HP, CurrentLeader.MaxHP, CurrentLeader.SpriteMap.Width);
            }
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
            if (TimeOfDay > 20)
            {
                g.Draw(GameScreens.GameScreen.sprPixel, new Rectangle((int)Position.X, (int)Position.Y, CurrentLeader.SpriteMap.Width, CurrentLeader.SpriteMap.Height), Color.White);
            }
        }

        public override List<ActionPanel> OnMenuSelect(int ActivePlayerIndex, ActionPanelHolder ListActionMenuChoice)
        {
            List<ActionPanel> DicActionPanel = new List<ActionPanel>();

            for (int U = UnitsAliveInSquad - 1; U >= 0; --U)
            {
                foreach (ActionPanel OptionalPanel in ArrayUnit[U].OnMenuSelect(ActivePlayerIndex, this, ListActionMenuChoice))
                {
                    if (OptionalPanel != null && !DicActionPanel.Contains(OptionalPanel))
                    {
                        DicActionPanel.Add(OptionalPanel);
                    }
                }
            }

            return DicActionPanel;
        }

        public bool IsUnitAtPosition(Vector3 PositionToCheck)
        {
            return CurrentLeader.UnitStat.IsUnitAtPosition(Position, PositionToCheck);
        }

        public void UpdateSquad()
        {
            IsDead = false;

            CurrentLeaderIndex = -1;
            CurrentWingmanAIndex = -1;
            CurrentWingmanBIndex = -1;

            for (int U = 0; U < ArrayUnit.Length; U++)
            {
                if (ArrayUnit[U].HP > 0)
                {
                    if (CurrentLeader == null)
                    {
                        CurrentLeaderIndex = U;
                    }
                    else if (CurrentWingmanA == null && ArrayUnit[U] != CurrentLeader)
                    {
                        CurrentWingmanAIndex = U;
                    }
                    else if (CurrentWingmanB == null && ArrayUnit[U] != CurrentLeader && ArrayUnit[U] != CurrentWingmanA)
                    {
                        CurrentWingmanBIndex = U;
                    }
                }
            }

            if (CurrentLeader == null)
                IsDead = true;
        }

        public void ReloadSkills(Dictionary<string, Unit> DicUnitType, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
             Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            for (int U = 0; U < ArrayUnit.Length; ++U)
            {
                ArrayUnit[U].ReloadSkills(DicUnitType[ArrayUnit[U].UnitTypeName], DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            }
        }

        public Unit At(int Index)
        {
            if (Index < ArrayUnit.Length)
                return ArrayUnit[Index];
            else
                return null;
        }

        public int IndexOf(Unit ActiveUnit)
        {
            for (int U = ArrayUnit.Length - 1; U>=0; --U)
            {
                if (ArrayUnit[U] == ActiveUnit)
                    return U;
            }
            return -1;
        }

        public void SetLeader(int LeaderIndex)
        {
            CurrentLeaderIndex = LeaderIndex;

            if (LeaderIndex < 0 || LeaderIndex >= ArrayUnit.Length)
                CurrentLeaderIndex = -1;
        }

        public void SetWingmanA(int WingmanAIndex)
        {
            CurrentWingmanAIndex = WingmanAIndex;

            if (WingmanAIndex < 0 || WingmanAIndex >= ArrayUnit.Length)
                CurrentWingmanAIndex = -1;
        }

        public void SetWingmanB(int WingmanBIndex)
        {
            CurrentWingmanBIndex = WingmanBIndex;

            if (WingmanBIndex < 0 || WingmanBIndex >= ArrayUnit.Length)
                CurrentWingmanBIndex = -1;
        }

        public void SetNewLeader(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public void SetNewWingmanA(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public void SetNewWingmanB(Unit NewLeader)
        {
            ArrayUnit[CurrentLeaderIndex] = NewLeader;
        }

        public static Squad LoadSquadWithProgression(BinaryReader BR, List<Character> ListTeamCharacter, ContentManager Content, Dictionary<string, Unit> DicUnitType,
            Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            string SquadName = BR.ReadString();
            bool IsNameLocked = BR.ReadBoolean();
            bool IsLeaderLocked = BR.ReadBoolean();
            bool IsWingmanALocked = BR.ReadBoolean();
            bool IsWingmanBLocked = BR.ReadBoolean();

            int UnitsInSquad = BR.ReadInt32();
            Unit NewLeader = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            Unit NewWingmanA = null;
            Unit NewWingmanB = null;

            if (UnitsInSquad >= 2)
                NewWingmanA = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            if (UnitsInSquad >= 3)
                NewWingmanB = Unit.LoadUnitWithProgress(BR, ListTeamCharacter, Content, DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            Squad NewSquad = new Squad(SquadName, NewLeader, NewWingmanA, NewWingmanB);
            NewSquad.IsNameLocked = IsNameLocked;
            NewSquad.IsLeaderLocked = IsLeaderLocked;
            NewSquad.IsWingmanALocked = IsWingmanALocked;
            NewSquad.IsWingmanBLocked = IsWingmanBLocked;

            return NewSquad;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(SquadName);
            BW.Write(IsNameLocked);
            BW.Write(IsLeaderLocked);
            BW.Write(IsWingmanALocked);
            BW.Write(IsWingmanBLocked);

            BW.Write(UnitsInSquad);
            for (int U = 0; U < ArrayUnit.Length; U++)
                ArrayUnit[U].Save(BW);
        }

        public override string ToString()
        {
            return SquadName;
        }
    }

    public class UnitMap3D
    {
        public struct UnitMap3DVertex
        {
            public Vector3 Position;
            public Vector2 UV;
            public float Time;

            public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * 3 + sizeof(float) * 2, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
             );

            public const int SizeInBytes = sizeof(float) * 3 + sizeof(float) * 2 + sizeof(float);
        }

        public UnitMap3DVertex[] particles;
        private DynamicVertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        public readonly Effect UnitEffect3D;
        private EffectParameterCollection parameters;
        // Shortcuts for accessing frequently changed effect parameters.
        private EffectParameter effectViewParameter;

        private EffectParameter effectProjectionParameter;
        private EffectParameter effectViewportScaleParameter;
        private EffectParameter effectTimeParameter;

        public UnitMap3D(GraphicsDevice g, Effect UnitEffect3D, Texture2D Sprite, int NumberOfImages, float currentTime = 0f)
        {
            this.UnitEffect3D = UnitEffect3D.Clone();

            parameters = this.UnitEffect3D.Parameters;

            // Look up shortcuts for parameters that change every frame.
            effectViewParameter = parameters["View"];
            effectProjectionParameter = parameters["Projection"];
            effectViewportScaleParameter = parameters["ViewportScale"];
            effectTimeParameter = parameters["CurrentTime"];
            parameters["AnimationSpeed"].SetValue(1f);
            parameters["Size"].SetValue(new Vector2((Sprite.Width / NumberOfImages) * 2f, Sprite.Height * 2f));
            parameters["t0"].SetValue(Sprite);

            // Set the values of parameters that do not change.
            parameters["NumberOfImages"].SetValue(NumberOfImages);

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            effectProjectionParameter.SetValue(Projection);

            particles = new UnitMap3DVertex[4];

            particles[0].UV = new Vector2(0, 0);
            particles[1].UV = new Vector2(1, 0);
            particles[2].UV = new Vector2(1, 1);
            particles[3].UV = new Vector2(0, 1);

            for (int i = 0; i < 4; ++i)
            {
                particles[i].Time = currentTime;
            }

            // Create a dynamic vertex buffer.
            vertexBuffer = new DynamicVertexBuffer(g, UnitMap3DVertex.VertexDeclaration,
                                                   4, BufferUsage.WriteOnly);

            // Create and populate the index buffer.
            ushort[] indices = new ushort[6];

            indices[0] = (ushort)(0);
            indices[1] = (ushort)(1);
            indices[2] = (ushort)(2);

            indices[3] = (ushort)(0);
            indices[4] = (ushort)(2);
            indices[5] = (ushort)(3);

            indexBuffer = new IndexBuffer(g, typeof(ushort), indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);
        }

        public void SetViewMatrix(Matrix View)
        {
            effectViewParameter.SetValue(View);
        }

        public void SetPosition(float X, float Y, float Z)
        {
            Vector3 Position = new Vector3(X, Y, Z);

            for (int i = 0; i < 4; ++i)
            {
                particles[i].Position = Position;
            }
            vertexBuffer.SetData(particles);
        }

        public void Draw(GraphicsDevice GraphicsDevice, float currentTime = 0f)
        {
            // Restore the vertex buffer contents if the graphics device was lost.
            if (vertexBuffer.IsContentLost)
            {
                vertexBuffer.SetData(particles);
            }

            // Set an effect parameter describing the viewport size. This is
            // needed to convert particle sizes into screen space point sizes.
            effectViewportScaleParameter.SetValue(new Vector2(0.5f / GraphicsDevice.Viewport.AspectRatio, -0.5f));

            // Set an effect parameter describing the current time. All the vertex
            // shader particle animation is keyed off this value.
            effectTimeParameter.SetValue(currentTime);

            // Set the particle vertex and index buffer.
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;

            // Activate the particle effect.
            foreach (EffectPass pass in UnitEffect3D.CurrentTechnique.Passes)
            {
                pass.Apply();

                // If the active particles are all in one consecutive range,
                // we can draw them all in a single call.
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
            }
        }
    }
}
