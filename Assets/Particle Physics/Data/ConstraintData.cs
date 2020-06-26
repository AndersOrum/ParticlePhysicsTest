using Unity.Collections;
using Unity.Mathematics;

namespace ParticlePhysics.Data
{
    public struct ConstraintData 
    {

        public NativeArray<int> particleIDs;

        public NativeArray<float> stiffness;
        public NativeArray<float> damping;
        public NativeArray<float> targetLength;

        public NativeArray<float3> solvedForceA;
        public NativeArray<float3> solvedForceB;

        public int count;

        public void Initialize(int constraintCount)
        {
            particleIDs = new NativeArray<int>(constraintCount * 2, Allocator.Persistent);
            stiffness = new NativeArray<float>(constraintCount, Allocator.Persistent);
            damping = new NativeArray<float>(constraintCount, Allocator.Persistent);
            targetLength = new NativeArray<float>(constraintCount, Allocator.Persistent);

            solvedForceA = new NativeArray<float3>(constraintCount, Allocator.Persistent);
            solvedForceB = new NativeArray<float3>(constraintCount, Allocator.Persistent);

            count = constraintCount;
        }

        public void Dispose()
        {
            if(particleIDs.IsCreated)
            {
                particleIDs.Dispose();
                stiffness.Dispose();
                damping.Dispose();
                targetLength.Dispose();
                solvedForceA.Dispose();
                solvedForceB.Dispose();
            }


        }
    }
}

