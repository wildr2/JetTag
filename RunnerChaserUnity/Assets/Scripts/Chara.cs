using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ControlScheme { WASD, Arrows }

public class Chara : MonoBehaviour
{
    // Debug
    public bool unlim_powers;

    // Player Info
    public int PlayerID { get; private set; }
    public Color PlayerColor { get; private set; }
    private ControlScheme control_scheme;

    // References
    private Rigidbody2D rb;
    public Transform graphics;
    public ParticleSystem bump_ps;

    // Movement
    public PhysicsMaterial2D physmat_normal, physmat_springs;
    private float radius = 0.5f;
    private Vector2 start_pos;
    private Vector2 prev_pos;
    private float speed, normal_speed = 20f;

    // Warp
    private float warp_secs = 1f;
    private Queue<Vector2> pos_history, velocity_history;

    // Other State
    private bool chaser = false;
    private Power power = Power.Warp;
    
    // Events
    public Action<Chara, Chara> on_tag;
    

    // PUBLIC ACCESSORS

    public bool IsChaser()
    {
        return chaser;
    }
    public KeyCode GetActionKeyCode()
    {
        return 
            control_scheme == ControlScheme.Arrows ? KeyCode.Slash :
            control_scheme == ControlScheme.WASD ? KeyCode.Q : KeyCode.None;
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

        pos_history = new Queue<Vector2>();
        velocity_history = new Queue<Vector2>();
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
        if (Time.timeScale > 0 && GetInputPower()) UsePower();
    }
    private void FixedUpdate()
    {
        Vector2 move_input = GetInputMove();
        Vector2 dir = move_input.normalized;

        rb.AddForce(dir * speed, ForceMode2D.Force);
        prev_pos = transform.position;

        // Warp history
        pos_history.Enqueue(prev_pos);
        velocity_history.Enqueue(rb.velocity);
        if (pos_history.Count > warp_secs / Time.fixedDeltaTime) pos_history.Dequeue();
        if (velocity_history.Count > warp_secs / Time.fixedDeltaTime) velocity_history.Dequeue();
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
        if (power == Power.Dash) StartCoroutine(Dash());
        else if (power == Power.Blink) Blink();
        else if (power == Power.Cloak) StartCoroutine(Cloak());
        else if (power == Power.Repel) Repel();
        else if (power == Power.Springs) StartCoroutine(Springs());
        else if (power == Power.Swap) Swap();
        else if (power == Power.Warp) Warp();

        if (!unlim_powers) power = Power.None;
    }
    private IEnumerator Dash()
    {
        speed = normal_speed * 5f;
        yield return new WaitForSeconds(0.25f);
        speed = normal_speed;
    }
    private void Blink()
    {
        Vector2 dir = ((Vector2)transform.position - prev_pos).normalized;
        //Vector2 dir = GetInputMove();
        float dist = 5;
        Vector2 pos = (Vector2)transform.position + dir * dist;

        while (dist > 0 && Physics2D.OverlapCircle(pos, radius))
        {
            dist -= 0.1f;
            pos = (Vector2)transform.position + dir * dist;
        }
        if (dist <= 0)
        {
            // Fail
        }
        else
        {
            // Success
            transform.position = pos;
            //rb.velocity = dir * rb.velocity.magnitude;
        }
        
    }
    private void Swap()
    {
        Chara opponent = GameManager.Instance.charas[1 - PlayerID];
        Vector2 pos = transform.position;
        transform.position = opponent.transform.position;
        opponent.transform.position = pos;
    }
    private void Repel()
    {
        Chara opponent = GameManager.Instance.charas[1 - PlayerID];
        Vector2 v = opponent.transform.position - transform.position;
        float dist = Mathf.Max(radius * 2f, v.magnitude);
        float force = 200f / Mathf.Pow(dist, 2);
        opponent.rb.AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);
    }
    private void Warp()
    {
        transform.position = pos_history.Peek();
        rb.velocity = velocity_history.Peek();
    }
    public IEnumerator Cloak()
    {
        graphics.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        graphics.gameObject.SetActive(true);
    }
    public IEnumerator Springs()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.sharedMaterial = physmat_springs;

        yield return new WaitForSeconds(1.5f);

        col.sharedMaterial = physmat_normal;
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
        return Input.GetKeyDown(GetActionKeyCode());
    }
}
