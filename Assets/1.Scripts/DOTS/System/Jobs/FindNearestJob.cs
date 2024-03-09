using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Mathematics;
using System.Collections.Generic;
using _1.Scripts.DOTS.Components___Tags;

namespace _1.Scripts.DOTS.System.Jobs
{
    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct FindNearestJob : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> SampleUnits;
        [ReadOnly] public ComponentLookup<SampleUnitComponentData> SampleUnitComponents;
        [ReadOnly] public MapMakerComponentData MapMaker;

        public void Execute(in SampleUnitComponentData currentUnit, EnabledRefRW<AttackTag> attackTag, ref TargetEntityData target)
        {
            bool found = false;
            int2 targetIndex = new(0,0);
            float dist = math.INFINITY;

            //가장 가까운 적 유닛 찾기
            for(int i=0; i<SampleUnits.Length; i++){
                if(SampleUnitComponents[SampleUnits[i]].team != currentUnit.team){
                    float newDist = math.distancesq(currentUnit.index, SampleUnitComponents[SampleUnits[i]].index);
                    if(newDist < dist){
                        dist = newDist;
                        targetIndex = SampleUnitComponents[SampleUnits[i]].index;
                        target.targetEntity = SampleUnits[i];
                        found = true;
                    }
                }
            }

            if (found){
                //찾은 타겟이 범위 안에 있을 시 Attack Tag 활성화
                if(math.abs(currentUnit.index.x - targetIndex.x) + math.abs(currentUnit.index.y - targetIndex.y) <= 1 ){
                    attackTag.ValueRW = true;
                }
            }
        }
    }
}