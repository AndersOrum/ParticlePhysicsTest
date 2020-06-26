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
    public class SphericalGravitySystem : GravitySystem
    {
        [BurstCompile]
        private struct SphericalGravityJob : IJobParallelFor
        {
            [ReadOnly]
            public float gravityConstant;

            [NativeDisableParallelForRestriction]  [ReadOnly]
            public NativeArray<float3> positions;

            [NativeDisableParallelForRestriction]
            public NativeArray<float3> accumulatedForces;

            [NativeDisableParallelForRestriction]
            [ReadOnly]
            public NativeArray<float> mass;

            [NativeDisableParallelForRestriction]
            [ReadOnly]
            public NativeArray<int> emitterNodeIDs;

            [ReadOnly]
            public NativeArray<int> recieverNodeIDs;

            public void Execute(int i)
            {
                int recieverNodeID = recieverNodeIDs[i];

                for (int j = 0; j < emitterNodeIDs.Length; j++)
                {
                    int emitterNodeID = emitterNodeIDs[j];


                    if (emitterNodeID == recieverNodeID)
                        return;

                    float3 direction = positions[emitterNodeID] - positions[recieverNodeID];

                    float distance = math.length(direction) + float.Epsilon; // Adding epsilon to prevent Zero Division errors

                    direction /= distance; // normalize
                    accumulatedForces[recieverNodeID] += gravityConstant * mass[recieverNodeID] *mass[emitterNodeID] / (distance * distance) * direction;

                }
            }
        }

        public override JobHandle UpdateSystem(ParticleData particleData, GravityData gravityData, JobHandle inputDependency = default)
        {
            SphericalGravityJob gravityJob = new SphericalGravityJob()
            {
                gravityConstant = gravityData.gravityConstant,
                positions = particleData.positions,
                accumulatedForces = particleData.accumulatedForces,
                mass = particleData.mass,
                emitterNodeIDs = gravityData.sphericalEmitterParticleIDs,
                recieverNodeIDs = gravityData.sphericalRecieverParticleIDs,
            };

            JobHandle gravityJobHandle = gravityJob.Schedule(gravityData.recieverCount, 64, inputDependency);

            return gravityJobHandle;
        }
    }
}
