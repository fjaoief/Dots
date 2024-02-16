using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

namespace _1.Scripts.DOTS.Authoring_baker_
{
    public class MapTIleAuthoringAuthoring : MonoBehaviour
    {
        private class MapTIleAuthoringBaker : Baker<MapTIleAuthoringAuthoring>
        {
            public override void Bake(MapTIleAuthoringAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MapTIleAuthoringComponentData());
            }
        }
    }

    public struct MapTIleAuthoringComponentData : IComponentData
    { 
        public int2 index;
        public int soldier;
        // 0이면 없음 1이면 빨강팀 2면 파랑팀 있음
    }
}