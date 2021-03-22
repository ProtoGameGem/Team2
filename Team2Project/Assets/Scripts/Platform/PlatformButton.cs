using UnityEngine;

public class PlatformButton : MonoBehaviour
{
    public bool ButtonPressed = false;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            if (!ButtonPressed)
            {
                ButtonPressed = true;
                anim.SetBool("Pressed", ButtonPressed);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "PushableObject" || collision.tag == "NonePushableObject")
        {
            if (ButtonPressed)
            {
                ButtonPressed = false;
                anim.SetBool("Pressed", ButtonPressed);
            }
        }
    }
}
