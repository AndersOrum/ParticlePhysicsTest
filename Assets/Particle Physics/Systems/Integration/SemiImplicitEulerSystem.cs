using ParticlePhysics.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ParticlePhysics.Systems
{ 
    public class SemiImplicitEulerSystem :IntegrationSystem 
    {
        [BurstCompile]
        public struct SemiImplicitEulerJob : IJobParallelFor
        {
            [ReadOnly]
            public float deltaTime;

            public NativeArray<float3> positions;

            public NativeArray<float3> velocities;

            [ReadOnly]
            public NativeArray<float> inverseMass;

            public NativeArray<float3> accumulatedForces;

            public void Execute(int i)
            {
                // calculate new velocity from current acceleration 
                velocities[i] += accumulatedForces[i] * inverseMass[i] * deltaTime;

                // calculate new position from new velocity
                positions[i] += velocities[i] * deltaTime;

                // reset accumulator
                accumulatedForces[i] = 0;

            }
        }

        public override JobHandle UpdateSystem(ParticleData particleData, float deltaTime, JobHandle inputDependency = default)
        {
            SemiImplicitEulerJob integrationJob = new SemiImplicitEulerJob()
            {
                deltaTime = deltaTime,
                positions = particleData.positions,
                velocities = particleData.velocities,
                inverseMass = particleData.inverseMass,
                accumulatedForces = particleData.accumulatedForces,
            };

            JobHandle integrationJobHandle = integrationJob.Schedule(particleData.count, 64, inputDependency);

            return integrationJobHandle;
        }
    }
}