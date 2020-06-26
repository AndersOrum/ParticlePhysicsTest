using Unity.Collections;
using Unity.Mathematics;

namespace ParticlePhysics.Data
{
    public struct GravityData 
    {

        public float3 linearGravity;
        public float  gravityConstant;


        public NativeArray<int> linearGravityParticleIDs;
        public NativeArray<int> sphericalEmitterParticleIDs;
        public NativeArray<int> sphericalRecieverParticleIDs;

        public int emitterCount;
        public int recieverCount;

        public void Initialize(int linearCount, int sphericalEmitterCount, int sphericalRecieverCount)
        {
            linearGravityParticleIDs = new NativeArray<int>(linearCount, Allocator.Persistent);
            sphericalEmitterParticleIDs = new NativeArray<int>(sphericalEmitterCount, Allocator.Persistent);
            sphericalRecieverParticleIDs = new NativeArray<int>(sphericalRecieverCount, Allocator.Persistent);

            emitterCount = sphericalEmitterCount;
            recieverCount = sphericalRecieverCount;
        }

        public void Dispose()
        {
            if(linearGravityParticleIDs.IsCreated)
                linearGravityParticleIDs.Dispose();

            if (sphericalEmitterParticleIDs.IsCreated)
                sphericalEmitterParticleIDs.Dispose();

            if (sphericalRecieverParticleIDs.IsCreated)
                sphericalRecieverParticleIDs.Dispose();

        }
    }
}

