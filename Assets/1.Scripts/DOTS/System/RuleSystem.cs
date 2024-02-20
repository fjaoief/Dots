using _1.Scripts.DOTS.System.Jobs;
using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using _1.Scripts.DOTS.Components___Tags;

namespace _1.Scripts.DOTS.System
{
    public partial struct RuleSystem : ISystem
    {
        EntityQuery behaviorTagQuery;
        EntityQuery unitQuery;
    
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
            behaviorTagQuery = new EntityQueryBuilder(Allocator.Temp).WithAny<AttackTag, MovingTag, LazyTag>().Build(ref state);
            unitQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SampleUnitComponentData>().Build(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;

            //Attack Tag, Moving Tag, Lazy Tag 중 하나라도 가진 엔티티가 없을 경우
            if(behaviorTagQuery.IsEmpty){
                //행동을 결정해야 함(공격할 타겟 찾기 or 이동할 위치 찾기)
                //Debug.Log("FIND JOB START");
                NativeArray<SampleUnitComponentData> sampleUnits = unitQuery.ToComponentDataArray<SampleUnitComponentData>(Allocator.TempJob);
                MapMakerComponentData mapMaker = SystemAPI.GetSingleton<MapMakerComponentData>();
                FindDestIndexJob findDestIndexJob = new(){
                    MapMaker = mapMaker,
                    SampleUnits = sampleUnits,
                };
                findDestIndexJob.ScheduleParallel();
                state.Dependency.Complete();
                sampleUnits.Dispose();
            }
            else{

            }

  //          int x = 0;
            // foreach (var (transform, sampleUnit) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<SampleUnitComponentData>>())
             {
                 //각 엔티티의 길찾기 알고리즘은 여기에
                 //가장 가까운 적을 찾고 가까워지기 위한 빈칸을 목적 타일로 설정
                 //해당 칸으로 이동(인덱스 변경, 타일 공백 여부 변경)
                 //인덱스를 통해 주변 타일의 공백 여부 계산
                 
                 /*var index = sampleUnit.ValueRW.index;
                 if(transform.ValueRW.Position.y < 2)
                 {
                 transform.ValueRW.Position.y += dt * (float)0.1;
                 }*/
                 
                 //이동. 나중에 job으로 실행하는 것으로 변경해야 함
                 
                 
                 /*
                  * 공격+피격
                  */
                 
                 
                 
                 /* 순서 설정용 코드
                sampleUnit.ValueRW.order = x;
                 x++;*/
             }
//             x = 0;
            
             
             
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}