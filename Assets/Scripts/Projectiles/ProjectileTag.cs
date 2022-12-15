using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ProjectileTag : IComponentData
{
    public float2 moveDir;
}