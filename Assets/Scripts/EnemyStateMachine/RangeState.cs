using UnityEngine;
using System.Collections;

public class RangeState : IEnemyState
{

    private Enemy enemy;                // Reference to the enemy.

    private float throwTimer;           // Timer for the cool down between throws. 
    private float throwCoolDown;        // Time between each throw of the dagger so the computer can't infinitely throw.
    private bool canThrow;              // Tells the enemy if he can or can't throw a knife.

    #region Methods { Enter(Enemy), Execute(), Exit(), OnTriggerEnter2D(Collider2D),ThrowKnife() }
    /// <summary>
    /// Sets up the information for the state.
    /// </summary>
    /// <param name="enemy"></param>
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;     // Gets a reference to the enemy.
        throwTimer = 0f;        // sets the throw timer to 0;
        throwCoolDown = 0.5f;   // cool down between throws.
    }
    /// <summary>
    /// perfoems the Logic for this state.
    /// </summary>
    public void Execute()
    {
        if (enemy.InMeleeRange)         // checks if it is in melee state.
        {
            enemy.ChangeState(new MeleeState());    // Sets the state to melee.
        }
        if (enemy.InThrowRange)         // if the player is in throw range
        {
            ThrowKnife();               // Handle the throw logic.
        }
        if(!enemy.InMeleeRange && !enemy.InThrowRange)  // Checks if the enemy is not in melee or throw raneg then sends the enemy back to patrol.
        {
            enemy.ChangeState(new PatrolState());   // Enters a new patrol state.
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {
        
    }
    /// <summary>
    /// Logic for the Range state.
    /// </summary>
    private void ThrowKnife()
    {
        throwTimer += Time.deltaTime;       // Increments the throw timer.

        if (throwTimer >= throwCoolDown)    // checks if the throw timer has exceeded the cool down amount.
        {
            canThrow = true;                // tells the enemy he can throw a knife.
            throwTimer = 0;                 // Resets the throw timer.
            throwCoolDown = 3f;             // Sets the throw cooldown to a larger amount after the intial toss.
        }
        if (canThrow)
        {
            canThrow = false;               // resets canThrow to false.
            enemy.MyAnimator.SetTrigger("throw");   // Plays the throw animation.
        }
    }
    #endregion
}