using System;
using UnityEngine;

namespace DataProcessing
{
    internal static class PathStepExtensions
    {
        private static readonly (Vector2 position, double cost)[] NeighboursTemplate = {
            (new Vector2(1, 0), 1),
            (new Vector2(0, 1), 1),
            (new Vector2(-1, 0), 1),
            (new Vector2(0, -1), 1),
            (new Vector2(1, 1), Math.Sqrt(2)),
            (new Vector2(1, -1), Math.Sqrt(2)),
            (new Vector2(-1, 1), Math.Sqrt(2)),
            (new Vector2(-1, -1), Math.Sqrt(2))
        };

        public static void Fill(this PathStep[] buffer, PathStep parent, Vector2 target)
        {
            int i = 0;
            foreach ((Vector2 position, double cost) in NeighboursTemplate)
            {
                Vector2 nodePosition = position + parent.Position;
                double traverseDistance = parent.TraverseDistance + cost;
                buffer[i++] = new PathStep(nodePosition, target, traverseDistance);
            }
        }
    }
}