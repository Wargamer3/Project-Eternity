using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum CatalogPage
    {
        Comfort,
        Electronics,
        Surfaces,
        Plumbing,
        Decorative,
        Hobby,
        Miscellaneous,
    }

    public enum PlaceRule
    {
        Floor,
        Surface,
        Wall,
        Ceiling,
    }

    public enum ApproachSide
    {
        Front,
        Any,
    }

    public class CatalogItem
    {
        public string Name;
        public CatalogPage Page;
        public string Description;
        public int Price;
        public int Comfort;
        public int Food;
        public int Fun;
        public int Hygiene;
        public int Room;
        public Vector3 Swatch;
        public string Model;
        public float ModelMeters;
        public float Span;
        public float Depth;
        public float Rise;
        public Vector3 ModelShift;
        public PlaceRule Rule;
        public float Mount;
        public ApproachSide Approach;
        public int ReachTiles;
        public int SpreadTiles;
        public bool Seat;
    }

    public static class BuildCatalog
    {
        // The FBX packs are authored several times larger than a person. Half of the measured size sits next to the older furniture.
        public const float FbxFit = 0.5f;

        public static float FitOf(CatalogItem Card)
        {
            if (Card == null || string.IsNullOrEmpty(Card.Model) || !Card.Model.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
            {
                return 1f;
            }

            return FbxFit;
        }

        public static readonly CatalogPage[] Pages = new CatalogPage[]
        {
            CatalogPage.Comfort,
            CatalogPage.Electronics,
            CatalogPage.Surfaces,
            CatalogPage.Plumbing,
            CatalogPage.Decorative,
            CatalogPage.Hobby,
            CatalogPage.Miscellaneous,
        };

        static readonly CatalogItem[] Items = new CatalogItem[]
        {
            Item(CatalogPage.Comfort, "Bed", 400, "A plain bed. It is enough to sleep.", 8, 0, 0, 0, 1, new Vector3(0.45f, 0.32f, 0.7f)),
            Item(CatalogPage.Comfort, "Chair", 80, "A seat for one. Fine for a meal.", 3, 0, 0, 0, 1, new Vector3(0.55f, 0.28f, 0.22f)),
            Item(CatalogPage.Comfort, "Sofa", 250, "Room for two. Better than standing.", 6, 0, 1, 0, 2, new Vector3(0.35f, 0.45f, 0.7f)),
            Item(CatalogPage.Comfort, "Diner Seat", 100, "A booth seat. Comfortable enough for supper.", 3, 1, 0, 0, 1, new Vector3(0.7f, 0.15f, 0.18f)),
            Item(CatalogPage.Electronics, "Fridge", 500, "Keeps food cold. The household eats from here.", 0, 6, 0, 0, 1, new Vector3(0.75f, 0.8f, 0.85f)),
            Item(CatalogPage.Electronics, "Television", 300, "A small screen. It fills an evening.", 1, 0, 5, 0, 1, new Vector3(0.15f, 0.15f, 0.18f)),
            Item(CatalogPage.Electronics, "Computer", 800, "A desk machine. It sits on a table or counter.", 0, 0, 4, 0, 1, new Vector3(0.7f, 0.7f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Electronics/Computer.glb", 0.601f, 0.4f, 0.4f, 0.43f, new Vector3(-0.046f, 0f, -0.017f), PlaceRule.Surface),
            Item(CatalogPage.Electronics, "Wall Television", 350, "A screen for the wall. Tall furniture in front of it gets in the way.", 1, 0, 5, 0, 1, new Vector3(0.12f, 0.12f, 0.14f), "Content/Life Sim/Assets/Models/Build Mode/Electronics/Wall Television.glb", 0.908f, 1.32f, 0.08f, 0.65f, new Vector3(0f, -0.44f, -0.022f), PlaceRule.Wall),
            Item(CatalogPage.Electronics, "Stereo", 200, "Music for the room.", 0, 0, 3, 0, 2, new Vector3(0.2f, 0.2f, 0.22f)),
            Item(CatalogPage.Surfaces, "Gearhead Table", 220, "A work table. Machines sit on it.", 1, 0, 1, 0, 1, new Vector3(0.35f, 0.38f, 0.42f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Gearhead Table.glb", 0.5f, 0.53f, 0.52f, 0.36f),
            Item(CatalogPage.Surfaces, "Table", 120, "A surface for plates and elbows.", 1, 1, 0, 0, 1, new Vector3(0.55f, 0.4f, 0.25f)),
            Item(CatalogPage.Surfaces, "Counter", 180, "Kitchen counter. Food prep starts here.", 0, 2, 0, 0, 1, new Vector3(0.85f, 0.85f, 0.8f)),
            Item(CatalogPage.Surfaces, "Desk", 150, "A writing surface.", 1, 0, 1, 0, 1, new Vector3(0.4f, 0.28f, 0.18f)),
            Item(CatalogPage.Surfaces, "End Table", 60, "A small table beside a seat.", 0, 0, 0, 0, 1, new Vector3(0.5f, 0.38f, 0.24f)),
            Item(CatalogPage.Plumbing, "Cheap Toilet", 80, "A cheap toilet. It still flushes.", 0, 0, 0, 4, 0, new Vector3(0.92f, 0.9f, 0.84f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Cheap Toilet.glb", 0.862f, 0.4f, 0.51f, 0.62f),
            Item(CatalogPage.Plumbing, "Toilet", 300, "The necessary fixture.", 1, 0, 0, 6, 0, new Vector3(0.9f, 0.9f, 0.92f)),
            Item(CatalogPage.Plumbing, "Sink", 200, "Hands and dishes.", 0, 0, 0, 4, 1, new Vector3(0.75f, 0.82f, 0.85f)),
            Item(CatalogPage.Plumbing, "Shower", 450, "A quick wash.", 1, 0, 0, 8, 0, new Vector3(0.6f, 0.75f, 0.85f)),
            Item(CatalogPage.Plumbing, "Bathtub", 600, "A long soak.", 4, 0, 1, 7, 1, new Vector3(0.8f, 0.88f, 0.92f)),
            Item(CatalogPage.Decorative, "College Fountain", 900, "A college fountain. It fills the room.", 0, 0, 2, 0, 6, new Vector3(0.55f, 0.7f, 0.85f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/College Foutain.glb", 0.719f, 1.32f, 1.32f, 0.52f, new Vector3(0.5f, 0.008f, -0.5f)),
            Item(CatalogPage.Decorative, "Plant", 40, "A pot of leaves.", 0, 0, 0, 0, 3, new Vector3(0.25f, 0.55f, 0.28f)),
            Item(CatalogPage.Decorative, "Tree", 80, "A tree for the yard.", 0, 0, 0, 0, 4, new Vector3(0.2f, 0.45f, 0.22f)),
            Item(CatalogPage.Decorative, "Painting", 150, "Something on the wall to look at.", 0, 0, 1, 0, 5, new Vector3(0.7f, 0.55f, 0.3f)),
            Item(CatalogPage.Decorative, "Rug", 90, "Covers a bit of floor.", 1, 0, 0, 0, 2, new Vector3(0.55f, 0.2f, 0.25f)),
            Item(CatalogPage.Decorative, "Lamp", 70, "A light for the corner.", 0, 0, 0, 0, 2, new Vector3(0.9f, 0.85f, 0.55f)),
            Item(CatalogPage.Hobby, "Easel", 220, "For painting, or meaning to.", 0, 0, 5, 0, 2, new Vector3(0.75f, 0.65f, 0.45f)),
            Item(CatalogPage.Hobby, "Bookshelf", 180, "Books, and a place to put them.", 0, 0, 4, 0, 3, new Vector3(0.4f, 0.26f, 0.16f)),
            Item(CatalogPage.Hobby, "Chess Table", 160, "A game that takes a while.", 1, 0, 4, 0, 1, new Vector3(0.3f, 0.32f, 0.28f)),
            Item(CatalogPage.Hobby, "Guitar", 250, "Loud, if someone plays it.", 0, 0, 6, 0, 1, new Vector3(0.6f, 0.35f, 0.18f)),
            Item(CatalogPage.Miscellaneous, "Trash Can", 30, "Where things go when they are done.", 0, 0, 0, 1, 0, new Vector3(0.35f, 0.38f, 0.4f)),
            Item(CatalogPage.Miscellaneous, "Mirror", 100, "A look at yourself.", 0, 0, 0, 2, 2, new Vector3(0.75f, 0.82f, 0.88f)),
            Item(CatalogPage.Miscellaneous, "Clock", 50, "It tells the time. That is all.", 0, 0, 0, 0, 1, new Vector3(0.85f, 0.8f, 0.7f)),
            Item(CatalogPage.Miscellaneous, "Laundry Basket", 40, "Clothes wait here.", 0, 0, 0, 1, 0, new Vector3(0.55f, 0.6f, 0.7f)),
            Item(CatalogPage.Miscellaneous, "Combat Dummy", 80, "A practice target. A character can strike it.", 0, 0, 0, 0, 0, new Vector3(0.72f, 0.2f, 0.18f), null, 0f, 0.5f, 0.5f, 0.9f),
            Item(CatalogPage.Miscellaneous, "Dungeon Gate", 0, "The stair down into the dungeon.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.42f, 0.28f), "Content/Life Sim/Assets/Models/Dungeon/stairs_walled.gltf", 4f, 1.25f, 1f, 1f, new Vector3(0f, 0f, -2f)),
            Item(CatalogPage.Miscellaneous, "Tent", 120, "A camp bed. Sleep here and the rest still counts.", 4, 0, 0, 0, 0, new Vector3(0.45f, 0.42f, 0.28f)),
            Item(CatalogPage.Hobby, "Garden Plot", 60, "A patch of soil for herbs and vegetables.", 0, 2, 0, 0, 1, new Vector3(0.35f, 0.45f, 0.22f)),
            Item(CatalogPage.Hobby, "Fishing Spot", 80, "A place to sit and fish.", 0, 2, 1, 0, 0, new Vector3(0.3f, 0.45f, 0.62f)),
            Item(CatalogPage.Hobby, "Forge", 400, "A blacksmith's fire.", 0, 0, 0, 0, 1, new Vector3(0.4f, 0.28f, 0.22f)),
            Item(CatalogPage.Hobby, "Alchemy Bench", 350, "Bottles, burners, and a working surface.", 0, 0, 0, 0, 1, new Vector3(0.45f, 0.55f, 0.35f)),
            Item(CatalogPage.Electronics, "Kitchen Fridge", 500, "A tall fridge.", 0, 6, 0, 0, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Appliances/Kitchen_Fridge.fbx", 3.308f, 0.927f, 0.930f, 2.364f, new Vector3(-0.000f, 0.067f, -0.007f), PlaceRule.Floor),
            Item(CatalogPage.Electronics, "Oven", 350, "A kitchen oven.", 0, 4, 0, 0, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Appliances/Kitchen_Oven.fbx", 1.617f, 0.722f, 0.801f, 1.156f, new Vector3(-0.000f, 0.055f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Electronics, "Large Oven", 480, "A wider oven.", 0, 5, 0, 0, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Appliances/Kitchen_Oven_Large.fbx", 1.593f, 0.837f, 0.765f, 1.138f, new Vector3(-0.000f, 0.022f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Bunk Bed", 400, "Two bunks, one footprint.", 7, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Bed_Bunk.fbx", 3.031f, 1.550f, 2.857f, 2.166f, new Vector3(-0.186f, 0.000f, 0.028f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "King Bed", 520, "A wide bed. Two can sleep.", 8, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Bed_King.fbx", 1.215f, 2.190f, 2.825f, 0.868f, new Vector3(-0.000f, -0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Single Bed", 320, "A bed for one.", 6, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Bed_Single.fbx", 1.355f, 1.414f, 2.816f, 0.968f, new Vector3(-0.023f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Chair 1", 80, "A seat for one.", 3, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Chair_1.fbx", 1.768f, 0.544f, 0.590f, 1.263f, new Vector3(-0.000f, -0.032f, 0.003f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Chair 2", 80, "A seat for one.", 3, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Chair_2.fbx", 1.689f, 0.544f, 0.590f, 1.207f, new Vector3(-0.000f, -0.032f, 0.003f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Chair 3", 80, "A seat for one.", 3, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Chair_3.fbx", 1.694f, 0.544f, 0.599f, 1.211f, new Vector3(-0.000f, -0.038f, 0.007f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Chair 4", 80, "A seat for one.", 3, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Chair_4.fbx", 1.768f, 0.544f, 0.590f, 1.263f, new Vector3(-0.000f, -0.032f, 0.003f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Corner Couch", 420, "An L-shaped couch.", 7, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_L.fbx", 2.261f, 4.012f, 3.092f, 1.616f, new Vector3(0.492f, 0.783f, 0.015f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Large Couch 1", 360, "A long couch.", 6, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Large1.fbx", 2.106f, 3.913f, 1.424f, 1.505f, new Vector3(-0.000f, 0.321f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Large Couch 2", 360, "A long couch.", 6, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Large2.fbx", 2.108f, 3.710f, 1.424f, 1.506f, new Vector3(0.000f, 0.229f, 0.015f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Large Couch 3", 380, "A long low couch.", 6, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Large3.fbx", 1.088f, 5.063f, 2.159f, 0.778f, new Vector3(-0.020f, 0.179f, 0.015f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Medium Couch 1", 280, "A couch for two or three.", 5, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Medium1.fbx", 1.910f, 3.350f, 1.575f, 1.365f, new Vector3(0.000f, -0.082f, 0.013f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Medium Couch 2", 280, "A couch for two or three.", 5, 0, 1, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Medium2.fbx", 1.959f, 3.229f, 1.429f, 1.400f, new Vector3(-0.000f, 0.054f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Small Couch 1", 200, "A short couch.", 4, 0, 0, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Small1.fbx", 1.910f, 2.062f, 1.575f, 1.365f, new Vector3(0.000f, -0.082f, 0.013f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Small Couch 2", 200, "A short couch.", 4, 0, 0, 0, 1, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Couch_Small2.fbx", 1.269f, 1.510f, 1.424f, 0.907f, new Vector3(-0.000f, 0.321f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Comfort, "Stool", 40, "A backless seat.", 2, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Comfort/Stool.fbx", 1.372f, 0.483f, 0.507f, 0.981f, new Vector3(-0.000f, -0.000f, 0.009f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Carpet 1", 90, "A rug. You can walk on it.", 1, 0, 0, 0, 2, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Carpet_1.fbx", 0.037f, 1.523f, 2.210f, 0.026f, new Vector3(-0.007f, -0.009f, 0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Carpet 2", 110, "A rug. You can walk on it.", 1, 0, 0, 0, 2, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Carpet_2.fbx", 0.037f, 2.210f, 2.274f, 0.026f, new Vector3(-0.091f, 0.033f, 0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Round Carpet", 80, "A round rug. You can walk on it.", 1, 0, 0, 0, 2, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Carpet_Round.fbx", 0.016f, 1.781f, 1.809f, 0.011f, new Vector3(0.000f, -0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Short Round Column", 80, "A short round column.", 0, 0, 0, 0, 2, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Column_Round1.fbx", 1.773f, 1.001f, 1.052f, 1.267f, new Vector3(0.000f, 0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Round Column", 120, "A round column.", 0, 0, 0, 0, 2, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Column_Round2.fbx", 4.001f, 1.001f, 1.052f, 2.859f, new Vector3(-0.000f, 0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Tall Round Column", 160, "A tall round column.", 0, 0, 0, 0, 2, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Column_Round3.fbx", 5.429f, 1.001f, 1.052f, 3.880f, new Vector3(0.000f, 0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Square Column", 120, "A square column.", 0, 0, 0, 0, 2, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Column_SquareBig.fbx", 4.159f, 0.648f, 0.648f, 2.972f, new Vector3(-0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Short Square Column", 80, "A short square column.", 0, 0, 0, 0, 2, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Column_SquareSmall.fbx", 2.447f, 0.648f, 0.648f, 1.749f, new Vector3(-0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Double Curtains", 120, "Curtains for a wide window.", 0, 0, 0, 0, 2, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Curtains_Double.fbx", 4.344f, 2.479f, 0.341f, 3.105f, new Vector3(-0.040f, -0.124f, 0.006f), PlaceRule.Wall),
            Item(CatalogPage.Decorative, "Curtains", 70, "Curtains for one window.", 0, 0, 0, 0, 2, new Vector3(0.45f, 0.32f, 0.55f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Curtains_Single.fbx", 4.344f, 1.166f, 0.341f, 3.105f, new Vector3(-0.040f, -0.183f, 0.000f), PlaceRule.Wall),
            Item(CatalogPage.Decorative, "Houseplant 1", 40, "A pot of leaves.", 0, 0, 0, 0, 3, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_1.fbx", 0.717f, 0.550f, 0.496f, 0.513f, new Vector3(-0.093f, 0.024f, -0.002f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 2", 25, "A small pot of leaves.", 0, 0, 0, 0, 2, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_2.fbx", 0.385f, 0.179f, 0.179f, 0.275f, new Vector3(-0.000f, -0.000f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 3", 35, "A pot of leaves.", 0, 0, 0, 0, 2, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_3.fbx", 0.845f, 0.245f, 0.242f, 0.604f, new Vector3(-0.002f, -0.000f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 4", 35, "A pot of leaves.", 0, 0, 0, 0, 2, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_4.fbx", 0.928f, 0.242f, 0.242f, 0.663f, new Vector3(-0.000f, -0.000f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 5", 30, "A low pot of leaves.", 0, 0, 0, 0, 2, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_5.fbx", 0.424f, 0.366f, 0.378f, 0.303f, new Vector3(-0.020f, -0.025f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 6", 55, "A wide pot of leaves.", 0, 0, 0, 0, 3, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_6.fbx", 0.959f, 0.743f, 0.750f, 0.685f, new Vector3(-0.045f, -0.046f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 7", 55, "A wide pot of leaves.", 0, 0, 0, 0, 3, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_7.fbx", 0.882f, 0.833f, 0.872f, 0.630f, new Vector3(-0.045f, 0.024f, -0.002f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Houseplant 8", 50, "A tall pot of leaves.", 0, 0, 0, 0, 3, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Houseplant_8.fbx", 1.557f, 0.370f, 0.370f, 1.112f, new Vector3(0.026f, -0.005f, -0.001f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Tall Shelf", 90, "A narrow shelf.", 0, 0, 0, 0, 2, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_1.fbx", 3.167f, 0.505f, 0.466f, 2.263f, new Vector3(0.000f, 0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Shelf", 120, "A tall shelf.", 0, 0, 0, 0, 3, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_2.fbx", 4.438f, 0.800f, 0.540f, 3.172f, new Vector3(0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Large Shelf", 180, "A wide tall shelf.", 0, 0, 0, 0, 3, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_Large.fbx", 4.438f, 2.416f, 0.540f, 3.172f, new Vector3(0.531f, -0.000f, 0.024f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Small Shelf 1", 40, "A short shelf for the wall.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_Small1.fbx", 0.577f, 1.389f, 0.393f, 0.413f, new Vector3(0.000f, 0.000f, 0.021f), PlaceRule.Wall),
            Item(CatalogPage.Decorative, "Small Shelf 2", 30, "A short shelf for the wall.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_Small2.fbx", 0.390f, 0.743f, 0.393f, 0.279f, new Vector3(0.000f, 0.000f, 0.021f), PlaceRule.Wall),
            Item(CatalogPage.Decorative, "Small Shelf 3", 30, "A short shelf for the wall.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Decorative/Shelf_Small3.fbx", 0.390f, 0.743f, 0.393f, 0.279f, new Vector3(0.000f, 0.000f, 0.021f), PlaceRule.Wall),
            Item(CatalogPage.Hobby, "Bookcase", 180, "A case of books.", 0, 0, 4, 0, 3, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Hobbies/Bookshelf.fbx", 4.438f, 1.657f, 0.540f, 3.172f, new Vector3(-0.000f, -0.000f, 0.024f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Ceiling Light 1", 70, "A light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling1.fbx", 0.822f, 0.315f, 0.312f, 0.587f, new Vector3(-0.000f, -0.005f, 0.823f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light 2", 70, "A light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling2.fbx", 1.012f, 0.258f, 0.255f, 0.723f, new Vector3(-0.000f, -0.004f, 1.013f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light 3", 60, "A light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling3.fbx", 0.678f, 0.384f, 0.384f, 0.484f, new Vector3(-0.000f, -0.000f, 0.678f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light 4", 90, "A light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling4.fbx", 1.297f, 0.554f, 0.554f, 0.927f, new Vector3(-0.000f, -0.000f, 1.297f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light 5", 50, "A flat light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling5.fbx", 0.136f, 0.380f, 0.399f, 0.097f, new Vector3(-0.000f, -0.000f, 0.134f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light 6", 50, "A flat light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Ceiling6.fbx", 0.129f, 0.380f, 0.399f, 0.092f, new Vector3(-0.000f, -0.000f, 0.126f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Ceiling Light", 40, "A small ceiling light.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_CeilingSingle.fbx", 0.478f, 0.189f, 0.199f, 0.342f, new Vector3(-0.000f, -0.000f, 0.476f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Chandelier", 220, "A hanging chandelier.", 0, 0, 0, 0, 3, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Chandelier.fbx", 0.519f, 0.581f, 0.581f, 0.371f, new Vector3(-0.000f, -0.000f, 0.519f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Cube Light", 80, "A cube light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Cube.fbx", 1.035f, 0.403f, 0.403f, 0.740f, new Vector3(-0.000f, -0.000f, 1.034f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Cube Light 2", 80, "A cube light for the ceiling.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Cube2.fbx", 0.922f, 0.396f, 0.458f, 0.659f, new Vector3(-0.000f, 0.000f, 0.921f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Desk Light", 40, "A lamp for a table.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Desk.fbx", 0.573f, 0.180f, 0.324f, 0.410f, new Vector3(-0.000f, 0.101f, -0.000f), PlaceRule.Surface),
            Item(CatalogPage.Decorative, "Floor Light 1", 60, "A lamp for the floor.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Floor1.fbx", 1.603f, 0.201f, 0.211f, 1.146f, new Vector3(-0.000f, 0.000f, -0.014f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Floor Light 2", 50, "A lamp for the floor.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Floor2.fbx", 0.757f, 0.224f, 0.235f, 0.541f, new Vector3(-0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Floor Light 3", 55, "A lamp for the floor.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Floor3.fbx", 0.894f, 0.286f, 0.301f, 0.639f, new Vector3(-0.000f, -0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Floor Light 4", 50, "A lamp for the floor.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Floor4.fbx", 0.763f, 0.251f, 0.251f, 0.545f, new Vector3(-0.000f, -0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Facet Light", 90, "A faceted ceiling light.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Icosahedron.fbx", 0.880f, 0.359f, 0.343f, 0.629f, new Vector3(-0.000f, -0.000f, 0.878f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Facet Light 2", 110, "A longer faceted ceiling light.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Icosahedron2.fbx", 0.880f, 1.162f, 0.359f, 0.629f, new Vector3(0.008f, 0.000f, 0.880f), PlaceRule.Ceiling),
            Item(CatalogPage.Decorative, "Small Light", 25, "A tiny lamp for a table.", 0, 0, 0, 0, 1, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Small.fbx", 0.106f, 0.111f, 0.117f, 0.076f, new Vector3(0.000f, -0.000f, 0.110f), PlaceRule.Surface),
            Item(CatalogPage.Decorative, "Stand Light 1", 90, "A tall floor lamp.", 0, 0, 0, 0, 2, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Stand1.fbx", 2.994f, 0.553f, 0.581f, 2.139f, new Vector3(-0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Decorative, "Stand Light 2", 90, "A tall floor lamp.", 0, 0, 0, 0, 2, new Vector3(0.9f, 0.82f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Lighting/Light_Stand2.fbx", 2.945f, 0.484f, 0.484f, 2.105f, new Vector3(-0.000f, -0.000f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Fireplace", 400, "A fireplace.", 2, 0, 0, 0, 3, new Vector3(0.5f, 0.48f, 0.45f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Fireplace.fbx", 2.561f, 2.322f, 0.827f, 1.830f, new Vector3(-0.000f, 0.086f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Cylinder Bin", 30, "A round bin.", 0, 0, 0, 1, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Trashcan_Cylindric.fbx", 1.074f, 0.539f, 0.539f, 0.768f, new Vector3(-0.000f, -0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Green Bin", 40, "A large green bin.", 0, 0, 0, 1, 1, new Vector3(0.28f, 0.5f, 0.3f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Trashcan_Green.fbx", 2.211f, 0.870f, 1.137f, 1.580f, new Vector3(-0.002f, -0.075f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Large Bin", 35, "A large bin.", 0, 0, 0, 1, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Trashcan_Large.fbx", 1.846f, 0.874f, 0.874f, 1.319f, new Vector3(-0.000f, 0.000f, 0.005f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Small Bin 1", 20, "A small bin.", 0, 0, 0, 1, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Trashcan_Small1.fbx", 0.712f, 0.359f, 0.219f, 0.509f, new Vector3(-0.000f, 0.011f, 0.007f), PlaceRule.Floor),
            Item(CatalogPage.Miscellaneous, "Small Bin 2", 20, "A small bin.", 0, 0, 0, 1, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Miscellaneous/Trashcan_Small2.fbx", 0.560f, 0.332f, 0.306f, 0.400f, new Vector3(-0.000f, -0.000f, 0.070f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Bathroom Tub", 600, "A tub for a long soak.", 4, 0, 1, 7, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Bathtub.fbx", 1.260f, 1.149f, 2.787f, 0.901f, new Vector3(-0.000f, -0.174f, 0.008f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Bathroom Mirror 1", 80, "A mirror for the wall.", 0, 0, 0, 1, 2, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Mirror1.fbx", 0.930f, 0.463f, 0.073f, 0.664f, new Vector3(-0.000f, 0.011f, 0.465f), PlaceRule.Wall),
            Item(CatalogPage.Plumbing, "Bathroom Mirror 2", 60, "A smaller wall mirror.", 0, 0, 0, 1, 2, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Mirror2.fbx", 0.774f, 0.384f, 0.027f, 0.553f, new Vector3(-0.000f, -0.011f, 0.387f), PlaceRule.Wall),
            Item(CatalogPage.Plumbing, "Shower Stall", 450, "A closed shower.", 0, 0, 0, 8, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Shower1.fbx", 3.548f, 1.355f, 1.353f, 2.536f, new Vector3(-0.124f, -0.078f, 0.095f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Bathroom Sink", 180, "A sink on a stand.", 0, 0, 0, 4, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Sink.fbx", 1.447f, 0.993f, 0.583f, 1.034f, new Vector3(-0.000f, 0.104f, -0.151f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Bathroom Toilet", 280, "A toilet.", 0, 0, 0, 6, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Toilet.fbx", 1.901f, 0.855f, 1.209f, 1.358f, new Vector3(-0.000f, -0.054f, 0.013f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Bathroom Toilet 2", 220, "A smaller toilet.", 0, 0, 0, 5, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Toilet2.fbx", 1.008f, 0.794f, 1.100f, 0.720f, new Vector3(-0.000f, 0.022f, 0.013f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Toilet Paper", 10, "A roll on the wall.", 0, 0, 0, 1, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_ToiletPaper.fbx", 0.344f, 0.214f, 0.125f, 0.246f, new Vector3(0.016f, -0.000f, 0.098f), PlaceRule.Wall),
            Item(CatalogPage.Plumbing, "Toilet Paper Pile", 15, "Spare rolls.", 0, 0, 0, 1, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_ToiletPaperPile.fbx", 0.375f, 0.239f, 0.250f, 0.268f, new Vector3(-0.000f, 0.000f, -0.002f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Towel", 20, "A towel on the wall.", 0, 0, 0, 1, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_Towel.fbx", 0.654f, 0.847f, 0.115f, 0.467f, new Vector3(-0.031f, -0.046f, 0.621f), PlaceRule.Wall),
            Item(CatalogPage.Plumbing, "Washing Machine", 320, "Washes clothes.", 0, 0, 0, 3, 1, new Vector3(0.7f, 0.72f, 0.75f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Bathroom_WashingMachine.fbx", 1.470f, 0.923f, 0.946f, 1.051f, new Vector3(-0.000f, 0.044f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Plumbing, "Kitchen Sink", 200, "A sink for dishes.", 0, 1, 0, 4, 1, new Vector3(0.9f, 0.9f, 0.92f), "Content/Life Sim/Assets/Models/Build Mode/Plumbing/Kitchen_Sink.fbx", 1.924f, 0.722f, 0.801f, 1.375f, new Vector3(-0.000f, 0.055f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Dresser 1", 160, "A low dresser.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Drawer_1.fbx", 1.353f, 1.997f, 0.799f, 0.967f, new Vector3(0.000f, 0.001f, 0.009f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Dresser 2", 200, "A long dresser.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Drawer_2.fbx", 1.305f, 2.884f, 0.799f, 0.933f, new Vector3(0.000f, 0.001f, 0.009f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Dresser 3", 220, "A long dresser.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Drawer_3.fbx", 1.577f, 2.968f, 0.955f, 1.127f, new Vector3(-0.000f, 0.000f, 0.000f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Dresser 4", 180, "A taller dresser.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Drawer_4.fbx", 2.070f, 1.997f, 0.799f, 1.479f, new Vector3(-0.000f, -0.004f, 0.007f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Dresser 5", 160, "A low dresser.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Drawer_5.fbx", 1.332f, 1.997f, 0.824f, 0.952f, new Vector3(-0.000f, 0.021f, -0.011f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Kitchen Drawers 1", 140, "One row of kitchen drawers.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_1Drawers.fbx", 1.614f, 0.722f, 0.801f, 1.153f, new Vector3(0.000f, 0.055f, -0.007f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Kitchen Drawers 2", 150, "Two rows of kitchen drawers.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_2Drawers.fbx", 1.625f, 0.722f, 0.801f, 1.161f, new Vector3(-0.000f, 0.055f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Kitchen Drawers 3", 160, "Three rows of kitchen drawers.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_3Drawers.fbx", 1.625f, 0.722f, 0.801f, 1.161f, new Vector3(-0.000f, 0.055f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Kitchen Cabinet 1", 140, "A kitchen cabinet.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_Cabinet1.glb", 1.567f, 0.722f, 0.766f, 1.120f, new Vector3(-0.000f, 0.030f, -0.007f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Kitchen Cabinet 2", 140, "A kitchen cabinet.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_Cabinet2.fbx", 1.567f, 0.722f, 0.766f, 1.120f, new Vector3(-0.000f, 0.030f, -0.007f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Small Kitchen Cabinet", 90, "A short kitchen cabinet.", 0, 1, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Kitchen_CabinetSmall.fbx", 0.947f, 0.615f, 0.766f, 0.677f, new Vector3(-0.000f, 0.030f, -0.000f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Nightstand 1", 60, "A table beside a bed.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/NightStand_1.fbx", 0.825f, 0.680f, 0.680f, 0.590f, new Vector3(-0.000f, -0.000f, -0.004f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Nightstand 2", 70, "A table beside a bed.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/NightStand_2.fbx", 0.945f, 0.680f, 0.680f, 0.676f, new Vector3(-0.000f, -0.000f, -0.004f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Nightstand 3", 60, "A table beside a bed.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/NightStand_3.fbx", 0.827f, 0.680f, 0.680f, 0.591f, new Vector3(-0.000f, -0.000f, -0.002f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Large Round Table", 220, "A round table for several people.", 1, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Table_RoundLarge.fbx", 1.121f, 2.654f, 1.387f, 0.801f, new Vector3(0.000f, 0.000f, -0.005f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Small Round Table", 90, "A small round table.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Table_RoundSmall.fbx", 1.147f, 1.387f, 1.387f, 0.820f, new Vector3(-0.000f, -0.000f, 0.003f), PlaceRule.Floor),
            Item(CatalogPage.Surfaces, "Small Round Table 2", 90, "A small round table.", 0, 0, 0, 0, 1, new Vector3(0.55f, 0.36f, 0.18f), "Content/Life Sim/Assets/Models/Build Mode/Surfaces/Table_RoundSmall2.fbx", 1.208f, 1.387f, 1.387f, 0.863f, new Vector3(-0.000f, -0.000f, 0.063f), PlaceRule.Floor),
        };

        static BuildCatalog()
        {
            Use("Bed", ApproachSide.Front, 1, 1, false);
            Use("Chair", ApproachSide.Front, 1, 0, true);
            Use("Sofa", ApproachSide.Front, 1, 1, true);
            Use("Diner Seat", ApproachSide.Front, 1, 0, true);
            Use("Fridge", ApproachSide.Front, 1, 0, false);
            Use("Television", ApproachSide.Front, 4, 2, false);
            Use("Computer", ApproachSide.Front, 1, 0, false);
            Use("Wall Television", ApproachSide.Front, 4, 2, false);
            Use("Stereo", ApproachSide.Any, 2, 0, false);
            Use("Table", ApproachSide.Any, 1, 0, false);
            Use("Gearhead Table", ApproachSide.Any, 1, 0, false);
            Use("Counter", ApproachSide.Any, 1, 0, false);
            Use("Desk", ApproachSide.Front, 1, 0, false);
            Use("End Table", ApproachSide.Any, 1, 0, false);
            Use("Cheap Toilet", ApproachSide.Front, 1, 0, false);
            Use("Toilet", ApproachSide.Front, 1, 0, false);
            Use("Sink", ApproachSide.Front, 1, 0, false);
            Use("Shower", ApproachSide.Front, 1, 0, false);
            Use("Bathtub", ApproachSide.Front, 1, 1, false);
            Use("College Fountain", ApproachSide.Any, 2, 0, false);
            Use("Painting", ApproachSide.Front, 2, 1, false);
            Use("Mirror", ApproachSide.Front, 1, 0, false);
            Use("Easel", ApproachSide.Front, 1, 0, false);
            Use("Bookshelf", ApproachSide.Front, 1, 1, false);
            Use("Chess Table", ApproachSide.Any, 1, 0, false);
            Use("Combat Dummy", ApproachSide.Any, 1, 0, false);
            Use("Dungeon Gate", ApproachSide.Front, 1, 0, false);
            Use("Tent", ApproachSide.Front, 1, 0, false);
            Use("Garden Plot", ApproachSide.Front, 1, 0, false);
            Use("Fishing Spot", ApproachSide.Front, 1, 0, false);
            Use("Forge", ApproachSide.Front, 1, 0, false);
            Use("Alchemy Bench", ApproachSide.Front, 1, 0, false);
            Use("King Bed", ApproachSide.Front, 1, 1, false);
            Use("Single Bed", ApproachSide.Front, 1, 0, false);
            Use("Bunk Bed", ApproachSide.Front, 1, 0, false);
            Use("Chair 1", ApproachSide.Front, 1, 0, true);
            Use("Chair 2", ApproachSide.Front, 1, 0, true);
            Use("Chair 3", ApproachSide.Front, 1, 0, true);
            Use("Chair 4", ApproachSide.Front, 1, 0, true);
            Use("Stool", ApproachSide.Front, 1, 0, true);
            Use("Corner Couch", ApproachSide.Front, 1, 1, true);
            Use("Large Couch 1", ApproachSide.Front, 1, 1, true);
            Use("Large Couch 2", ApproachSide.Front, 1, 1, true);
            Use("Large Couch 3", ApproachSide.Front, 1, 1, true);
            Use("Medium Couch 1", ApproachSide.Front, 1, 1, true);
            Use("Medium Couch 2", ApproachSide.Front, 1, 1, true);
            Use("Small Couch 1", ApproachSide.Front, 1, 0, true);
            Use("Small Couch 2", ApproachSide.Front, 1, 0, true);
            Use("Kitchen Fridge", ApproachSide.Front, 1, 0, false);
            Use("Oven", ApproachSide.Front, 1, 0, false);
            Use("Large Oven", ApproachSide.Front, 1, 0, false);
            Use("Bathroom Tub", ApproachSide.Front, 1, 1, false);
            Use("Shower Stall", ApproachSide.Front, 1, 0, false);
            Use("Bathroom Sink", ApproachSide.Front, 1, 0, false);
            Use("Bathroom Toilet", ApproachSide.Front, 1, 0, false);
            Use("Bathroom Toilet 2", ApproachSide.Front, 1, 0, false);
            Use("Kitchen Sink", ApproachSide.Front, 1, 0, false);
            Use("Washing Machine", ApproachSide.Front, 1, 0, false);
            Use("Bookcase", ApproachSide.Front, 1, 1, false);
            Use("Fireplace", ApproachSide.Front, 1, 0, false);
            Use("Large Round Table", ApproachSide.Any, 1, 0, false);
            Use("Small Round Table", ApproachSide.Any, 1, 0, false);
            Use("Small Round Table 2", ApproachSide.Any, 1, 0, false);
            MountAt("Curtains", 0f);
            MountAt("Double Curtains", 0f);
            MountAt("Small Shelf 1", 1.2f);
            MountAt("Small Shelf 2", 1.2f);
            MountAt("Small Shelf 3", 1.2f);
            MountAt("Bathroom Mirror 1", 1.35f);
            MountAt("Bathroom Mirror 2", 1.4f);
            MountAt("Towel", 1.15f);
            MountAt("Toilet Paper", 0.85f);
            CatalogItem Sofa = Find("Sofa");
            if (Sofa != null)
            {
                Sofa.Span = 2f;
            }
        }

        static void Use(string Name, ApproachSide Approach, int Reach, int Spread, bool Seat)
        {
            CatalogItem Card = Find(Name);
            if (Card == null)
            {
                return;
            }

            Card.Approach = Approach;
            Card.ReachTiles = Reach;
            Card.SpreadTiles = Spread;
            Card.Seat = Seat;
        }

        static void MountAt(string Name, float Mount)
        {
            CatalogItem Card = Find(Name);
            if (Card == null)
            {
                return;
            }

            Card.Mount = Mount;
        }

        public static string Title(CatalogPage Page)
        {
            if (Page == CatalogPage.Comfort)
            {
                return "Seating & Beds";
            }

            if (Page == CatalogPage.Electronics)
            {
                return "Electronics";
            }

            if (Page == CatalogPage.Surfaces)
            {
                return "Surfaces";
            }

            if (Page == CatalogPage.Plumbing)
            {
                return "Plumbing";
            }

            if (Page == CatalogPage.Decorative)
            {
                return "Decorative";
            }

            if (Page == CatalogPage.Hobby)
            {
                return "Hobby";
            }

            return "Miscellaneous";
        }

        public static List<CatalogItem> In(CatalogPage Page)
        {
            List<CatalogItem> Found = new List<CatalogItem>();
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Page == Page)
                {
                    Found.Add(Items[i]);
                }
            }

            return Found;
        }

        public static List<CatalogItem> In(CatalogPage Page, HouseholdState Home)
        {
            List<CatalogItem> Found = In(Page);
            if (Home == null)
            {
                return Found;
            }

            List<CatalogItem> Open = new List<CatalogItem>();
            for (int i = 0; i < Found.Count; i++)
            {
                if (Trades.Offered(Found[i].Name, Home))
                {
                    Open.Add(Found[i]);
                }
            }

            return Open;
        }

        public static CatalogItem Find(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return null;
            }

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Name == Name)
                {
                    return Items[i];
                }
            }

            return null;
        }

        public static void Draw(CustomSpriteBatch g, CatalogPage Page, int Index, bool OnGrab, int Funds, bool OuterFocused)
        {
            Texture2D Pixel = GameScreen.sprPixel;
            if (Pixel == null)
            {
                return;
            }

            int X = 70;
            int Y = 64;
            int W = Constants.Width - 70 * 2;
            int H = 430;
            g.Draw(Pixel, new Rectangle(X, Y, W, H), new Color(48, 78, 130, 230));
            g.Draw(Pixel, new Rectangle(X, Y, W, 36), new Color(30, 52, 96, 255));

            int TabX = X + 12;
            for (int i = 0; i < Pages.Length; i++)
            {
                bool Here = Pages[i] == Page && !OuterFocused;
                g.Draw(Pixel, new Rectangle(TabX, Y + 6, 100, 24), Here ? new Color(230, 190, 70, 255) : new Color(20, 36, 70, 255));
                TextHelper.DrawText(g, Short(Pages[i]), new Vector2(TabX + 6, Y + 8), Here ? Color.Gray : Color.White);
                TabX += 106;
            }

            TextHelper.DrawText(g, Title(Page), new Vector2(X + 16, Y + 48), Color.White);
            g.Draw(Pixel, new Rectangle(X + W - 150, Y + 44, 130, 26), OnGrab ? new Color(230, 190, 70, 255) : new Color(20, 90, 50, 255));
            TextHelper.DrawTextRightAligned(g, "[G] Grab Mode", new Vector2(X + W - 30, Y + 48), OnGrab ? Color.Gray : Color.White);

            List<CatalogItem> Stock = In(Page);
            int RowH = 32;
            int Visible = System.Math.Max(1, (H - 124) / RowH);
            int First = 0;
            if (Index >= Visible)
            {
                First = Index - Visible + 1;
            }

            if (First > Stock.Count - Visible)
            {
                First = System.Math.Max(0, Stock.Count - Visible);
            }

            int RowY = Y + 84;
            int Last = System.Math.Min(Stock.Count, First + Visible);
            for (int i = First; i < Last; i++)
            {
                bool Selected = i == Index && !OnGrab && !OuterFocused;
                g.Draw(Pixel, new Rectangle(X + 12, RowY, 180, 28), Selected ? new Color(220, 230, 245, 255) : new Color(20, 40, 80, 180));
                TextHelper.DrawText(g, Stock[i].Name, new Vector2(X + 20, RowY + 4), Selected ? Color.Gray : Color.White);
                RowY += RowH;
            }

            if (Stock.Count == 0 || Index < 0 || Index >= Stock.Count)
            {
                DrawFunds(g, Pixel, X, Y, W, H, Funds);
                return;
            }

            CatalogItem Card = Stock[Index];
            int CardX = X + 214;
            TextHelper.DrawText(g, Card.Name, new Vector2(CardX, Y + 84), Color.White);
            DrawPreview(g, Card, new Rectangle(CardX, Y + 112, 200, 150));
            TextHelper.DrawText(g, "$ " + Card.Price, new Vector2(CardX + 220, Y + 120), Color.Gold);
            int StatY = Y + 150;
            StatY = Stat(g, "Comfort", Card.Comfort, CardX + 220, StatY);
            StatY = Stat(g, "Food", Card.Food, CardX + 220, StatY);
            StatY = Stat(g, "Fun", Card.Fun, CardX + 220, StatY);
            StatY = Stat(g, "Hygiene", Card.Hygiene, CardX + 220, StatY);
            Stat(g, "Room", Card.Room, CardX + 220, StatY);
            Wrap(g, Card.Description, CardX, Y + 276);
            DrawFunds(g, Pixel, X, Y, W, H, Funds);
        }

        static void DrawFunds(CustomSpriteBatch g, Texture2D Pixel, int X, int Y, int W, int H, int Funds)
        {
            g.Draw(Pixel, new Rectangle(X, Y + H - 36, W, 36), new Color(20, 40, 70, 255));
            TextHelper.DrawText(g, "Funds", new Vector2(X + 16, Y + H - 28), Color.White);
            TextHelper.DrawText(g, "$ " + Funds, new Vector2(X + W - 140, Y + H - 28), Color.Gold);
        }

        static int Stat(CustomSpriteBatch g, string Name, int Value, int X, int Y)
        {
            if (Value <= 0)
            {
                return Y;
            }

            TextHelper.DrawText(g, Name + "  " + Value, new Vector2(X, Y), Color.White);
            return Y + 22;
        }

        static void Wrap(CustomSpriteBatch g, string Text, int X, int Y)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }

            string[] Words = Text.Split(' ');
            string Line = "";
            int Row = 0;
            for (int i = 0; i < Words.Length; i++)
            {
                string Next = Line.Length == 0 ? Words[i] : Line + " " + Words[i];
                if (Next.Length > 58 && Line.Length > 0)
                {
                    TextHelper.DrawText(g, Line, new Vector2(X, Y + Row * 20), Color.LightGray);
                    Row++;
                    Line = Words[i];
                }
                else
                {
                    Line = Next;
                }
            }

            if (Line.Length > 0)
            {
                TextHelper.DrawText(g, Line, new Vector2(X, Y + Row * 20), Color.LightGray);
            }
        }

        static void DrawPreview(CustomSpriteBatch g, CatalogItem Card, Rectangle Frame)
        {
            g.End();
            GraphicsDevice Device = g.GraphicsDevice;
            Viewport Previous = Device.Viewport;
            RasterizerState PreviousRaster = Device.RasterizerState;
            Device.Viewport = new Viewport(Frame.X, Frame.Y, Frame.Width, Frame.Height);
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
            Device.Clear(ClearOptions.DepthBuffer, Color.Transparent, 1f, 0);
            FurnitureItem Box = new FurnitureItem();
            float Fit = FitOf(Card);
            Box.Width = 24f * Fit;
            Box.Height = 18f * Fit;
            Box.Length = 24f * Fit;
            Box.Swatch = Card.Swatch;
            Box.Model = Card.Model;
            Box.ModelMeters = Card.ModelMeters;
            Box.ModelShift = Card.ModelShift;
            if (!string.IsNullOrEmpty(Card.Model) && Card.Rise > 0f)
            {
                Box.Height = 32f * Card.Rise * Fit;
                Box.Width = 32f * (Card.Span > 0f ? Card.Span : 0.75f) * Fit;
                Box.Length = 32f * (Card.Depth > 0f ? Card.Depth : 0.75f) * Fit;
            }

            Vector3 Target = new Vector3(0f, Box.Height * 0.5f, 0f);
            float Radius = 0.5f * new Vector3(Box.Width, Box.Height, Box.Length).Length();
            if (Radius < 4f)
            {
                Radius = 4f;
            }

            Vector3 Back = Vector3.Normalize(new Vector3(1f, 0.72f, 1f));
            float Distance = Radius / (float)System.Math.Sin(MathHelper.PiOver4 * 0.5f) * 1.35f;
            Vector3 Eye = Target + Back * Distance;
            Matrix View = Matrix.CreateLookAt(Eye, Target, Vector3.Up);
            float Aspect = Frame.Width / (float)System.Math.Max(1, Frame.Height);
            float Near = System.Math.Max(0.1f, Distance * 0.02f);
            float Far = Distance + Radius * 4f;
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Aspect, Near, Far);
            PlacedBoxDraw.Show(Device, Box, View, Projection);
            Device.Viewport = Previous;
            Device.RasterizerState = PreviousRaster;
            g.Begin();
        }

        static string Short(CatalogPage Page)
        {
            if (Page == CatalogPage.Comfort)
            {
                return "Seating";
            }

            if (Page == CatalogPage.Electronics)
            {
                return "Electric";
            }

            if (Page == CatalogPage.Surfaces)
            {
                return "Surfaces";
            }

            if (Page == CatalogPage.Plumbing)
            {
                return "Plumbing";
            }

            if (Page == CatalogPage.Decorative)
            {
                return "Decor";
            }

            if (Page == CatalogPage.Hobby)
            {
                return "Hobby";
            }

            return "Misc";
        }

        static CatalogItem Item(CatalogPage Page, string Name, int Price, string Description, int Comfort, int Food, int Fun, int Hygiene, int Room, Vector3 Swatch, string Model = null, float ModelMeters = 0f, float Span = 0f, float Depth = 0f, float Rise = 0f, Vector3 ModelShift = default, PlaceRule Rule = PlaceRule.Floor)
        {
            CatalogItem Card = new CatalogItem();
            Card.Page = Page;
            Card.Name = Name;
            Card.Price = Price;
            Card.Description = Description;
            Card.Comfort = Comfort;
            Card.Food = Food;
            Card.Fun = Fun;
            Card.Hygiene = Hygiene;
            Card.Room = Room;
            Card.Swatch = Swatch;
            Card.Model = Model;
            Card.ModelMeters = ModelMeters;
            Card.Span = Span;
            Card.Depth = Depth;
            Card.Rise = Rise;
            Card.ModelShift = ModelShift;
            Card.Rule = Rule;
            Card.Mount = -1f;
            return Card;
        }
    }
}
