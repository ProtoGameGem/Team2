using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformButton : MonoBehaviour
{
    public bool ButtonPressed = false;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            ButtonPressed = true;
            anim.SetBool("Pressed", ButtonPressed);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            ButtonPressed = false;
            anim.SetBool("Pressed", ButtonPressed);
        }
    }
}
