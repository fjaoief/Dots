using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace _1.Scripts.DOTS.System
{
    public partial struct RuleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
            var query = SystemAPI.QueryBuilder().WithAll<MapTIleAuthoringComponentData>().WithAll<LocalTransform>().Build();
            var queryMask = query.GetEntityQueryMask();
            //블확실함. 나중에 시간 들여서 확실한 시스템 순서 정해야함. 성능적으로 문제 없으면 업데이트문에 던져버리면 됨
            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
  //          int x = 0;
            var dt = SystemAPI.Time.DeltaTime;
            
             foreach (var (transform, sampleUnit) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<SampleUnitComponentData>>())
             {
                 //각 엔티티의 길찾기 알고리즘은 여기에
                 //가장 가까운 적을 찾고 가까워지기 위한 빈칸을 목적 타일로 설정
                 //해당 칸으로 이동(인덱스 변경, 타일 공백 여부 변경)
                 //인덱스를 통해 주변 타일의 공백 여부 계산
                 
                 var index = sampleUnit.ValueRW.index;
                 if(transform.ValueRW.Position.y < 2)
                 {
                 transform.ValueRW.Position.y += dt * (float)0.1;
                 }
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