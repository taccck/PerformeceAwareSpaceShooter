using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class ProjectileMovementSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimEcb;
    protected override void OnCreate()
    {
        beginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = beginSimEcb.CreateCommandBuffer().AsParallelWriter();
        float deltaTime = Time.DeltaTime;

        Entities
        .WithAll<ProjectileTag>()
        .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, in ProjectileTag tag) =>
        {
            float2 moveDist = tag.moveDir * deltaTime * 2f;
            translation.Value += new float3(moveDist.x, moveDist.y, z: 0f);
            if (math.abs(translation.Value.x) > 20 || math.abs(translation.Value.y) > 20)
            {
                commandBuffer.DestroyEntity(entityInQueryIndex, entity);
                return;
            }

            float2 pos = new(translation.Value.x, translation.Value.y);

            //Entities
            //.WithAll<WeaponTag>()
            //.ForEach((Entity entity, in Translation weaponTrans) =>
            //{
            //    float w = math.abs(weaponTrans.Value.x - pos.x);
            //    float h = math.abs(weaponTrans.Value.y - pos.y);
            //    if (w * w + h * h >= 1f)
            //        commandBuffer.DestroyEntity(entityInQueryIndex, entity);
            //}).ScheduleParallel();

        }).ScheduleParallel();
    }
}