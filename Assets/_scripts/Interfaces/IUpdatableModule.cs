using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComTest.Interfaces
{
    internal interface IUpdatableModule
    {
        void OnUpdate(float deltaTime);
    }
}
