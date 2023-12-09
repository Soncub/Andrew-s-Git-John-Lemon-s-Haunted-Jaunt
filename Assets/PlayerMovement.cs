using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float sprintSpeed = 5f;
    public float regularSpeed = 2f;
    public float sprintDuration = 5f;
    public float sprintCooldown = 10f;
    public GameObject cooldowntext;
    public GameObject sprinttext;

    private bool isSprinting = false;
    private float sprintTimer = 5f;
    private float cooldownTimer = 10f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleSprintInput();
        UpdateSprintCooldown();
        MovePlayer();  // Added this line to move the player based on the speed.
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        m_Animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            m_AudioSource.Stop();
        }

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);
    }

    void OnAnimatorMove()
    {
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        m_Rigidbody.MoveRotation(m_Rotation);
    }

    void HandleSprintInput()
    {
        if (Input.GetKey(KeyCode.Space) && !isSprinting && cooldownTimer <= 0f)
        {
            StartSprint();
        }

        if (isSprinting)
        {
            sprintTimer -= Time.deltaTime;

            if (sprintTimer <= 0f)
            {
                StopSprint();
                cooldowntext.SetActive(true);
            }
        }
    }

    void MovePlayer()
    {
        float currentSpeed = isSprinting ? sprintSpeed : regularSpeed;
        Vector3 velocity = m_Movement * currentSpeed;
        m_Rigidbody.velocity = new Vector3(velocity.x, m_Rigidbody.velocity.y, velocity.z);
    }

    void StartSprint()
    {
        isSprinting = true;
        sprintTimer = sprintDuration;
    }

    void StopSprint()
    {
        sprinttext.SetActive(false);
        isSprinting = false;
        cooldownTimer = sprintCooldown;
    }

    void UpdateSprintCooldown()
    {
       

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                cooldownTimer = 0f;
                cooldowntext.SetActive(false);
                sprinttext.SetActive(true);
            }
        }
        if (cooldownTimer >= 10f)
        {
            cooldowntext.SetActive(true);
        }
    }
}
