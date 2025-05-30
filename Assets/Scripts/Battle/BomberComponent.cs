namespace Battle
{
    public class BomberComponent : WeaponComponent
    {
        public AttackBoxComponent container;

        protected override void DoAttack()
        {
            container?.AttackBox.OpenAttackWindow();
        }
    }

}

