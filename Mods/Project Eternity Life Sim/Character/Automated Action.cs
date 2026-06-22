using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class AutomatedAction
    {
        public readonly string ActionName;
        public readonly PlayerCharacter Owner;

        protected AutomatedAction(string ActionName, PlayerCharacter Owner)
        {
            this.ActionName = ActionName;
            this.Owner = Owner;
        }

        public abstract void OnStarted();

        public abstract void OnCancel();

        public abstract void Update(GameTime gameTime);

        public abstract void DoRead(ByteReader BR);

        public abstract void DoWrite(ByteWriter BW);

        protected abstract AutomatedAction Copy();

        public abstract void Draw(CustomSpriteBatch g);

        public abstract void DrawIcon(CustomSpriteBatch g, Vector2 Position);
    }
}
