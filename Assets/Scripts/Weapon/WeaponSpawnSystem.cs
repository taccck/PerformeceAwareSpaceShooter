using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;

public partial class WeaponSpawnSystem : SystemBase
{
    private EntityQuery weaponQuery;
    private BeginSimulationEntityCommandBufferSystem beginSimECB;
    private Entity prefab;

    protected override void OnCreate()
    {
        weaponQuery = GetEntityQuery(ComponentType.ReadWrite<WeaponTag>());
        beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<WeaponAuthoringComponent>().Prefab;
            return;
        }
        var settings = GetSingleton<GameSettingsComponent>();
        var commandBuffer = beginSimECB.CreateCommandBuffer();
        var count = weaponQuery.CalculateEntityCountWithoutFiltering();
        var weaponPrefab = prefab;

        Job
        .WithCode(() =>
        {
            for (int i = count; i < 10; ++i)
            {
                int sign = i % 2 == 0 ? 1 : -1;
                Translation trans = new() { Value = new(i * sign * 100 + 5, 0, 0) };
                Entity e = commandBuffer.Instantiate(weaponPrefab);
                commandBuffer.SetComponent(e, trans);
            }

        }).Schedule();

        beginSimECB.AddJobHandleForProducer(Dependency);
    }
}