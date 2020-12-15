using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ProjectEternity.Core.Editor
{
    public partial class BaseEditorItem : BaseEditor
    {
        UInt16 LastID;
        string LastName;

        public BaseEditorItem()
        {
            InitializeComponent();
        }
        protected UInt16 GetNextID(string TablePath)
        {
            FileStream FS = new FileStream(TablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);
            UInt16 ItemID = 0;

            //Read everything
            while (!SR.EndOfStream)
            {
                string[] TextBuffer = SR.ReadLine().Split(';');
                ItemID = Convert.ToUInt16(TextBuffer[0]);
                ItemID += 1;
            }

            FS.Close();
            SR.Close();

            return ItemID;
        }

        protected bool UpdateIDTable(string TablePath, string ItemName, UInt16 ItemID, bool ForceOverwrite = false)
        {
            if (LastID != ItemID || LastName != ItemName)
            {
                LastID = ItemID;
                LastName = ItemName;
                UInt16 CurrentItemID = ItemID;
                string CurrentItemName = ItemName;
                bool Overwrite = false;

                FileStream FS = new FileStream(TablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamReader SR = new StreamReader(FS);

                #region Look for existing ID

                //Read everything
                while (!SR.EndOfStream)
                {
                    string[] TextBuffer = SR.ReadLine().Split(';');
                    UInt16 ReadItemID = Convert.ToUInt16(TextBuffer[0]);
                    string ReadItemName = TextBuffer[1];

                    //If the item is using a registred ID, ask to overwrite it.
                    if (ReadItemID == CurrentItemID)
                    {
                        if (!ForceOverwrite)
                        {
                            System.Windows.Forms.DialogResult MessageBoxResult = MessageBox.Show("Error: an item ( " + ReadItemName + " ) is already registred with this ID ( " + ReadItemID + " )\r\nThis will overwrite it?", "Error", MessageBoxButtons.OKCancel);
                            if (LastName != ItemName || MessageBoxResult == System.Windows.Forms.DialogResult.OK)
                            {
                                Overwrite = true;
                                break;//Proceed to the save dialog.
                            }
                            else
                                return false;//Else, if you don't overwrite it, stop.
                        }
                        else
                            Overwrite = true;
                    }
                }

                #endregion

                if (Overwrite)//Overwrite the current Item.
                {
                    SR.DiscardBufferedData();//Clear the buffer created by the last read call.
                    SR.BaseStream.Seek(0, SeekOrigin.Begin);//Reset the cursor.
                    FileStream OverwriteFS = new FileStream(TablePath + "tmp", FileMode.Create);
                    StreamWriter OverwriteSW = new StreamWriter(OverwriteFS);
                    //Read everything and add it to the temporary file.
                    while (!SR.EndOfStream)
                    {
                        string[] TextBuffer = SR.ReadLine().Split(';');
                        UInt16 ReadItemID = Convert.ToUInt16(TextBuffer[0]);
                        string ReadItemName = TextBuffer[1];

                        //If the item is using the same ID, overwrite it.
                        if (ReadItemID == CurrentItemID)
                            OverwriteSW.WriteLine(CurrentItemID + ";" + CurrentItemName);
                        else
                            OverwriteSW.WriteLine(ReadItemID + ";" + ReadItemName);
                    }
                    OverwriteSW.Flush();
                    OverwriteSW.Close();
                    SR.Close();
                    File.Copy(TablePath + "tmp", TablePath, true);
                    File.Delete(TablePath + "tmp");
                }
                else//Don't overwrite, just add it to the end.
                {
                    StreamWriter SW = new StreamWriter(FS);
                    SW.BaseStream.Seek(0, SeekOrigin.End);
                    SW.WriteLine(CurrentItemID + ";" + CurrentItemName);
                    SW.Flush();
                    SW.Close();
                }
            }
            return true;
        }

        protected void DeleteFromIDTable(string TablePath, UInt16 ItemID)
        {
            FileStream FS = new FileStream(TablePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamReader SR = new StreamReader(FS);
            FileStream UpdatedFS = new FileStream(TablePath + "tmp", FileMode.Create);
            StreamWriter UpdatedSW = new StreamWriter(UpdatedFS);

            #region Look for existing ID

            //Read everything
            while (!SR.EndOfStream)
            {
                string[] TextBuffer = SR.ReadLine().Split(';');
                UInt16 ReadItemID = Convert.ToUInt16(TextBuffer[0]);
                string ReadItemName = TextBuffer[1];

                //If the item is using a registred ID, ask to overwrite it.
                if (ReadItemID != ItemID)
                    UpdatedSW.WriteLine(ReadItemID + ";" + ReadItemName);
            }

            #endregion

            SR.DiscardBufferedData();//Clear the buffer created by the last read call.
            SR.BaseStream.Seek(0, SeekOrigin.Begin);//Reset the cursor.
            UpdatedSW.Flush();
            UpdatedSW.Close();
            SR.Close();
            File.Copy(TablePath + "tmp", TablePath, true);
            File.Delete(TablePath + "tmp");
        }

        public override object GetItemKey()
        {
            return Convert.ToUInt16(txtID.Text);
        }
    }
}
