using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public delegate void DeadEventHandler();


public class Player : Character {

    #region Fields & Varaibles
    private static Player instance;     // creates a singleton for the class so only one will be active at any given time.

    [SerializeField]
    private ScriptEventSystem eventScript;  // used to scripted events when the player shouldn;t be in control of the character.

    [SerializeField]
    private Transform startPos; // Respawn point for the player.

    [SerializeField]
    private Transform OffScreenFallDeath;   // Off screen fall point below this the player is killed and then respawned.

    public event DeadEventHandler Dead;     // Event fired when the player dies.
    // initializes and creates the singleton.
    public static Player Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    private SpriteRenderer spriteRender;    // Sprite Render.

    [SerializeField]
    private float jumpForce;    // Jump force allplied to the Y direction when jump is pressed.

    [SerializeField]
    private Transform[] groundPoints;   // Points used to help detect ground collision.

    [SerializeField]
    private float groundRadius;     // the raduis to use when checking for grounded.

    [SerializeField]
    private LayerMask whatIsGround; // A layer mask to tell the character what is to be considered ground.
   
    [SerializeField]
    private bool airControl;    // enables or disables the ability to change direction in mid-air. 

    public Rigidbody2D MyRigidbody { get; set; } //reference varaible for the characters ridig body 2d.

    private bool immortal = false;  // used to set when the player is in the immortal state after taking damage. 
    [SerializeField]
    private float immortalTimer;    // the amount of seconds the player is immortal after taking damage.
   
    public bool Jump { get; set; }  // Getter / setter for Jump
    public bool Slide { get; set; } // Getter / setter for Slide
    public bool OnGround { get; set; }  // Getter / setter for when the player is on the ground.
    public bool SceneTrigger { get; set; }  // Used to intialize a transition after a scripted event.
    [SerializeField]
    private int numOfLives; // Number of lives the player has.
    // Getter / Setter for telling if a player is dead.
    public override bool IsDead
    {
        get
        {
            if (health.CurrentVal <= 0) {
                OnDead();
            }
            return health.CurrentVal <= 0;
        }
    }
    // Getter for scripte event system.
    public ScriptEventSystem EventScript
    {
        get
        {
            return eventScript;
        }
    }
    // Getter/setter for nplayers life total.
    public int NumOfLives
    {
        get
        {
            return numOfLives;
        }

        set
        {
            numOfLives = value;
        }
    }

    [SerializeField]
    private bool perspectiveMode = true;    // Used to setup the transition between perspective view and orangrathic view during the game play.
    [SerializeField]
    private Camera mainCamera;  // Reference to the scene camera.

    #endregion

    #region Unity Methods {Awake(), Start(), FixedUpdate(), OnTriggerEnter2D(Collider2D), OnTriggerExit2D(Collidr2D)}
    public override void Awake()
    {
        base.Awake();
        
    }

    // Use this for initialization
    public override void Start () {
        base.Start();
        MyRigidbody = GetComponent<Rigidbody2D>();      // Sets the reference to tje rigidbody of the character by grabing it of the player.
        spriteRender = GetComponent<SpriteRenderer>();  // Sets the reference to the sprite render of the character.
	}

    // Update is called once per frame
    public override void Update()
    {
        base.Update();                  // Calls the Update() of the parent class.
        if (!TakingDamage && !IsDead)   //Checks if tyhe player is taking damage or dead.
        {
            if (transform.position.y <= OffScreenFallDeath.position.y)  // if the player falls below the edge of the world marker they are killed.
            {
                Death();
            }

            HandleInput();  // Gets the players input.
        }
    }
    // Called ona fixed time approx 30 or 60 frames depending on settings.
    void FixedUpdate()
    {
        if (EventScript.IsControllable) // Checks if the player is in control of the character or not.
        {
            if (!TakingDamage && !IsDead)   // If the player's not taking damage or dead then they are free to move.
            {
                float horizontal = Input.GetAxis("Horizontal");     // Gets the direction the player is moving. Ranges between (-1, 1).

                OnGround = IsGrounded();    // Checks if the player is grounded.

                HandleMovement(horizontal); // handles the calcualtions and execution fo moving the player across the screen.

                Flip(horizontal);   // Flips the players character depending on which way they are moving.

                HandleLayers(); // Changes the layers of the animator depending on whats going on.
            }
        }
        else
        {
            return;
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        // Checks if the player is standing on a moving platform if the are they are then made a child of the platform and follwo it.
        if(other.tag == "MovingPlatform")
        {
            transform.SetParent(other.transform);
        }
        if(other.name == "eventEnd")    // Checks when the player character enters a end of event trigger.
        {
            SceneTrigger = true;    // lets the game know that the scripted scene has ended.
        }
        // Checks if the camera swict trigger has been entered and if the game is currently in perspective mode.
        if(other.tag == "cameraSwitch" && perspectiveMode)
        {
            perspectiveMode = false;        // Tracker bool to keep track of when the camera is and is not in perspective mode.
            StartCoroutine(cameraChange()); // Switches the camera between perspective and ornigrathic view.
        }else if(other.tag == "cameraSwitch" && !perspectiveMode)   // Switch the camera back to perspective mode.
        {
            perspectiveMode = true;
            StartCoroutine(cameraChange());
        }
        if(other.name == "CameraSwitch 2" && !perspectiveMode)  // Fails safe switch used to make sure that the camera is in the right mode.
        {
            perspectiveMode = true;
            StartCoroutine(cameraChange());
        }
    }
    // Changes the camera between prespective and ornagrathic.
    private IEnumerator cameraChange()
    {
        float fadeTime = GameManager.Instance.gameObject.GetComponent<Fading>().BeginFade(1);   // Calls the Begin fade on the Gamemanager object and fades the screen black..
        yield return new WaitForSeconds(fadeTime);      // pauses the process for the alloted time.
        if (!perspectiveMode)       // if not supoose to be in perspective mode then set the orthograthic to true.
            mainCamera.orthographic = true;
        else
            mainCamera.orthographic = false;    // when in perspective mode set orthographic to false.
        GameManager.Instance.gameObject.GetComponent<Fading>().BeginFade(-1); // Fades the screen from black back to normal.
    }
    // Notifies the game when the player leaves a trigger.
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "MovingPlatform")
        {
            transform.SetParent(null);  //Once we are no longer in the trigger of a moving platform remove the player as a child.
        }
    }
    #endregion

    #region Methods {OnDeath(), HandleInput(), HandleMovement(float), IsGrounded(), HandleLayers(), ThrowKnife(int), AddHeath(int), IndicateImmortal(),TakeDamage(),Death()}
    // if he player is not permentaly dead calls the dead function.
    public void OnDead()
    {
        if(Dead != null)
        {
            Dead();
        }
    }
    // Handles the input of the player. In this case it changes the animation based on the button pressed.
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");    // Plays the melee attack animation.
        }
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("slide");     // plays the slide animation.
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyAnimator.SetTrigger("jump");      // Plays the jump animation.
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            MyAnimator.SetTrigger("throw");     // Plays the throw animation.
        }
        if (Input.GetKeyDown(KeyCode.Return) && !EventScript.IsControllable)
        {
            HandleMovement(EventScript.Speed);          //runs the handlemovemt forcing the player character to move during a scripted event.
            StartCoroutine(EventScript.EventRun());     // calls the IEnumator that handles the timing for the scripted event.
        }
    }

    // handles the movement of the player.
    private void HandleMovement(float horizontal)
    {
        if(MyRigidbody.velocity.y < 0)
        {
            MyAnimator.SetBool("land", true);   // if the player is traveling down plays the land aniamtion.
        }
        if(!Attack && !Slide && (OnGround || airControl))   
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y); //moves the player.
        }
        if(Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));    // applies force so the player can jump.
        }
        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));    // sets the animator to play the run animation.
    }

    // Flips the sprite when moving from lft to right.
    private void Flip(float horizontal)
    {
        if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) 
        {
            ChangeDirection();
        }
    }
    // Returns true if the player is touching the ground.
    private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y <= 0)    // Checks to make sure we are not jumping.
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);    // checks to see if the players feet are on the ground and adjust the sptrite depending on how far they penatrate.
                for(int i = 0; i< colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject) // checks for collision if even one of the poitns is touching then the player is grounded.
                    {
                       
                        return true;
                    }
                }
            }
        }
        return false;
    }
    // Switches the animation layers between ground and air.
    private void HandleLayers()
    {
        if (!OnGround)
        {
           MyAnimator.SetLayerWeight(1, 1); // Sets layer to Air
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);    // sets layer to ground.
        }
    }
    // throws the players range weapon.
    public override void ThrowKnife(int value)
    {
        if (!OnGround && value == 1 || OnGround && value == 0)
        {
            base.ThrowKnife(value);
        }
    }
    // Adds health back to the player.
    public void AddHeath(int value)
    {
        health.CurrentVal += value;
    }
    //  Causes a visual flash fo the player to propmt the player that they are currently immortal. By disabling and enabling the sprite renderer.
    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            spriteRender.enabled = false;
            yield return new WaitForSeconds(0.1f);
            spriteRender.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }
    // handles the logic for taking damage and calling the immortality feature set.
    public override IEnumerator TakeDamage(Collider2D other)
    {
        if (!immortal)
        {
            health.CurrentVal -= 10;    // removes 10 health from the player when attacked.

            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");    // Plays the damage aniamtion.
                immortal = true;
                StartCoroutine(IndicateImmortal()); // runs the immortal indicator
                yield return new WaitForSeconds(immortalTimer);

                immortal = false; // turns off immortality.
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);    // Sets the animation layer to the ground layer.
                MyAnimator.SetTrigger("die");   // plays the death animation.
            }

        }
    }
    // Called when the player is killed.
    public override void Death()
    {
        numOfLives--;   // removes a life from th eplayer.
        if(numOfLives<=0)   // if all the lives are goen it calls the gam over screen.
        {
            GameManager.Instance.ChanegLevel(0);
        }
        MyRigidbody.velocity = Vector2.zero;    // resets the players movement to 0.
        MyAnimator.SetTrigger("idle");          // sets the player back to idle.
        if (health.CurrentVal != health.MaxVal) // makes sure the player has full health after respawn.
        {
            health.CurrentVal = health.MaxVal;
        }
        if (!perspectiveMode)   // checks the camera and makes sure its back in perspective mode.
        {
            perspectiveMode = true;
            mainCamera.orthographic = false;
        }
        transform.position = startPos.position; // spawns the player at the location.
        GameManager.Instance.PlayerLives(); // updates the display of lives the player has.
    }
    #endregion
}
