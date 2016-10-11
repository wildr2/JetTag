using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ControlScheme { WASD, Arrows }

public class Chara : MonoBehaviour
{
    // Player Info
    public int PlayerID { get; private set; }
    public Color PlayerColor { get; private set; }
    private ControlScheme control_scheme;

    // References
    private Rigidbody2D rb;
    public Transform graphics;
    public ParticleSystem bump_ps;

    // Movement
    private Vector2 start_pos;
    private Vector2 prev_pos;
    private float speed, normal_speed = 20f;

    // Other State
    private bool chaser = false;
    private Power power = Power.None;
    
    // Events
    public Action<Chara, Chara> on_tag;
    

    // PUBLIC ACCESSORS

    public bool IsChaser()
    {
        return chaser;
    }


    // PUBLIC MODIFIERS

    public void Initialize(int id, ControlScheme controls, Color color)
    {
        this.PlayerID = id;
        this.PlayerColor = color;
        this.control_scheme = controls;

        start_pos = transform.position;
        Setup();

        GameManager.Instance.on_reset += Setup;
    }
    public void Setup()
    {
        transform.position = start_pos;
        graphics.gameObject.SetActive(true);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        speed = normal_speed;
    }
    public void SetChaser()
    {
        chaser = true;
        graphics.GetComponent<SpriteRenderer>().color = PlayerColor;
    }
    public void SetRunner()
    {
        chaser = false;
        graphics.GetComponent<SpriteRenderer>().color = Color.white;
    }


    // PRIVATE / PROTECTED MODIFIERS

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (GetInputPower()) UsePower();
    }
    private void FixedUpdate()
    {
        Vector2 move_input = GetInputMove();
        Vector2 dir = move_input.normalized;

        rb.AddForce(dir * speed, ForceMode2D.Force);
        prev_pos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Chara other = col.collider.GetComponent<Chara>();
        if (other != null)
        {
            if (chaser)
            {
                if (on_tag != null) on_tag(this, other);
            } 
            else
            {
                graphics.gameObject.SetActive(false);
            }
        }
        else
        {
            bump_ps.Play();
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Pickup pu = collider.GetComponent<Pickup>();
        if (pu != null)
        {
            power = pu.power;
        }
    }

    private void UsePower()
    {
        if (power == Power.Dash)
        {
            StartCoroutine(Dash());
        }
        power = Power.None;
    }
    private IEnumerator Dash()
    {
        speed = normal_speed * 5f;
        yield return new WaitForSeconds(0.25f);
        speed = normal_speed;
    }

    private Vector2 GetInputMove()
    {
        bool left=false, right=false, up=false, down=false;

        switch (control_scheme)
        {
            case ControlScheme.Arrows:
                left = Input.GetKey(KeyCode.LeftArrow);
                right = Input.GetKey(KeyCode.RightArrow);
                up = Input.GetKey(KeyCode.UpArrow);
                down = Input.GetKey(KeyCode.DownArrow);
                break;
            case ControlScheme.WASD:
                left = Input.GetKey(KeyCode.A);
                right = Input.GetKey(KeyCode.D);
                up = Input.GetKey(KeyCode.W);
                down = Input.GetKey(KeyCode.S);
                break;
        }

        Vector2 input = new Vector2();
        input.x = Convert.ToInt32(right) - Convert.ToInt32(left);
        input.y = Convert.ToInt32(up) - Convert.ToInt32(down);

        return input;
    }
    private bool GetInputPower()
    {
        switch (control_scheme)
        {
            case ControlScheme.Arrows:
                return Input.GetKeyDown(KeyCode.Slash);
            case ControlScheme.WASD:
                return Input.GetKeyDown(KeyCode.Q);
        }
        return false;
    }
}
