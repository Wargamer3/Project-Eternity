using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Item
{
    public class TagSystem
    {
        private List<string> ListTag;

        public TagSystem()
        {
            ListTag = new List<string>();
        }

        public void AddTag(string NewTag)
        {
            ListTag.Add(NewTag);
        }

        public void AddTags(params string[] NewTags)
        {
            ListTag.AddRange(NewTags);
        }

        public void RemoveTag(string TagToRemove)
        {
            ListTag.Remove(TagToRemove);
        }

        public bool ContainsTag(string TagToCheck)
        {
            return ListTag.Contains(TagToCheck);
        }
    }
}
