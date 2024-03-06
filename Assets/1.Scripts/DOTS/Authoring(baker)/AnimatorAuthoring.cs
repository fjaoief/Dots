using Unity.Entities;
using UnityEngine;

namespace NSprites
{
    public class AnimatorAuthoring : MonoBehaviour
    {
        private class AnimationSettingsBaker : Baker<AnimatorAuthoring>
        {
            public override void Bake(AnimatorAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new AnimationSettings
                {
                    IdleHash = Animator.StringToHash("idle"),
                    WalkHash = Animator.StringToHash("walk"),
                    AttackHash = Animator.StringToHash("attack")
                });
            }
        }
    }
}