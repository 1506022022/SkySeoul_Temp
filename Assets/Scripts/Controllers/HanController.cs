using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanController : BaseController
{
    private HanCharacter hanCharacter;

    protected override void Start()
    {
        base.Start();

        hanCharacter = baseCharacter as HanCharacter;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CustomActionInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            hanCharacter.RobotSelect();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            hanCharacter.RobotSelect();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            hanCharacter.RobotSkill();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            hanCharacter.RobotAction();
        }
        if (Input.GetKey(KeyCode.T))
        {
            hanCharacter.RobotAction();
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            hanCharacter.RobotAction();
        }
    }
}
