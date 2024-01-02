using System;
using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.Scripts.Selectors;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{

    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptChangeCameraType : DeathmatchMapScript
        {
            public enum CameraTypes { Regular, Perspective }

            private CameraTypes _CameraType;

            public ScriptChangeCameraType()
                : this(null)
            {
                IsEnded = false;
            }

            public ScriptChangeCameraType(DeathmatchMap Map)
                : base(Map, 100, 50, "Change Camera Type", new string[] { "Change" }, new string[] { "Change Completed" })
            {
                IsEnded = false;

            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                if (_CameraType == CameraTypes.Perspective)
                {
                    Map.LayerManager.LayerHolderDrawable = new Map3DDrawable(Map, Map.LayerManager, GameScreen.GraphicsDevice);
                }
                else
                {
                    Map.LayerManager.LayerHolderDrawable = new Map2DDrawable(Map, Map.LayerManager);
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
                _CameraType = (CameraTypes)BR.ReadByte();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write((byte)_CameraType);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptChangeCameraType(Map);
            }

            #region Properties


            [CategoryAttribute("Camera"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public CameraTypes CameraType
            {
                get
                {
                    return _CameraType;
                }
                set
                {
                    _CameraType = value;
                }
            }
            
            #endregion
        }
    }
}
