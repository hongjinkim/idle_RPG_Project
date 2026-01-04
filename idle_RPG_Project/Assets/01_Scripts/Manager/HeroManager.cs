using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroManager : BaseManager
{

    [Title("Main Character")]
    [ShowInInspector] public Hero PlayerHero { get; private set; }
    [ShowInInspector] private CharacterStat _heroStat => PlayerHero != null ? PlayerHero.Stat : null;

    // HeroManager 고유 기능은 여기에 작성
    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        // 초기화 로직 작성
        // 씬에서 Player 객체 찾기
        PlayerHero = FindFirstObjectByType<Hero>();
        if (PlayerHero == null)
        {
            Debug.LogError("HeroManager: Player object not found in the scene!");
        }
        TestLoadHero(PlayerHero);
    }

    private void TestLoadHero(Hero hero)
    {
        hero.Setup(MainSystem.Data.Hero.GetClone("HERO_001"));
    }

}