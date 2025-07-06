using System;
using Unity.Behavior;

[BlackboardEnum]
public enum BattleState
{
    Idle,
	Fleeing,
	Attacking,
	Patrolling,
	Finding
}
