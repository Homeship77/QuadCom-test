using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataProcessing
{
    public class Path 
    {
        private const int MaxNeighbours = 8;
        private readonly PathStep[] neighbours = new PathStep[MaxNeighbours];

        private readonly int maxSteps;
        private readonly IBinaryHeap<Vector2, PathStep> frontier;
        private readonly HashSet<Vector2> ignoredPositions;
        private readonly List<Vector2> output;
        private readonly IDictionary<Vector2, Vector2> links;
        private readonly List<int[]> _levelData;
        private readonly IReadOnlyCollection<Vector2> _obstacles;

        public Path(List<int[]> levelData, IReadOnlyCollection<Vector2> obstacles, int maxSteps = int.MaxValue, int initialCapacity = 0)
        {
            if (maxSteps <= 0) 
                throw new ArgumentOutOfRangeException(nameof(maxSteps));
            if (initialCapacity < 0) 
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));

            _levelData = levelData;
            _obstacles = obstacles;

            this.maxSteps = maxSteps;
            frontier = new BinaryHeap<Vector2, PathStep>(a => a.Position, initialCapacity);
            ignoredPositions = new HashSet<Vector2>(initialCapacity);
            output = new List<Vector2>(initialCapacity);
            links = new Dictionary<Vector2, Vector2>(initialCapacity);
        }

        public bool Calculate(Vector2 start, Vector2 target, out IReadOnlyCollection<Vector2> path)
        {
            if (_levelData == null) throw new ArgumentNullException(nameof(_levelData));

            if (!GenerateNodes(start, target, _obstacles))
            {
                path = Array.Empty<Vector2>();
                return false;
            }

            output.Clear();
            output.Add(target);

            while (links.TryGetValue(target, out target)) 
                output.Add(target);
            path = output;
            return true;
        }

        private bool GenerateNodes(Vector2 start, Vector2 target, IReadOnlyCollection<Vector2> obstacles)
        {
            frontier.Clear();
            ignoredPositions.Clear();
            links.Clear();

            frontier.Enqueue(new PathStep(start, target, 0));
            ignoredPositions.UnionWith(obstacles);
            var step = 0;
            while (frontier.Count > 0 && step++ <= maxSteps)
            {
                PathStep current = frontier.Dequeue();
                ignoredPositions.Add(current.Position);

                if (current.Position.Equals(target)) 
                    return true;

                GenerateFrontierNodes(current, target);
            }

            // All nodes analyzed - no path detected.
            return false;
        }

        private bool CheckAvaliableDirections(PathStep parent, PathStep nextStep)
        {
            int prntX = (int)parent.Position.x;
            int prntY = (int)parent.Position.y;
            int nextX = (int)nextStep.Position.x;
            int nextY = (int)nextStep.Position.y;
            if (nextX < _levelData.Count && prntX < _levelData.Count && nextY >= 0 && nextX >= 0 && prntX >=0 && prntY >= 0)
            {
                if (nextY < _levelData[nextX].Length && prntY < _levelData[prntX].Length)
                {
                    return Mathf.Abs(_levelData[nextX][nextY] - _levelData[prntX][prntY]) <= 1;
                }
                else
                    return false;
            }
            else
                return false;
        }

        private void GenerateFrontierNodes(PathStep parent, Vector2 target)
        {
            neighbours.Fill(parent, target);
            foreach (PathStep newNode in neighbours)
            {
                // Position is already checked or occupied by an obstacle.
                if (ignoredPositions.Contains(newNode.Position)) 
                    continue;

                //node height difference is too high
                if (!CheckAvaliableDirections(parent, newNode)) 
                    continue;

                // Node is not present in queue.
                if (!frontier.TryGet(newNode.Position, out PathStep existingNode))
                {
                    frontier.Enqueue(newNode);
                    links[newNode.Position] = parent.Position;
                }

                // Node is present in queue and new optimal path is detected.
                else if (newNode.TraverseDistance < existingNode.TraverseDistance)
                {
                    frontier.Modify(newNode);
                    links[newNode.Position] = parent.Position;
                }
            }
        }
    }
}