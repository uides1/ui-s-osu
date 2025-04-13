// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osuTK;

namespace osu.Game.Rulesets.Catch.UI
{
    public partial class BackgroundDisplay : Container
    {
        private readonly Dictionary<(float X, float Y, float Size), bool> fragments = new Dictionary<(float X, float Y, float Size), bool>();
        private readonly Container fragmentContainer;
        private readonly Sprite backgroundSprite;

        public BackgroundDisplay()
        {
            RelativeSizeAxes = Axes.Both;

            Children = new Drawable[]
            {
                backgroundSprite = new Sprite
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Alpha = 0.3f,
                    FillMode = FillMode.Fill,
                },
                fragmentContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.7f,
                    Masking = true,
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(IWorkingBeatmap beatmap)
        {
            var background = beatmap.GetBackground(); // Correct method to retrieve background
            if (background != null)
                backgroundSprite.Texture = background;
        }

        public void RegisterFragment(float x, float y, float size)
        {
            fragments[(x, y, size)] = false;
        }

        public void CollectFragment(float x, float y, float size)
        {
            if (!fragments.ContainsKey((x, y, size))) return;

            fragments[(x, y, size)] = true;

            var sprite = new Sprite
            {
                Texture = backgroundSprite.Texture,
                Size = new Vector2(size),
                Position = new Vector2(x, y),
                Alpha = 0,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
            };

            fragmentContainer.Add(sprite);

            // Animate fragment collection
            sprite.FadeIn(300, Easing.OutQuint)
                 .ScaleTo(1.2f)
                 .Then()
                 .ScaleTo(1f, 500, Easing.OutElastic);

            // Show completion effect at 100%
            if (getCompletion() >= 1)
            {
                onFullCompletion();
            }
        }

        private void onFullCompletion()
        {
            // Fade in the full background
            backgroundSprite.FadeTo(0.8f, 1000, Easing.OutQuint);
            fragmentContainer.FadeOut(1000);
        }

        private float getCompletion()
        {
            if (fragments.Count == 0) return 0;

            int collected = 0;
            foreach (bool fragmentCollected in fragments.Values)
            {
                if (fragmentCollected) collected++;
            }

            return (float)collected / fragments.Count;
        }
    }
}
