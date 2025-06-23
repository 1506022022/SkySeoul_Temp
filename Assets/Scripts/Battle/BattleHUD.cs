using Microlight.MicroBar;
using UnityEngine;

namespace Battle
{
    public class BattleHUD
    {
        readonly MicroBar playerBar;
        readonly MicroBar enemyBar;
        readonly Canvas HUD;

        public BattleHUD()
        {
            HUD = new GameObject("HUD").AddComponent<Canvas>();
            GameObject.DontDestroyOnLoad(HUD);
            HUD.renderMode = RenderMode.ScreenSpaceOverlay;

            playerBar = GameObject.Instantiate(Resources.Load<GameObject>("Player HP Bar").GetComponent<MicroBar>());
            playerBar.transform.SetParent(HUD.transform, false);
            playerBar.Initialize(1);

            enemyBar = GameObject.Instantiate(Resources.Load<GameObject>("Enemy HP Bar").GetComponent<MicroBar>());
            enemyBar.transform.SetParent(HUD.transform, false);
            enemyBar.Initialize(1);
        }
        ~BattleHUD()
        {
            GameObject.Destroy(HUD.gameObject);
        }
        public void UpdatePlayer(CharacterComponent character)
        {
            if (playerBar == null) return;
            playerBar.UpdateBar(character.HP.Ratio);
        }
        public void UpdateMonster(CharacterComponent character)
        {
            if (enemyBar == null) return;
            enemyBar.UpdateBar(character.HP.Ratio);
        }
    }
}
