using Unity.Collections;
using Unity.Mathematics;

namespace ParticlePhysics.Data
{
    public struct ParticleData 
    {

        public NativeArray<float3> positions;
        public NativeArray<float3> velocities;
        public NativeArray<float> mass;
        public NativeArray<float> inverseMass;
        public NativeArray<float3> accumulatedForces;

        public NativeArray<float> radius;

        public int count;

        public void Initialize(int particleCount)
        {
            positions = new NativeArray<float3>(particleCount, Allocator.Persistent);
            velocities = new NativeArray<float3>(particleCount, Allocator.Persistent);
            mass = new NativeArray<float>(particleCount, Allocator.Persistent);
            inverseMass = new NativeArray<float>(particleCount, Allocator.Persistent);
            accumulatedForces = new NativeArray<float3>(particleCount, Allocator.Persistent);
            radius = new NativeArray<float>(particleCount, Allocator.Persistent);

            count = particleCount;
        }

        public void Dispose()
        {
            if (positions.IsCreated)
            {
                positions.Dispose(); 
                velocities.Dispose();
                mass.Dispose();
                inverseMass.Dispose();
                accumulatedForces.Dispose();
                radius.Dispose();
            }
        }
    }
}

