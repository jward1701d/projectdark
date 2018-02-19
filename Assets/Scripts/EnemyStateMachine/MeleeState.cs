using UnityEngine;
using System.Collections;
using System;

public class MeleeState : IEnemyState
{
    private Enemy enemy;            // Reference to the enemy.

    private float attackTimer;      // Timer to track the time between attacks.    
    private float attackCoolDown = 0.5f;    // Cool down timer between attacks.
    private bool canAttack;         // Checks for is the enemy can or can not attack.

    #region Methods { Enter(Enemy), Execute(), Exit(), OnTriggerEnter2D(Collider2D),Attac() }
    /// <summary>
    /// Logic for this state.
    /// </summary>
    /// <param name="enemy"></param>
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;     // Sets the reference to the enemy.
        attackTimer = 0f;       // Sets the timer to 0;
    }
    /// <summary>
    /// Runs the Logic for this state.
    /// </summary>
    public void Execute()
    {
        Attack();   // Runs the ,elee attack logic.
        if (enemy.InThrowRange && !enemy.InMeleeRange)  // Checks if the player moves out of melee range.
        {
            enemy.ChangeState(new RangeState());        // Sets the state to RangeState if the player is out of melee range.
        }else if(enemy.Target == null)                  // Chekcs if the target has been lost.
        {
            enemy.ChangeState(new IdleState());         // Sets the state to Idle if the player is out of sight and range.
        }
    }

    public void Exit()
    {
        
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }
    /// <summary>
    /// Perfoms the melee attack logic.
    /// </summary>
    private void Attack()
    {
        attackTimer += Time.deltaTime;              // Increments the timer.

        if (attackTimer >= attackCoolDown)          // checks if the timer has exceeded the cool down.
        {
            canAttack = true;                       // tells the enemy they can attack.
            attackTimer = 0;                        // resets the attack timer to 0;
            attackCoolDown = 3f;                    // sets a new cool down between attacks after the initial swing.
        }
        if (canAttack)
        {
            canAttack = false;                      // sets the canAttack back to false so the enemy can't repeatedly attack.
            enemy.MyAnimator.SetTrigger("attack");  // Sets the aniamtion to play the attack aniamtion.
        }
    }
    #endregion
}
