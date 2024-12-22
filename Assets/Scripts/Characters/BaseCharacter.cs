using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] private float walkspeed = 0.5f;
    [SerializeField] private float runspeed = 1.5f;
    [SerializeField] private float dodgespeed = 4.0f;
    [SerializeField] private float jumpForce = 7.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    private float speed;
    private float horizontal;
    private float vertical;

    private bool isMove;
    private bool isDodge;

    public bool grounded = true;
    private static float groundOffset = -0.14f;
    private static float groundRadius = 0.28f;
    public LayerMask groundLayers;

    private float verticalVelocity;
    private float limitVelocity = 53.0f;

    private Transform cameraTransform;

    private CharacterController controller;
    protected Animator animator;

    protected virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        cameraTransform = Camera.main.transform;

        speed = walkspeed;
        isMove = true;
    }

    
    protected virtual void Update()
    {
        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            FallAni(false);
        }
        else
        {
            FallAni(true);
        }

        if (verticalVelocity < limitVelocity)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    private void Move()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 dir = (cameraForward * vertical + cameraRight * horizontal) * speed;

        if (dir != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }

        dir.y += verticalVelocity;
        controller.Move(dir * Time.deltaTime);

        MoveAni(horizontal, vertical);
    }

    public void SetMove(float _horizontal, float _vertical)
    {
        if (!isMove)
        {
            horizontal = 0f;
            vertical = 0f;
            return;
        }

        horizontal = _horizontal;
        vertical = _vertical;
    }

    public void SetJump()
    {
        if (grounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);

            JumpAni();
        }
    }

    public void Aim()
    {
        Debug.Log("Aim");
    }

    public void SetCrouch()
    {
        Debug.Log("Crouch");
    }

    public void Run(bool isRun)
    {
        RunAni(isRun);

        if (isRun)
        {
            speed = runspeed;
        }
        else
        {
            speed = walkspeed;
        }
    }

    public void Dodge()
    {
        if (!isDodge)
        {
            StartCoroutine(DodgeLock());
        }
        DodgeAni();
    }

    IEnumerator DodgeLock()
    {
        isDodge = true;
        SetMove(false);

        yield return new WaitForSeconds(1.33f);

        SetMove(true);
        isDodge = false;
    }

    public void LockOn()
    {
        Debug.Log("LockOn");
    }

    public void SetMove(bool value)
    {
        isMove = value;
    }

    public abstract void UseQuickSlot(int index);

    public abstract void ChangeWeapon(int change);

    public abstract void FireEnter();
    public abstract void FireStay();
    public abstract void FireExit();
    public abstract void AimEnter();
    public abstract void AimStay();
    public abstract void AimExit();

    protected abstract void MoveAni(float horizontal, float vertical);
    protected abstract void JumpAni();
    protected abstract void FallAni(bool isFall);
    protected abstract void RunAni(bool isRun);
    protected abstract void FireAni();
    protected abstract void DodgeAni();
}
