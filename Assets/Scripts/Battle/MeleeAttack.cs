namespace Battle
{
    public class MeleeAttack : IBullet
    {
        readonly AttackBox attackBox;

        public MeleeAttack(AttackBoxComponent component)
        {
            this.attackBox = component.AttackBox;
        }
        void IBullet.OnFire()
        {
            attackBox.OpenAttackWindow();
        }
    }

}
