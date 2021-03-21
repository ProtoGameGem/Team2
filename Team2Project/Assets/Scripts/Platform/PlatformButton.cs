using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformButton : MonoBehaviour
{
    public bool ButtonPressed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            ButtonPressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            ButtonPressed = false;
        }
    }
}
