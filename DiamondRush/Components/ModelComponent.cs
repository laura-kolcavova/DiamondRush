using MonoECS.Ecs;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiamondRush.Components
{
    public class ModelComponent : IEntityComponent
    {
        public string Name { get; set; }
    }
}
