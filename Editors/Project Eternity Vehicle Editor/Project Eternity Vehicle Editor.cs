using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.Core.Vehicle;

namespace ProjectEternity.Editors.VehicleEditor
{
    public partial class VehicleEditor : BaseEditor
    {
        private enum ItemSelectionChoices { VehicleSprite };

        private ItemSelectionChoices ItemSelectionChoice;

        public VehicleEditor()
        {
            InitializeComponent();
        }

        public VehicleEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                //Create the Part file.
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Empty Script list.

                FS.Close();
                BW.Close();
            }

            LoadVehicle(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathVehicles }, "Vehicles/", new string[] { ".pev" }, typeof(VehicleEditor)),
                new EditorInfo(new string[] { GUIRootPathVehicleSprites }, "Vehicles/Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtVehicleSpriteName.Text);
            BW.Write((float)txtMaxMV.Value);
            BW.Write((float)txtAcceleration.Value);
            BW.Write((float)txtMaxSpeed.Value);
            BW.Write((float)txtTurnSpeed.Value);
            BW.Write((float)txtMaxClimbAngle.Value);
            BW.Write(cbVehicleType.Text);
            BW.Write((int)txtMaxHP.Value);

            BW.Write(VehiclePreviewViewerControl.ListSeat.Count);
            for (int S = 0; S < VehiclePreviewViewerControl.ListSeat.Count; ++S)
            {
                VehiclePreviewViewerControl.ListSeat[S].Save(BW);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadVehicle(string VehiclePath)
        {
            VehiclePreviewViewerControl.Preload();

            string Name = VehiclePath.Substring(0, VehiclePath.Length - 4).Substring(17);

            this.Text = Name + " - Project Eternity Vehicle Editor";

            Vehicle LoadedVehicle = new Vehicle(Name, VehiclePreviewViewerControl.content);

            VehiclePreviewViewerControl.SetSprite(LoadedVehicle.SpritePath);

            txtVehicleSpriteName.Text = LoadedVehicle.SpritePath;
            txtMaxMV.Value = (decimal)LoadedVehicle.MaxMV;
            txtAcceleration.Value = (decimal)LoadedVehicle.Acceleration;
            txtMaxSpeed.Value = (decimal)LoadedVehicle.MaxSpeed;
            txtTurnSpeed.Value = (decimal)LoadedVehicle.TurnSpeed;
            txtMaxClimbAngle.Value = (decimal)LoadedVehicle.MaxClimbAngle;
            cbVehicleType.Text = LoadedVehicle.ControlType;
            txtMaxHP.Value = LoadedVehicle.MaxHP;

            VehiclePreviewViewerControl.ListSeat = LoadedVehicle.ListSeat;

            for (int S = 0; S < LoadedVehicle.ListSeat.Count; ++S)
            {
                lsSeats.Items.Add(LoadedVehicle.ListSeat[S].Name);
            }

            UpdateSeatsText();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void btnSelectVehicleSprite_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.VehicleSprite;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathVehicleSprites));
        }

        private void btnAddSeat_Click(object sender, EventArgs e)
        {
            VehiclePreviewViewerControl.ListSeat.Add(new VehicleSeat());
            lsSeats.Items.Add("New Seat");
            lsSeats.SelectedIndex = lsSeats.Items.Count - 1;
        }

        private void btnRemoveSeat_Click(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehiclePreviewViewerControl.ListSeat.RemoveAt(lsSeats.SelectedIndex);
                lsSeats.Items.RemoveAt(lsSeats.SelectedIndex);
            }
        }

        private void lsSeats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                txtSeatName.Text = ActiveSeat.Name;
                ckSeatCanDrive.Checked = ActiveSeat.CanDrive;
                ckSeatIsVisible.Checked = ActiveSeat.IsVisible;
                ckIsProtected.Checked = ActiveSeat.IsProtected;
                txtSeatPositionX.Value = (decimal)ActiveSeat.SeatOffset.X;
                txtSeatPositionY.Value = (decimal)ActiveSeat.SeatOffset.Y;
                txtSeatHeight.Value = (decimal)ActiveSeat.Height;
            }
        }

        #region Seat Info

        private void txtSeatName_TextChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Name = txtSeatName.Text;
                UpdateSeatsText();
            }
        }

        private void ckSeatCanDrive_CheckedChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.CanDrive = ckSeatCanDrive.Checked;
                UpdateSeatsText();
            }
        }

        private void ckSeatIsVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.IsVisible = ckSeatIsVisible.Checked;
                UpdateSeatsText();
            }
        }

        private void ckIsProtected_CheckedChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.IsProtected = ckIsProtected.Checked;
                UpdateSeatsText();
            }
        }

        private void txtSeatPositionX_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.SeatOffset.X = (float)txtSeatPositionX.Value;
                UpdateSeatsText();
            }
        }

        private void txtSeatPositionY_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.SeatOffset.Y = (float)txtSeatPositionY.Value;
                UpdateSeatsText();
            }
        }

        private void txtSeatHeight_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Height = (float)txtSeatHeight.Value;
                UpdateSeatsText();
            }
        }

        private void UpdateSeatsText()
        {
            for (int S = 0; S < VehiclePreviewViewerControl.ListSeat.Count; S++)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[S];

                lsSeats.Items[S] = ActiveSeat.Name + " - " + ActiveSeat.SeatOffset.X + " - " + ActiveSeat.SeatOffset.Y;
            }
        }

        #endregion

        private void btnSeatSelectWeapon_Click(object sender, EventArgs e)
        {

        }

        private void txtWeaponMinAngleLateral_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Weapon.MinAngleLateral = (float)txtWeaponMinAngleLateral.Value;
            }
        }

        private void txtWeaponMaxAngleLateral_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Weapon.MaxAngleLateral = (float)txtWeaponMaxAngleLateral.Value;
            }
        }

        private void txtWeaponMinAngleUpward_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Weapon.MinAngleUpward = (float)txtWeaponMinAngleUpward.Value;
            }
        }

        private void txtWeaponMaxAngleUpward_ValueChanged(object sender, EventArgs e)
        {
            if (lsSeats.SelectedIndex >= 0)
            {
                VehicleSeat ActiveSeat = VehiclePreviewViewerControl.ListSeat[lsSeats.SelectedIndex];
                ActiveSeat.Weapon.MaxAngleUpward = (float)txtWeaponMaxAngleUpward.Value;
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.VehicleSprite:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(25);
                        txtVehicleSpriteName.Text = Name;
                        VehiclePreviewViewerControl.SetSprite(Name);
                        break;
                }
            }
        }
    }
}
