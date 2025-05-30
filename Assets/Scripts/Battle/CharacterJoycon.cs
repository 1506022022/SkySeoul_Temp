using UnityEngine;

namespace Battle
{
    public interface IController
    {
        public void Update();
    }
    public static class PlayerControl
    {
        public static void UpdateAttack(CharacterComponent character)
        {
            var attack = Input.GetButtonDown("Fire1");
            if (attack)
            {
                character.DoAttack();
            }
        }
        public static void UpdateInteraction(CharacterComponent character)
        {
            var input = Input.GetButtonDown("Interaction");
            if (input && InteractionSystem.TryGetInteraction(character.transform, out var interaction))
            {
                character.DoInteraction();
            }
        }

        public static void UpdateJump(CharacterComponent character)
        {
            var jump = Input.GetButtonDown("Jump");
            if (jump)
            {
                character.DoJump();
            }
        }
        public static void UpdateCancel(CharacterComponent character)
        {
            var cancle = Input.GetButtonDown("Cancel");
            if (cancle)
            {
                character.DoCancel();
            }
        }
        public static void UpdateMovement(CharacterComponent character)
        {
            var dir = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical");
            if (dir == Vector3.zero) return;
            if (Input.GetKey(KeyCode.LeftShift)) character.DoRun(dir);
            else character.DoMove(dir);
        }
        public static void UpdateSliding(CharacterComponent character)
        {
            if (FlagHelper.HasFlag(character.State, BodyState.Run) &&
                Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
            {
                character.DoSlide();
            }
        }
        public static void UpdateLand(CharacterComponent character)
        {
            if (!FlagHelper.HasFlag(character.State, BodyState.Grounded) && character.IsGrounded)
            {
                character.DoLand();
            }
        }
    }

    public abstract class CharacterJoycon : IController
    {
        readonly CharacterComponent character;
        public CharacterJoycon(CharacterComponent character)
        {
            this.character = character;
        }

        public void Update()
        {
            PlayerControl.UpdateMovement(character);
            PlayerControl.UpdateAttack(character);
            PlayerControl.UpdateJump(character);
            PlayerControl.UpdateSliding(character);
            PlayerControl.UpdateInteraction(character);
            PlayerControl.UpdateCancel(character);
            PlayerControl.UpdateLand(character);
        }

    }
    public class HanZoomInJoycon : IController
    {
        readonly CharacterComponent character;
        public HanZoomInJoycon(CharacterComponent character)
        {
            this.character = character;
        }
        public void Update()
        {
            if (this.UpdateZoomOut()) return;
            PlayerControl.UpdateMovement(character);
            PlayerControl.UpdateAttack(character);
            PlayerControl.UpdateJump(character);
            PlayerControl.UpdateInteraction(character);
            PlayerControl.UpdateCancel(character);
            PlayerControl.UpdateLand(character);
        }
        bool UpdateZoomOut()
        {
            if (Input.GetKey(KeyCode.Mouse1)) return false;
            character.SetController(new HanZoomOutJoycon(character));
            character.SetAnimator(new HanZoomOutAnimator());
            if (character is ZoomCharacterComponent zoom) zoom.OnZoomOut();
            return true;
        }
    }

    public class HanZoomOutJoycon : IController
    {
        readonly CharacterComponent character;
        public HanZoomOutJoycon(CharacterComponent character)
        {
            this.character = character;
        }

        public void Update()
        {
            if (UpdateZoomIn()) return;
            PlayerControl.UpdateMovement(character);
            PlayerControl.UpdateJump(character);
            PlayerControl.UpdateSliding(character);
            PlayerControl.UpdateInteraction(character);
            PlayerControl.UpdateCancel(character);
            PlayerControl.UpdateLand(character);
        }
        bool UpdateZoomIn()
        {
            if (!Input.GetKey(KeyCode.Mouse1)) return false;
            character.SetController(new HanZoomInJoycon(character));
            if (character is ZoomCharacterComponent zoom) zoom.OnZoomIn();
            return true;
        }
    }
}
