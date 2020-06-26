using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace ParticlePhysics.Systems
{
    public class PhysicsSubSystems 
    {
        public virtual JobHandle UpdateSystem(JobHandle inputDependency = default)
        {

            return inputDependency;
        }
    }
}
