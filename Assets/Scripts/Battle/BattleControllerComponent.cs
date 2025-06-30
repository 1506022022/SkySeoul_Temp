using Character;
using UnityEngine;
using Util;

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

            var characters = FindObjectsByType<EntityBaseComponent>(FindObjectsSortMode.InstanceID);
            Enumerator.InvokeFor(characters, c => { if (c is IActor actor) BattleController.JoinCharacter(actor); });

            var oldCharacters = FindObjectsByType<CharacterComponent>(FindObjectsSortMode.InstanceID);
            Enumerator.InvokeFor(oldCharacters, c => { if (c is IActor actor) BattleController.JoinCharacter(actor); });
        }
    }
}
