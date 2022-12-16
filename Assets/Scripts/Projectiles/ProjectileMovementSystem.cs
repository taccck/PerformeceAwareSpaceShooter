using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ProjectileMovementSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimEcb;

    protected override void OnCreate()
    {
        beginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        GameSettingsComponent gameSettings = GetSingleton<GameSettingsComponent>();
        EntityCommandBuffer.ParallelWriter commandBuffer = beginSimEcb.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;

        NativeList<float3> collidablePos = new NativeList<float3>(11, Allocator.TempJob);

        Entities
            .WithAll<WeaponTag>()
            .ForEach((in Translation trans) => { collidablePos.Add(new float3(trans.Value.x, trans.Value.y, .25f)); })
            .Run();


        Entities
            .WithAll<PlayerTag>()
            .ForEach((in Translation trans) => { collidablePos.Add(new float3(trans.Value.x, trans.Value.y, 1f)); })
            .Run();

        Entities
            .WithAll<ProjectileTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in ProjectileTag tag) =>
            {
                var moveDist = tag.moveDir * deltaTime * gameSettings.bulletVelocity;
                translation.Value += new float3(moveDist.x, moveDist.y, 0f);
                if (math.abs(translation.Value.x) > 20 || math.abs(translation.Value.y) > 20)
                {
                    commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                    return;
                }

                float2 pos = new(translation.Value.x, translation.Value.y);
                for (var i = 0; i < collidablePos.Length; i++)
                {
                    float w = pos.x - collidablePos[i].x;
                    float h = pos.y - collidablePos[i].y;
                    if (w * w + h * h <= collidablePos[i].z)
                    {
                        commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                        return;
                    }
                }
            }).Run();

        collidablePos.Dispose();
    }
}