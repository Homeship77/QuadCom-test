using System.Collections.Generic;
using UnityEngine;

namespace DataProcessing
{
    public interface IPath
    {
        bool Calculate(Vector2 start, Vector2 target, IReadOnlyCollection<Vector2> obstacles, out IReadOnlyCollection<Vector2> path);
    }
}