// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Skinning.Default;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Catch.Objects.Drawables
{
    public partial class DrawableFruit : DrawablePalpableCatchHitObject
    {
        private BackgroundFragment fragment;

        public DrawableFruit()
            : this(null)
        {
        }

        public DrawableFruit(Fruit? h)
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
                    // Randomly select a portion of the background
                    float x = RandomSingle(2) * tex.Width;
                    float y = RandomSingle(3) * tex.Height;
                    float size = tex.Width * 0.1f * (1 + RandomSingle(4) * 0.5f); // Random size between 10-15% of width

                    fragment.SetBackgroundPortion(tex, new RectangleF(x, y, size, size));
                }
            }

            ScalingContainer.Children = new Drawable[]
            {
                fragment,
                new SkinnableDrawable(
                    new CatchSkinComponentLookup(CatchSkinComponents.Fruit),
                    _ => new FruitPiece())
                {
                    Alpha = 0.5f // Make the fruit semi-transparent to show background
                }
            };
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();
            ScalingContainer.Rotation = (RandomSingle(1) - 0.5f) * 40;
        }
    }
}
