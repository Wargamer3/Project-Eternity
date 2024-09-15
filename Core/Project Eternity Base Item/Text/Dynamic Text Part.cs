using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core.Item
{
    public abstract class DynamicTextPart
    {
        public string ID;
        public DynamicTextPart Parent;
        public readonly DynamicText Owner;
        public string OriginalText;
        public readonly Dictionary<string, string> DicSubTag;
        public List<DynamicTextPart> ListSubTextSection;

        public bool Rainbow;
        public bool Wave;
        public Vector2 Position;
        public float MaxWidth;
        public float MaxHeight;

        public DynamicTextPart(DynamicText Owner, string OriginalText, string Prefix)
        {
            this.Owner = Owner;
            DicSubTag = new Dictionary<string, string>();
            ListSubTextSection = new List<DynamicTextPart>();

            if (!string.IsNullOrEmpty(Prefix))
            {
                this.OriginalText = string.Empty;
            }
            else
            {
                this.OriginalText = OriginalText;
            }
        }

        public virtual void SetParent(DynamicTextPart Parent)
        {
            this.Parent = Parent;
        }

        public virtual void OnTextRead(string TextRead)
        {
            DynamicTextPart NewTextPart = Owner.DefaultProcessor.ParseText(TextRead);
            NewTextPart.SetParent(this);
            ListSubTextSection.Add(NewTextPart);
        }

        public void ProcessInternalText(string TextToParse, ref int i)
        {
            while (TextToParse[i] == '{' && i + 1 < TextToParse.Length && TextToParse[i + 1] != '{')
            {
                int j = i + 1;
                ++i;

                while (i < TextToParse.Length)
                {
                    string SubTag = TextToParse.Substring(i, 1);
                    ++i;

                    if (SubTag == "}")
                    {
                        SubTag = TextToParse.Substring(j, i - j - 1);
                        string[] ArraySubTagPart = SubTag.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (ArraySubTagPart.Length > 1)
                        {
                            DicSubTag.Add(ArraySubTagPart[0], ArraySubTagPart[1]);
                        }
                        else
                        {
                            DicSubTag.Add(ArraySubTagPart[0], null);
                        }
                        break;
                    }
                }
            }
        }

        public void ProcessText(string TextToParse, ref int i)
        {
            string WorkingText = string.Empty;

            for (; i < TextToParse.Length; i++)
            {
                string ActiveText = TextToParse.Substring(i, 1);

                if (ActiveText == "{")
                {
                    if (i + 1 < TextToParse.Length && TextToParse.Substring(i, 2) == "{{")
                    {
                        if (WorkingText.Length > 0)
                        {
                            OnTextRead(WorkingText);
                            WorkingText = string.Empty;
                        }

                        char ActiveChar = ' ';
                        int j = i;
                        while (ActiveChar != ':')
                        {
                            ++j;
                            ActiveChar = TextToParse.Substring(j, 1)[0];
                        }

                        string ActiveTag = TextToParse.Substring(i + 2, j - i - 1);
                        i = j + 1;

                        foreach (DynamicTextProcessor ActiveProcessor in Owner.ListProcessor)
                        {
                            DynamicTextPart AvailableTextPart = ActiveProcessor.GetTextObject(ActiveTag);

                            if (AvailableTextPart != null)
                            {
                                AvailableTextPart.SetParent(this);
                                AvailableTextPart.ProcessInternalText(TextToParse, ref i);
                                AvailableTextPart.ProcessText(TextToParse, ref i);
                                ListSubTextSection.Add(AvailableTextPart);
                                break;
                            }
                        }
                    }
                }
                else if (i + 1 < TextToParse.Length && TextToParse.Substring(i, 2) == "}}")
                {
                    ++i;
                    if (WorkingText.Length > 0)
                    {
                        OnTextRead(WorkingText);
                        WorkingText = string.Empty;
                        return;
                    }
                }
                else
                {
                    WorkingText += ActiveText;
                }
            }

            if (WorkingText.Length > 0)
            {
                OnTextRead(WorkingText);
                WorkingText = string.Empty;
            }
        }

        protected float GetStartingXPositionOnLine(Vector2 ActivePosition)
        {
            Rectangle LineCollisionBox = new Rectangle((int)ActivePosition.X, (int)ActivePosition.Y, 1, (int)Owner.LineHeight);

            foreach (Rectangle ActiveObstacle in Owner.ListObstacle)
            {
                if (LineCollisionBox.Intersects(ActiveObstacle))
                {
                    return ActiveObstacle.Right;
                }
            }

            return ActivePosition.X;
        }

        protected float GetRemainingSpaceOnLine(Vector2 ActivePosition)
        {
            Rectangle LineCollisionBox = new Rectangle((int)ActivePosition.X, (int)ActivePosition.Y, (int)Owner.TextMaxWidthInPixel, (int)Owner.LineHeight);

            foreach (Rectangle ActiveObstacle in Owner.ListObstacle)
            {
                if (LineCollisionBox.Intersects(ActiveObstacle))
                {
                    return ActiveObstacle.X - ActivePosition.X;
                }
            }

            return MaxWidth - ActivePosition.X;
        }

        //Retun end position
        public abstract Vector2 UpdatePosition();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(CustomSpriteBatch g, Vector2 Offset);
    }
}
