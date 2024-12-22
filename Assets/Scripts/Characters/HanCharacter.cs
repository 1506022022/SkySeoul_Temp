using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanCharacter : BaseCharacter
{
    public Transform handTransform;
    [SerializeField] private Transform aimTransform;

    public float baseHP = 250.0f;
    public float additionalHP = 0;
    public float maxHP;
    public float currentHP = 90.0f;

    public float baseSP = 100.0f;
    public float additionalSP = 0;
    public float maxSP;
    public float currentSP = 80.0f;

    public float baseSH = 170.0f;
    public float additionalSH = 0;
    public float maxSH;
    public float currentSH = 170.0f;

    public int baseCost = 4;
    public int additionalCost = 0;
    public int maxCost;
    public int currentCost = 4;

    public float baseHackper = 0;
    public float additionalHackper = 0;
    public float maxHackper;

    private float shotTime;
    private float shotDelay = 0.5f;


    private HanInventoryManager inventoryManager;
    public int currentHand = -1;

    protected override void Start()
    {
        base.Start();

        inventoryManager = GameManager.Instance.inventoryManager as HanInventoryManager;

        SetHP(0);
        SetSP(0);
        SetSH(0);
        SetHackper(0);
    }

    protected override void Update()
    {
        base.Update();

        shotTime += Time.deltaTime;
    }

    public void RobotSelect()
    {
        Debug.Log("RobotSelect");
    }

    public void RobotSkill()
    {
        Debug.Log("RobotSkill");
    }

    public void RobotAction()
    {
        Debug.Log("RobotAction");
    }

    public void RobotRelease()
    {
        Debug.Log("RobotRelease");
    }

    public void Hacking()
    {
        Debug.Log("Hacking");
    }

    protected override void MoveAni(float horizontal, float vertical)
    {
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    protected override void FallAni(bool isFall)
    {
        animator.SetBool("IsFall", isFall);
    }

    protected override void JumpAni()
    {
        animator.SetTrigger("Jump");
    }

    protected override void RunAni(bool isRun)
    {
        animator.SetBool("IsRun", isRun);
    }

    protected override void FireAni()
    {
        animator.Play("Shot");
    }

    protected override void DodgeAni()
    {
        animator.SetTrigger("Roll");
    }

    public override void UseQuickSlot(int index)
    {
        if ((GameManager.Instance.inventoryManager as HanInventoryManager).quickSlot[index] != null)
        {
            Debug.Log((GameManager.Instance.inventoryManager as HanInventoryManager).quickSlot[index]);
        }
    }

    public override void ChangeWeapon(int change)
    {
        SetHand(change);

        GameManager.Instance.hudManager.SetWheelUI();
    }

    public override void FireEnter()
    {

    }

    public override void FireStay()
    {
        if (currentHand == 0)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;

                Debug.DrawRay(handTransform.position, ray.direction * 100f, Color.red, 2f);
            }
            else
            {
                Vector3 direction = ray.direction;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;

                Debug.DrawRay(handTransform.position, ray.direction * 100f, Color.red, 2f);
            }

            if (shotTime >= shotDelay)
            {
                shotTime = 0f;

                FireAni();
            }
        }
    }

    public override void FireExit()
    {
        
    }

    public override void AimEnter()
    {
        
    }

    public override void AimStay()
    {
        if (currentHand == 0)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                Vector3 direction = hit.point - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;

                Debug.DrawRay(handTransform.position, ray.direction * 100f, Color.red, 2f);
            }
            else
            {
                Vector3 direction = ray.direction;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;

                Debug.DrawRay(handTransform.position, ray.direction * 100f, Color.red, 2f);
            }
        }
    }

    public override void AimExit()
    {
        
    }

    public void SetHand(int change)
    {
        if (inventoryManager.gun == null && inventoryManager.phone == null && inventoryManager.laptop == null)
        {
            currentHand = -1;
            ClearHand();
            return;
        }

        currentHand = (currentHand + change + 3) % 3;

        while (true)
        {
            switch (currentHand)
            {
                case 0:
                    if (inventoryManager.gun != null)
                    {
                        OnHandGun();
                        return;
                    }
                    break;
                case 1:
                    if (inventoryManager.phone != null)
                    {
                        OnHandPhone();
                        return;
                    }
                    break;
                case 2:
                    if (inventoryManager.laptop != null)
                    {
                        OnHandLaptop();
                        return;
                    }
                    break;
            }

            currentHand = (currentHand + change + 3) % 3;
        }
    }

    public void SetHand()
    {
        if (inventoryManager.gun == null && inventoryManager.phone == null && inventoryManager.laptop == null)
        {
            currentHand = -1;
            ClearHand();
            return;
        }

        while (true)
        {
            switch (currentHand)
            {
                case 0:
                    if (inventoryManager.gun != null)
                    {
                        OnHandGun();
                        return;
                    }
                    break;
                case 1:
                    if (inventoryManager.phone != null)
                    {
                        OnHandPhone();
                        return;
                    }
                    break;
                case 2:
                    if (inventoryManager.laptop != null)
                    {
                        OnHandLaptop();
                        return;
                    }
                    break;
            }

            currentHand = (currentHand + 4) % 3;
        }
    }

    private void OnHandGun()
    {
        ClearHand();

        GameObject prefab = (GameManager.Instance.inventoryManager as HanInventoryManager).gun.gunInfo.itemPrefab;

        if (prefab != null)
        {
            Instantiate(prefab, handTransform);
            animator.SetLayerWeight(1, 1);
        }
    }

    private void OnHandPhone()
    {
        ClearHand();


    }

    private void OnHandLaptop()
    {
        ClearHand();


    }

    private void ClearHand()
    {
        foreach (Transform child in handTransform)
        {
            Destroy(child.gameObject);
        }

        animator.SetLayerWeight(1, 0);
    }

    public void SetHP(float value)
    {
        additionalHP += value;
        maxHP = baseHP + additionalHP;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void SetSP(float value)
    {
        additionalSP += value;
        maxSP = baseSP + additionalSP;
        if (currentSP > maxSP)
            currentSP = maxSP;
    }

    public void SetSH(float value)
    {
        additionalSH += value;
        maxSH = baseSH + additionalSH;
        if (currentSH > maxSH)
            currentSH = maxSH;
    }

    public void SetCost(int value)
    {
        additionalCost += value;
        maxCost = baseCost + additionalCost;
        currentCost += value;
        if (currentCost > maxCost)
            currentCost = maxCost;
        else if (currentCost < 0)
            currentCost = 0;
    }

    public void SetHackper(float value)
    {
        additionalHackper += value;
        maxHackper = baseHackper + additionalHackper;
    }
}
