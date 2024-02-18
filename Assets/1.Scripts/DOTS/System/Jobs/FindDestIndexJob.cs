using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using _1.Scripts.DOTS.Authoring_baker_;
using Unity.Mathematics;
using System.Collections.Generic;

namespace _1.Scripts.DOTS.System.Jobs
{
    [BurstCompile]
    public partial struct FindDestIndexJob : IJobEntity
    {
        [ReadOnly] public NativeArray<SampleUnitComponentData> SampleUnits;
        [ReadOnly] public MapMakerComponentData MapMaker;

        public void Execute(ref SampleUnitComponentData sampleUnit)
        {
            NativeArray<SampleUnitComponentData> sampleUnitsInOrder = new(SampleUnits.Length, Allocator.Temp);
            SampleUnits.CopyTo(sampleUnitsInOrder);
            sampleUnitsInOrder.Sort(new SampleUnitIdexComp{ OriginIndex = sampleUnit.index });
            
            sampleUnit.destIndex = new int2(sampleUnit.index);
            for(int i=0; i<sampleUnitsInOrder.Length; i++){
                if(sampleUnitsInOrder[i].team != sampleUnit.team){
                    sampleUnit.destIndex = sampleUnitsInOrder[i].index;
                    int dx = (int)math.sign(sampleUnitsInOrder[i].index.x - sampleUnit.index.x);
                    int dy =  (int)math.sign(sampleUnitsInOrder[i].index.y - sampleUnit.index.y);
                    sampleUnit.destIndex = sampleUnit.index + new int2(dx, dy);
                    break;
                }
            }

            sampleUnitsInOrder.Dispose();
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