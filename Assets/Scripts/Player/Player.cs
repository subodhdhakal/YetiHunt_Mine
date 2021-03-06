﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    //variable for Diamond
    private int diamonds = 0;

    

    Rigidbody2D _rigid;
    private Animator _playeranim;

    [SerializeField]
    private float MaxHealth = 5;
    [SerializeField]
    private float speed = 0;
    [SerializeField]
    private float jumpForce = 5.0f;
    [SerializeField]
    private LayerMask ground_layer;
    private bool need_jump_reset = false;

    //Player HealthBar
    public Slider healthbar;

    public float Health { get; set; }

    public GameObject sword;

    // Start is called before the first frame update

    void Start()
    {
        _rigid = this.GetComponent<Rigidbody2D>();
        _playeranim = this.GetComponentInChildren<Animator>();

        Health = MaxHealth;

        healthbar.value = CalculateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(Health>0)
            Movement();
    }

    void Movement()
    {
        float moveSpeed = CrossPlatformInputManager.GetAxisRaw("Horizontal");

        if ((moveSpeed < 0 && transform.localScale.x < 0) || (moveSpeed > 0 && transform.localScale.x > 0))
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);

        _rigid.velocity = new Vector2(moveSpeed * speed, _rigid.velocity.y);
        _playeranim.SetFloat("moveSpeed", Mathf.Abs(moveSpeed));
    }

    public void Jump()
    {
        Debug.Log("jump.." + Time.time);
        if (!isGrounded())
            return;
        _playeranim.SetTrigger("jump");
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.PlayerJump);
        _rigid.velocity = new Vector2(_rigid.velocity.x, jumpForce);
        StartCoroutine(NeedJumpResetRoutine());
    }

    bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, ground_layer.value);
        //(1 << 8) layermask is 32-bit int array. To make layermask = 8(ground layer) , we need to bit-shift 1

        //Debug.DrawRay(transform.position, Vector2.down * 0.6f, Color.green); //Raycast Line Drawe

        if (hit.collider != null)
        {
            Debug.Log("hit:" + hit.collider.name); //Raycast collision check

            if (need_jump_reset == false)   // if not jumping - player is on ground
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator NeedJumpResetRoutine()
    {
        need_jump_reset = true;
        yield return new WaitForSeconds(0.1f); //wait 10th of second
        need_jump_reset = false;
    }

    public void Damage()
    {
        if (Health <= 0)
            return;

        Debug.Log("Player Damage called from Player script");
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.PlayerHurt);
        //_playeranim.SetTrigger("");
        float damageValue = 0.2f;
        Health -= damageValue;
        healthbar.value = CalculateHealth();

        if (Health <= 0)
        {
            SoundManagerScript.PlaySound(SoundManagerScript.Sound.PlayerDie);
            _playeranim.SetTrigger("Death");
            Debug.Log("Destroy object called player");
            StartCoroutine(DieAnimationResetRoutine());
            
        }
    }

    public void attack()
    {
        _playeranim.SetTrigger("Attack");
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.swordswing);
        //Debug.Log("Attack Animation has been played");
    }

    public void attack2()
    {
        _playeranim.SetTrigger("Attack2");
        SoundManagerScript.PlaySound(SoundManagerScript.Sound.PlayerAttack);
        // Debug.Log("Attack 1 Animation has been played");
    }

    float CalculateHealth()
    {
        return Health / MaxHealth;
    }

    IEnumerator DieAnimationResetRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(this.gameObject);
        UIManager.Instance.DisplayGameEndScreen("You lose!\nDiamonds Collected: " + diamonds);
    }

    public void addDiamond()
    {
        diamonds++;
        UIManager.Instance.diamondText.text = "Diamonds: " + diamonds ;
    }

    public int getDiamond()
    {
        return diamonds;
    }
}