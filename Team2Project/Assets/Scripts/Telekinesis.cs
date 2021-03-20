using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] List<GameObject> ObjectsInField = new List<GameObject>();
    Vector3 size;
    Vector3 pos;

    private void Start()
    {
        size = new Vector3(transform.localScale.x * 1.4f, transform.localScale.y * 1.225f, transform.localScale.z);
        pos = transform.parent.position + new Vector3(0.2f, 0, 0);
    }

    private void OnDisable()
    {
        ObjectsInField.Clear();
    }

    private void Update()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, size, 0);
        int i = 0;
        while (i < hits.Length)
        {
            Collider2D hit = hits[i];
            Debug.Log(hit.gameObject.layer);
            if (hit.gameObject.layer == 10)
            {
                if (!ObjectsInField.Contains(hit.gameObject))
                {
                    ObjectsInField.Add(hit.gameObject);
                }
            }
            i++;
        }


        for (int j = 0; j < ObjectsInField.Count; j++)
        {
            bool has = false;
            foreach (var hit in hits)
            {
                if (hit.gameObject == ObjectsInField[j])
                {
                    has = true;
                    break;
                }
            }

            if (!has)
            {
                ObjectsInField.RemoveAt(j);
            }
        }

    }

}
