using System.Collections.Generic;

namespace ProjectEternity.Core.Editor
{
    public struct ItemContainer
    {
        //Item logic name, Item full path
        public Dictionary<string, string> ListItem;

        public readonly string ContainerGUIPath;
        public readonly string ContainerRootPath;

        public ItemContainer(string ContainerGUIPath, string ContainerRootPath, Dictionary<string, string> ListItem)
        {
            this.ContainerGUIPath = ContainerGUIPath;
            this.ContainerRootPath = ContainerRootPath;
            this.ListItem = ListItem;
        }

        public ItemContainer(ItemContainer Clone)
        {
            ContainerGUIPath = Clone.ContainerGUIPath;
            ContainerRootPath = Clone.ContainerRootPath;
            ListItem = new Dictionary<string, string>(Clone.ListItem.Count);
            foreach (KeyValuePair<string, string> Item in Clone.ListItem)
            {
                ListItem.Add(Item.Key, Item.Value);
            }
        }
    }
}
