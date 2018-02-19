using UnityEngine;
using System.Collections;

/// <summary>
/// public interface for the AI state machine.
/// </summary>
public interface IEnemyState
{
    void Execute();                         // Execute class
    void Enter(Enemy enemy);                // Enter class
    void Exit();                            // exit class
    void OnTriggerEnter(Collider2D other);  // On trigger Enter class.
}
