using Unity.Entities;

[GenerateAuthoringComponent]
public struct WeaponAuthoringComponent : IComponentData
{
    public Entity Prefab;
}