using QuadComTest.EventSystems;
using QuadComTest.Interfaces;
using System;
using UnityEngine;

namespace UserInput
{
    public class MouseRayCastProcessor : IUpdatableModule
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        public MouseRayCastProcessor(IUserEvent handler)
        {
            handler.OnUpdateEvent += OnUpdate;
        }


        public void OnUpdate(float deltaTime)
        {
            var mousePosition = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = MouseRayCastPoint(mousePosition);
                EventManager.RaiseEvent<IUserEvent>(handler => handler.SelectStartPos(_startPosition));
            }
            if (Input.GetMouseButtonDown(1))
            {
                _endPosition = MouseRayCastPoint(mousePosition);
                EventManager.RaiseEvent<IUserEvent>(handler => handler.SelectEndPos(_endPosition));
            }

            if (!_startPosition.Equals(Vector3.zero) && !_endPosition.Equals(Vector3.zero))
            {
                EventManager.RaiseEvent<IUserEvent>(handler => handler.ReadyToPathCalculate(_startPosition, _endPosition));
                _startPosition = Vector3.zero;
                _endPosition = Vector3.zero;
            }
        }

        
        private Vector3 MouseRayCastPoint(Vector2 mousePos)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit[] hits = new RaycastHit[10];
            var num = Physics.RaycastNonAlloc(ray, hits);
            float maxY = -100f;
            Vector3 selectedPos = Vector3.zero;
            for (int i = 0; i < num; i++)
            {
                if (hits[i].transform.position.y > maxY)
                {
                    maxY = hits[i].point.y;
                    selectedPos = hits[i].point;
                    selectedPos.y = hits[i].transform.position.y;
                }
            }
            Vector3 resValue = Vector3.zero;
            EventManager.RaiseEvent<IUserEvent>(handler => handler.GetPrecisePosition(selectedPos, out resValue));
            return resValue;
        }
    }
}