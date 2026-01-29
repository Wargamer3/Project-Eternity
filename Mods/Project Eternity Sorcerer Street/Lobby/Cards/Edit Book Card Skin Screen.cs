using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework.Content.Builder;
using System.Threading;
using FMOD;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class EditBookCardSkinScreen : GameScreen
    {
        #region Ressources

        protected FMODSound sndButtonOver;
        protected FMODSound sndButtonClick;

        private SpriteFont fntMenuTextBigger;
        private SpriteFont fntArial26;
        private SpriteFont fntOxanimumBoldTitle;
        private SpriteFont fntOxanimumLightBigger;
        private SpriteFont fntOxanimumRegular;

        private CardSymbols Symbols;

        #endregion

        private readonly Player ActivePlayer;
        private CardInfo ActiveCard;
        private AnimatedModelTransparent Map3DModel;

        protected TextButton CancelButton;
        protected TextButton OKButton;

        protected TextInput RoomNameInput;

        private IUIElement[] ArrayMenuButton;

        private int CursorIndex;
        private bool IsChangingName;

        public EditBookCardSkinScreen(Player ActivePlayer, CardInfo ActiveCard)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            this.ActivePlayer = ActivePlayer;
            InitCard(ActiveCard);
        }

        public override void Load()
        {
            sndButtonOver = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Over.wav");
            sndButtonClick = new FMODSound(FMODSystem, "Content/Triple Thunder/Menus/SFX/Button Click.wav");

            fntMenuTextBigger = Content.Load<SpriteFont>("Fonts/Arial18");
            fntArial26 = Content.Load<SpriteFont>("Fonts/Arial26");
            fntOxanimumRegular = Content.Load<SpriteFont>("Fonts/Oxanium Regular");
            fntOxanimumLightBigger = Content.Load<SpriteFont>("Fonts/Oxanium Light Bigger");
            fntOxanimumBoldTitle = GameScreen.ContentFallback.Load<SpriteFont>("Fonts/Oxanium Bold Title");

            Symbols = CardSymbols.Symbols;

            float Ratio = Constants.Height / 2160f;
            int MenuWidth = (int)(1000 * Ratio);
            int MenuHeight = (int)(60 * Ratio);
            int MenuX = Constants.Width / 2 - MenuWidth / 2;
            int MenuY = Constants.Height / 2 - MenuHeight / 2;

            OKButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}OK}}", "Deathmatch/Lobby Menu/Popup/Button Small Blue", new Vector2((int)(1600 * Ratio), (int)(680 * Ratio)), 4, 1, Ratio, OnButtonOver, OnCreateARoomPressed);
            CancelButton = new TextButton(Content, "{{Text:{Font:Oxanium Bold Bigger}{Centered}{Color:243, 243, 243, 255}Cancel}}", "Deathmatch/Lobby Menu/Popup/Button Small Grey", new Vector2((int)(2200 * Ratio), (int)(680 * Ratio)), 4, 1, Ratio, OnButtonOver, Cancel);

            RoomNameInput = new TextInput(fntOxanimumLightBigger, sprPixel, sprPixel, new Vector2(1420 * Ratio, 506 * Ratio), new Vector2(MenuWidth, MenuHeight));

            ArrayMenuButton = new IUIElement[]
            {
                RoomNameInput, 
                CancelButton, OKButton,
            };
        }

        private void InitCard(CardInfo ActiveCard)
        {
            this.ActiveCard = ActiveCard;

            if (ActiveCard.Card is CreatureCard)
            {
                Map3DModel = new AnimatedModelTransparent(((CreatureCard)ActiveCard.Card).GamePiece.Unit3DModel);
            }
        }

        public override void Update(GameTime gameTime)
        {
            SorcererStreetInventoryScreen.CubeBackground.Update(gameTime);

            if (IsChangingName)
            {
                foreach (IUIElement ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Update(gameTime);
                }
                return;
            }

            if (InputHelper.InputConfirmReleased())
            {
                switch (CursorIndex)
                {
                    case 0:
                        CreateNewSkin();
                        break;

                    case 1:
                        if (ActiveCard.SelectedSkinIndex >= 0)
                        {
                            RoomNameInput.SetText(ActiveCard.CardSkin.Name);
                            RoomNameInput.Select();
                            IsChangingName = true;
                        }
                        break;

                    case 2:
                        Thread ChangeTextureThread = new Thread(ChangeTexture);
                        ChangeTextureThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        ChangeTextureThread.Start();
                        ChangeTextureThread.Join(); //Wait for the thread to end
                        break;

                    case 3:
                        Thread ChangeModelThread = new Thread(ChangeModel);
                        ChangeModelThread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        ChangeModelThread.Start();
                        ChangeModelThread.Join(); //Wait for the thread to end
                        break;
                }
            }
            else if (InputHelper.InputUpReleased())
            {
                --CursorIndex;
            }
            else if (InputHelper.InputDownReleased() && ActiveCard.QuantityOwned - 1 > 0)
            {
                ++CursorIndex;
            }
            else if (KeyboardHelper.KeyReleased(Microsoft.Xna.Framework.Input.Keys.E))
            {
                CreateNewSkin();
            }
            else if (InputHelper.InputRightReleased() && (ActiveCard.ListOwnedCardSkin.Count > 0 || ActiveCard.ListOwnedCardAlt.Count > 0))
            {
                ActiveCard.SelectedSkinIndex++;

                if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardSkin[ActiveCard.SelectedSkinIndex].CardSkin;
                }
                else if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardAlt[ActiveCard.SelectedSkinIndex - ActiveCard.ListOwnedCardSkin.Count].CardSkin;
                }
                else
                {
                    ActiveCard.SelectedSkinIndex = -1;
                    ActiveCard.CardSkin = ActiveCard.Card;
                }

                Map3DModel = new AnimatedModelTransparent(((CreatureCard)ActiveCard.CardSkin).GamePiece.Unit3DModel);
            }
            else if (InputHelper.InputLeftReleased() && (ActiveCard.ListOwnedCardSkin.Count > 0 || ActiveCard.ListOwnedCardAlt.Count > 0))
            {
                ActiveCard.SelectedSkinIndex--;

                if (ActiveCard.SelectedSkinIndex < 0)
                {
                    ActiveCard.SelectedSkinIndex = ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count - 1;
                    ActiveCard.CardSkin = ActiveCard.Card;
                }
                else if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardSkin[ActiveCard.SelectedSkinIndex].CardSkin;
                }
                else if (ActiveCard.SelectedSkinIndex < ActiveCard.ListOwnedCardSkin.Count + ActiveCard.ListOwnedCardAlt.Count)
                {
                    ActiveCard.CardSkin = ActiveCard.ListOwnedCardAlt[ActiveCard.SelectedSkinIndex - ActiveCard.ListOwnedCardSkin.Count].CardSkin;
                }

                Map3DModel = new AnimatedModelTransparent(((CreatureCard)ActiveCard.CardSkin).GamePiece.Unit3DModel);
            }
        }

        private void OnButtonOver()
        {
            sndButtonOver.Play();
        }

        protected void OnCreateARoomPressed()
        {
            sndButtonClick.Play();

            ActiveCard.CardSkin.Name = RoomNameInput.Text;
            IsChangingName = false;
        }

        public void Cancel()
        {
            sndButtonClick.Play();

            IsChangingName = false;
        }

        private void CreateNewSkin()
        {
            Card NewSkinCard = new CreatureCard((CreatureCard)ActiveCard.Card, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
            ActiveCard.ListOwnedCardSkin.Add(new CardSkinInfo(ActiveCard.Card.Name, ActiveCard.Card.Name, ActiveCard.Card.Name, NewSkinCard, true));
            ActiveCard.SelectedSkinIndex = ActiveCard.ListOwnedCardSkin.Count - 1;
            ActiveCard.CardSkin = NewSkinCard;
        }

        private void ChangeTexture()
        {
            if (ActiveCard.SelectedSkinIndex < 0)
            {
                return;
            }

            var SpriteFileDialog = new OpenFileDialog()
            {
                FileName = "Select a sprite to import",
                Filter = "Sprite files (*.png)|*.png",
                Title = "Open sprite file"
            };

            if (SpriteFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = SpriteFileDialog.FileName;
                string fileName = SpriteFileDialog.SafeFileName;
                fileName = fileName.Substring(0, fileName.Length - 4);
                ContentBuilder Builder = new ContentBuilder();
                Builder.Add(filePath, fileName, "TextureImporter", "TextureProcessor");
                string buildError = Builder.Build();

                string NewSpriteFileName = ActiveCard.Card.Name + ActiveCard.SelectedSkinIndex;
                string NewSpriteRootFolder = "Content/Sorcerer Street/Creature Cards";
                string NewSpriteFileFolder = Path.GetDirectoryName(ActiveCard.Card.Path.Substring(0, ActiveCard.Card.Path.Length - ActiveCard.Card.Name.Length));
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, NewSpriteRootFolder + "\\" + NewSpriteFileFolder);

                ActiveCard.CardSkin.sprCard = Content.Load<Texture2D>("Sorcerer Street/Creature Cards/" + NewSpriteFileFolder + " \\" + NewSpriteFileName);
            }
        }

        private void ChangeModel()
        {
            if (ActiveCard.SelectedSkinIndex < 0)
            {
                return;
            }

            var ModelFileDialog = new OpenFileDialog()
            {
                FileName = "Select a model to import",
                Filter = "Sprite files (*.fbx)|*.fbx",
                Title = "Open model file"
            };

            if (ModelFileDialog.ShowDialog() == DialogResult.OK)
            {
                string NewSkinRootFolder = "Content/Sorcerer Street/Models/Creatures";
                string NewSkinFileFolder = Path.GetDirectoryName(ActiveCard.Card.Path.Substring(0, ActiveCard.Card.Path.Length - ActiveCard.Card.Name.Length)) + "\\" + ActiveCard.Card.Name + ActiveCard.SelectedSkinIndex;
                string FinalFolder = NewSkinRootFolder + "\\" + NewSkinFileFolder;

                string ModelFilePath = ModelFileDialog.FileName;
                string ModelFileName = ModelFileDialog.SafeFileName;
                ModelFileName = ModelFileName.Substring(0, ModelFileName.Length - 4);
                ContentBuilder Builder = new ContentBuilder();
                Builder.Add(ModelFilePath, ModelFileName, "FbxImporter", "AnimationProcessor");

                var SpriteFileDialog = new OpenFileDialog()
                {
                    FileName = "Select a sprite to import",
                    Filter = "Sprite files (*.png)|*.png",
                    Title = "Open sprite file"
                };

                if (SpriteFileDialog.ShowDialog() == DialogResult.OK)
                {
                    for (int F = 0; F < SpriteFileDialog.FileNames.Length; F++)
                    {
                        var SpriteFilePath = SpriteFileDialog.FileNames[F];
                        var SpriteFileName = SpriteFileDialog.SafeFileNames[F];
                        SpriteFileName = SpriteFileName.Substring(0, SpriteFileName.Length - 4);
                        Builder.Add(SpriteFilePath, SpriteFileName, "TextureImporter", "TextureProcessor");
                    }
                    string buildError = Builder.Build();

                    Builder.CopyBuildOutput(FinalFolder);
                }

                ((CreatureCard)ActiveCard.CardSkin).GamePiece.Unit3DModel = AnimatedModelTransparent.Load(Content, "Sorcerer Street/Models/Creatures/" + NewSkinFileFolder + "/" + ModelFileName);
                Map3DModel = new AnimatedModelTransparent(((CreatureCard)ActiveCard.CardSkin).GamePiece.Unit3DModel);
                Map3DModel.DisableLights();
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            SorcererStreetInventoryScreen.CubeBackground.BeginDraw(g);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float Ratio = Constants.Height / 2160f;
            Color ColorBox = Color.FromNonPremultiplied(204, 204, 204, 255);
            Color ColorText = Color.FromNonPremultiplied(65, 70, 65, 255);

            SorcererStreetInventoryScreen.CubeBackground.Draw(g, true);

            int DrawX = (int)(210 * Ratio);
            int DrawY = (int)(58 * Ratio);
            g.DrawString(fntOxanimumBoldTitle, "CARD EDIT", new Vector2(DrawX, DrawY), ColorText);
            g.Draw(ActiveCard.CardSkin.sprCard, new Rectangle(Constants.Width / 4, Constants.Height / 2, (int)(652 * Ratio), (int)(816 * Ratio)), null, Color.White, 0f, new Vector2(ActiveCard.CardSkin.sprCard.Width / 2, ActiveCard.CardSkin.sprCard.Height / 2), SpriteEffects.None, 0f);
            ActiveCard.CardSkin.DrawCardInfo(g, Symbols, fntMenuTextBigger, ActivePlayer, 0, (int)(300 * Ratio));

            int BoxWidth = (int)(700 * Ratio);
            int BoxHeight = (int)(60 * 5 * Ratio);
            DrawX = Constants.Width / 2 - BoxWidth / 2;
            DrawY = (int)(380 * Ratio);

            if (IsChangingName)
            {
                BoxWidth = (int)(1200 * Ratio);
                BoxHeight = (int)(360 * Ratio);
                DrawX = Constants.Width / 2 - BoxWidth / 2;
                DrawY = (int)(406 * Ratio);

                MenuHelper.DrawBox(g, new Vector2(DrawX, DrawY), BoxWidth, BoxHeight);

                DrawX += (int)(80 * Ratio);
                DrawY += (int)(20 * Ratio);
                g.DrawString(fntMenuTextBigger, "Enter a new name", new Vector2(DrawX, DrawY), SorcererStreetMap.TextColor);

                DrawX += (int)(20 * Ratio);
                DrawY += (int)(80 * Ratio);
                BoxWidth = (int)(1000 * Ratio);
                g.Draw(sprPixel, new Rectangle(DrawX, DrawY, BoxWidth, (int)(60 * Ratio)), Color.FromNonPremultiplied(0, 0, 0, 200));

                foreach (IUIElement ActiveButton in ArrayMenuButton)
                {
                    ActiveButton.Draw(g);
                }
            }
            else
            {
                MenuHelper.DrawBox(g, new Vector2(DrawX, DrawY), BoxWidth, BoxHeight);
                DrawX = Constants.Width / 2;
                DrawY += (int)(60 * Ratio);
                g.DrawStringCentered(fntMenuTextBigger, "Create New Skin", new Vector2(DrawX, DrawY), SorcererStreetMap.TextColor);
                DrawY += (int)(60 * Ratio);
                g.DrawStringCentered(fntMenuTextBigger, "Change Name", new Vector2(DrawX, DrawY), SorcererStreetMap.TextColor);
                DrawY += (int)(60 * Ratio);
                g.DrawStringCentered(fntMenuTextBigger, "Change Card Image", new Vector2(DrawX, DrawY), SorcererStreetMap.TextColor);
                DrawY += (int)(60 * Ratio);
                g.DrawStringCentered(fntMenuTextBigger, "Change 3D Model", new Vector2(DrawX, DrawY), SorcererStreetMap.TextColor);

                DrawX = Constants.Width / 2 - BoxWidth / 2;
                DrawY = (int)(406 * Ratio);
                g.Draw(sprPixel, new Rectangle(DrawX, DrawY + (int)(60 * Ratio * CursorIndex), BoxWidth, (int)(60 * Ratio)), Color.FromNonPremultiplied(255, 255, 255, 200));
            }

            DrawX = (int)(212 * Ratio);
            DrawY = (int)(2008 * Ratio);
            g.DrawString(fntOxanimumRegular,
                  " [Left-Right] Change Skins"
                + " [E] Create New Skin"
                + " [Q] Toggle Info"
                + " [Z] Return", new Vector2(DrawX, DrawY), ColorText);

            DrawX = Constants.Width / 2;
            DrawY = (int)(1708 * Ratio);

            if (ActiveCard.SelectedSkinIndex < 0)
            {
                g.DrawStringCentered(fntOxanimumRegular, "Current Skin: Default", new Vector2(DrawX, DrawY), ColorText);
            }
            else
            {
                g.DrawStringCentered(fntOxanimumRegular, "Current Skin "+ (ActiveCard.SelectedSkinIndex + 1) + ":" + ActiveCard.CardSkin.Name, new Vector2(DrawX, DrawY), ColorText);
            }

            g.End();
            g.Begin();

            var World = Matrix.CreateRotationX(MathHelper.ToRadians(180)) * Matrix.CreateRotationY(MathHelper.ToRadians(-45)) * Matrix.CreateRotationX(MathHelper.ToRadians(45))
                * Matrix.CreateScale(2f) * Matrix.CreateTranslation(Constants.Width / 2, Constants.Height / 2 + 140, 0);
            float aspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Height, 0, 600, -700);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Projection = HalfPixelOffset * Projection;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GameScreen.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GameScreen.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1f, 0);

            Map3DModel.Draw3D(GraphicsDevice, Matrix.Identity, Projection, World);
        }
    }
}
