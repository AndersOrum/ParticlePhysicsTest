using ParticlePhysics.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ParticlePhysics.Systems
{ 
    public class IntegrationSystem : PhysicsSubSystems
    {
        public virtual JobHandle UpdateSystem(ParticleData particleData, float deltaTime, JobHandle inputDependency = default)
        {
            return inputDependency;
        }
    }
}