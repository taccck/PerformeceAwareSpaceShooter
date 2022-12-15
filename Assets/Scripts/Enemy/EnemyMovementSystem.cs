using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;

public partial class EnemyMovementSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimEcb;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameSettingsComponent>();
        beginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = beginSimEcb.CreateCommandBuffer().AsParallelWriter();
        var gameSettings = GetSingleton<GameSettingsComponent>();
        float acc = gameSettings.weaponMaxSpeed * Time.DeltaTime * 3;

        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 14));
        float2 mousePos = mousePosition;

        Entities
        .WithAll<EnemyTag>()
        .ForEach((Entity entity, int entityInQueryIndex, ref PhysicsVelocity velocity, in Translation translation) =>
        {
            velocity.Linear = new(velocity.Linear.x, velocity.Linear.y, -translation.Value.z);

            if(math.abs(translation.Value.x) > 20 || math.abs(translation.Value.y) > 20)
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
        }).ScheduleParallel();
    }
}