using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    public class VisualNovelBackground
    {
        public string Name;
        public string Path;
        public Texture2D Sprite;

        public VisualNovelBackground(string Name, string Path, Texture2D Sprite)
        {
            this.Name = Name;
            this.Path = Path;
            this.Sprite = Sprite;
        }

        public override bool Equals(object obj)
        {
            VisualNovelBackground OtherBackground = obj as VisualNovelBackground;
            if (OtherBackground == null)
                return false;
            else
            {
                if (Name == OtherBackground.Name && Path == OtherBackground.Path)
                    return true;

                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
