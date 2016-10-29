﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Chara : MonoBehaviour
{
    // Debug
    public bool unlim_powers;

    // Player Info
    public int PlayerID { get; private set; }
    public Color PlayerColor { get; private set; }

    // References
    private Rigidbody2D rb;
    public Transform graphics;
    public ParticleSystem bump_particles_prefab, explosion_prefab;
    public ParticleSystem smoke_ps;
    private CameraShake camshake;

    // Movement
    public bool alive = true;
    public PhysicsMaterial2D physmat_normal, physmat_springs;
    private Vector2 des_move_dir;
    private float radius = 0.5f;
    private Vector2 start_pos;
    private Vector2 prev_pos;
    private float speed, normal_speed = 20f;
    private Coroutine squash_routine;
    private Waypoints waypoints;

    // Warp
    private float warp_secs = 1.5f;
    private Queue<Vector2> pos_history, velocity_history;

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

    public void Initialize(int id, Color color)
    {
        this.PlayerID = id;
        this.PlayerColor = color;

        start_pos = transform.position;
        Setup();

        if ((ControlScheme)InputExt.GetPlayerScheme(id) == ControlScheme.AI)
        {
            StartCoroutine(UpdateAI());
        }
        else
        {
            StartCoroutine(UpdateHuman());
        }

        GameManager.Instance.on_reset += Setup;
    }
    public void Setup()
    {
        alive = true;

        graphics.gameObject.SetActive(true);
        transform.position = start_pos;
        graphics.gameObject.SetActive(true);
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        rb.angularVelocity = 0;
        speed = normal_speed;

        smoke_ps.Play();

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
        waypoints = FindObjectOfType<Waypoints>();
        camshake = Camera.main.GetComponent<CameraShake>();
    }
    private IEnumerator UpdateHuman()
    {
        while (true)
        {
            while (Time.timeScale == 0) yield return null;

            des_move_dir = new Vector2(
                InputExt.GetAxis(PlayerID, Control.X),
                InputExt.GetAxis(PlayerID, Control.Y)).normalized;

            if (InputExt.GetKeyDown(PlayerID, Control.Action))
                UsePower();

            yield return null;
        }
    }
    private IEnumerator UpdateAI()
    {
        GameManager gm = GameManager.Instance;
        Chara opponent = gm.charas[1 - PlayerID];
        Vector2 waypoint = Vector2.zero;

        while (true)
        {
            while (Time.timeScale == 0) yield return null;

            if (IsChaser())
            {
                des_move_dir = (opponent.transform.position - transform.position).normalized;
                yield return null;
            }
            else
            {
                // Choose wp
                float dist_to_opp = 0;
                foreach (Vector2 wp in waypoints.Points)
                {
                    float dist = Vector2.Distance(wp, opponent.transform.position);
                    if (dist > dist_to_opp)
                    {
                        waypoint = wp;
                        dist_to_opp = dist;
                    }
                }

                for (float t = 0; t < 0.1f; t += Time.deltaTime)
                {
                    des_move_dir = (waypoint - (Vector2)transform.position).normalized;
                    yield return null;
                }
            }            
        }
    }
    private void FixedUpdate()
    {
        if (!alive || Time.timeScale == 0) return;

        rb.AddForce(des_move_dir * speed, ForceMode2D.Force);
        prev_pos = transform.position;

        // Warp history
        pos_history.Enqueue(prev_pos);
        velocity_history.Enqueue(rb.velocity);

        if (pos_history.Count > warp_secs / Time.fixedDeltaTime) pos_history.Dequeue();
        if (velocity_history.Count > warp_secs / Time.fixedDeltaTime) velocity_history.Dequeue();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!alive) return;

        Chara other = col.collider.GetComponent<Chara>();
        if (other != null)
        {
            if (chaser)
            {
                if (on_tag != null) on_tag(this, other);

                // Explosion
                ParticleSystem ps = Instantiate(explosion_prefab);
                ps.startColor = PlayerColor;
                ps.transform.position = col.contacts[0].point;
                camshake.Shake(CamShakeType.VeryStrong);

                // Hide and freeze
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                graphics.gameObject.SetActive(false);
                smoke_ps.Stop();
                alive = false;
            } 
            else
            {
                // Correct position
                transform.position = (Vector2)other.transform.position 
                    + col.contacts[0].normal * radius * 2f;

                // Hide and freeze
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                graphics.gameObject.SetActive(false);
                smoke_ps.Stop();
                alive = false;
            }
        }
        else if (col.collider.CompareTag("Wall"))
        {
            // Wall collision

            // Particles
            ParticleSystem ps = Instantiate(bump_particles_prefab);
            ps.transform.position = transform.position; // col.contacts[0].point;
            ps.startColor = chaser ? PlayerColor :Color.white;

            // Squash
            if (squash_routine != null) StopCoroutine(squash_routine);
            squash_routine = StartCoroutine(Squash(col));

            // Cam shake
            float i = Mathf.Pow(Mathf.Min(col.relativeVelocity.magnitude / 30f, 1), 4);
            i *= i;
            camshake.Shake(Mathf.Lerp(0.05f, 0.1f, i), Mathf.Lerp(0, 0.5f, i), 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!alive) return;

        Pickup pu = collider.GetComponent<Pickup>();
        if (pu != null)
        {
            power = pu.power;
            camshake.Shake(CamShakeType.Strong);
        }
    }

    private void UsePower()
    {
        if (power == Power.None || !alive || Time.timeScale == 0) return;

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
        camshake.Shake(CamShakeType.StrongNoF);
        speed = normal_speed * 5f;
        yield return new WaitForSeconds(0.25f);
        speed = normal_speed;
    }
    private void Blink()
    {
        //Vector2 dir = ((Vector2)transform.position - prev_pos).normalized;
        Vector2 dir = des_move_dir;
        float dist = 7;
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

        camshake.Shake(CamShakeType.Strong);
    }
    private void Swap()
    {
        Chara opponent = GameManager.Instance.charas[1 - PlayerID];
        Vector2 pos = transform.position;
        transform.position = opponent.transform.position;
        opponent.transform.position = pos;

        camshake.Shake(CamShakeType.Strong);
    }
    private void Repel()
    {
        Chara opponent = GameManager.Instance.charas[1 - PlayerID];
        Vector2 v = opponent.transform.position - transform.position;
        float dist = Mathf.Max(radius * 2f, v.magnitude);
        float force = 200f / Mathf.Pow(dist, 1.5f);
        opponent.rb.AddForceAtPosition(v.normalized * force, transform.position, ForceMode2D.Impulse);

        camshake.Shake(CamShakeType.Strong);
    }
    private void Warp()
    {
        transform.position = pos_history.Peek();
        rb.velocity = velocity_history.Peek();

        camshake.Shake(CamShakeType.Strong);
    }
    public IEnumerator Cloak()
    {
        camshake.Shake(CamShakeType.Strong);
        graphics.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        graphics.gameObject.SetActive(true);
    }
    public IEnumerator Springs()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.sharedMaterial = physmat_springs;
        camshake.Shake(CamShakeType.Strong);

        yield return new WaitForSeconds(1.5f);

        col.sharedMaterial = physmat_normal;
    }

    private IEnumerator Squash(Collision2D col)
    {
        graphics.rotation = Quaternion.Euler(0, 0, 
            Mathf.Atan2(col.contacts[0].normal.y, col.contacts[0].normal.x) * Mathf.Rad2Deg + 90);

        float amount = Mathf.Min(col.relativeVelocity.magnitude / 30f, 1) * 0.25f;

        Vector2 scale = Vector2.one;
        for (float t = 0; t < 1; t += Time.deltaTime * 8f)
        {
            scale.x = Mathf.Lerp(1 + amount, 1, t);
            scale.y = 1 - (scale.x - 1);
            graphics.transform.localScale = scale;

            yield return null;
        }

        graphics.transform.localScale = Vector2.one;
    }
}
