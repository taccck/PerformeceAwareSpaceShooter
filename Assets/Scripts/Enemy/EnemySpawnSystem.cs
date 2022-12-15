using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Physics;
using Unity.Mathematics;

public partial class EnemySpawnSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem beginSimECB;
    private Entity prefab;
    private Random random;

    private float time;

    protected override void OnCreate()
    {
        random = new Random(69420);
        beginSimECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameSettingsComponent>();

    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var settings = GetSingleton<GameSettingsComponent>();
        if (prefab == Entity.Null)
        {
            prefab = GetSingleton<EnemyAuthoringComponent>().Prefab;
            time = settings.timeBetweenWaves - 3;
            return;
        }

        time += Time.DeltaTime;
        if (time < settings.timeBetweenWaves) return;
        time = 0;

        Random generator = random;
        var commandBuffer = beginSimECB.CreateCommandBuffer();
        var enemyPrefab = prefab;

        Job
        .WithCode(() =>
        {
            for (int i = 0; i < settings.numEnemies; ++i)
            {
                float x = (i % 25f) - 13;
                float y = i / 25 + 12;
                Translation trans = new() { Value = new(x, y, 0) };
                EnemyTag enemy = new() { shootTimer = generator.NextFloat(0, 1) };
                Entity e = commandBuffer.Instantiate(enemyPrefab);
                commandBuffer.SetComponent(e, trans);
                commandBuffer.SetComponent(e, enemy);
            }
        }).Schedule();

        beginSimECB.AddJobHandleForProducer(Dependency);
    }
}