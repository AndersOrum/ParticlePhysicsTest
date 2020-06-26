using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ParticlePhysics.Components
{
    [System.Serializable]
    public struct ParticleComponent : IComponentData
    {
        public int particleID;
        public float3 position;
        public float3 velocity;
        public float mass;
        public float radius;

       
    }

    public class Particle : ComponentDataProxy<ParticleComponent>
    {

    }
}