using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class knife : MonoBehaviour {

    [SerializeField]
    private float speed;

    [SerializeField]
    private int damage;

    private Rigidbody2D myRigidbody;

    private Vector2 direction;

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    #region Unity Methods {Start(), Initialize(Vector2), FixedUpdate(), Update(),OnBecameInvisible()}
    // Use this for initialization
    void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
	}

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }

    void FixedUpdate()
    {
        myRigidbody.velocity = direction * speed;
    }

	// Update is called once per frame
	void Update () {
	
	}
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    #endregion
}
