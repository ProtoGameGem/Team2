using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideColiider : MonoBehaviour
{
    GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.GetComponentInParent<Character>().gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        Character character = parent.GetComponent<Character>();

        if (!character.Flying)
        {
            if (tag == "Wall")
            {

            }
            else if (tag == "PushableObject")
            {
                character.InteractDreamRoll = collision.gameObject;
            }
        }
    }
}
