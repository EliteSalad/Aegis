using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{ 

public float maxSpeed = 8f;
public float jumpSpeed = 12f;
public bool platformRelativeJump = false;
public bool allowWallGrab = true;
public bool allowWallJump = true;
public bool disableGravityDuringWallGrab = false;
public LayerMask wallGrabMask;
public float wallJumpControlDelay = 0.15f;
float wallJumpControlDelayLeft = 0;
bool climbing = false;
    bool canClimb = false;

float xVel = 0.0f;
float yVel = 0.0f;
    // Relative to our transform/pivot point, where are we testing for grabbing?
    // Logically, this should probably be around where the character's hand will
    // be during the grabbing animation.
    public Vector2 grabPoint = new Vector2(0.45f, 0f);

// Bookkeeping Variables
MovingPlatform movingPlatform;      // The moving platform we are touching
Animator anim;                      // Our animator
bool groundedLastFrame = false;     // Were we grounded last frame? Used by IsGrounded to eliminate top-of-arc issues.
bool jumping = false;               // Is the player commanding us to jump?

// Use this for initialization
void Start()
{
    anim = GetComponent<Animator>();

    xVel = GetComponent<Rigidbody2D>().velocity.x;
    yVel = GetComponent<Rigidbody2D>().velocity.y;
}

void OnCollisionEnter2D(Collision2D col)
{
        // check collisiosn to see if touching a moving platform, 
        //if yes grab a copy of it's velocity to become new zero point of player
    MovingPlatform mp = col.transform.root.GetComponent<MovingPlatform>();
    if (mp != null)
    {
        Debug.Log("movingPlatform: " + mp.gameObject.name);
        movingPlatform = mp;
    }
}
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "climbable")
        {
            canClimb = true;
            Debug.Log("canClimb = true");
        }
        
            ////canClimb = false;
            ////Debug.Log("canClimb = true");
        


    }

    /// Determines if the character is grounded based on having a zero velocity relative to their platform.
    bool IsGrounded()
{
    if (Mathf.Abs(RelativeVelocity().y) < 0.1f)
    {

            //because player can be stationary at apex of jump check 2 frames of stillness before grounded is true
            if (groundedLastFrame)
            {
               // if (canClimb == true  && Input.GetAxis("Vertical") > 0)
                return true; }

        groundedLastFrame = true;
    }
    else
    {
        groundedLastFrame = false;
    }

    return false;
}


/// Determines if we're grabbing a surface.
bool IsGrabbing()
{
    if (allowWallGrab == false)
        return false;

    // If pushing in the direction facing and an OverlapCircle test indicates a grabbable surface at the grabPoint, return true.
    //will cast a circle at grab point, and if it's overlapping something grabbable
    return ((Input.GetAxisRaw("Horizontal") > 0 && this.transform.localScale.x > 0) || (Input.GetAxisRaw("Horizontal") < 0 && this.transform.localScale.x < 0)) &&
        Physics2D.OverlapCircle(this.transform.position + new Vector3(grabPoint.x * this.transform.localScale.x, grabPoint.y, 0), 0.2f, wallGrabMask);
}

void Update()
{
    //Non physics updates
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire1"))
        jumping = true;

        if (Input.GetAxis("Vertical") >= 0.5 || Input.GetAxis("Vertical") <= -0.5)
        { climbing = true; }
        else
        { climbing = false;
            Debug.Log("climbing = false");
                }

        //canClimb = false;
        //Debug.Log("canClimb = false");
    }


/// Our velocity relative to the platform we're on, if any.
/// <returns>The velocity.</returns>
Vector2 RelativeVelocity()
{
    return GetComponent<Rigidbody2D>().velocity - PlatformVelocity();
}


/// The velocity of the platform we're on (or zero)
/// <returns>The velocity.</returns>
Vector2 PlatformVelocity()
{
    if (movingPlatform == null)
        return Vector2.zero;

    return movingPlatform.GetComponent<Rigidbody2D>().velocity;
}

// Update is called once per physics loop
void FixedUpdate()
{
    bool isGrounded = IsGrounded();
    bool isGrabbing = !isGrounded && wallJumpControlDelayLeft <= 0 && IsGrabbing();

    if (movingPlatform != null && !groundedLastFrame && !isGrabbing && !isGrounded)
    {
        // We aren't grounded or grabbing.  Making sure to clear our platform.
        movingPlatform = null;
    }

    // FIXME: This results in weird drifting with our current colliders
    if (disableGravityDuringWallGrab)
    {
        if (isGrabbing)
            GetComponent<Rigidbody2D>().gravityScale = 0;
        else
            GetComponent<Rigidbody2D>().gravityScale = 1;
    }

        // We start off by assuming we are maintaining our velocity.
        xVel = GetComponent<Rigidbody2D>().velocity.x;
        yVel = GetComponent<Rigidbody2D>().velocity.y;

        // If we're grounded, maintain our velocity at platform velocity, with slight downward pressure to maintain the collision.
        if (isGrounded)
    {
        yVel = PlatformVelocity().y - 0.01f;
    }

    // Some moves (like walljumping) might introduce a delay before x-velocity is controllable
    wallJumpControlDelayLeft -= Time.deltaTime;

    if (isGrounded || isGrabbing)
    {
        wallJumpControlDelayLeft = 0;   // Clear the delay if we're in contact with the ground/wall
    }

    // Allow x-velocity control
    if (wallJumpControlDelayLeft <= 0)
    {
        xVel = Input.GetAxis("Horizontal") * maxSpeed;
        xVel += PlatformVelocity().x;
    }

    if (isGrabbing && RelativeVelocity().y <= 0)
    {
     //gravity and friction scale to stop sliding 

        yVel = PlatformVelocity().y; //equals the up and down velocity of patform it's on 

        if (RelativeVelocity().x * transform.localScale.x <= 0)
        {
            xVel = PlatformVelocity().x;
        }
    }
        if (canClimb && climbing)
        {
            yVel = Input.GetAxis("Vertical") * maxSpeed;
          
        }
        if (climbing == false)
        {
            canClimb = false;
            Debug.Log("canclimb = false");
        }

        if (jumping && (isGrounded || (isGrabbing && allowWallJump)))
    {
        // platform velocity doesnt affect jumps unless button clicked 
        yVel = jumpSpeed;
        if (platformRelativeJump)
            yVel += PlatformVelocity().y;

        if (isGrabbing)
        {
            xVel = -maxSpeed * this.transform.localScale.x;
            wallJumpControlDelayLeft = wallJumpControlDelay;
        }
    }
    jumping = false;


    // Apply the calculate velocity to our rigidbody
    GetComponent<Rigidbody2D>().velocity = new Vector2( xVel, yVel);

    // Update facing
    Vector3 scale = this.transform.localScale;
    if (scale.x < 0 && Input.GetAxis("Horizontal") > 0)
    {
        scale.x = 1;
    }
    else if (scale.x > 0 && Input.GetAxis("Horizontal") < 0)
    {
        scale.x = -1;
    }
    this.transform.localScale = scale;

    // Update animations
    anim.SetFloat("xSpeed", Mathf.Abs(RelativeVelocity().x));

    if (isGrabbing)
        anim.SetFloat("ySpeed", Mathf.Abs(1000));
    else
        anim.SetFloat("ySpeed", RelativeVelocity().y);
}
}
