using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;

public partial class WeaponMovementSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        var gameSettings = GetSingleton<GameSettingsComponent>();
        float acc = gameSettings.weaponMaxSpeed * Time.DeltaTime * 3;

        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 14));
        float2 mousePos = mousePosition;

        Entities
        .WithAll<WeaponTag>()
        .ForEach((Entity entity, ref PhysicsVelocity velocity, in Translation translation) =>
        {
            float moveWidth = mousePos.x - translation.Value.x;
            float moveHeight = mousePos.y - translation.Value.y;
            float distToMouse = math.sqrt(moveWidth * moveWidth + moveHeight * moveHeight);
            if (distToMouse < 1.25f)
                return;

            float2 modVel = new(velocity.Linear.x, velocity.Linear.y);
            float velMag = math.sqrt(modVel.x * modVel.x + modVel.y * modVel.y);
            float2 moveDir = new(x: moveWidth / distToMouse, y: moveHeight / distToMouse);
            float2 velDir = new(x: modVel.x / velMag, y: modVel.y / velMag);

            //reduce speed when at max to allow turning
            if (velMag + acc > gameSettings.weaponMaxSpeed)
            {
                float turnAmount = moveDir.x * velDir.x + moveDir.y * velDir.y;
                turnAmount = math.clamp(1 + turnAmount, 0f, 1f);
                modVel -= velDir * (acc * turnAmount);
            }

            modVel += moveDir * acc;
            velocity.Linear = new(modVel.x, modVel.y, -translation.Value.z);
        }).ScheduleParallel();
    }
}