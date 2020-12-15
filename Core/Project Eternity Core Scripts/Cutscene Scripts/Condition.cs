using System;
using System.IO;
using System.ComponentModel;

namespace ProjectEternity.Core.Scripts
{
    public sealed partial class ScriptingScriptHolder
    {
        public class ScriptConditionScriptEnd : CutsceneActionScript
        {
            public enum ScriptCategories { CharacterAnimation, GraphicAnimation, VisualNovel, BattleMap, Scripts };

            private ScriptCategories _ScriptCategory;
            private UInt32 _TargetID;

            public ScriptConditionScriptEnd()
                : base(140, 70, "Condition", new string[] { "Check Condition" }, new string[] { "Condition is true", "Condition is false" })
            {
                _ScriptCategory = ScriptCategories.CharacterAnimation;
                _TargetID = 0;
            }

            public override void ExecuteTrigger(int Index)
            {
                throw new NotImplementedException();
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                throw new NotImplementedException();
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                ScriptCategory = (ScriptConditionScriptEnd.ScriptCategories)BR.ReadInt32();
                TargetID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write((int)ScriptCategory);
                BW.Write(TargetID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptConditionScriptEnd();
            }

            #region Properties

            [CategoryAttribute("Condition Attributes"),
            DescriptionAttribute("The category from which the Script comes from."),
            DefaultValueAttribute(ScriptCategories.CharacterAnimation)]
            public ScriptCategories ScriptCategory
            {
                get
                {
                    return _ScriptCategory;
                }
                set
                {
                    _ScriptCategory = value;
                }
            }

            [CategoryAttribute("Condition Attributes"),
            DescriptionAttribute("The Identification number of the targeted Script."),
            DefaultValueAttribute(0)]
            public UInt32 TargetID
            {
                get
                {
                    return _TargetID;
                }
                set
                {
                    _TargetID = value;
                }
            }

            #endregion
        }
    }
}
