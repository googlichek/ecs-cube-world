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
    public class RaycastHitJob : MonoBehaviour
    {
        [SerializeField] [Range(0, 100)]
        private float _distance = 10f;

        [SerializeField]
        private bool _shouldCollectAllHits = false;

        [SerializeField]
        private bool _shouldDrawSurfaceNormals = false;

        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        private NativeList<Unity.Physics.RaycastHit> _raycastHits;
        private NativeList<DistanceHit> _distancetHits;

        private RaycastInput _raycastInput = new RaycastInput();

        public struct RaycastJob : IJob
        {
            public RaycastInput RaycastInput;
            public NativeList<Unity.Physics.RaycastHit> RaycastHits;
            public bool ShouldCollectAllHits;

            [ReadOnly] public PhysicsWorld World;

            public void Execute()
            {
                if (ShouldCollectAllHits)
                {
                    World.CastRay(RaycastInput, ref RaycastHits);
                }
                else if (World.CastRay(RaycastInput, out Unity.Physics.RaycastHit hit))
                {
                    RaycastHits.Add(hit);
                }
            }
        }

        void Awake()
        {
            _raycastHits = new NativeList<Unity.Physics.RaycastHit>(Allocator.Persistent);
            _distancetHits = new NativeList<DistanceHit>(Allocator.Persistent);

            _buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<StepPhysicsWorld>();
        }

        void LateUpdate()
        {
            _stepPhysicsWorld.FinalSimulationJobHandle.Complete();

            var origin = transform.position;
            var rayDiraction = transform.forward * _distance;

            _raycastHits.Clear();
            _distancetHits.Clear();

            _raycastInput = new RaycastInput()
            {
                Start = origin,
                End = origin + rayDiraction,
                Filter = CollisionFilter.Default
            };

            var job = new RaycastJob
            {
                RaycastInput = _raycastInput,
                RaycastHits = _raycastHits,
                ShouldCollectAllHits = _shouldCollectAllHits,
                World = _buildPhysicsWorld.PhysicsWorld
            };

            job.Schedule().Complete();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_raycastInput.Start, _raycastInput.End - _raycastInput.Start);

            foreach (var hit in _raycastHits)
            {
                Assert.IsTrue(hit.RigidBodyIndex >= 0 && hit.RigidBodyIndex < _buildPhysicsWorld.PhysicsWorld.NumBodies);
                Assert.IsTrue(math.abs(math.lengthsq(hit.SurfaceNormal) - 1.0f) < 0.01f);

                Gizmos.color = Color.magenta;
                Gizmos.DrawRay(_raycastInput.Start, hit.Position - _raycastInput.Start);
                Gizmos.DrawSphere(hit.Position, 0.02f);

                if (_shouldDrawSurfaceNormals)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(hit.Position, hit.SurfaceNormal);
                }
            }
        }

        void OnDestroy()
        {
            _raycastHits.Dispose();
            _distancetHits.Dispose();
        }
    }
}
