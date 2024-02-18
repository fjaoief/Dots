using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using Unity.Mathematics;

public class SampleUnitAuthoring : MonoBehaviour
{
    
    public float movementspeed;
    public int hp;
    public int dmg;
    public class SampleUnitAuthoringBaker : Baker<SampleUnitAuthoring>
    {
        public override void Bake(SampleUnitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SampleUnitComponentData{
                movementspeed = authoring.movementspeed, 
                hp = authoring.hp, 
                dmg = authoring.dmg,
                team = 0,
                });
        }
    }
}

public struct SampleUnitComponentData : IComponentData
{
    public int order; //순서
    public int2 index; // 타일
    public int2 destIndex;
    public float movementspeed; //이동 속도
    public int hp; 
    public int dmg;
    public int team;
}
