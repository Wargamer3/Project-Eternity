using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class PartialAnimation : AnimationClass
    {
        public Dictionary<string, VisibleTimeline> DicInheritedObject;//Used to access a particular AnimationObject at any time.
        public ComplexAnimation Owner;

        public PartialAnimation(string AnimationPath, ComplexAnimation Owner)
            : base(AnimationPath)
        {
            DicInheritedObject = new Dictionary<string, VisibleTimeline>();
            this.Owner = Owner;
        }

        public override void Load()
        {
            base.Load();
        }

        public override void Init()
        {
        }

        public override void OnVisibleTimelineSpawn(AnimationLayer ActiveLayer, VisibleTimeline ActiveBitmap)
        {
            base.OnVisibleTimelineSpawn(ActiveLayer, ActiveBitmap);

            if (DicInheritedObject.ContainsKey(ActiveBitmap.Name))
                DicInheritedObject[ActiveBitmap.Name] = ActiveBitmap;
            else
                DicInheritedObject.Add(ActiveBitmap.Name, ActiveBitmap);
        }

        public override void OnMarkerTimelineSpawn(AnimationLayer ActiveLayer, MarkerTimeline ActiveMarker)
        {
            base.OnMarkerTimelineSpawn(ActiveLayer, ActiveMarker);

            if (DicInheritedObject.ContainsKey(ActiveMarker.Name))
                DicInheritedObject[ActiveMarker.Name] = ActiveMarker;
            else
                DicInheritedObject.Add(ActiveMarker.Name, ActiveMarker);
        }

        public override void OnPolygonCutterTimelineSpawn(AnimationLayer ActiveLayer, PolygonCutterTimeline ActivePolygonCutter)
        {
            base.OnPolygonCutterTimelineSpawn(ActiveLayer, ActivePolygonCutter);

            if (DicInheritedObject.ContainsKey(ActivePolygonCutter.Name))
                DicInheritedObject[ActivePolygonCutter.Name] = ActivePolygonCutter;
            else
                DicInheritedObject.Add(ActivePolygonCutter.Name, ActivePolygonCutter);
        }

        public override void OnVisibleTimelineDeath(VisibleTimeline RemovedBitmap)
        {
            base.OnVisibleTimelineDeath(RemovedBitmap);

            if (DicInheritedObject.ContainsKey(RemovedBitmap.Name))
                DicInheritedObject[RemovedBitmap.Name] = null;
        }

        public override void OnMarkerTimelineDeath(MarkerTimeline RemovedMarker)
        {
            base.OnMarkerTimelineDeath(RemovedMarker);

            if (DicInheritedObject.ContainsKey(RemovedMarker.Name))
                DicInheritedObject[RemovedMarker.Name] = null;
        }

        public override void OnPolygonCutterTimelineDeath(PolygonCutterTimeline RemovedPolygonCutter)
        {
            base.OnPolygonCutterTimelineDeath(RemovedPolygonCutter);

            if (DicInheritedObject.ContainsKey(RemovedPolygonCutter.Name))
                DicInheritedObject[RemovedPolygonCutter.Name] = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
