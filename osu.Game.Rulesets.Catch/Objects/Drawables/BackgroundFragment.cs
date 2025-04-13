// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace osu.Game.Rulesets.Catch.Objects.Drawables
{
    public partial class BackgroundFragment : Container
    {
        private readonly Sprite fragmentSprite;
        private readonly Container maskContainer;

        public RectangleF? FragmentBounds { get; private set; }
        public Texture? FragmentTexture => fragmentSprite.Texture;

        public BackgroundFragment()
        {
            Size = new Vector2(CatchHitObject.OBJECT_RADIUS * 2);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                maskContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = CatchHitObject.OBJECT_RADIUS,
                    BorderThickness = 0,
                    Child = fragmentSprite = new Sprite
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                }
            };
        }

        public void SetBackgroundPortion(Texture texture, RectangleF sourceBounds)
        {
            fragmentSprite.Texture = texture;
            fragmentSprite.Size = new Vector2(
                sourceBounds.Width / sourceBounds.Height * Height,
                Height);
            fragmentSprite.Position = new Vector2(
                -sourceBounds.X / texture.Width * fragmentSprite.Width,
                -sourceBounds.Y / texture.Height * fragmentSprite.Height);

            FragmentBounds = sourceBounds;
        }

        protected override void Update()
        {
            base.Update();

            // Create circular gradient mask
            float radius = Size.X / 2;
            maskContainer.CornerRadius = radius;

            // Fade out edges for smoother appearance
            maskContainer.EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = Colour4.Black,
                Roundness = 50,
                Radius = radius * 0.3f,
            };
        }
    }
}
