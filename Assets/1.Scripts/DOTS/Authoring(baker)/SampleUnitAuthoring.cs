using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using Unity.Mathematics;

public class SampleUnitAuthoring : MonoBehaviour
{
    
    public int movementspeed;
    public int hp;
    public int dmg;
    public class SampleUnitAuthoringBaker : Baker<SampleUnitAuthoring>
    {
        public override void Bake(SampleUnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SampleUnitComponentData{movementspeed = authoring.movementspeed, hp = authoring.hp, dmg = authoring.dmg});
        }
    }
}

public struct SampleUnitComponentData : IComponentData
{
    public int2 index;
    public int movementspeed;
    public int hp;
    public int dmg;
}
