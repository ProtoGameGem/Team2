using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] List<GameObject> ObjectsInField = new List<GameObject>();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.layer == 10)
            {
                Debug.Log(collision.gameObject); 
                if (!ObjectsInField.Contains(collision.gameObject))
                {
                    ObjectsInField.Add(collision.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            ObjectsInField.Remove(collision.gameObject);
        }
    }
}
