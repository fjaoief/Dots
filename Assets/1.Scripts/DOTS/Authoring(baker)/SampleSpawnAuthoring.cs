using Unity.Entities;
using UnityEngine;

namespace _1.Scripts.DOTS.Authoring_baker_
{
    public class SampleSpawnAuthoring : MonoBehaviour
    {
        public GameObject sampleUnit;
        public int number;
        public class SampleSpawnAuthoringBaker : Baker<SampleSpawnAuthoring>
        {
            public override void Bake(SampleSpawnAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SampleSpawnData()
                {
                    SampleEntityPrefab = GetEntity(authoring.sampleUnit, TransformUsageFlags.Dynamic),
                    number = authoring.number
                });
            }
        }
    }
    
    public struct SampleSpawnData : IComponentData
    {
        public Entity SampleEntityPrefab;
        //타일맵 가로 세로 갯수
        public int number;
    }
}