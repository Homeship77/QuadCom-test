using System;
using UnityEngine;

namespace QuadComTest.Interfaces
{
    public interface IUserEvent: IGlobalSubscriber
    {
        void ReadyToPathCalculate(Vector3 startPos, Vector3 endPos);
        void SelectStartPos(Vector3 startPos);
        void SelectEndPos(Vector3 endPos);

        void GetPrecisePosition(Vector3 pointPos, out Vector3 precPos);
        event Action<float> OnUpdateEvent;
    }
}
