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


        if (tag == "Wall" || tag == "Floor")
        {
            float h = Input.GetAxis("Horizontal");
            if (h > 0)
            {
                character.HitRightWall = true;
            }
            else if (h < 0)
            {
                character.HitLeftWall = true;
            }

        }

        if (!character.Flying)
        {

            if (tag == "PushableObject")
            {
                character.InteractDreamRoll = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        Character character = parent.GetComponent<Character>();

        if (tag == "Wall" || tag == "Floor")
        {
            character.HitRightWall = false;
            character.HitLeftWall = false;
        }
    }
}
