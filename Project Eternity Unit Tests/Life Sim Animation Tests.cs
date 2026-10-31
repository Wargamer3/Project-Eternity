using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimAnimationTests
    {
        [TestMethod]
        public void FootSolverKeepsBothBonesAndReachesTheTarget()
        {
            System.Numerics.Vector3 Hip = new System.Numerics.Vector3(0f, 1f, 0f);
            System.Numerics.Vector3 Knee = new System.Numerics.Vector3(0f, 0.45f, 0.25f);
            System.Numerics.Vector3 Ankle = new System.Numerics.Vector3(0.05f, 0.02f, 0.02f);
            System.Numerics.Vector3 Target = new System.Numerics.Vector3(0.2f, 0.15f, 0.35f);
            Assert.IsTrue(FootPlant.Solve(Hip, Knee, Ankle, Target, out System.Numerics.Vector3 NewKnee, out System.Numerics.Vector3 NewAnkle));
            Assert.AreEqual(System.Numerics.Vector3.Distance(Hip, Knee), System.Numerics.Vector3.Distance(Hip, NewKnee), 0.001f);
            Assert.AreEqual(System.Numerics.Vector3.Distance(Knee, Ankle), System.Numerics.Vector3.Distance(NewKnee, NewAnkle), 0.001f);
            Assert.AreEqual(0f, System.Numerics.Vector3.Distance(Target, NewAnkle), 0.02f);
            System.Numerics.Vector3 OldBend = System.Numerics.Vector3.Cross(System.Numerics.Vector3.Normalize(Ankle - Hip), Knee - Hip);
            System.Numerics.Vector3 NewBend = System.Numerics.Vector3.Cross(System.Numerics.Vector3.Normalize(NewAnkle - Hip), NewKnee - Hip);
            Assert.IsTrue(System.Numerics.Vector3.Dot(OldBend, NewBend) > 0f);
        }

        [TestMethod]
        public void PlantedFootFollowsTheGroundAndALiftedFootClearsIt()
        {
            Assert.AreEqual(2f, FootPlant.FootHeight(1f, 2f, 1f, 0.3f), 0.001f);
            Assert.AreEqual(0.4f, FootPlant.FootHeight(1f, 0.4f, 1f, 0.3f), 0.001f);
            Assert.AreEqual(1.8f, FootPlant.FootHeight(1.8f, 1.2f, 1f, 0.3f), 0.001f);
            Assert.AreEqual(1.5f, FootPlant.FootHeight(1.4f, 1.5f, 1f, 0.3f), 0.001f);
            System.Numerics.Vector3 Flat = FootPlant.SlopeNormal(0f, 0f);
            Assert.AreEqual(0f, Flat.X, 0.001f);
            Assert.AreEqual(1f, Flat.Y, 0.001f);
            System.Numerics.Vector3 Ramp = FootPlant.SlopeNormal(1f, 0f);
            Assert.IsTrue(Ramp.X < 0f);
            Assert.IsTrue(Ramp.Y > 0f);
        }

        [TestMethod]
        public void WalkingPlaysTheSecondClipAndStandingReturnsToTheFirst()
        {
            string Folder = ModelFolder();
            string[] Files = Directory.GetFiles(Folder, "*.glb");
            Assert.IsTrue(Files.Length > 0);
            CopyShaderBesideTheTest(Folder);

            using (Form Window = new Form())
            {
                GraphicsDevice Device = NewDevice(Window);
                try
                {
                string WalkFile = null;
                string IdleOnly = null;
                foreach (string FilePath in Files)
                {
                    using (GltfModel Body = new GltfModel(Device, FilePath))
                    {
                        Assert.IsTrue(Body.AnimationNames.Count >= 1, Path.GetFileName(FilePath));
                        Assert.AreEqual(Body.AnimationNames[MovementClips.Idle], Body.ActiveAnimationName);
                        if (Body.AnimationNames.Count > MovementClips.Walk && WalkFile == null)
                        {
                            WalkFile = FilePath;
                        }

                        if (Body.AnimationNames.Count <= MovementClips.Walk && IdleOnly == null)
                        {
                            IdleOnly = FilePath;
                        }
                    }
                }

                Assert.IsNotNull(WalkFile);
                PlayerCharacter Character = new PlayerCharacter(null, null, Vector3.Zero);
                Character.WearBody(Device, WalkFile);
                GltfModel Worn = Character.BodyModel;
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Idle], Worn.ActiveAnimationName);

                PathSegment Segment = new PathSegment();
                Segment.Kind = Traversal.Walk;
                Segment.Polyline.Add(Vector3.Zero);
                Segment.Polyline.Add(new Vector3(0f, 80f, 0f));
                FollowPath Walk = new FollowPath(Character, new System.Collections.Generic.List<PathSegment> { Segment });
                Walk.OnStarted();
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Walk], Worn.ActiveAnimationName);

                Worn.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.2)));
                double Advanced = Worn.AnimationSeconds;
                Assert.IsTrue(Walk.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05))));
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Walk], Worn.ActiveAnimationName);
                Assert.IsTrue(Worn.AnimationSeconds >= Advanced);
                Assert.IsTrue(Character.Position.Y > 0f);

                Character.Move(Vector3.Zero, 0, 0.05);
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Idle], Worn.ActiveAnimationName);

                GameTime Step = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(0.05));
                for (int i = 0; i < 400 && Walk.Update(Step); i++)
                {
                }

                Assert.IsFalse(Walk.Update(Step));
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Idle], Worn.ActiveAnimationName);

                Character.PlayClip("Run");
                Assert.AreEqual(Worn.AnimationNames[MovementClips.Idle], Worn.ActiveAnimationName);
                Worn.Dispose();

                if (IdleOnly != null)
                {
                    Character.WearBody(Device, IdleOnly);
                    Character.PlayClip("Walking");
                    Assert.AreEqual(Character.BodyModel.AnimationNames[MovementClips.Idle], Character.BodyModel.ActiveAnimationName);
                    Character.BodyModel.Dispose();
                }
                }
                finally
                {
                    try
                    {
                        Device.Dispose();
                    }
                    catch (NullReferenceException)
                    {
                    }
                }
            }
        }

        static GraphicsDevice NewDevice(Form Window)
        {
            Window.FormBorderStyle = FormBorderStyle.None;
            Window.ShowInTaskbar = false;
            Window.ClientSize = new System.Drawing.Size(640, 480);
            PresentationParameters Parameters = new PresentationParameters();
            Parameters.DeviceWindowHandle = Window.Handle;
            Parameters.BackBufferWidth = 640;
            Parameters.BackBufferHeight = 480;
            Parameters.BackBufferFormat = SurfaceFormat.Color;
            Parameters.DepthStencilFormat = DepthFormat.Depth24;
            Parameters.PresentationInterval = PresentInterval.Immediate;
            Parameters.IsFullScreen = false;
            return new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, Parameters);
        }

        static void CopyShaderBesideTheTest(string ModelFolder)
        {
            string Shader = Path.GetFullPath(Path.Combine(ModelFolder, "..", "..", "..", "Shaders", "SkinnedBones.xnb"));
            string DestinationDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Shaders");
            Directory.CreateDirectory(DestinationDir);
            File.Copy(Shader, Path.Combine(DestinationDir, "SkinnedBones.xnb"), true);
        }

        static string ModelFolder()
        {
            DirectoryInfo Dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (Dir != null)
            {
                string Candidate = Path.Combine(Dir.FullName, "Project Eternity", "Project Eternity", "bin", "Debug", "Content", "Life Sim", "Assets", "Models");
                if (Directory.Exists(Candidate))
                {
                    return Candidate;
                }

                Dir = Dir.Parent;
            }

            return @"D:\CuldceptSaga\New Game\Project Eternity\Project Eternity\bin\Debug\Content\Life Sim\Assets\Models";
        }
    }
}
