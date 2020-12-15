using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using FMOD;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class BattleMapSelect : GameScreen
    {
        private struct MissionInfo
        {
            public readonly string MissionName;
            public readonly string MissionPath;
            public readonly string MissionDescription;
            public readonly Texture2D MissionImage;
            public readonly Texture2D MissionImageThumbnail;

            public MissionInfo(string MissionName, string MissionPath, string MissionDescription, Texture2D MissionImage, Texture2D MissionImageThumbnail)
            {
                this.MissionName = MissionName;
                this.MissionPath = MissionPath;
                this.MissionDescription = MissionDescription;
                this.MissionImage = MissionImage;
                this.MissionImageThumbnail = MissionImageThumbnail;
            }
        }

        private FMODSound sndButtonOver;
        private FMODSound sndButtonClick;

        private Texture2D sprBackground;
        private AnimatedSprite MapSelectBackgroundButton;

        private SpriteFont fntText;

        private readonly RoomInformations Room;
        private readonly BattleSelect Owner;
        private readonly List<MissionInfo> ListMissionInfo;

        private MissionInfo SelectedMap;

        public BattleMapSelect(RoomInformations Room, BattleSelect Owner)
        {
            this.Room = Room;
            this.Owner = Owner;
            ListMissionInfo = new List<MissionInfo>();
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            sprBackground = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Select Background");

            MapSelectBackgroundButton = new AnimatedSprite(Content, "Triple Thunder/Menus/Wait Room/Map Select Button Background", new Vector2(0, 0), 0, 1, 4);

            LoadMaps();

            SelectedMap = ListMissionInfo[0];
        }

        public override void Update(GameTime gameTime)
        {
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        public void LoadMaps()
        {
            ListMissionInfo.Clear();
            ListMissionInfo.Add(new MissionInfo("Random", "Battle/Random", "",
                Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/Random"),
                Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/Random_s")));

            DirectoryInfo MapDirectory = new DirectoryInfo(Content.RootDirectory + "/Maps/Triple Thunder/Battle/");

            FileInfo[] ArrayMapFile = MapDirectory.GetFiles("*.ttm");
            foreach (FileInfo ActiveFile in ArrayMapFile)
            {
                FileStream FS = new FileStream(ActiveFile.FullName, FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                string BackgroundName = BR.ReadString();

                Rectangle CameraBounds = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32(), BR.ReadInt32());
                string BGMPath = BR.ReadString();
                string Description = BR.ReadString();

                BR.Close();
                FS.Close();

                string FileName = ActiveFile.Name.Remove(ActiveFile.Name.Length - 4);

                Texture2D sprMissionImage = null;
                Texture2D sprMissionImageThumnail = null;

                if (File.Exists("Content/Triple Thunder/Menus/Wait Room/Map Icon " + FileName + ".xnb"))
                {
                    sprMissionImage = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/ " + FileName);
                    sprMissionImageThumnail = Content.Load<Texture2D>("Triple Thunder/Menus/Wait Room/Map Icons/ " + FileName + "_s");
                }

                ListMissionInfo.Add(new MissionInfo(FileName, "Battle/" + FileName, Description, sprMissionImage, sprMissionImageThumnail));
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int DrawX = Constants.Width / 2 - sprBackground.Width / 2;
            int DrawY = Constants.Height / 2 - sprBackground.Height / 2;
            g.Draw(sprBackground, new Vector2(DrawX, DrawY), Color.White);

            for (int M = 0; M < ListMissionInfo.Count; ++M)
            {
                MapSelectBackgroundButton.Draw(g, new Vector2(DrawX + 250, DrawY + 172 + M * 41), Color.White);
                g.DrawString(fntText, ListMissionInfo[M].MissionName, new Vector2(DrawX + 136, DrawY + 162 + M * 41), Color.White);

                if (ListMissionInfo[M].MissionImage != null)
                {
                    g.Draw(ListMissionInfo[0].MissionImage, new Vector2(DrawX + 20, DrawY + 38), Color.White);
                }

                if (ListMissionInfo[M].MissionImage != null)
                {
                    g.Draw(ListMissionInfo[M].MissionImageThumbnail, new Vector2(DrawX + 26, DrawY + 155 + M * 41), Color.White);
                }
            }
        }
    }
}
