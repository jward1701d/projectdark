using UnityEngine;
using System.Collections;

public class PatrolState : IEnemyState
{

    private float patrolTimer;          // Timer for the patrol state.
    private float patrolDuration;       // Time to stay in the patrol state.

    private Enemy enemy;                // reference to the enemy.

    #region Methods { Enter(Enemy), Execute(), Exit(), OnTriggerEnter2D(Collider2D),Patrol() }
    /// <summary>
    /// Sets up the logic.
    /// </summary>
    /// <param name="enemy"></param>
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;             // Sets the reference to enemy for this state.
        patrolDuration = Random.Range(1, 5);    // Sets a random time to be in this state.
        patrolTimer = 0f;               // Sets the start of the timer to 0.
    }

    public void Execute()
    {
        Patrol();           // Handles the logic for this state.
        enemy.Move();       // Moves the enemy during this state.
        if (enemy.Target != null && enemy.InThrowRange)   // Checks to see if the enemy has a target and if it is in thro range.  
        {
            enemy.ChangeState(new RangeState());        // Sets the state to rangeState.
        }
        else if(enemy.Target != null && enemy.InMeleeRange) //Checks if the enemy has a target and if it is in melee range. 
        {
            enemy.ChangeState(new MeleeState());        // Sets the state to meleeState.
        }
    }

    public void Exit()
    {
    }

    public void OnTriggerEnter(Collider2D other)
    {
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;              // Counts down the time in this state.

        if (patrolTimer >= patrolDuration)          // checks if the eney has been in the state too long.
        {
            enemy.ChangeState(new IdleState());     // changes the state back to idle.
        }
    }
    #endregion
}