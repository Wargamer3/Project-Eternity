using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Core.Magic
{
    public class MagicEditor : GameScreen
    {
        private MagicSpell ActiveMagicSpell;
        public Vector2 CursorPosition;
        public List<MagicElement> ListMagicElement;
        public Texture2D sprMagicCircle;
        public ActionPanelHolder ListActionMenuChoice;
        public float ZoomLevel;
        public Point CameraOffset;
        public MagicElement MagicElementSelected;
        public int SelectionRadius;

        public Matrix Transform;

        private readonly Projectile2DContext GlobalProjectileContext;
        private readonly Projectile2DParams.SharedProjectileParams SharedParams;

        public MagicEditor(MagicSpell ActiveMagicSpell, Projectile2DContext GlobalProjectileContext, Projectile2DParams.SharedProjectileParams SharedParams)
        {
            this.ActiveMagicSpell = ActiveMagicSpell;
            this.GlobalProjectileContext = GlobalProjectileContext;
            this.SharedParams = SharedParams;
            ZoomLevel = 1f;

            SelectionRadius = 10;
            MagicElementSelected = null;

            ListMagicElement = new List<MagicElement>();
            ListMagicElement.AddRange(ActiveMagicSpell.ListMagicCore);
            ListActionMenuChoice = new ActionPanelHolder();
        }

        public override void Load()
        {
            sprMagicCircle = Content.Load<Texture2D>("Triple Thunder/Magic Ring");

            for (int i = ListMagicElement.Count - 1; i >= 0; --i)
            {
                MagicElement ActiveMagicElement = ListMagicElement[i];
                InitMagicElement(ActiveMagicElement);
            }
        }

        public void InitMagicElement(MagicElement ActiveMagicElement)
        {
            ActiveMagicElement.InitGraphics(Content);
            
            foreach (MagicElement FollowingMagicElement in ActiveMagicElement.ListLinkedMagicElement)
            {
                InitMagicElement(FollowingMagicElement);
                ListMagicElement.Add(FollowingMagicElement);
            }
        }

        public override void Update(GameTime gameTime)
        {
            CursorPosition = new Vector2(MouseHelper.MouseStateCurrent.X, MouseHelper.MouseStateCurrent.Y);

            Point Center = new Point(-CameraOffset.X - Constants.Width / 2, -CameraOffset.Y - Constants.Height / 2);

            Transform = Matrix.CreateTranslation(-Center.X, -Center.Y, 0)
                                                * Matrix.CreateScale(ZoomLevel) *
                                                Matrix.CreateTranslation(CameraOffset.X, CameraOffset.Y, 0);

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Update(gameTime);
            }

            DoUpdateEditor(gameTime);

            if (KeyboardHelper.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                FinishEditing();
                ActiveMagicSpell.ComputeSpell();
                RemoveScreen(this);
            }
        }

        public void FinishEditing()
        {
            ActiveMagicSpell.ListMagicCore.Clear();
            foreach (MagicElement ActiveElement in ListMagicElement)
            {
                if (ActiveElement is MagicCore)
                {
                    MagicCore ActiveCore = (MagicCore)ActiveElement;
                    bool IsRoot = true;

                    foreach (MagicElement OtherElement in ListMagicElement)
                    {
                        if (OtherElement == ActiveElement)
                            continue;

                        if (OtherElement.ListLinkedMagicElement.Contains(ActiveElement))
                        {
                            IsRoot = false;
                            break;
                        }
                    }

                    if (IsRoot)
                    {
                        ActiveMagicSpell.ListMagicCore.Add(ActiveCore);
                    }
                }
            }
        }

        public void DoUpdateEditor(GameTime gameTime)
        {
            if (MouseHelper.InputLeftButtonPressed())
            {
                if (CursorPosition.X >= Constants.Width - 200 && CursorPosition.Y <= 30)
                {
                    if (ListActionMenuChoice.HasMainPanel && !(ListActionMenuChoice.Last() is MagicElementSelectionPanel))
                    {
                        ListActionMenuChoice.RemoveAllActionPanels();
                    }
                    if (!ListActionMenuChoice.HasMainPanel)
                    {
                        ListActionMenuChoice.AddToPanelListAndSelect(new MagicElementSelectionPanel(this));
                    }
                    else if(ListActionMenuChoice.HasMainPanel && ListActionMenuChoice.Last() is MagicElementSelectionPanel)
                    {
                        ListActionMenuChoice.RemoveAllActionPanels();
                    }
                }
                else if (CursorPosition.X >= Constants.Width - 400 && CursorPosition.Y <= 30)
                {
                    ListActionMenuChoice.RemoveAllActionPanels();
                    ListActionMenuChoice.AddToPanelListAndSelect(new MagicPreviewerPanel(Content, ActiveMagicSpell, ListActionMenuChoice, GlobalProjectileContext, SharedParams));
                }
                else
                {
                    if (ListActionMenuChoice.HasMainPanel && CursorPosition.X >= Constants.Width - 200)
                    {
                        return;
                    }

                    if (ListActionMenuChoice.HasMainPanel && ListActionMenuChoice.Last() is MagicAttributesEditonPanel)
                    {
                        ListActionMenuChoice.RemoveAllActionPanels();
                    }

                    foreach (MagicElement ActiveMagicElement in ListMagicElement)
                    {
                        if (Math.Sqrt(Math.Pow(CursorPosition.X - ActiveMagicElement.Position.X, 2) + Math.Pow(CursorPosition.Y - ActiveMagicElement.Position.Y, 2)) < ActiveMagicElement.Radius)
                        {
                            MagicElementSelected = ActiveMagicElement;
                            ListActionMenuChoice.AddToPanelListAndSelect(new MagicAttributesEditonPanel(this, ActiveMagicElement));
                            break;
                        }
                    }
                }
            }
            else if (MouseHelper.InputLeftButtonReleased())
            {
                MagicElementSelected = null;
            }
            else if (MouseHelper.InputLeftButtonHold())
            {
                if (MagicElementSelected != null)
                {
                    MagicElementSelected.Position = CursorPosition;
                    MagicElementSelected.IsValid = true;

                    //Unlink all Magic
                    foreach (MagicElement ActiveMagicElement in ListMagicElement)
                    {
                        ActiveMagicElement.ListLinkedMagicElement.Remove(MagicElementSelected);
                        if (ActiveMagicElement == MagicElementSelected)
                            continue;

                        double DistanceBetweenMagicElement = Math.Sqrt(Math.Pow(MagicElementSelected.Position.X - ActiveMagicElement.Position.X, 2)
                                                                        + Math.Pow(MagicElementSelected.Position.Y - ActiveMagicElement.Position.Y, 2));

                        float TotalRadius = MagicElementSelected.Radius + ActiveMagicElement.Radius;

                        // Collision
                        if (DistanceBetweenMagicElement <= TotalRadius)
                        {
                            MagicElementSelected.IsValid = false;
                            return;
                        }
                    }

                    //Recompute all Magic links
                    foreach (MagicElement ActiveMagicElement in ListMagicElement)
                    {
                        if (ActiveMagicElement == MagicElementSelected)
                            continue;

                        double DistanceBetweenMagicElement = Math.Sqrt(Math.Pow(MagicElementSelected.Position.X - ActiveMagicElement.Position.X, 2)
                                                                        + Math.Pow(MagicElementSelected.Position.Y - ActiveMagicElement.Position.Y, 2));

                        float TotalRadius = MagicElementSelected.Radius + ActiveMagicElement.Radius;

                        // Collision
                        if (DistanceBetweenMagicElement > TotalRadius && DistanceBetweenMagicElement < TotalRadius + SelectionRadius)
                        {
                            ActiveMagicElement.ListLinkedMagicElement.Add(MagicElementSelected);
                            break;
                        }
                    }
                }
                else if (KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                {
                    CameraOffset.X += MouseHelper.MouseStateCurrent.X - MouseHelper.MouseStateLast.X;
                    CameraOffset.Y += MouseHelper.MouseStateCurrent.Y - MouseHelper.MouseStateLast.Y;

                    ZoomLevel += (MouseHelper.MouseStateCurrent.ScrollWheelValue - MouseHelper.MouseStateLast.ScrollWheelValue) / 100;
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(sprPixel, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
            DrawBox(g, new Vector2(0, 0), Constants.Width, 30, Color.Black);
            TextHelper.DrawText(g, "Name: Test Magic", new Vector2(5, 5), Color.White);
            TextHelper.DrawTextRightAligned(g, "Total cost: " + 15, new Vector2(435, 5), Color.White);
            DrawBox(g, new Vector2(Constants.Width - 400, 0), 200, 30, Color.Black);
            TextHelper.DrawTextMiddleAligned(g, "Preview Spell",
                new Vector2(Constants.Width - 300, 5), Color.White);
            DrawBox(g, new Vector2(Constants.Width - 200, 0), 200, 30, Color.Black);
            TextHelper.DrawTextMiddleAligned(g, "Add Magic Element",
                new Vector2(Constants.Width - 100, 5), Color.White);

            g.End();

            g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.Identity);

            foreach (MagicElement ActiveMagicElement in ListMagicElement)
            {
                if (ActiveMagicElement.IsValid)
                {
                    if (ActiveMagicElement.ListLinkedMagicElement.Count > 0)
                    {
                        g.Draw(sprMagicCircle, new Rectangle((int)ActiveMagicElement.Position.X,
                                                            (int)ActiveMagicElement.Position.Y,
                                                            (int)ActiveMagicElement.Radius * 2,
                                                            (int)ActiveMagicElement.Radius * 2), null,
                                                            Color.Blue,
                                                            0f,
                                                            new Vector2(sprMagicCircle.Width * 0.5f), SpriteEffects.None, 1);
                    }
                    else
                    {
                        g.Draw(sprMagicCircle, new Rectangle((int)ActiveMagicElement.Position.X,
                                                            (int)ActiveMagicElement.Position.Y,
                                                            (int)ActiveMagicElement.Radius * 2,
                                                            (int)ActiveMagicElement.Radius * 2), null,
                                                            Color.White,
                                                            0f,
                                                            new Vector2(sprMagicCircle.Width * 0.5f), SpriteEffects.None, 1);
                    }
                }
                else
                {
                    g.Draw(sprMagicCircle, new Rectangle((int)ActiveMagicElement.Position.X,
                                                        (int)ActiveMagicElement.Position.Y,
                                                        (int)ActiveMagicElement.Radius * 2,
                                                        (int)ActiveMagicElement.Radius * 2), null,
                                                        Color.Red,
                                                        0f,
                                                        new Vector2(sprMagicCircle.Width * 0.5f), SpriteEffects.None, 1);
                }

                foreach (MagicElement FollowingMagicElement in ActiveMagicElement.ListLinkedMagicElement)
                {
                    Vector2 Point1 = ActiveMagicElement.Position;
                    Vector2 Point2 = FollowingMagicElement.Position;
                    Vector2 Difference = Vector2.Normalize(Point2 - Point1);

                    Point1 = Point1 + Difference * ActiveMagicElement.Radius;
                    Point2 = Point2 - Difference * ActiveMagicElement.Radius;
                    DrawLine(g, Point1, Point2, Color.Black, 2);
                }


                TextHelper.DrawTextMiddleAligned(g, ActiveMagicElement.Name,
                    new Vector2(ActiveMagicElement.Position.X,
                                ActiveMagicElement.Position.Y - 10), Color.White);
            }

            g.End();

            g.Begin();

            if (ListActionMenuChoice.HasMainPanel)
            {
                ListActionMenuChoice.Last().Draw(g);
            }
        }
    }
}
