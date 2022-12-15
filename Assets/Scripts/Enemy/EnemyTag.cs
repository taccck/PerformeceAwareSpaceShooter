using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemyTag : IComponentData
{
    public float shootTimer;
}