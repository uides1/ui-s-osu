// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Catch.Objects;
using System.Collections.Generic;

namespace osu.Game.Rulesets.Catch.Beatmaps
{
    public static class CatchBeatmapStatistics
    {
        public static (int fruits, int droplets, int tinyDroplets) GetObjectCounts(IBeatmap beatmap)
        {
            int fruits = 0;
            int droplets = 0;
            int tinyDroplets = 0;

            // First count direct objects
            foreach (var hitObject in beatmap.HitObjects)
            {
                switch (hitObject)
                {
                    case Fruit:
                        fruits++;
                        break;

                    case TinyDroplet:
                        tinyDroplets++;
                        break;

                    case Droplet droplet when !(droplet is TinyDroplet):
                        droplets++;
                        break;
                }

                // Count nested objects from juice streams
                if (hitObject is JuiceStream juice)
                {
                    var nestedCounts = CountNestedObjects(juice);
                    fruits += nestedCounts.fruits;
                    droplets += nestedCounts.droplets;
                    tinyDroplets += nestedCounts.tinyDroplets;
                }
            }

            System.Diagnostics.Debug.WriteLine($"[Catch Statistics] Fruits: {fruits}, Droplets: {droplets}, Tiny Droplets: {tinyDroplets}");
            return (fruits, droplets, tinyDroplets);
        }

        private static (int fruits, int droplets, int tinyDroplets) CountNestedObjects(JuiceStream juice)
        {
            int fruits = 0;
            int droplets = 0;
            int tinyDroplets = 0;

            foreach (var nested in juice.NestedHitObjects)
            {
                switch (nested)
                {
                    case Fruit:
                        fruits++;
                        break;

                    case TinyDroplet:
                        tinyDroplets++;
                        break;

                    case Droplet:
                        droplets++;
                        break;
                }
            }

            return (fruits, droplets, tinyDroplets);
        }
    }
}
