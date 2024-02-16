using Unity.Entities;
using UnityEngine;

namespace _1.Scripts.DOTS.Authoring_baker_
{
    public class MapMakerAuthoring : MonoBehaviour
    {
        public GameObject MapPrefab;
        //타일맵 가로 세로 갯수
        public int number;
        //맵 한 칸 넓이
        public float width;
        private class MapMakerBaker : Baker<MapMakerAuthoring>
        {
            public override void Bake(MapMakerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MapMakerComponentData
                {
                    MapPrefab = GetEntity(authoring.MapPrefab, TransformUsageFlags.Dynamic),
                    number = authoring.number,
                    width = authoring.width
                });
            }
        }
    }

    public struct MapMakerComponentData : IComponentData
    {
        public Entity MapPrefab;
        //타일맵 가로 세로 갯수
        public int number;
        //맵 한 칸 넓이
        public float width;
    }
}