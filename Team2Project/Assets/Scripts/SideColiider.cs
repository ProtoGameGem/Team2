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
            if (character.Dir > 0)
            {
                character.HitRightWall = true;
                character.HitLeftWall = false;
            }
            else if (character.Dir < 0)
            {
                character.HitLeftWall = true;
                character.HitRightWall = false;
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
