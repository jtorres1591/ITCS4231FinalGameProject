using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expiration : MonoBehaviour
{
    public float expirationTime;
    // Start is called before the first frame update
    void Start()
    {
        // Start the expiration time.
        StartCoroutine(Expire(expirationTime));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // A countdown until the object despawns.
    IEnumerator Expire(float countdownTime) {
        yield return new WaitForSeconds(countdownTime);
        Destroy(gameObject);
    }
}
