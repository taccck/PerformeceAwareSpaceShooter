using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public partial class ProjectileSpawnSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimECB;
    private Entity prefab;

    protected override void OnCreate()
    {
        beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSettingsComponent>();

    }

    protected override void OnUpdate()
    {
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<ProjectileAuthoringComponent>().Prefab;
            return;
        }
        var settings = GetSingleton<GameSettingsComponent>();
        var commandBuffer = beginSimECB.CreateCommandBuffer();
        var projectilePrefab = prefab;
        float deltaTime = Time.DeltaTime;
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 14));
        float2 mousePos = mousePosition;

        Entities
        .ForEach((Entity entity, ref EnemyTag enemyTag, in Translation trans) =>
        {
            enemyTag.shootTimer += deltaTime;
            if (enemyTag.shootTimer < 1) return;
            enemyTag.shootTimer = 0;

            ProjectileTag tag = new() { moveDir = new(mousePos.x - trans.Value.x, mousePos.y - trans.Value.y) };
            Entity e = commandBuffer.Instantiate(projectilePrefab);
            commandBuffer.SetComponent(e, trans);
            commandBuffer.SetComponent(e, tag);

        }).Schedule();

        beginSimECB.AddJobHandleForProducer(Dependency);
    }
}