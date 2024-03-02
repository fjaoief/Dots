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

        public void Execute(in SampleUnitComponentData sampleUnit, EnabledRefRW<AttackTag> attackTag, ref TargetEntityData target)
        {
            // 유닛들이 담긴 배열을 복사하고 복사한 배열 속 유닛들을 현재 유닛에서 가까운 순으로 정렬
            NativeArray<Entity> sampleUnitsInOrder = new(SampleUnits.Length, Allocator.Temp);
            SampleUnits.CopyTo(sampleUnitsInOrder);
            sampleUnitsInOrder.Sort(new SampleUnitIdexComp{ OriginIndex = sampleUnit.index, SampleUnitComponentLookup = SampleUnitComponents });
            
            bool found = false;
            int2 targetIndex = new(0,0);
            for(int i=0; i<sampleUnitsInOrder.Length; i++){
                if(SampleUnitComponents[sampleUnitsInOrder[i]].team != sampleUnit.team){  
                    targetIndex = new int2(SampleUnitComponents[sampleUnitsInOrder[i]].index);
                    target.targetEntity = sampleUnitsInOrder[i];
                    found = true;
                    break;
                }
            }

            if (found){
                //찾은 타겟이 범위 안에 있을 시 Attack Tag 활성화
                if(math.abs(sampleUnit.index.x - targetIndex.x) + math.abs(sampleUnit.index.y - targetIndex.y) <= 1 ){
                    attackTag.ValueRW = true;
                }
            }
        }
    }

    public struct SampleUnitIdexComp : IComparer<Entity>
    {
        [ReadOnly] public ComponentLookup<SampleUnitComponentData> SampleUnitComponentLookup;
        [ReadOnly] public int2 OriginIndex;

        public readonly int Compare(Entity a, Entity b)
        {
            float distA = math.distancesq(OriginIndex, SampleUnitComponentLookup[a].index);
            float distB = math.distancesq(OriginIndex, SampleUnitComponentLookup[b].index);
            if (distA < distB)
                return -1;
            else if (distA > distB)
                return 1;
            else
                return 0;
        }
    }
}