using System.Collections.Generic;

namespace ProjectEternity.Core.Editor
{
    public class MenuFilter
    {
        public int MenuCount;
        public Dictionary<string, MenuFilter> ListMenu;
        //Item logic name, Item full path
        public Dictionary<string, string> ListItem;

        public MenuFilter()
        {
            this.ListMenu = new Dictionary<string, MenuFilter>();
            ListItem = null;
            MenuCount = 0;
        }
    }
}
