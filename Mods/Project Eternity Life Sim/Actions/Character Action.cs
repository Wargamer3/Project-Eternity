using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CharacterAction
    {
        public string Name;
        public string Description;

        public List<Trait> ListTraits;
        public List<string> ListTraitsRelativePath;

        public byte ActionCost;
        public List<ActionEffect> ListActionEffect;
        public List<AICharacterAction> ListAIAction;
        public List<string> ListAIActionPath;

        public Texture2D sprIcon;
        public bool IsLocked;
        public byte LockedLevel;
        public bool IsVisible;

        public ActionPanel GetActionPanel()
        {
            return null;
        }

        public CharacterAction(string FilePath, ContentManager Content)
        {
            FileStream FS = new FileStream("Content/Life Sim/Character Actions/" + FilePath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListTraitsCount = BR.ReadByte();
            ListTraits = new List<Trait>(ListTraitsCount);
            ListTraitsRelativePath = new List<string>(ListTraitsCount);
            for (int T = 0; T < ListTraitsCount; ++T)
            {
                ListTraitsRelativePath.Add(BR.ReadString());
            }

            ActionCost = BR.ReadByte();
            byte ListActionCount = BR.ReadByte();
            ListActionEffect = new List<ActionEffect>(ListActionCount);

            for (int A = 0; A < ListActionCount; ++A)
            {
                string ActionPath = BR.ReadString();

                ListActionEffect.Add(LifeSimParams.DicActionEffect[ActionPath].LoadCopy(BR));
            }

            byte ListAIActionCount = BR.ReadByte();
            ListAIAction = new List<AICharacterAction>(ListAIActionCount);
            ListAIActionPath = new List<string>(ListAIActionCount);

            for (int A = 0; A < ListAIActionCount; ++A)
            {
                string AIActionPath = BR.ReadString();

                ListAIActionPath.Add(AIActionPath);

                if (!string.IsNullOrEmpty(AIActionPath))
                {
                }
            }

            BR.Close();
            FS.Close();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
