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
    public class LinearGravitySystem : GravitySystem
    {
        [BurstCompile]
        public struct LinearGravityJob : IJobParallelFor
        {
            [ReadOnly]
            public float3 gravity;

            [ReadOnly]
            public NativeArray<float> mass;

            public NativeArray<float3> accumulatedForces;

            public void Execute(int i)
            {
                accumulatedForces[i] += mass[i] * gravity;
            }
        }


        public override JobHandle UpdateSystem(ParticleData particleData, GravityData gravityData, JobHandle inputDependency = default)
        {
            LinearGravityJob gravityJob = new LinearGravityJob()
            {
                gravity = gravityData.linearGravity,
                mass = particleData.mass,
                accumulatedForces = particleData.accumulatedForces,
            };

            JobHandle gravityJobHandle = gravityJob.Schedule(particleData.count, 64, inputDependency);

            //gravityJobHandle.Complete();
            return gravityJobHandle;
        }
    }
}
