using Unity.Entities;

public class SetGameSettingsSystem : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float asteroidVelocity = 1f;
    public float playerVelocity = 150f;
    public float weaponMaxSpeed = 10f;
    public float bulletVelocity = 5f;
    public int timeBetweenWaves = 12;

    public int numEnemies = 50;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var settings = default(GameSettingsComponent);

        settings.asteroidVelocity = asteroidVelocity;
        settings.weaponMaxSpeed = weaponMaxSpeed;
        settings.weaponMaxSpeedSqr = weaponMaxSpeed * weaponMaxSpeed;
        settings.bulletVelocity = bulletVelocity;
        settings.playerVelocity = playerVelocity;

        settings.numEnemies = numEnemies;
        settings.timeBetweenWaves = timeBetweenWaves;
        dstManager.AddComponentData(entity, settings);
    }
}