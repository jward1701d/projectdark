using UnityEngine;
using System.Collections;

public class CollisionTrigger : MonoBehaviour {

    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private BoxCollider2D platformTrigger;

    #region Unity Methods{Start(), OnTriggerEnter2D(Collider2D), OnTriggerExit2D(Collider2D)}
    // Use this for initialization
    void Start () {
        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
	}
	
	void OnTriggerEnter2D(Collider2D other)
    {
        //if(other.gameObject.name == "Player" || other.gameObject.name == "Enemy")
        if(other.tag == "Player" || other.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        //if(other.gameObject.name == "Player" || other.gameObject.name == "Enemy")
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(platformCollider, other, false);
        }
    }
    #endregion
}
