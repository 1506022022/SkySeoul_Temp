using UnityEngine;

namespace Battle
{
    public class BattleControllerComponent : MonoBehaviour
    {
        BattleController BattleController;

        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            BattleController = new BattleController();
            var characters = FindObjectsByType<CharacterComponent>(FindObjectsSortMode.InstanceID);
            for (int i = 0; i < characters.Length; i++)
            {
                BattleController.JoinCharacter(characters[i]);
            }
        }
    }
}
