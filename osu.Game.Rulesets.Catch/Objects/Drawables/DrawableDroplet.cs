// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Skinning.Default;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Catch.Objects.Drawables
{
    public partial class DrawableDroplet : DrawablePalpableCatchHitObject
    {
        private BackgroundFragment fragment;
        private float startRotation;

        public DrawableDroplet()
            : this(null)
        {
        }

        public DrawableDroplet(CatchHitObject? h)
            : base(h)
        {
        }

        [BackgroundDependencyLoader]
        private void load(IWorkingBeatmap beatmap)
        {
            fragment = new BackgroundFragment();
            if (beatmap.Background != null)
            {
                var tex = beatmap.Background.Texture;
                if (tex != null)
                {
                    // Randomly select a portion of the background - smaller than fruits
                    float x = RandomSingle(2) * tex.Width;
                    float y = RandomSingle(3) * tex.Height;
                    float size = tex.Width * 0.05f * (1 + RandomSingle(4) * 0.5f); // Random size between 5-7.5% of width

                    fragment.SetBackgroundPortion(tex, new RectangleF(x, y, size, size));
                }
            }

            ScalingContainer.Children = new Drawable[]
            {
                fragment,
                new SkinnableDrawable(
                    new CatchSkinComponentLookup(CatchSkinComponents.Droplet),
                    _ => new DropletPiece())
                {
                    Alpha = 0.5f
                }
            };
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();
            startRotation = RandomSingle(1) * 20;
        }

        protected override void Update()
        {
            base.Update();

            double preemptProgress = (Time.Current - (HitObject.StartTime - InitialLifetimeOffset)) / (HitObject.TimePreempt + 2000);
            ScalingContainer.Rotation = (float)Interpolation.Lerp(startRotation, startRotation + 720, preemptProgress);
        }
    }
}
