using UnityEngine;
using System.Collections;

public class OneUpPickup : MonoBehaviour {

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player.Instance.NumOfLives += 1;
            Destroy(gameObject);
            GameManager.Instance.PlayerLives();
        }
    }
}
