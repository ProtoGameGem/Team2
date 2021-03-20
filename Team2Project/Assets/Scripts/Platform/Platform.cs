using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Dictionary<string, Transform> Passangers = new Dictionary<string, Transform>(); 
   
    public bool ForRide;
    bool SetOnce = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ForRide)
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PushableObject" || collision.gameObject.tag == "NonePushableObject")
            {
                if (!SetOnce)
                {
                    SetOnce = true;
                    if (!Passangers.ContainsKey(collision.gameObject.name))
                    {
                        Passangers.Add(collision.gameObject.name, collision.transform.parent);
                    }
                }
                collision.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "PushableObject" || collision.gameObject.tag == "NonePushableObject")
        {
            collision.transform.parent = Passangers[collision.gameObject.name];

            if (Passangers.ContainsKey(collision.gameObject.name))
            {
                Passangers.Remove(collision.gameObject.name);
            }
            SetOnce = false;
        }
    }
}
