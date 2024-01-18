using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Goals : NetworkBehaviour
{
    public GameObject explosion;
    public int id;//каждые из ворот соответствуют номеру игрока, назначается в инспекторе
    private float direction=1f;
    private void Update()
    {
        if (!isServer) { return; }
        transform.Translate(direction * Time.deltaTime, 0, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Destroy(Instantiate(explosion, other.transform.position, other.transform.rotation), 1f);
            if (!isServer) { return; }

            Skorer.instance.MakeGoal(other.name.Length - 1, id);

            Destroy(other.gameObject);
        }
        if (other.CompareTag("Wall")) { direction = -direction; }
    }
}
