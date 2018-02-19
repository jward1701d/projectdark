using UnityEngine;
using System.Collections;

public class DeathBehaviour : StateMachineBehaviour {

    private float respawntime = 5f;
    private float deathTimer;
    private int spawnAmount;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        deathTimer = 0;
        if (animator.GetComponent<Character>().HasPickup)
        {
            spawnAmount = 1;
        }
        else
        {
            spawnAmount = 2;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        deathTimer += Time.deltaTime;

        if(deathTimer >= respawntime)
        {
            animator.GetComponent<Character>().Death();
        }

        if (spawnAmount <= 1 && animator.GetComponent<Enemy>().HasPickup)
        {
            // GameObject tmp = (GameObject)
            if (animator.GetComponent<Enemy>().SpawnHeart)
            {
                Instantiate(
                    animator.GetComponent<Enemy>().Heart,
                    animator.GetComponent<Enemy>().HeartSpawn.position,
                    Quaternion.identity);
            }
            if (animator.GetComponent<Enemy>().SpawnOneUp)
            {
                Instantiate(
                    animator.GetComponent<Enemy>().OneUp,
                    animator.GetComponent<Enemy>().OneUpSpawn.position,
                    Quaternion.identity);
            }
            spawnAmount++;
        }
        //spawnAmount++;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    //Debug.Log("drop heart.");
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
