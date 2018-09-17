using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerController : MonoBehaviour
{

    private bool isJumping = false;
    private bool isOnFloor = false;   
    private bool isFalling = false;
    private int numberJumps;
   
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator anim;
    private float touchValue = 0;

    [Header("Movement Variables")]
    [Range(1,10)]
    public float speed = 8f;

    [Range(10, 30)]
    public float jumpForce = 15f;

    [Range(0, 1)]
    public float radius = 0.2f;

    [Range(1, 2)]
    public int totalJumps = 2;

    [Range(0, 1)]
    public float stopJumpMultiplier = 0.5f;

    [Header("Object Variables")]
    public Transform groundCheck;    
    public GameObject dust;

    public LayerMask layerGround;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        numberJumps = totalJumps;
    }

    // Update is called once per frame
    void Update()
    {
        
        isOnFloor = Physics2D.OverlapCircle(groundCheck.position, radius, layerGround);
        
        if (body.velocity.y < 0f)
            isFalling = true;

        if (isOnFloor)
            numberJumps = totalJumps;

        if (Input.GetButtonDown("Jump"))
        {
            PressJump();
        }
        else if (Input.GetButtonUp("Jump"))
        {
            RelesedJump();
        }

        PlayerAnimation();
        
    }

    private void FixedUpdate()
    {
        MovePlayer(Input.GetAxisRaw("Horizontal"));
    }

    public void TouchMove(float value)
    {
        touchValue = value;
    }

    private void MovePlayer(float move)
    {

        if (!isOnFloor)
            body.velocity = new Vector2(move * (speed * 0.75f), body.velocity.y);
        else
            body.velocity = new Vector2(move * speed, body.velocity.y);

        if ((move > 0 && sprite.flipX == true) || (move < 0 && sprite.flipX == false))
        {
            Flip();
        }

        JumpPlayer();
    }
    
    public void PressJump()
    {

        if (numberJumps > 0)
        {
            numberJumps--;
            isJumping = true;
        }
        
    }

    public void RelesedJump()
    {
        if (body.velocity.y > 0)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y);
        }

    }

    private void JumpPlayer()
    {        
        if (isJumping)
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            isJumping = false;
            SpawnDust();
        }        
    }

    private void Flip()
    {
        sprite.flipX = !sprite.flipX;            
    }

    private void PlayerAnimation()
    {
        anim.SetFloat("Speed", Mathf.Abs(body.velocity.x));
        anim.SetBool("IsJumping", Mathf.Abs(body.velocity.y) > 0);
        anim.SetFloat("VelY", body.velocity.y);

        //landing animation
        if (isFalling && isOnFloor && body.velocity.y == 0f)
        {
            isFalling = false;
            SpawnDust();
        }
    }
      

    private void SpawnDust()
    {       
        if (dust != null)
        {
            Instantiate(dust, transform.position, Quaternion.identity);
        }
    }
   
}
