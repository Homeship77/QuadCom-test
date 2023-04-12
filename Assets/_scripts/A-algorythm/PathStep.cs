using System;
using UnityEngine;

namespace DataProcessing
{
    internal readonly struct PathStep : IComparable<PathStep>
    {
        private readonly double estimatedTotalCost;
        private readonly double heuristicDistance;

        public PathStep(Vector2 position, Vector2 target, double traverseDistance)
        {
            Position = position;
            TraverseDistance = traverseDistance;
            heuristicDistance = 0;
            estimatedTotalCost =0;
            heuristicDistance = DistanceEstimate(position, target);
            estimatedTotalCost = traverseDistance + heuristicDistance;
        }

        public double DistanceEstimate(Vector2 position, Vector2 target)
        {
            var diff = position - target;
            int linearSteps = (int)Math.Abs(Math.Abs(diff.y) - Math.Abs(diff.x));
            int diagonalSteps = (int)Math.Max(Math.Abs(diff.y), Math.Abs(diff.x)) - linearSteps;
            return linearSteps + Math.Sqrt(2) * diagonalSteps;
        }

        public Vector2 Position { get; }
        public double TraverseDistance { get; }

        public int CompareTo(PathStep other)
            => estimatedTotalCost.CompareTo(other.estimatedTotalCost);
    }
}