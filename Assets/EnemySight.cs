using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour {

    [SerializeField]
    private Enemy enemy;        // Reference to the enemy character.

    [SerializeField]
    private Collider2D[] other = new Collider2D[4];     // Collideders that should be ignored.

    // Sets the enemy sight box to ignore certian physic colliders.
    private void Awake()
    {
        for (int i = 0; i < other.Length; i++)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other[i], true);  // cycles through the different collidr that need to be ignored and ignores them.
        }
    }
    // if the Player enters the sight box sets the player as the target.
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            enemy.Target = other.gameObject;
        }
    }
    // if the player gets out of sight waits for 1 second then removes the player as traget and returns to patrol state.
    private IEnumerator LostSight()
    {
        yield return new WaitForSeconds(1f);
        enemy.RemoveTarget();
    }
    // after the player leaves the sight box runs the LostSight coroutine. 
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(LostSight());
        }
    }

}
