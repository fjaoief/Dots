using _1.Scripts.DOTS.Authoring_baker_;
using _1.Scripts.DOTS.Components___Tags;
using _1.Scripts.DOTS.System.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace _1.Scripts.DOTS.System
{
    public partial struct SampleUnitMoveSystem : ISystem
    {
        EntityQuery MovingTagQuery;
        EntityQuery unitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //
            state.RequireForUpdate<MapMakerComponentData>();
            MovingTagQuery = new EntityQueryBuilder(Allocator.Temp).WithAny<MovingTag>().Build(ref state);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            MapMakerComponentData mapMaker = SystemAPI.GetSingleton<MapMakerComponentData>();
            //var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            if(!MovingTagQuery.IsEmpty){
                Debug.Log("Moving");
                new MovementJob
            {
                Time = (float)SystemAPI.Time.DeltaTime,
                MapMaker = mapMaker
                //ECBWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
            state.Dependency.Complete();
            }
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}