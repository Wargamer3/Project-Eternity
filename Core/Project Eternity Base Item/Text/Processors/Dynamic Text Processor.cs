using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Item
{
    public abstract class DynamicTextProcessor
    {
        public DynamicTextProcessor()
        {
        }

        public abstract void Load(ContentManager Content);

        public abstract DynamicTextPart ParseText(string Text);
    }
}
