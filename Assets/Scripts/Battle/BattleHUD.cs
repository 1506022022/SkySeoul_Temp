using Character;
using Microlight.MicroBar;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public class BattleHUD : IDisposable,IInitializable
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
        public void UpdatePlayer(IHP health)
        {
            if (playerBar == null) return;
            playerBar.UpdateBar(health.HP.Ratio);
        }
        public void UpdateMonster(IHP health)
        {
            if (enemyBar == null) return;
            enemyBar.UpdateBar(health.HP.Ratio);
        }

        public void Dispose()
        {
            HUD.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            HUD.gameObject.SetActive(true);
        }
    }
}
