using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaroButton : MonoBehaviour
{
    [SerializeField] GameObject Taros;
    Animator anim;
    bool ButtonPressed = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        Taros.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            if (!ButtonPressed)
            {
                anim.SetBool("Pressed", true);
                Taros.SetActive(true);
                ButtonPressed = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            if (ButtonPressed)
            {
                anim.SetBool("Pressed", false);
                Taros.SetActive(false);
                ButtonPressed = false;
            }
        }
    }
}
