using System;
using System.Text;
using System.IO;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Units.Modular;

namespace ProjectEternity.Editors.UnitModularEditor
{
    public partial class ProjectEternityPartEditor : BaseEditor
    {
        private PartTypes PartType;

        public ProjectEternityPartEditor()
        {
            InitializeComponent();
        }

        public ProjectEternityPartEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            this.PartType = (PartTypes)Params[0];
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadPart(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { "Units/Modular/Head/Antena" },    "Deathmatch/Units/Modular/Head/Antena/",      new string[] { ".pep" }, typeof(ProjectEternityPartEditor),      true, new object[] { PartTypes.HeadAntena       }),
                new EditorInfo(new string[] { "Units/Modular/Head/Ears" },      "Deathmatch/Units/Modular/Head/Ears/",        new string[] { ".pep" }, typeof(ProjectEternityPartEditor),        true, new object[] { PartTypes.HeadEars         }),
                new EditorInfo(new string[] { "Units/Modular/Head/Eyes" },      "Deathmatch/Units/Modular/Head/Eyes/",        new string[] { ".pep" }, typeof(ProjectEternityPartEditor),        true, new object[] { PartTypes.HeadEyes         }),
                new EditorInfo(new string[] { "Units/Modular/Head/CPU" },       "Deathmatch/Units/Modular/Head/CPU/",         new string[] { ".pep" }, typeof(ProjectEternityPartEditor),         true, new object[] { PartTypes.HeadCPU          }),

                new EditorInfo(new string[] { "Units/Modular/Torso/Core" },     "Deathmatch/Units/Modular/Torso/Core/",       new string[] { ".pep" }, typeof(ProjectEternityPartEditor),       true, new object[] { PartTypes.TorsoCore        }),
                new EditorInfo(new string[] { "Units/Modular/Torso/Radiator" }, "Deathmatch/Units/Modular/Torso/Radiator/",   new string[] { ".pep" }, typeof(ProjectEternityPartEditor),   true, new object[] { PartTypes.TorsoRadiator    }),
                new EditorInfo(new string[] { "Units/Modular/Torso/Shell" },    "Deathmatch/Units/Modular/Torso/Shell/",      new string[] { ".pep" }, typeof(ProjectEternityPartEditor),      true, new object[] { PartTypes.Shell       }),

                new EditorInfo(new string[] { "Units/Modular/Arm/Shell",
                                              "Units/Modular/Shell" },           "Deathmatch/Units/Modular/Arm/Shell/",        new string[] { ".pep" }, typeof(ProjectEternityPartEditor),        true, new object[] { PartTypes.Shell         }),
                new EditorInfo(new string[] { "Units/Modular/Arm/Strength",
                                              "Units/Modular/Strength" },        "Deathmatch/Units/Modular/Arm/Strength/",     new string[] { ".pep" }, typeof(ProjectEternityPartEditor),     true, new object[] { PartTypes.Strength      }),

                new EditorInfo(new string[] { "Units/Modular/Legs/Shell",
                                              "Units/Modular/Shell" },           "Deathmatch/Units/Modular/Legs/Shell/",       new string[] { ".pep" }, typeof(ProjectEternityPartEditor),       true, new object[] { PartTypes.Shell        }),
                new EditorInfo(new string[] { "Units/Modular/Legs/Strength",
                                              "Units/Modular/Strength" },        "Deathmatch/Units/Modular/Legs/Strength/",    new string[] { ".pep" }, typeof(ProjectEternityPartEditor),    true, new object[] { PartTypes.Strength     }),

                new EditorInfo(new string[] { "Units/Modular/Generic/Shell",
                                              "Units/Modular/Shell",
                                              "Units/Modular/Arm/Shell",
                                              "Units/Modular/Legs/Shell" },      "Deathmatch/Units/Modular/Generic/Shell/",    new string[] { ".pep" }, typeof(ProjectEternityPartEditor),    true, new object[] { PartTypes.Shell     }),

                new EditorInfo(new string[] { "Units/Modular/Generic/Strength",
                                              "Units/Modular/Strength",
                                              "Units/Modular/Arm/Strength",
                                              "Units/Modular/Legs/Strength"},    "Deathmatch/Units/Modular/Generic/Strength/", new string[] { ".pep" }, typeof(ProjectEternityPartEditor), true, new object[] { PartTypes.Strength  })
            };
            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtDescription.Text);
            BW.Write((int)txtPrice.Value);

            BW.Write((int)txtBaseHP.Value);
            BW.Write((int)txtBaseEN.Value);
            BW.Write((int)txtBaseArmor.Value);
            BW.Write((int)txtBaseMobility.Value);
            BW.Write((int)txtBaseMovement.Value);

            BW.Write((int)txtMEL.Value);
            BW.Write((int)txtRNG.Value);
            BW.Write((int)txtDEF.Value);
            BW.Write((int)txtSKL.Value);
            BW.Write((int)txtEVA.Value);
            BW.Write((int)txtHIT.Value);

            FS.Close();
            BW.Close();
        }

        /// <summary>
        /// Load a Part item at selected path.
        /// </summary>
        /// <param name="PartPath">Path from which to open the Part.</param>
        private void LoadPart(string PartPath)
        {
            string Name = PartPath.Substring(0, PartPath.Length - 4).Substring(22);
            switch(PartType)
            {
                case PartTypes.HeadAntena:
                    Name = Name.Substring(12);
                    break;

                case PartTypes.HeadEars:
                    Name = Name.Substring(10);
                    break;

                case PartTypes.HeadEyes:
                    Name = Name.Substring(10);
                    break;

                case PartTypes.HeadCPU:
                    Name = Name.Substring(9);
                    break;

                case PartTypes.TorsoCore:
                    Name = Name.Substring(11);
                    break;

                case PartTypes.TorsoRadiator:
                    Name = Name.Substring(15);
                    break;
            }
            Part LoadedPart = new Part(Name, PartType);

            //Update the editor's members.
            txtPrice.Text = LoadedPart.Price.ToString();
            txtDescription.Text = LoadedPart.Description;

            //Update the editor's controls.
            txtBaseHP.Text = LoadedPart.BaseHP.ToString();
            txtBaseEN.Text = LoadedPart.BaseEN.ToString();
            txtBaseArmor.Text = LoadedPart.BaseArmor.ToString();
            txtBaseMobility.Text = LoadedPart.BaseMobility.ToString();
            txtBaseMovement.Text = LoadedPart.BaseMovement.ToString();

            txtMEL.Text = LoadedPart.MEL.ToString();
            txtRNG.Text = LoadedPart.RNG.ToString();
            txtDEF.Text = LoadedPart.DEF.ToString();
            txtSKL.Text = LoadedPart.SKL.ToString();
            txtEVA.Text = LoadedPart.EVA.ToString();
            txtHIT.Text = LoadedPart.HIT.ToString();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }
    }
}
