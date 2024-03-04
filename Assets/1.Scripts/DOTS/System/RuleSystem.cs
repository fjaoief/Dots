using _1.Scripts.DOTS.System.Jobs;
using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using _1.Scripts.DOTS.Components___Tags;
using Unity.VisualScripting;

namespace _1.Scripts.DOTS.System
{
    public partial struct RuleSystem : ISystem
    {
        EntityQuery behaviorTagQuery;
        EntityQuery unitQuery;
        EntityQuery tileQuery;
        EntityQuery spawnerQuery;

        Entity spawnerEntity;

        ComponentLookup<SampleUnitComponentData> sampleUnitLookup;
        ComponentLookup<StartPause> startLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            /*
             var query = SystemAPI.QueryBuilder().WithAll<MapTIleAuthoringComponentData>().WithAll<LocalTransform>().Build();
             var queryMask = query.GetEntityQueryMask();
             //블확실함. 나중에 시간 들여서 확실한 시스템 순서 정해야함. 성능적으로 문제 없으면 업데이트문에 던져버리면 됨
             state.RequireForUpdate(query);
             */
            state.RequireForUpdate<MapMakerComponentData>();
            state.RequireForUpdate<SampleSpawnData>();
            behaviorTagQuery = new EntityQueryBuilder(Allocator.Temp).WithAny<AttackTag, MovingTag, LazyTag>().Build(ref state);
            unitQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SampleUnitComponentData>().Build(ref state);
            tileQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<MapTileAuthoringComponentData>().Build(ref state);
            spawnerQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<StartPause>().Build(ref state);
            sampleUnitLookup = state.GetComponentLookup<SampleUnitComponentData>(true);
            startLookup = state.GetComponentLookup<StartPause>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            sampleUnitLookup.Update(ref state);
            startLookup.Update(ref state);

            //spawnerEntity = SystemAPI.GetSingletonEntity<StartPause>();

            // NativeArray<Entity> spawner = spawnerQuery.ToEntityArray(Allocator.TempJob);
            // spawnerEntity = spawner[0];

            // if (!startLookup.IsComponentEnabled(spawnerEntity))
            // {
            //     return;
            // }



            //Attack Tag, Moving Tag, Lazy Tag 중 하나라도 가진 엔티티가 없을 경우
            if (behaviorTagQuery.IsEmpty)
            {
                MapMakerComponentData mapMaker = SystemAPI.GetSingleton<MapMakerComponentData>();
                //Debug.Log("Find behavior");
                //타일 배열
                //인덱스가 (3, 5)인 타일 = tiles[3 + 5 * mapMaker.number]
                NativeArray<MapTileAuthoringComponentData> tiles = tileQuery.ToComponentDataArray<MapTileAuthoringComponentData>(Allocator.TempJob);

                //행동을 결정해야 함(공격할 타겟 찾기 or 이동할 위치 찾기)
                //Debug.Log("FIND JOB START");
                NativeArray<Entity> sampleUnits = unitQuery.ToEntityArray(Allocator.TempJob);

                FindNearestJob findNearestJob = new()
                {
                    MapMaker = mapMaker,
                    SampleUnits = sampleUnits,
                    SampleUnitComponents = sampleUnitLookup,
                };

                findNearestJob.ScheduleParallel();
                state.Dependency.Complete();

                //체력 감소
                foreach (var (unit, target) in SystemAPI.Query<RefRO<SampleUnitComponentData>, RefRW<TargetEntityData>>().WithAll<AttackTag>())
                {
                    SystemAPI.GetComponentRW<SampleUnitComponentData>(target.ValueRW.targetEntity).ValueRW.hp -= unit.ValueRO.dmg;
                }

                //체력 0인 유닛 파괴
                EntityCommandBuffer ecb = new(Allocator.Temp);
                foreach (var (unit, entity) in SystemAPI.Query<RefRW<SampleUnitComponentData>>().WithEntityAccess())
                {
                    if (unit.ValueRW.hp <= 0)
                    {
                        MapTileAuthoringComponentData tile = tiles[unit.ValueRO.index.x + unit.ValueRO.index.y * mapMaker.number];
                        tile.soldier = 0;
                        tiles[unit.ValueRO.index.x + unit.ValueRO.index.y * mapMaker.number] = tile;
                        ecb.DestroyEntity(entity);
                    }
                }
                ecb.Playback(state.EntityManager);
                sampleUnits.Dispose();

                //Attack을 하지 않은 유닛들이 순차적으로 이동
                for (int i = 0; i < 2; i++)
                {
                    foreach (var (unit, target, movingTag, lazyTag) in SystemAPI.Query<RefRW<SampleUnitComponentData>, RefRO<TargetEntityData>, EnabledRefRW<MovingTag>, EnabledRefRW<LazyTag>>()
                        .WithDisabled<AttackTag>().WithDisabled<MovingTag>().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                    {
                        if (!state.EntityManager.Exists(target.ValueRO.targetEntity)
                            || !SystemAPI.HasComponent<SampleUnitComponentData>(target.ValueRO.targetEntity)) continue;
                        int2 targetIndex = SystemAPI.GetComponentRO<SampleUnitComponentData>(target.ValueRO.targetEntity).ValueRO.index;
                        int dx = (int)math.sign(targetIndex.x - unit.ValueRO.index.x);
                        int dy = (int)math.sign(targetIndex.y - unit.ValueRO.index.y);
                        int2 unitIndex = unit.ValueRO.index;
                        MapTileAuthoringComponentData currentTile = tiles[unitIndex.x + unitIndex.y * mapMaker.number];
                        if (dx != 0)
                        {
                            MapTileAuthoringComponentData nextTile = tiles[unitIndex.x + dx + unitIndex.y * mapMaker.number];
                            if (nextTile.soldier == 0)
                            {
                                unit.ValueRW.destIndex = unitIndex + new int2(dx, 0);
                                currentTile.soldier = 0;
                                tiles[unitIndex.x + unitIndex.y * mapMaker.number] = currentTile;
                                nextTile.soldier = 1;
                                tiles[nextTile.index.x + nextTile.index.y * mapMaker.number] = nextTile;
                                lazyTag.ValueRW = false;
                                movingTag.ValueRW = true;
                                continue;
                            }
                        }
                        if (dy != 0)
                        {
                            MapTileAuthoringComponentData nextTile = tiles[unitIndex.x + (unitIndex.y + dy) * mapMaker.number];
                            if (nextTile.soldier == 0)
                            {
                                unit.ValueRW.destIndex = unitIndex + new int2(0, dy);
                                currentTile.soldier = 0;
                                tiles[unitIndex.x + unitIndex.y * mapMaker.number] = currentTile;
                                nextTile.soldier = 1;
                                tiles[nextTile.index.x + nextTile.index.y * mapMaker.number] = nextTile;
                                lazyTag.ValueRW = false;
                                movingTag.ValueRW = true;
                                continue;
                            }
                        }
                        if (i == 0) lazyTag.ValueRW = true;
                        else lazyTag.ValueRW = false;
                    }
                }

                tileQuery.CopyFromComponentDataArray(tiles);
                tiles.Dispose();

                //테스트용(현재 Attack Tag를 제거하는 로직이 따로 없기 때문에 여기서 바로 제거)
                foreach (var attackTag in SystemAPI.Query<EnabledRefRW<AttackTag>>())
                {
                    attackTag.ValueRW = false;
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}