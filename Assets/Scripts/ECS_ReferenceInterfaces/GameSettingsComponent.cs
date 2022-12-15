using Unity.Entities;

public struct GameSettingsComponent : IComponentData
{
    public float asteroidVelocity;
    public float weaponMaxSpeedSqr;
    public float weaponMaxSpeed;
    public float bulletVelocity;
    public int numEnemies;
    public int timeBetweenWaves;
    public float playerVelocity;
}