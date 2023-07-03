
using DefaultNamespace;
using DefaultNamespace.UI;
using Unity.BossRoom.Infrastructure;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterEntryPoint<GameManager>(Lifetime.Singleton).AsSelf();
        builder.RegisterComponentInHierarchy<BulletSpawner>();
        builder.RegisterComponentInHierarchy<IngredientBarUI>();
        builder.RegisterComponentInHierarchy<PanelResourcesUI>();
        builder.RegisterComponentInHierarchy<SoundManager>();
        builder.RegisterComponentInHierarchy<UserInput>();
        builder.RegisterComponentInHierarchy<ResultPanelUI>();

        builder.RegisterInstance(new MessageChannel<ShareData.GameState>()).AsImplementedInterfaces();
        builder.RegisterInstance(new MessageChannel<ShareData.CurrentIngredient>()).AsImplementedInterfaces();
        builder.RegisterInstance(new MessageChannel<ShareData.PendingIngredient>()).AsImplementedInterfaces();
        builder.RegisterInstance(new MessageChannel<ShareData.CastBullet>()).AsImplementedInterfaces();
        builder.RegisterInstance(new MessageChannel<ShareData.MultiplyBullet>()).AsImplementedInterfaces();
        builder.RegisterInstance(new MessageChannel<ShareData.UpdateIngredientPoint>()).AsImplementedInterfaces();
        
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
