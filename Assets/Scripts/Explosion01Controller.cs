using UnityEngine;
using Mirror;
using System.Collections;

public class Explosion01Controller : NetworkBehaviour
{
    [SerializeField] private float liveTime = 1.5f;

    void Start()
    {
        StartCoroutine(die());
    }

    private IEnumerator die()
    {
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }



}
