using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {

    #region Fields / Variables
    [SerializeField]
    protected Stat health;      // Health for the player and enemy.

    public abstract bool IsDead { get; }    // Tells if the character is dead or not.

    public bool TakingDamage { get; set; }  // Used to check if damage is being taken.

    [SerializeField]
    protected Transform knifePOS;   // Transform used for the spaning point of the range weapon.

    [SerializeField]
    protected float movementSpeed;  // the movement speed of the character.

    protected bool facingRight;     // A bool for telling if the character is facing right or left.

    [SerializeField]
    protected GameObject knife;     // Range melee weapon game object.

    [SerializeField]
    private List<string> damageSources;     // A list of the different souces of damage.

    public bool Attack { get; set; }    // used to tell when a character is doing a melee attack.

    public Animator MyAnimator { get; private set; }    // Reference to the animator to control the animations of the character.

    public bool Throw { get; set; } // Range attack

    public abstract void Death();   // Handles the events upon the death of the character.

    [SerializeField]
    private EdgeCollider2D swordCollider;   // Collider used to detect collison of the sword weapon.
    // Getter
    public EdgeCollider2D SwordCollider
    {
        get
        {
            return swordCollider;
        }
    }

    [SerializeField]
    protected bool hasPickup;   // Used to let the system know if a character can drop an item ro not.
    // Getter/Setter
    public bool HasPickup
    {
        get
        {
            return hasPickup;
        }

        set
        {
            hasPickup = value;
        }
    }
    #endregion

    #region Unity Methods {Awake(), Start(), Update(), OnTriggerEnter2D(Collider2D)}
    public virtual void Awake()
    {
        health.Initialize();
    }

    // Use this for initialization
    public virtual void Start () {
        facingRight = true;     // Sets facingRigth to true because on intial spawn character is always facing right.
        MyAnimator = GetComponent<Animator>();  // Gets a reference to the animator for the character.
    }
	
	// Update is called once per frame
	public virtual void Update () {
	
	}
    // Trigger event when a character enters a trigger.
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage(other));
        }
    }
    #endregion

    #region Methods{ShangeDirection(), ThrowKnife(int), TakeDamage(), MeleeAttack()}
    /// <summary>
    /// Changes the direction the sprite is facing.
    /// </summary>
    public virtual void ChangeDirection()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);  // Reverses the sprite along the X axis.
    }
    /// <summary>
    /// Spawns the knfie at the specified location during a ranged attack action.
    /// </summary>
    /// <param name="value"></param>
    public virtual void ThrowKnife(int value)
    {
        if (facingRight)
        {
            GameObject tmp = (GameObject)Instantiate(knife, knifePOS.position, Quaternion.Euler(new Vector3(0, 0, -90))); // creates a copy of the item at the location.
            tmp.GetComponent<knife>().Initialize(Vector2.right); // moves the created object to the right.
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(knife, knifePOS.position, Quaternion.Euler(new Vector3(0, 0, 90)));  // creates a copy of the item at the location.
            tmp.GetComponent<knife>().Initialize(Vector2.left); // moves the created object to the left.
        }
    }
    /// <summary>
    /// Handles the application of damage based on collision.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public abstract IEnumerator TakeDamage(Collider2D other);
    /// <summary>
    /// handles the melee combat.
    /// </summary>
    public void MeleeAttack()
    {
        SwordCollider.enabled = true;  // Enables the sword collider when the melee attack button is pressed.
    }

    #endregion
}
