using UnityEngine;
using System.Collections;

public class IdleState : IEnemyState
{
    private Enemy enemy;            // Reference to the enemy.

    private float idleTimer;        // timer to remain in this state.

    private float idleDuration;     // The amount fo time to staye in this state.


    #region Methods { Enter(Enemy), Execute(), Exit(), OnTriggerEnter2D(Collider2D),Idle() }
    /// <summary>
    /// Setups up the things needed for this state.
    /// </summary>
    /// <param name="enemy"></param>
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;                     // Sets a reference to the enemy this state controls.
        idleDuration = Random.Range(1, 5);      // Sets a random amount fo time this enamy will remain in this state.
    }
    /// <summary>
    /// Runs the logic for this state.
    /// </summary>
    public void Execute()
    {
        Idle();         // Calls the Idle method.

        if (enemy.Target != null && enemy.InThrowRange)     // Checks that if the enemy has a tasrget and sees if it is in throw range.
        {
            enemy.ChangeState(new RangeState());            // Changes to the range state.
        }
        else if (enemy.Target != null && enemy.InMeleeRange)    // Checks to see if the enemy has a target and if it is in melee range.
        {
            enemy.ChangeState(new MeleeState());            // Changes to the melee state.
        }
        else 
        {
            enemy.ChangeState(new PatrolState());           // if enemy has no target it will go into a new patrol state.
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {
        if(other.tag == "Knife")
        {
            enemy.Target = Player.Instance.gameObject;      // Sets the player to the target if they hit the enemy with a knife from distance.
        }
    }
    /// <summary>
    /// Logic for the Idle state.
    /// </summary>
    private void Idle()
    {
        enemy.MyAnimator.SetFloat("speed", 0);      // Sets the animation to the idle aniamtion.

        idleTimer += Time.deltaTime;                // tracks the time the enemy is in this state. 

        if(idleTimer >= idleDuration)               // checks that the time in this state is longer than the time it's suppose to be in it.
        {
            enemy.ChangeState(new PatrolState());   // Sets the enemy to patorl after the idle timer expires.
        }
    }
    #endregion
}