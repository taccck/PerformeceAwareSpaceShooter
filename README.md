# PerformeceAwareSpaceShooter

The project uses DOTS functionality, mainly ecs which I’ve used as follows: components are the data placed onto entities made from prefabs that gets instantiated onto a sub scene by a system, other systems then manages those objects during runtime. Enemies for example are a prefab (Assets\Prefabs\Enemy.prefab) containing a mesh, a physics body, a physics shape and an enemy tag (Assets\Scripts\Enemy\EnemyTag.cs) that get instantiated as an entity from the enemy spawn system (Assets\Scripts\Enemy\EnemySpawnSystem.cs) and is then moved and deleted by the enemy movement system (Assets\Scripts\Enemy\EnemyMovementSystem.cs) with Entities.Foreach(). Players, weapons and projectiles all follow a similar life cycle. 

Build in Builds.zip\Window\SamNilsson_PerformeceAwareSpaceShooter.exe
