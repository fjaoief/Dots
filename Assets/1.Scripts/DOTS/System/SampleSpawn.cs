using _1.Scripts.DOTS.Authoring_baker_;
using _1.Scripts.DOTS.Components___Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _1.Scripts.DOTS.System
{
    public partial struct SampleSpawn : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SampleSpawnData>();
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            
            var SampleSpawner = SystemAPI.GetSingleton<SampleSpawnData>();
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var SampleUnits = new NativeArray<Entity>(SampleSpawner.number, Allocator.Temp);
            ecb.Instantiate(SampleSpawner.SampleEntityPrefab, SampleUnits);

            var query = SystemAPI.QueryBuilder().WithAll<SampleUnitComponentData>().WithAll<LocalTransform>().WithAspect<SampleUnitAspect>().Build();
            var queryMask = query.GetEntityQueryMask();
            int x = 0;
            int y = 0;
            
            foreach (var SampleUnit in SampleUnits)
            {
                ecb.SetComponentForLinkedEntityGroup(SampleUnit, queryMask, new SampleUnitComponentData
                {
                    index = new int2(x, y),
                    hp = 3,
                    movementspeed = 0.1f,
                    dmg = 1,
                    team = x%2
                });
                ecb.SetComponentEnabled<MovingTag>(SampleUnit, false);
                ecb.SetComponentEnabled<AttackTag>(SampleUnit, false);
                ecb.SetComponentEnabled<LazyTag>(SampleUnit, false);
                
                ecb.SetComponent(SampleUnit, new LocalTransform()
                {
                    Position = new float3 (x, y, 0),
                    Scale = 5
                });
                if (x != 100)
                {
                    x++;
                }
                else
                {
                    x = 0;
                    y++;
                }
                
            }
            ecb.Playback(state.EntityManager);

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}