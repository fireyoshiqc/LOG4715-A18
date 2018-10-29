using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour 
{
	bool facingRight = true;							// For determining which way the player is currently facing.

    [SerializeField] float HorizontalAcceleration = 1f;
	[SerializeField] float maxSpeed = 10f;				// The fastest the player can travel in the x axis.
	[SerializeField] float jumpForce = 400f;			// Amount of force added when the player jumps.		
	[SerializeField] float wallJumpMult = 1f;			// Relative power of a Wall Jump.	
	[SerializeField] float WallHorizontalForce = 40000f;	// Amount of force added when the player walljump (horizontal).	

	[Range(0, 1)]
	[SerializeField] float crouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	
	[SerializeField] bool airControl = false;			// Whether or not a player can steer while jumping;
	[SerializeField] LayerMask whatIsGround;			// A mask determining what is ground to the character
	[SerializeField] LayerMask whatIsWall;  			// A mask determining what is walls to the character
	
	Transform groundCheck;								// A position marking where to check if the player is grounded.
	float groundedRadius = .12f;							// Radius of the overlap circle to determine if grounded
	bool grounded = false;								// Whether or not the player is grounded.
	Transform ceilingCheck;								// A position marking where to check for ceilings
	float ceilingRadius = .01f;							// Radius of the overlap circle to determine if the player can stand up
	Animator anim;										// Reference to the player's animator component.

    //Multijump management
    [SerializeField] int maxAirJumps = 1;
    private int AirJumpCounter = 0;


    //Airjump management
    [SerializeField] bool wallJumpResetsAirJumps = false;
    Transform wallCheckRight;
    Transform wallCheckLeft;
    float walledRadius = 0.2f;
    bool walledRight = false;
    bool walledLeft = false;

    //Charged Jump Management
    [SerializeField] float maxJumpForce = 1200f;			// Maximum force added when the player charges jumps.
    [SerializeField] float fullChargeTime = 2;			// Maximum force added when the player charges jumps.
    float nextChargedJumpForce = 0f;
    bool isChargingJump = false;
    

    public bool Grounded
    {
        get
        {
            return grounded;
        }
        private set
        {
            grounded = value;
        }
    }

    void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("GroundCheck");
		ceilingCheck = transform.Find("CeilingCheck");
        wallCheckRight = transform.Find("WallCheckRight");
        wallCheckLeft = transform.Find("WallCheckLeft");
        anim = GetComponent<Animator>();
	}


	void FixedUpdate()
	{
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		Grounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		anim.SetBool("Ground", Grounded);
        
        walledRight = Physics2D.OverlapCircle(wallCheckRight.position, walledRadius, whatIsWall);
        walledLeft = Physics2D.OverlapCircle(wallCheckLeft.position, walledRadius, whatIsWall);

        // Set the vertical animation
        anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
	}


	public void Move(float move, bool crouch, bool jump, bool jumpRelease)
	{


		// If crouching, check to see if the character can stand up
		if(!crouch && anim.GetBool("Crouch"))
		{
            if(isChargingJump)
            {
                Jump(0f, nextChargedJumpForce);
            }
            
            nextChargedJumpForce = jumpForce;
            isChargingJump = false;

			// If the character has a ceiling preventing them from standing up, keep them crouching
			if( Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
				crouch = true;
		}

		// Set whether or not the character is crouching in the animator
		anim.SetBool("Crouch", crouch);



		//only control the player if grounded or airControl is turned on
		if(Grounded || airControl)
		{

            // Reduce the speed if crouching by the crouchSpeed multiplier
            move = (crouch ? move * crouchSpeed : move);

            float slowing = Grounded ? 0.35f : 0.95f;
            float xSpeed = GetComponent<Rigidbody2D>().velocity.x * slowing;

            xSpeed += move * HorizontalAcceleration;

            xSpeed = Mathf.Clamp(xSpeed, -maxSpeed, +maxSpeed);
			// Move the character
            GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed, GetComponent<Rigidbody2D>().velocity.y);

			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				Flip();

			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
		}

        if(Grounded)
        {
            //If grounded, restore all jumps
            AirJumpCounter = maxAirJumps;
            
            if (isChargingJump) {
                if(jumpRelease)
                {
                    Jump(0f, nextChargedJumpForce);
                    nextChargedJumpForce = jumpForce;
                    isChargingJump = false;
                }
            }
        }

        if ( !Grounded && (walledRight || walledLeft) && jump)
        {
            //We have contacc on a wall and want to jump

            //NOBODY EVER TOUCH THIS
            float flippedForce = walledLeft ? 1 : -1;
            float flippedAgain = facingRight ? 1 : -1;

            Jump(flippedForce * flippedAgain * WallHorizontalForce, jumpForce * wallJumpMult);
            if (wallJumpResetsAirJumps)
            {
                AirJumpCounter = maxAirJumps;
            }
        }

        if (anim.GetBool("Crouch") && jump)
        {
            isChargingJump = true;
        }

        // If the player should jump...
        // Player must have remaining jumps
        if ((Grounded || !(walledRight || walledLeft) &&  AirJumpCounter > 0) && jump) {
            // Add a vertical force to the player.
            if (!isChargingJump)
            {
                Jump(0f, jumpForce);
            }

            anim.SetBool("Ground", false);


            //Consume a jump
            if (!Grounded)
            {
                AirJumpCounter--;
            }
        }

        if (isChargingJump)
        {
            
            nextChargedJumpForce = Mathf.Min(nextChargedJumpForce + (maxJumpForce - jumpForce)/60*fullChargeTime, maxJumpForce);
        }
	}

    void Jump(float x, float y)
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
        GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y));
    }
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
