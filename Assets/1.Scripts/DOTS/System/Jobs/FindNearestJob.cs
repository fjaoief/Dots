using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Mathematics;
using System.Collections.Generic;
using _1.Scripts.DOTS.Components___Tags;

namespace _1.Scripts.DOTS.System.Jobs
{
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct FindNearestJob : IJobEntity
    {
        [ReadOnly] public NativeArray<SampleUnitComponentData> SampleUnits;
        [ReadOnly] public MapMakerComponentData MapMaker;

        public void Execute(ref SampleUnitComponentData sampleUnit, EnabledRefRW<AttackTag> attackTag)
        {
            // 유닛들이 담긴 배열을 복사하고 복사한 배열 속 유닛들을 현재 유닛에서 가까운 순으로 정렬
            NativeArray<SampleUnitComponentData> sampleUnitsInOrder = new(SampleUnits.Length, Allocator.Temp);
            SampleUnits.CopyTo(sampleUnitsInOrder);
            sampleUnitsInOrder.Sort(new SampleUnitIdexComp{ OriginIndex = sampleUnit.index });
            
            bool found = false;
            for(int i=0; i<sampleUnitsInOrder.Length; i++){
                if(sampleUnitsInOrder[i].team != sampleUnit.team){  
                    sampleUnit.targetIndex = new int2(sampleUnitsInOrder[i].index);
                    found = true;
                    break;
                }
            }

            if (found){
                if(math.abs(sampleUnit.index.x - sampleUnit.targetIndex.x) + math.abs(sampleUnit.index.y - sampleUnit.targetIndex.y) == 1 ){
                    attackTag.ValueRW = true;
                }
            }
        }
    }

    public struct SampleUnitIdexComp : IComparer<SampleUnitComponentData>
    {
        [ReadOnly] public int2 OriginIndex;

        public readonly int Compare(SampleUnitComponentData a, SampleUnitComponentData b)
        {
            float distA = math.distancesq(OriginIndex, a.index);
            float distB = math.distancesq(OriginIndex, b.index);
            if (distA < distB)
                return -1;
            else if (distA > distB)
                return 1;
            else
                return 0;
        }
    }
}