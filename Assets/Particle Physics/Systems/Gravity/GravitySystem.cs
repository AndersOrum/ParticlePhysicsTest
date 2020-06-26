using ParticlePhysics.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ParticlePhysics.Systems
{
    public class GravitySystem : PhysicsSubSystems
    {
        public virtual JobHandle UpdateSystem(ParticleData particleData, GravityData gravityData, JobHandle inputDependency = default)
        {
    
            return inputDependency;
        }
    }
}