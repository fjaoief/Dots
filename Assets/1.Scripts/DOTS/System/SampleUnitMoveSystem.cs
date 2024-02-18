using _1.Scripts.DOTS.System.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace _1.Scripts.DOTS.System
{
    public partial struct SampleUnitMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            new MovementJob
            {
                Time = (float)SystemAPI.Time.ElapsedTime,
                ECBWriter = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}