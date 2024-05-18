
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Movimiento
    public float moveSpeed = 3f;
    public float jumpSpeed = 5f;
    public float fallMultiplier = 0.5f;
    public float lowJumpMultiplier = 1f;

    //Componentes
    Rigidbody2D Rb2D;
    public Transform MeleeAttackArea;

    //Banderas
    public bool canDoubleJump;
    public bool canMove;
    public bool canDash;
    public bool Dashing;
    public bool Attacking;
    public bool Shooting;
    public bool facingRignt;

    //Cooldowns
    public float dashCooldown = 0.5f;
    public float currentDashCooldown = 0;
    public float shootCooldown = 0.5f;
    public float currentShootCooldown = 0;

    //Duracion
    public float dashDuration = 0.2f;
    public float currentDashDuration = 0;
    public float meleeAttackDuration = 0.2f;
    public float currentMeleeAttackDuration = 0;

    //Ammo
    public float basicAmmoCount = 5;
    public float blastAmmoCount = 5;
    public float toxicAmmoCount = 5;
    public BulletTipe currentBulletTipe = BulletTipe.Basic;

    private void Awake()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        canMove = true;
        facingRignt = true;
    }

    private void Update()
    {
        AttackLogic();
        ChangeAmmo();
        CustomGravityLogic();
        DashLogic();
        DoubleJumpLogic();
        HorizontalMoveLogic();
        JumpLogic();
        ShootLogic();
    }
    void ChangeAmmo() 
    {
        if (Input.GetKeyDown("1") && currentBulletTipe != BulletTipe.Basic)
        {
            currentBulletTipe = BulletTipe.Basic;
            Debug.Log("Current Ammo Tipe is Basic");
        } 
        else if (Input.GetKeyDown("2") && currentBulletTipe != BulletTipe.Blast)
        {
            currentBulletTipe = BulletTipe.Blast;
            Debug.Log("Current Ammo Tipe is Blast");
        } 
        else if (Input.GetKeyDown("3") && currentBulletTipe != BulletTipe.Toxic)
        {
            currentBulletTipe = BulletTipe.Toxic;
            Debug.Log("Current Ammo Tipe is Toxic");
        }
    }
    void ShootLogic() 
    {
        if (canMove) 
        {
            if (Input.GetKey("c") && !Attacking && !Dashing) 
            {
                if (currentShootCooldown <= 0) 
                {
                    switch (currentBulletTipe) 
                    {
                        case BulletTipe.Basic:
                            if (basicAmmoCount > 0) 
                            {
                                Debug.Log("pium Basic");
                                basicAmmoCount--;
                                Debug.Log("Ammo Remaining: " + basicAmmoCount);
                            }
                            break;
                        case BulletTipe.Blast:
                            if (blastAmmoCount > 0)
                            {
                                Debug.Log("pium Blast");
                                blastAmmoCount--;
                                Debug.Log("Ammo Remaining: " + blastAmmoCount);
                            }
                            break;
                        case BulletTipe.Toxic:
                            if (toxicAmmoCount > 0)
                            {
                                Debug.Log("pium Toxic");
                                toxicAmmoCount--;
                                Debug.Log("Ammo Remaining: " + toxicAmmoCount);
                            }
                            break;
                    }
                    currentShootCooldown = shootCooldown;
                }
            }
        }
        if (currentShootCooldown > 0)
        {
            currentShootCooldown -= Time.deltaTime;
        }
    }
    void AttackLogic() 
    {
        if (canMove) 
        {
            if (Input.GetKeyDown("x") && !Attacking && !Dashing)
            {
                Attacking = true;
                currentMeleeAttackDuration = meleeAttackDuration;
                MeleeAttackArea.gameObject.SetActive(true);
            }
            if (Attacking) 
            {
                if (currentMeleeAttackDuration > 0) 
                {
                    currentMeleeAttackDuration -= Time.deltaTime;
                }
                else 
                {
                    Attacking = false;
                    MeleeAttackArea.gameObject.SetActive(false);
                }
            }
        }
    }
    void HorizontalMoveLogic() 
    {
        if (canMove && !Dashing)
        {
            if (Input.GetButton("Horizontal"))
            {
                if (Input.GetAxis("Horizontal") < 0)
                {
                    Rb2D.velocity = new Vector2(-moveSpeed, Rb2D.velocity.y);
                    if (!Attacking) 
                    { 
                        gameObject.transform.localScale = new Vector2(-1, 1);
                        facingRignt = false;
                    }
                }
                else if (Input.GetAxis("Horizontal") > 0)
                {
                    Rb2D.velocity = new Vector2(moveSpeed, Rb2D.velocity.y);
                    if (!Attacking) 
                    { 
                        gameObject.transform.localScale = new Vector2(1, 1);
                        facingRignt = true;
                    }
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
        if (CheckGround.isGrounded && !Dashing)
        {
            canDash = true;
        }
        // control del cooldown del dash
        if (currentDashCooldown > 0)
        {
            currentDashCooldown -= Time.deltaTime;
        }
        if (canMove)
        {
            // logica al activar el dash
            if (Input.GetKey("z") && currentDashCooldown <= 0 && !Dashing && canDash && !Attacking)
            {
                canDash = false;
                currentDashDuration = 0;
                Dashing = true;
                Rb2D.gravityScale = 0;
                if (!facingRignt)
                {
                    Rb2D.velocity = Vector2.left * moveSpeed * 3;
                }
                else
                {
                    Rb2D.velocity = Vector2.right * moveSpeed * 3;
                }
            }
            // logica durante la duracion del dash
            if (Dashing)
            {
                if (currentDashDuration < dashDuration)
                {
                    currentDashDuration += Time.deltaTime;

                }
                // logica despues de la duracion del dash
                else
                {
                    currentDashCooldown = dashCooldown;
                    Dashing = false;
                    Rb2D.gravityScale = 1;
                }
            }
        }
        else 
        {
            Dashing = false;
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
    public enum BulletTipe 
    {
        Basic,
        Blast,
        Toxic
    }
}