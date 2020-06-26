using ParticlePhysics.Data;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ParticlePhysics.Systems
{
    public class PhysicsWorldSystem : MonoBehaviour
    {
        public enum SimulationPreset {Cube, SphericalGravity, TennisRacketTheorim};
        public SimulationPreset simulationPreset;

        public enum IntegrationMethod {ExplicitEuler, SemiImplicitEuler};
        public IntegrationMethod integrationMethod;


        public float fixedTimeStep = 0.0025f;

        public float3 linearGravity;
        public float spericalGravityConstant;

   
        [SerializeField]
        private int particleCount;
        [SerializeField]
        private int constraintCount;

        // Systems
        private GravitySystem gravitySystem;
        private IntegrationSystem integrationSystem;
        private ConstraintSystem constraintSystem;

        // Data
        private GravityData gravityData;
        private ParticleData particleData;
        private ConstraintData constraintData;

        private void Start()
        {
            Time.fixedDeltaTime = fixedTimeStep;
            BuildSimulation(simulationPreset);
        }

        private void OnDisable()
        {
            gravityData.Dispose();
            particleData.Dispose();
            constraintData.Dispose();
        }

        private void Update()
        {
            // Update gravity to allow Inspector interaction 
            gravityData.linearGravity = linearGravity;
            gravityData.gravityConstant = spericalGravityConstant;

            Time.fixedDeltaTime = fixedTimeStep;


        }

        private void FixedUpdate()
        {

            JobHandle gravityHandle = gravitySystem.UpdateSystem(particleData, gravityData);

            JobHandle constraintHandle = constraintSystem.UpdateSystem(particleData, constraintData, gravityHandle);

            JobHandle integrationHandle = integrationSystem.UpdateSystem(particleData, fixedTimeStep, constraintHandle);

            integrationHandle.Complete();
        }

        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                for(int i = 0; i < particleCount; i++)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(particleData.positions[i], particleData.radius[i]);
                }

                for (int i = 0; i < constraintCount; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(particleData.positions[constraintData.particleIDs[i * 2]], particleData.positions[constraintData.particleIDs[i * 2 + 1]]);

                }

            }
        }

        private void BuildSimulation(SimulationPreset preset)
        {

            switch (integrationMethod)
            {
                case IntegrationMethod.ExplicitEuler:
                    integrationSystem = new ExplicitEulerSystem();
                    break;

                case IntegrationMethod.SemiImplicitEuler:
                    integrationSystem = new SemiImplicitEulerSystem();
                    break;
            }

            constraintSystem = new ConstraintSystem();

            switch (preset)
            {
                case SimulationPreset.Cube:
                    CubeDemo();
                    break;
                case SimulationPreset.SphericalGravity:
                    SphericalGravityDemo();
                    break;
                case SimulationPreset.TennisRacketTheorim:
                    TennisRacketTheorimDemo();
                    break;
               
               
            }
        }

        private void CubeDemo()
        {
            gravitySystem = new LinearGravitySystem();

            // Particle Setup
            particleData.Initialize(8);
            particleCount = particleData.count;

            // Setup particle positions
            particleData.positions[0] = new float3(-0.5f, 0.5f, 0.5f);
            particleData.positions[1] = new float3(0.5f, 0.5f, 0.5f);
            particleData.positions[2] = new float3(-0.5f, 0.5f, -0.5f);
            particleData.positions[3] = new float3(0.5f, 0.5f, -0.5f);

            particleData.positions[4] = new float3(-0.5f, -0.5f, 0.5f);
            particleData.positions[5] = new float3(0.5f, -0.5f, 0.5f);
            particleData.positions[6] = new float3(-0.5f, -0.5f, -0.5f);
            particleData.positions[7] = new float3(0.5f, -0.5f, -0.5f);


            for (int i = 0; i < particleCount; i++)
            {
                particleData.mass[i] = 5f;
                particleData.inverseMass[i] = 1 / particleData.mass[i];
                particleData.radius[i] = 0.1f;

            }

            
            particleData.mass[0] = 0f;
            particleData.inverseMass[0] = 0;


            // Constraint Setup
            constraintData.Initialize(24);
            constraintCount = constraintData.count;

            int constraintIndex = 0;
            // IDs setup
            // Vertical
            for (int i = 0; i < 4; i++)
            {
                constraintData.particleIDs[constraintIndex * 2    ] = i;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i + 4;
                constraintIndex++;
            }

            // Horizontal
            for (int i = 0; i < 2; i++)
            {
                // X-Axis
                constraintData.particleIDs[constraintIndex * 2    ] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 1;
                constraintIndex++;

                constraintData.particleIDs[constraintIndex * 2    ] = i * 4 + 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;


                // Z-Axis
                constraintData.particleIDs[constraintIndex * 2] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 2;
                constraintIndex++;

                constraintData.particleIDs[constraintIndex * 2] = i * 4 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;

            }

            // Cross Support 
            for (int i = 0; i < 2; i++)
            {

                // (Front View) \ 
                constraintData.particleIDs[constraintIndex * 2    ] = i * 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 2 + 5;
                constraintIndex++;

                // (Front View) /
                constraintData.particleIDs[constraintIndex * 2    ] = i * 2 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 2 + 4;
                constraintIndex++;


                // (Side View)  \
                constraintData.particleIDs[constraintIndex * 2] = i * 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 1 + 6;
                constraintIndex++;

                // (Side View)  /
                constraintData.particleIDs[constraintIndex * 2] = i * 1 + 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 1 + 4;
                constraintIndex++;


                // (Top View)  \
                constraintData.particleIDs[constraintIndex * 2] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;

                // (Top View)  /
                constraintData.particleIDs[constraintIndex * 2] = i * 4 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 2;
                constraintIndex++;

            }


            for (int i = 0; i < constraintCount; i++)
            {
                constraintData.stiffness[i] = 1000f;
                constraintData.damping[i] = 10f;
                constraintData.targetLength[i] = math.length(particleData.positions[constraintData.particleIDs[i * 2]] - particleData.positions[constraintData.particleIDs[i * 2 + 1]]);

                
            }
    
        }

        private void TennisRacketTheorimDemo()
        {
            gravitySystem = new LinearGravitySystem();

            // Particle Setup
            particleData.Initialize(9);
            particleCount = particleData.count;

            // Setup particle positions
            particleData.positions[0] = new float3(-0.5f, 1f, 0.5f);
            particleData.positions[1] = new float3(0.5f, 1f, 0.5f);
            particleData.positions[2] = new float3(-0.5f, 1f, -0.5f);
            particleData.positions[3] = new float3(0.5f, 1f, -0.5f);

            particleData.positions[4] = new float3(-0.5f, -1f, 0.5f);
            particleData.positions[5] = new float3(0.5f, -1f, 0.5f);
            particleData.positions[6] = new float3(-0.5f, -1f, -0.5f);
            particleData.positions[7] = new float3(0.5f, -1f, -0.5f);

            particleData.positions[8] = new float3(2f, 0f, 0f);



            for (int i = 0; i < particleCount; i++)
            {
                particleData.mass[i] = 5f;
                particleData.inverseMass[i] = 1 / particleData.mass[i];
                particleData.radius[i] = 0.1f;

            }
            particleData.mass[8] = 2.5f;
            particleData.inverseMass[8] = 1 / particleData.mass[8];

            float force = 100000f ;
            particleData.accumulatedForces[2] = new float3(0f, 0f, force);
            particleData.accumulatedForces[3] = new float3(0f, 0f, force);

            particleData.accumulatedForces[4] = new float3(0f, 0f, -force);
            particleData.accumulatedForces[5] = new float3(0f, 0f, -force);




            // Constraint Setup
            constraintData.Initialize(32);
            constraintCount = constraintData.count;

            int constraintIndex = 0;
            // IDs setup
            // Vertical
            for (int i = 0; i < 4; i++)
            {
                constraintData.particleIDs[constraintIndex * 2] = i;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i + 4;
                constraintIndex++;
            }

            // Horizontal
            for (int i = 0; i < 2; i++)
            {
                // X-Axis
                constraintData.particleIDs[constraintIndex * 2] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 1;
                constraintIndex++;

                constraintData.particleIDs[constraintIndex * 2] = i * 4 + 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;


                // Z-Axis
                constraintData.particleIDs[constraintIndex * 2] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 2;
                constraintIndex++;

                constraintData.particleIDs[constraintIndex * 2] = i * 4 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;

            }

            // Cross Support 
            for (int i = 0; i < 2; i++)
            {

                // (Front View) \ 
                constraintData.particleIDs[constraintIndex * 2] = i * 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 2 + 5;
                constraintIndex++;

                // (Front View) /
                constraintData.particleIDs[constraintIndex * 2] = i * 2 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 2 + 4;
                constraintIndex++;


                // (Side View)  \
                constraintData.particleIDs[constraintIndex * 2] = i * 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 1 + 6;
                constraintIndex++;

                // (Side View)  /
                constraintData.particleIDs[constraintIndex * 2] = i * 1 + 2;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 1 + 4;
                constraintIndex++;


                // (Top View)  \
                constraintData.particleIDs[constraintIndex * 2] = i * 4;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 3;
                constraintIndex++;

                // (Top View)  /
                constraintData.particleIDs[constraintIndex * 2] = i * 4 + 1;
                constraintData.particleIDs[constraintIndex * 2 + 1] = i * 4 + 2;
                constraintIndex++;

            }

            for (int i = 0; i < 8; i++)
            {
                constraintData.particleIDs[(i + constraintIndex) * 2] = i;
                constraintData.particleIDs[(i + constraintIndex) * 2 + 1] = 8;
                //constraintIndex++;
            }

            for (int i = 0; i < constraintCount; i++)
            {
                constraintData.stiffness[i] = 50000f;
                constraintData.damping[i] = 10f;
                constraintData.targetLength[i] = math.length(particleData.positions[constraintData.particleIDs[i * 2]] - particleData.positions[constraintData.particleIDs[i * 2 + 1]]);


            }

        }

        private void SphericalGravityDemo()
        {

         
            gravitySystem = new SphericalGravitySystem();

            constraintData.Initialize(0);
            // Particle Setup
            particleData.Initialize(2);
            particleCount = particleData.count;

            // Setup particle positions
            particleData.positions[0] = new float3(0, 0, 0);
            particleData.mass[0] = 100000f;
            particleData.inverseMass[0] = 1 / 100000f;
            particleData.radius[0] = particleData.mass[0] / 10000f;

            particleData.positions[1] = new float3(20f, 0f, 0f);
            particleData.velocities[1] = new float3(0f, 0f, 10f);

            particleData.mass[1] = 1;
            particleData.inverseMass[1] = 1 / 1f;
            particleData.radius[1] = particleData.mass[1] / 10f;

            // Gravity Data Setup
            gravityData.Initialize(0, 1, 1);

            gravityData.sphericalEmitterParticleIDs[0] = 0;
            gravityData.sphericalRecieverParticleIDs[0] = 1;



        }
    }
}
