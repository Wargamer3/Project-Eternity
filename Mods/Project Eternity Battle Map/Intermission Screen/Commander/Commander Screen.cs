using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class CommanderScreen : GameScreen
    {
        private Texture2D sprMapMenuBackground;

        private SpriteFont fntArial8;
        private SpriteFont fntArial10;
        private SpriteFont fntArial12;
        private SpriteFont fntFinlanderFont;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;
        private FMODSound sndCancel;

        private BoxScrollbar MapScrollbar;

        private const string MaxCPTitle = "Max CP + 1";
        private const string MaxCPDescription = "[Passive] Increase max CP by 1.";

        private readonly Roster PlayerRoster;

        public List<CommanderTechNode> ListAllNode;
        public List<CommanderTechTree> ListTechTree;

        private int MapScrollbarValue;

        public CommanderScreen(Roster PlayerRoster)
        {
            this.PlayerRoster = PlayerRoster;

            ListTechTree = new List<CommanderTechTree>();
        }

        public override void Load()
        {
            sprMapMenuBackground = Content.Load<Texture2D>("Menus/Status Screen/Background Black");

            fntArial8 = Content.Load<SpriteFont>("Fonts/Arial8");
            fntArial10 = Content.Load<SpriteFont>("Fonts/Arial10");
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
            sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
            sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
            sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

            MapScrollbar = new BoxScrollbar(new Vector2(Constants.Width - 20, 40), Constants.Height - 120, 10, OnGametypeScrollbarChange);

            PopulateGetterNode();
            PopulateMazingerNode();
            PopulateOGNode();
            PopulateGundamNode();
            PopulateNewJerseyNode();
            PopulateRADNode();
        }

        private void PopulateGetterNode()
        {
            CommanderTechTree SaotomeTechTree = new CommanderTechTree("Saotome Institute");
            ListTechTree.Add(SaotomeTechTree);
            CommanderTechNode SaotomeNode = new CommanderTechNode("Saotome Institute", "[Commander] Getter", new Vector2(), new Vector2(64, 64));
            SaotomeTechTree.RootNode = SaotomeNode;
            SaotomeTechTree.ListAllNode.Add(SaotomeNode);

            float NodeX = 0;
            float NodeY = 170;
            float NodeForkY = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("The Flame of Youth", "[Passive] Once per mission, Getter regenerate life when under 1 / 3 of HP.", new Vector2(0, 110)), SaotomeNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(0, NodeY), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(0, NodeY += 40), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(0, NodeY += 40), new Vector2(16, 16)), LastAddedNode);

            LastAddedNodeFork = LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(0, NodeY += 40), new Vector2(16, 16)), LastAddedNode);
            NodeForkY = NodeY;

            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Supplementary Getter Core", "[Skill] Call an helicopter to drop 1 EN crate\n\rRequires Gundam helicopter", new Vector2(NodeX -= 48, NodeY += 48), new Vector2(32, 32)), LastAddedNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Supplementary Getter Core 2", "[Skill] Call an helicopter to drop 2 EN crate\n\rRequires Gundam helicopter", new Vector2(NodeX -= 16, NodeY += 80), new Vector2(32, 32)), LastAddedNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Supplementary Getter Core 3", "[Skill] Call an helicopter to drop 3 EN crate\n\rRequires Gundam helicopter", new Vector2(NodeX -= 16, NodeY += 80), new Vector2(32, 32)), LastAddedNode);

            NodeX = 0;
            NodeY = NodeForkY;
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Evolution's Will", "[Skill] Resurrect a destroyed unit with 10 % of its HP", new Vector2(NodeX += 48, NodeY += 48), new Vector2(32, 32)), LastAddedNodeFork);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Evolution's Will 2", "[Skill] Resurrect a destroyed unit with 30 % of its HP", new Vector2(NodeX += 16, NodeY += 80), new Vector2(32, 32)), LastAddedNode);
            LastAddedNode = AddNode(SaotomeTechTree, new CommanderTechNode("Evolution's Will 3", "[Skill] Resurrect a destroyed unit with 50 % of its HP", new Vector2(NodeX += 16, NodeY += 80), new Vector2(32, 32)), LastAddedNode);
        }

        private void PopulateMazingerNode()
        {
            CommanderTechTree PhotonTechTree = new CommanderTechTree("Photon Power Laboratory");
            ListTechTree.Add(PhotonTechTree);
            CommanderTechNode PhotonNode = new CommanderTechNode("Photon Power Laboratory", "[Commander] Mazinger", new Vector2(), new Vector2(64, 64));
            PhotonTechTree.RootNode = PhotonNode;
            PhotonTechTree.ListAllNode.Add(PhotonNode);

            float NodeX = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(PhotonTechTree, new CommanderTechNode("Max HP + 500", "[Passive] Increase max HP of every allied units by 500.", new Vector2(110, 16)), PhotonNode);
            LastAddedNode = AddNode(PhotonTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(PhotonTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
        }

        private void PopulateOGNode()
        {
            CommanderTechTree OGTechTree = new CommanderTechTree("OG");
            ListTechTree.Add(OGTechTree);
            CommanderTechNode OGNode = new CommanderTechNode("OG", "[Commander] OG", new Vector2(), new Vector2(64, 64));
            OGTechTree.RootNode = OGNode;
            OGTechTree.ListAllNode.Add(OGNode);

            float NodeX = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(OGTechTree, new CommanderTechNode("Max HP + 500", "[Passive] Increase max HP of every allied units by 500.", new Vector2(110, 16)), OGNode);
            LastAddedNode = AddNode(OGTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(OGTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
        }

        private void PopulateGundamNode()
        {
            CommanderTechTree FederationTechTree = new CommanderTechTree("Federation Central Command / Jaburo");
            ListTechTree.Add(FederationTechTree);
            CommanderTechNode FederationNode = new CommanderTechNode("Federation Central Command / Jaburo", "[Commander] Gundam", new Vector2(), new Vector2(64, 64));
            FederationTechTree.RootNode = FederationNode;
            FederationTechTree.ListAllNode.Add(FederationNode);

            float NodeX = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode("Max HP + 500", "[Passive] Increase max HP of every allied units by 500.", new Vector2(110, 16)), FederationNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNodeFork = LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode("Spawn Unit", "[Active] Spawn Zaku.", new Vector2(NodeX += 40, 16)), LastAddedNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode("Spawn Unit", "[Active] Spawn Zaku.", new Vector2(NodeX += 60, 48)), LastAddedNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode("Spawn Unit", "[Active] Spawn Zaku.", new Vector2(NodeX += 60, 80)), LastAddedNode);
            NodeX = 300;
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNodeFork);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(FederationTechTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
        }

        private void PopulateNewJerseyNode()
        {
            CommanderTechTree NewJerseyTree = new CommanderTechTree("New Jersey City Council / Earth Coalition");
            ListTechTree.Add(NewJerseyTree);
            CommanderTechNode NewJerseyNode = new CommanderTechNode("New Jersey City Council / Earth Coalition", "[Commander] Mazinger", new Vector2(), new Vector2(64, 64));
            NewJerseyTree.RootNode = NewJerseyNode;
            NewJerseyTree.ListAllNode.Add(NewJerseyNode);

            float NodeX = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(NewJerseyTree, new CommanderTechNode("Max HP + 500", "[Passive] Increase max HP of every allied units by 500.", new Vector2(110, 16)), NewJerseyNode);
            LastAddedNode = AddNode(NewJerseyTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(NewJerseyTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
        }

        private void PopulateRADNode()
        {
            CommanderTechTree RADTree = new CommanderTechTree("Civilization Preservation Foundation");
            ListTechTree.Add(RADTree);
            CommanderTechNode RADNode = new CommanderTechNode("Civilization Preservation Foundation", "[Commander] RAD", new Vector2(), new Vector2(64, 64));
            RADTree.RootNode = RADNode;
            RADTree.ListAllNode.Add(RADNode);

            float NodeX = 170;
            CommanderTechNode LastAddedNode = null;
            CommanderTechNode LastAddedNodeFork = null;
            LastAddedNode = AddNode(RADTree, new CommanderTechNode("Max HP + 500", "[Passive] Increase max HP of every allied units by 500.", new Vector2(110, 16)), RADTree.RootNode);
            LastAddedNode = AddNode(RADTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX, 24), new Vector2(16, 16)), LastAddedNode);
            LastAddedNode = AddNode(RADTree, new CommanderTechNode(MaxCPTitle, MaxCPDescription, new Vector2(NodeX += 40, 24), new Vector2(16, 16)), LastAddedNode);
        }

        private CommanderTechNode AddNode(CommanderTechTree Owner, CommanderTechNode NewNode, CommanderTechNode Parent)
        {
            Owner.ListAllNode.Add(NewNode);

            if (Parent != null)
            {
                Parent.ListChildren.Add(NewNode);
            }

            return NewNode;
        }

        public override void Update(GameTime gameTime)
        {
            MapScrollbar.Update(gameTime);
        }

        private void OnGametypeScrollbarChange(float ScrollbarValue)
        {
            MapScrollbarValue = (int)ScrollbarValue;
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            DrawTechTrees(g);
            DrawBox(g, new Vector2(0, 0), Constants.Width, 40, Color.White);
            g.DrawString(fntFinlanderFont, "Commander System", new Vector2(5, 5), Color.White);
            MapScrollbar.Draw(g);

            DrawBox(g, new Vector2(0, Constants.Height - 80), Constants.Width, 80, Color.White);
            DrawBox(g, new Vector2(15, Constants.Height - 55), 32, 32, Color.White);
            g.DrawString(fntArial12, "Max HP + 500", new Vector2(60, Constants.Height - 75), Color.White);
            g.DrawString(fntArial10, "[Passive] Increase max HP of every allied units by 500", new Vector2(60, Constants.Height - 55), Color.White);
        }

        private void DrawTechTrees(CustomSpriteBatch g)
        {
            for (int T = 0; T < ListTechTree.Count; ++T)
            {
                TextHelper.DrawTextMultiline(g, TextHelper.FitToWidth(fntArial12, ListTechTree[T].Name, 100),
                    TextHelper.TextAligns.Center, 200 + T * 160, 40 - MapScrollbarValue * 50, 100);

                DrawNodes(g, ListTechTree[T].ListAllNode, T * 160, MapScrollbarValue);
            }
        }

        private void DrawNodes(CustomSpriteBatch g, List<CommanderTechNode> ListAllNode, float OffsetX, float OffsetY)
        {
            float NodeOffsetX = 200 + OffsetX;
            float NodeOffsetY = 110 - OffsetY * 50;

            foreach (CommanderTechNode ActiveNode in ListAllNode)
            {
                foreach (CommanderTechNode ActiveChildrenNode in ActiveNode.ListChildren)
                {
                    g.DrawLine(GameScreen.sprPixel, new Vector2(ActiveNode.Position.X + NodeOffsetX, ActiveNode.Position.Y + NodeOffsetY),
                        new Vector2(ActiveChildrenNode.Position.X + NodeOffsetX, ActiveChildrenNode.Position.Y + NodeOffsetY), Color.White);
                }
                if (ActiveNode.Position.Y > 0 && ActiveNode.Size.X > 16)
                {
                    DrawBox(g, new Vector2(ActiveNode.Position.X + NodeOffsetX - ActiveNode.Size.X / 2 - 5, ActiveNode.Position.Y + NodeOffsetY - ActiveNode.Size.Y / 2 - 5),
                        (int)ActiveNode.Size.X + 10, (int)ActiveNode.Size.Y + 10, Color.White);

                    TextHelper.DrawTextMultiline(g, fntArial8, TextHelper.FitToWidth(fntArial8, ActiveNode.Name, 100),
                        TextHelper.TextAligns.Center, ActiveNode.Position.X + NodeOffsetX, ActiveNode.Position.Y + ActiveNode.Size.Y + NodeOffsetY - 10, 100);
                }
                DrawBox(g, new Vector2(ActiveNode.Position.X + NodeOffsetX - ActiveNode.Size.X / 2, ActiveNode.Position.Y + NodeOffsetY - ActiveNode.Size.Y / 2),
                    (int)ActiveNode.Size.X, (int)ActiveNode.Size.Y, Color.Black);
            }
        }
    }
}
