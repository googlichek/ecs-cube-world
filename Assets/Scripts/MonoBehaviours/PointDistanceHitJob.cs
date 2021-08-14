using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Game.Scripts
{
    public class PointDistanceHitJob : MonoBehaviour
    {
        [SerializeField] [Range(0, 100)]
        private float _distance = 10f;

        [SerializeField]
        private bool _shouldCollectAllHits = true;

        [SerializeField]
        private bool _shouldDrawSurfaceNormals = false;

        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        private NativeList<DistanceHit> _distanceHits;

        private PointDistanceInput _pointDistanceInput = new PointDistanceInput();

        public struct PointDistanceJob : IJob
        {
            public PointDistanceInput PointDistanceInput;
            public NativeList<DistanceHit> DistanceHits;
            public bool ShouldCollectAllHits;

            [ReadOnly] public PhysicsWorld World;

            public void Execute()
            {
                if (ShouldCollectAllHits)
                {
                    World.CalculateDistance(PointDistanceInput, ref DistanceHits);
                }
                else if (World.CalculateDistance(PointDistanceInput, out DistanceHit hit))
                {
                    DistanceHits.Add(hit);
                }
            }
        }

        void Awake()
        {
            _distanceHits = new NativeList<DistanceHit>(Allocator.Persistent);

            _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        }

        void LateUpdate()
        {
            _stepPhysicsWorld.FinalSimulationJobHandle.Complete();

            var origin = transform.position;

            _distanceHits.Clear();

            _pointDistanceInput = new PointDistanceInput()
            {
                Position = origin,
                MaxDistance = _distance,
                Filter = CollisionFilter.Default
            };

            var job = new PointDistanceJob
            {
                PointDistanceInput = _pointDistanceInput,
                DistanceHits = _distanceHits,
                ShouldCollectAllHits = _shouldCollectAllHits,
                World = _buildPhysicsWorld.PhysicsWorld
            };

            job.Schedule().Complete();

            //foreach (var hit in _distanceHits)
            //{
            //    ZombieGameDataManager.Instance.World.EntityManager.DestroyEntity(hit.Entity);
            //}
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            foreach (var hit in _distanceHits)
            {
                Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < _buildPhysicsWorld.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(_pointDistanceInput.Position, hit.Position - (float3) transform.position);
                Gizmos.DrawSphere(hit.Position, 0.05f);

                if (_shouldDrawSurfaceNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(hit.Position, hit.SurfaceNormal);
                }
            }
        }

        void OnDestroy()
        {
            _distanceHits.Dispose();
        }
    }
}
