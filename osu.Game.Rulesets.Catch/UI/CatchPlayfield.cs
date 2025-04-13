// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Catch.Objects.Drawables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using System.Linq;

namespace osu.Game.Rulesets.Catch.UI
{
    public partial class CatchPlayfield : ScrollingPlayfield
    {
        /// <summary>
        /// The width of the playfield.
        /// The horizontal movement of the catcher is confined in the area of this width.
        /// </summary>
        public const float WIDTH = 512;

        /// <summary>
        /// The height of the playfield.
        /// This doesn't include the catcher area.
        /// </summary>
        public const float HEIGHT = 384;

        /// <summary>
        /// The center position of the playfield.
        /// </summary>
        public const float CENTER_X = WIDTH / 2;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) =>
            // only check the X position; handle all vertical space.
            base.ReceivePositionalInputAt(new Vector2(screenSpacePos.X, ScreenSpaceDrawQuad.Centre.Y));

        public readonly Catcher Catcher;

        public readonly CatcherArea CatcherArea;

        private readonly BackgroundDisplay backgroundDisplay;

        public Container UnderlayElements { get; private set; } = null!;

        private readonly IBeatmapDifficultyInfo difficulty;

        public CatchPlayfield(IBeatmapDifficultyInfo difficulty)
        {
            this.difficulty = difficulty;

            AddRangeInternal(new Drawable[]
            {
                backgroundDisplay = new BackgroundDisplay(),
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new[]
                    {
                        CatcherArea = new CatcherArea
                        {
                            Name = "Catcher Area",
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                },
                HitObjectContainer,
            });

            var droppedObjectContainer = new DroppedObjectContainer();
            Catcher = new Catcher(droppedObjectContainer, difficulty, backgroundDisplay)
            {
                X = CatchPlayfield.CENTER_X
            };
            CatcherArea.Add(Catcher);
            CatcherArea.Add(droppedObjectContainer);
        }

        protected override GameplayCursorContainer CreateCursor() => new CatchCursorContainer();

        [BackgroundDependencyLoader]
        private void load()
        {
            UnderlayElements = new Container
            {
                RelativeSizeAxes = Axes.Both,
            };

            RegisterPool<Droplet, DrawableDroplet>(50);
            RegisterPool<TinyDroplet, DrawableTinyDroplet>(50);
            RegisterPool<Fruit, DrawableFruit>(100);
            RegisterPool<Banana, DrawableBanana>(100);
            RegisterPool<JuiceStream, DrawableJuiceStream>(10);
            RegisterPool<BananaShower, DrawableBananaShower>(2);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // these subscriptions need to be done post constructor to ensure externally bound components have a chance to populate required fields (ScoreProcessor / ComboAtJudgement in this case).
            NewResult += onNewResult;
            RevertResult += onRevertResult;
        }

        public override void Add(DrawableHitObject h)
        {
            var drawnObject = (DrawableCatchHitObject)h;
            drawnObject.CheckPosition = (catchHitObject) => Catcher.CanCatch(catchHitObject);

            // Register the fragment if it's a fruit or droplet
            if (h is DrawablePalpableCatchHitObject palpable)
            {
                var fragment = palpable.ChildrenOfType<BackgroundFragment>().FirstOrDefault();
                if (fragment.FragmentTexture != null)
                {
                    var bounds = fragment.FragmentBounds.Value;
                    backgroundDisplay.RegisterFragment(bounds.X, bounds.Y, bounds.Width);
                }
            }

            base.Add(h);
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
            => CatcherArea.OnNewResult((DrawableCatchHitObject)judgedObject, result);

        private void onRevertResult(JudgementResult result)
            => CatcherArea.OnRevertResult(result);
    }
}
