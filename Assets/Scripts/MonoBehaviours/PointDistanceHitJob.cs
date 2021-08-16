using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
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

        private Entity _colsestEntity = Entity.Null;

        private Vector3 _lockOnPosition = Vector3.zero;

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

            if (!ZombieGameDataManager.Instance.World.EntityManager.Exists(_colsestEntity))
            {
                var closestDistance = Mathf.Infinity;

                foreach (var hit in _distanceHits)
                {
                    Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < _buildPhysicsWorld.PhysicsWorld.NumBodies);
                    Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                    var entity = _buildPhysicsWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                    var hasComponent =
                        ZombieGameDataManager.Instance.World.EntityManager.HasComponent<ZombieData>(entity);

                    if (closestDistance > hit.Distance && hasComponent)
                    {
                        closestDistance = hit.Distance;
                        _colsestEntity = entity;
                        _lockOnPosition = ZombieGameDataManager.Instance.World.EntityManager
                            .GetComponentData<Translation>(entity).Value;
                    }

                }
            }

            transform.LookAt(_lockOnPosition);
            ZombieGameDataManager.Instance.World.EntityManager.DestroyEntity(_colsestEntity);
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
