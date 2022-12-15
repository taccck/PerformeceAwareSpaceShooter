using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;

public partial class PlayerMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        var gameSettings = GetSingleton<GameSettingsComponent>();
        var deltaTime = Time.DeltaTime;

        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 14));
        float2 mousePos = mousePosition;

        float4 identity = new(0, 0, 0, 1);

        Entities
        .WithAll<PlayerTag>()
        .ForEach((Entity entity, ref PhysicsVelocity velocity, ref Rotation rotation, in Translation translation) =>
        {
            float width = mousePos.x - translation.Value.x;
            float height = mousePos.y - translation.Value.y;
            float dist = math.sqrt(width * width + height * height);
            float speed = math.lerp(0, gameSettings.playerVelocity, dist);
            float2 newVel = new(x: width / dist, y: height / dist);
            newVel *= speed * deltaTime;
            velocity.Linear = new(newVel.x, newVel.y, -translation.Value.z);

            rotation.Value.value = identity;
        }).ScheduleParallel();
    }
}