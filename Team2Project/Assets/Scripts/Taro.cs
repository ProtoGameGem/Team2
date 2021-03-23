using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taro : MonoBehaviour
{
    bool Consumed = false;
    [SerializeField] float RotSpeed = 300f;
    [SerializeField] float UpSpeed = 1f;
    float time;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!Consumed)
            {
                Consumed = true;
                Player.instance.CollectTaro();
                GetComponent<BoxCollider2D>().enabled = false;
                time = 0;
                Debug.Log("TaroCollected");
            }
        }
    }

    private void Update()
    {
        if (Consumed)
        {
            transform.Rotate(Vector3.up * RotSpeed * Time.deltaTime);
            transform.Translate(Vector3.up * UpSpeed * Time.deltaTime);
            transform.localScale = transform.localScale * 0.999f;

            time += Time.deltaTime;
            if (time > 2f)
            {
                Destroy(gameObject);
            }
        }
    }

}
