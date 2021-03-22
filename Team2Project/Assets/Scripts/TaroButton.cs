using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaroButton : MonoBehaviour
{
    [SerializeField] GameObject Taros;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Taros.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            anim.SetBool("Pressed", true);
            Taros.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            anim.SetBool("Pressed", false);
            Taros.SetActive(false);
        }
    }
}
