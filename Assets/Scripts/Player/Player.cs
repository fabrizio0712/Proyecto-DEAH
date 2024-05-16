using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    //Movimiento
    public float moveSpeed = 3f;
    public float jumpSpeed = 5f;
    public float fallMultiplier = 0.5f;
    public float lowJumpMultiplier = 1f;

    //Componentes
    Rigidbody2D Rb2D;
    SpriteRenderer spriteRenderer;
    public GameObject MeleeAttackArea;

    //Banderas
    public bool canDoubleJump;
    public bool canMove;
    public bool dashing;
    public bool attacking;

    //Cooldowns
    public float dashCooldown = 0.5f;
    public float currentDashCooldown = 0;

    //Duracion
    public float dashDuration = 0.2f;
    public float currentDashDuration = 0;
    public float meleeAttackDuration = 0.2f;
    public float currentMeleeAttackDuration = 0;

    private void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        canMove = true;
    }

    private void Update()
    {
        AttackLogic();
        HorizontalMoveLogic();
        JumpLogic();
        DoubleJumpLogic();
        CustomGravityLogic();
        DashLogic();
    }
    void AttackLogic() 
    {
        if (canMove) 
        {
            if (Input.GetKeyDown("x") && !attacking)
            {
                attacking = true;
                currentMeleeAttackDuration = meleeAttackDuration;
                MeleeAttackArea.SetActive(true);
            }
        }
        if (currentMeleeAttackDuration > 0)
        {
            currentMeleeAttackDuration -= Time.deltaTime;
        }
        else 
        {
            attacking = false;
            MeleeAttackArea.SetActive(false);
        }
    }
    void HorizontalMoveLogic() 
    {
        if (canMove)
        {
            if (Input.GetButton("Horizontal"))
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    Rb2D.velocity = new Vector2(-moveSpeed, Rb2D.velocity.y);
                    spriteRenderer.flipX = true;
                    MeleeAttackArea.transform.localScale = new Vector2(-1, 1);
                    
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    Rb2D.velocity = new Vector2(moveSpeed, Rb2D.velocity.y);
                    spriteRenderer.flipX = false;
                    MeleeAttackArea.transform.localScale = new Vector2(1, 1);
                }
            }
            else
            {
                Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
            }
        }
        
    }
    void JumpLogic() 
    {
        if (Input.GetButtonDown("Jump") && CheckGround.isGrounded && canMove)
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
        }
    }
    void DoubleJumpLogic() 
    {
        if (CheckGround.isGrounded)
        {
            canDoubleJump = true;
        }
        else
        {
            if (Input.GetButtonDown("Jump") && canDoubleJump && canMove)
            {
                Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpSpeed);
                canDoubleJump = false;
            }

        }
    }
    void DashLogic() 
    {
        // control del cooldown del dash
        if (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }
        // logica al activar el dash
        if (Input.GetKey("z") && currentDashCooldown <= 0 && !dashing && canMove) 
        {
            canMove = false;
            currentDashDuration = 0;
            dashing = true;
            Rb2D.gravityScale = 0;
            if (spriteRenderer.flipX)
            {
                Rb2D.velocity = Vector2.left * moveSpeed * 3;
            }
            else 
            {
                Rb2D.velocity = Vector2.right * moveSpeed * 3;
            }
        }
        // logica durante la duracion del dash
        if (dashing)
        {
            if (currentDashDuration < dashDuration)
            {
                currentDashDuration += Time.deltaTime;
                
            }
            // logica despues de la duracion del dash
            else
            {
                canMove = true;
                currentDashCooldown = dashCooldown;
                dashing = false;
                Rb2D.gravityScale = 1;
            }
        }
    }
    void CustomGravityLogic() 
    {
        if (!CheckGround.isGrounded)
        {
            if (Rb2D.velocity.y < 0)
            {
                Rb2D.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
            }
            if (Rb2D.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                Rb2D.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
            }
        }
    }
}