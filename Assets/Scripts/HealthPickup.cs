using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {
    [SerializeField]
    private int healAmount;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Player.Instance.AddHeath(healAmount);
            Destroy(gameObject);
        }
    }
}
