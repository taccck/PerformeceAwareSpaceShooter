using Unity.Entities;
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
    }

    protected override void OnUpdate()
    {
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<ProjectileAuthoringComponent>().Prefab;
            return;
        }
        EntityCommandBuffer commandBuffer = beginSimECB.CreateCommandBuffer();
        Entity projectilePrefab = prefab;
        float deltaTime = Time.DeltaTime;
        Vector2 mousePosition = Input.mousePosition;
        if (Camera.main == null) return;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 14));
        float2 mousePos = mousePosition;

        Entities
        .ForEach((ref EnemyTag enemyTag, in Translation trans) =>
        {
            enemyTag.shootTimer += deltaTime;
            if (enemyTag.shootTimer < 1) return;
            enemyTag.shootTimer = 0;

            float w = mousePos.x - trans.Value.x;
            float h = mousePos.y - trans.Value.y;
            float moveDirMag = math.sqrt(w * w + h * h);
            ProjectileTag tag = new (){ moveDir = new(w / moveDirMag, h / moveDirMag) };
            Entity e = commandBuffer.Instantiate(projectilePrefab);
            commandBuffer.SetComponent(e, trans);
            commandBuffer.SetComponent(e, tag);

        }).Schedule();

        beginSimECB.AddJobHandleForProducer(Dependency);
    }
}