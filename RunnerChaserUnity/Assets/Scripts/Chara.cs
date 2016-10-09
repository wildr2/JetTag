using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ControlScheme { WASD, Arrows }

public class Chara : MonoBehaviour
{
    private int player_id;
    private Color player_color;
    private ControlScheme control_scheme;

    private Rigidbody2D rb;
    public Transform graphics;
    public ParticleSystem bump_ps;

    private bool chaser = false;
    private bool frozen = false;
    private bool live = false;
    private Vector2 show_pos;
    private List<Vector2> pos_history;

    private Power power = Power.None;
    private float speed, normal_speed = 10f;

    public Action<Chara> on_collide_chara;




    // PUBLIC ACCESSORS

    public bool IsChaser()
    {
        return chaser;
    }
    public Color GetPlayerColor()
    {
        return player_color;
    }

    // PUBLIC MODIFIERS

    public void Initialize(int id, ControlScheme controls, Color color)
    {
        this.player_id = id;
        this.player_color = color;
        this.control_scheme = controls;
    }
    public void SetLive(bool live=true)
    {
        this.live = live;
        graphics.transform.position = transform.position;
        //if (live)
        //{
        //    pos_history.Clear();
        //}
    }
    public void SetChaser()
    {
        chaser = true;
        graphics.GetComponent<SpriteRenderer>().color = player_color;
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

        show_pos = transform.position;
        pos_history = new List<Vector2>();

        speed = normal_speed;
    }
    private void Update()
    {
        if (frozen) return;

        if (Input.GetKey(control_scheme == ControlScheme.Arrows ? KeyCode.RightShift : KeyCode.LeftShift))
            SetLive(true);
        else
            SetLive(false);

        if (live) show_pos = transform.position;
        else
        {
            if (pos_history.Count > 0)
            {
                float seconds_back = 3; //Mathf.Sin(Time.time) + 1;
                int i = Mathf.Max(0, pos_history.Count - 1 - (int)(seconds_back / Time.fixedDeltaTime));

                show_pos = pos_history[i];
            }
            //Color c = graphics.GetComponent<SpriteRenderer>().color;
            //graphics.GetComponent<SpriteRenderer>().color = Tools.SetColorAlpha(c, (5f-seconds_back) / 5f);
        }

        if (GetInputPower()) UsePower();

        graphics.transform.position = show_pos;
    }
    private void FixedUpdate()
    {
        Vector2 move_input = GetInputMove();
        Vector2 dir = move_input.normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);

        pos_history.Add(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Chara other = col.collider.GetComponent<Chara>();
        if (other != null)
        {
            if (on_collide_chara != null)
                on_collide_chara(this);

            if (chaser)
            {
                SetLive(true);
            }
            else
            {
                graphics.gameObject.SetActive(false);
            }

            frozen = true;
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
            Vector2 dir = (Vector2)transform.position - pos_history[pos_history.Count - 1];
            //transform.position = (Vector2)transform.position + dir * 20f;
            //rb.MovePosition(rb.position + dir * 20f);
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
