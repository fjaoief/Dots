using Unity.Entities;
using UnityEngine;

public class SampleUnitAuthoring : MonoBehaviour
{
    public class SampleUnitAuthoringBaker : Baker<SampleUnitAuthoring>
    {
        public override void Bake(SampleUnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SampleUnitComponentData());
        }
    }
}

public struct SampleUnitComponentData : IComponentData
{
}
