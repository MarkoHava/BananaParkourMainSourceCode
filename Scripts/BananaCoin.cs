using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BananaCoin : MonoBehaviour {


    public static int CollectedCoins = 0;
    public static int CoinsMaximumInLevel = 5;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == PublicConstants.Singleton.R_Player) {
            CollectedCoins++;
            Destroy(gameObject);
        }
    }

}
