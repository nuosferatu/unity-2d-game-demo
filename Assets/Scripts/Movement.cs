using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public int Cherry = 0;
    // 私有变量不在 Editor 中显示，除非加了 [SerializeField]
    // 如：[SerializeField]private Rigidbody2D rb;
    public Rigidbody2D rb;
    public Collider2D coll;
    public Animator anim;

    public float speed, jumpForce;
    // public Transform cellingCheck;
    public Transform groundCheck;
    public LayerMask ground;

    public bool isJump, isGround;
    bool jumpPressed;
    int jumpCount;

    // UI
    public Text CherryNum;

    private bool isHurt = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }

        SwitchAnim();
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);

        if (!isHurt)
        {
            GroundMovement();
            Jump();
        }
        

        

        
    }

    void GroundMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);

        if (horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }
    }

    void Jump()
    {
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        if (jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
        else if (jumpPressed && jumpCount > 0 && !isGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
    }

    // 角色动画切换
    void SwitchAnim()
    {
        anim.SetBool("idle", true);
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));
        if (isGround)
        {
            anim.SetBool("falling", false);

        }
        else if (!isGround && rb.velocity.y > 0) // 起跳过程
        {
            anim.SetBool("jumping", true);
        }
        else if (rb.velocity.y < 0) // 下落过程
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }


        if (isHurt)
        {
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0.0f);
            if (Mathf.Abs(rb.velocity.x) < 0.1f) {
                isHurt = false;
                anim.SetBool("idle", true);
                anim.SetBool("hurt", false);
                
            }
        }
    }

    // 收集物品
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collection")
        {
            Destroy(collision.gameObject);
            Cherry++;
            CherryNum.text = Cherry.ToString();
        }
    }

    // 消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (anim.GetBool("falling"))
            {
                isJump = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount = 1;
                jumpPressed = false;
                Destroy(collision.gameObject);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                isHurt = true;
                anim.SetBool("hurt", true);
                rb.velocity = new Vector2(-7, rb.velocity.y);
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                isHurt = true;
                anim.SetBool("hurt", true);
                rb.velocity = new Vector2(7, rb.velocity.y);
            }
        }
    }

}
