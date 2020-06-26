using ParticlePhysics.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace ParticlePhysics.Systems
{ 
    public class ConstraintSystem : PhysicsSubSystems 
    {
       
        [BurstCompile]
        public struct ElasticSpringConstraintJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<float3> positions;
            [ReadOnly]
            public NativeArray<float3> velocities;
            [ReadOnly]
            public NativeArray<float> inverseMass;

            [ReadOnly]
            public NativeArray<int> particleIDs;

            [ReadOnly]
            public NativeArray<float> stiffness;

            [ReadOnly]
            public NativeArray<float> damping;

            [ReadOnly]
            public NativeArray<float> targetLength;

            [WriteOnly]
            public NativeArray<float3> solvedForceA;

            [WriteOnly]
            public NativeArray<float3> solvedForceB;

            public void Execute(int constraintIndex)
            {

         
                int particleA = particleIDs[constraintIndex * 2];
                int particleB = particleIDs[constraintIndex * 2 + 1];

                // get the direction vector
                float3 direction = positions[particleA] - positions[particleB];
                float distance = math.length(direction) + float.Epsilon; // Adding epsilon to prevent Zero Division errors
                // Normalize
                direction /= distance;
                            
                //add spring strenght force
                float3 force = stiffness[constraintIndex] * (targetLength[constraintIndex] - distance) * direction;

                //add spring damping force
                force += -damping[constraintIndex] * math.dot(velocities[particleA] - velocities[particleB], direction) * direction;

               
                solvedForceA[constraintIndex] = force;
                solvedForceB[constraintIndex] = -force;

            }
        }

        [BurstCompile]
        public struct ForceSyncronizer : IJob
        {
            [ReadOnly]
            public int forceConstraintCount;

            [ReadOnly]
            public NativeArray<int> particleIDs;

            [ReadOnly]
            public NativeArray<float3> solvedForceA;

            [ReadOnly]
            public NativeArray<float3> solvedForceB;

            public NativeArray<float3> accumulatedForces;

            public void Execute()
            {
                for (int i = 0; i < forceConstraintCount; i++)
                {
                    accumulatedForces[particleIDs[i * 2 ]]    += solvedForceA[i];
                    accumulatedForces[particleIDs[i * 2 + 1]] += solvedForceB[i];
                }
            }
        }

        public virtual JobHandle UpdateSystem(ParticleData particleData, ConstraintData constraintData, JobHandle inputDependency = default)
        {
            ElasticSpringConstraintJob constraintJob = new ElasticSpringConstraintJob()
            {
                positions = particleData.positions,
                velocities = particleData.velocities,
                inverseMass = particleData.inverseMass,
                particleIDs = constraintData.particleIDs,
                stiffness = constraintData.stiffness,
                damping = constraintData.damping,
                targetLength = constraintData.targetLength,
                solvedForceA = constraintData.solvedForceA,
                solvedForceB = constraintData.solvedForceB,
            };
            JobHandle constraintJobHandle = constraintJob.Schedule(constraintData.count, 64, inputDependency);

            ForceSyncronizer syncJob = new ForceSyncronizer()
            {
                forceConstraintCount = constraintData.count,
                particleIDs = constraintData.particleIDs,
                solvedForceA = constraintData.solvedForceA,
                solvedForceB = constraintData.solvedForceB,
                accumulatedForces = particleData.accumulatedForces,
            };
            JobHandle syncJobHandle = syncJob.Schedule(constraintJobHandle);

            return syncJobHandle;
        }
    }
}