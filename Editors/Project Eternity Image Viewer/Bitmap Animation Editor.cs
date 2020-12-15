﻿using System;
using System.Drawing;
using System.IO;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.BitmapAnimationEditor
{
    public partial class ProjectEternityBitmapAnimationEditor : BaseEditor
    {
        public ProjectEternityBitmapAnimationEditor()
        {
            InitializeComponent();
        }

        public ProjectEternityBitmapAnimationEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
            }
            else
                LoadBitmapAnimation(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        /// <summary>
        /// Load a Bitmap Animation at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Bitmap Animation.</param>
        private void LoadBitmapAnimation(string BitmapAnimationPath)
        {
            string BitmapAnimationName = Path.GetFileNameWithoutExtension(BitmapAnimationPath);

            this.Text = BitmapAnimationName + " - Project Eternity Bitmap Animation Editor";

            if (Path.GetExtension(BitmapAnimationPath) == ".xnb")
            {
                BitmapAnimationViewerControl NewBitmapAnimationViewer = new BitmapAnimationViewerControl();
                NewBitmapAnimationViewer.AnimationPath = BitmapAnimationPath;
                NewBitmapAnimationViewer.Preload();
                panPictureContainer.Controls.Add(NewBitmapAnimationViewer);
                panPictureContainer.PerformLayout();
            }
            else
            {
                System.Windows.Forms.PictureBox pbPicture = new System.Windows.Forms.PictureBox();
                pbPicture.Location = new Point(0, 0);
                pbPicture.Name = "pbPicture";
                pbPicture.Size = new Size(406, 326);
                pbPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
                pbPicture.TabIndex = 0;
                pbPicture.TabStop = false;
                pbPicture.Image = Image.FromFile(BitmapAnimationPath);

                int SriptIndex = BitmapAnimationName.IndexOf("_strip");
                int ImageNumber = Convert.ToInt32(BitmapAnimationName.Substring(SriptIndex + 6));

                txtNumberOfImages.Text = ImageNumber.ToString();
                txtImageWidth.Text = (pbPicture.Image.Width / ImageNumber).ToString();
                txtImageHeight.Text = pbPicture.Image.Height.ToString();
            }
        }
    }
}
