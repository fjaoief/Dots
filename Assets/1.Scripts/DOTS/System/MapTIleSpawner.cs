using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _1.Scripts.DOTS.System
{
    public partial struct MapTileSpawner : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapMakerComponentData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //1번만 돌아야 해서
            state.Enabled = false;

            var MapMaker = SystemAPI.GetSingleton<MapMakerComponentData>();

            // 타일 생성
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var tiles = new NativeArray<Entity>(MapMaker.number*MapMaker.number, Allocator.Temp);
            ecb.Instantiate(MapMaker.MapPrefab, tiles);
            
            // 타일 정렬
            var query = SystemAPI.QueryBuilder().WithAll<MapTileAuthoringComponentData>().WithAll<LocalTransform>().Build();
            var queryMask = query.GetEntityQueryMask();
            int x = 0;
            int y = 0;
            
            
            foreach (var tile in tiles)
            {
                ecb.SetComponentForLinkedEntityGroup(tile, queryMask, new MapTileAuthoringComponentData
                {
                    index = new int2(x, y),
                    soldier = 0,
                });
                ecb.SetComponent(tile, new LocalTransform()
                {
                    Position = new float3 (x*MapMaker.width, y*MapMaker.width, 0),
                    Scale = 1
                });
                
                if (x != MapMaker.number-1)
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