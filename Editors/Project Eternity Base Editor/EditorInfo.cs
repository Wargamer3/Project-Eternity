using System;
using System.IO;
using System.Collections.Generic;

namespace ProjectEternity.Core.Editor
{
    public struct EditorInfo
    {
        public Type EditorType;
        public ItemContainer ItemContainer;
        public string[] ArrayLogicPath;//Used for grouping editors.
        public bool CanCreateNewItems;
        public object[] InitParams;

        public string[] ArrayFileExtention;

        public bool IsFolder;
        public string FolderPath;

        public EditorInfo(EditorInfo Clone)
        {
            this.EditorType = Clone.EditorType;
            this.ArrayLogicPath = Clone.ArrayLogicPath;
            this.ItemContainer = new ItemContainer(Clone.ItemContainer);
            this.InitParams = Clone.InitParams;
            this.CanCreateNewItems = Clone.CanCreateNewItems;
            this.ArrayFileExtention = Clone.ArrayFileExtention;
            this.IsFolder = false;
            FolderPath = "";
        }

        public EditorInfo(string[] ArrayLogicPath, string GUIContainerPath, string[] ArrayFileExtention, Type EditorType,
            bool CanCreateNewItems = true, object[] InitParams = null, bool AddNoneEntry = false)
        {
            this.EditorType = EditorType;
            this.ArrayLogicPath = ArrayLogicPath;
            this.InitParams = InitParams;
            this.CanCreateNewItems = CanCreateNewItems;
            this.ArrayFileExtention = ArrayFileExtention;
            this.IsFolder = false;
            FolderPath = "";

            string ContainerRootPath = "Content/" + GUIContainerPath;
            Dictionary<string, string> ListItem = new Dictionary<string, string>();

            if (AddNoneEntry)
            {
                ListItem.Add("None", null);
            }

            string[] Files;

            foreach (string FileExtention in ArrayFileExtention)
            {
                Files = Directory.GetFiles(ContainerRootPath, "*" + FileExtention, SearchOption.AllDirectories);
                foreach (string Item in Files)
                {
                    string CorrectedPath = Item.Replace("\\", "/");

                    //Handle case where GetFiles doesn't return the proper files.
                    if (Item.EndsWith(FileExtention))
                    {
                        ListItem.Add(CorrectedPath.Substring(0, CorrectedPath.Length - FileExtention.Length).Replace(ContainerRootPath, ""), CorrectedPath);
                    }
                }
            }

            ItemContainer = new ItemContainer(GUIContainerPath, ContainerRootPath, ListItem);
        }
    }
}
