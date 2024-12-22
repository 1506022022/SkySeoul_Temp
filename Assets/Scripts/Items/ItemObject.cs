using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int id;
    public int count;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            int remain = GameManager.Instance.inventoryManager.GetItem(id, count);

            if (remain == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                count = remain;
            }
        }
    }
}
