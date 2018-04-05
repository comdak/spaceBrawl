using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : Action
{
    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        RaycastHit hit;

        Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * controller.shipStats.attackRange, Color.red);

        if (Physics.SphereCast(controller.eyes.position, controller.shipStats.lookSphereCastRadius, controller.eyes.forward, out hit, controller.shipStats.attackRange)
            && hit.collider.CompareTag("Player"))
        {
            if (controller.CheckIfCountDownElapsed(controller.shipStats.attackRate))
            {
                controller.player.FireAsBot();
            }
        }
    }


}
