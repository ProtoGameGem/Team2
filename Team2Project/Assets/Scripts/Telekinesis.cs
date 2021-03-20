using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] List<GameObject> ObjectsInField = new List<GameObject>();
    [SerializeField] float speed = 5f;
    GameObject SelectedObj = null;
    Vector3 size;
    Vector3 pos;

    private void Start()
    {
        size = new Vector3(transform.localScale.x * 1.45f, transform.localScale.y * 1.225f, transform.localScale.z);
    }

    private void OnDisable()
    {
        ObjectsInField.Clear();
        if (SelectedObj != null)
        {
            SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 1;
            Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), false);
            ToggleAnim(false);
            SelectedObj = null;
        }
    }

    private void Update()
    {
        if (transform.parent.localScale.x >= 1)
        {
            pos = transform.parent.position + new Vector3(0.4f, 0, 0);
        }
        else if (transform.parent.localScale.x <= -1)
        {
            pos = transform.parent.position + new Vector3(-0.2f, 0, 0);
        }

        DetectObjects();
        SelectObject();
    }

    private void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                for (int i = 0; i < ObjectsInField.Count; i++)
                {
                    if (ObjectsInField[i] == hit.collider.gameObject)
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        if (SelectedObj != null)
                        {
                            SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 1;
                            Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), false);
                            ToggleAnim(false);
                        }
                        SelectedObj = hit.collider.gameObject;
                        Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), true);
                        ToggleAnim(true);
                    }
                }
            }   
        }
    }

    private void ToggleAnim(bool toogle)
    {
        if (SelectedObj != null)
        {
            if (SelectedObj.name.Contains("Dream"))
            {
                SelectedObj.GetComponent<Animator>().SetBool("Telekinesis", toogle);
            }
        }
    }

    private void FixedUpdate()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        if (SelectedObj != null)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            bool moving = false;
            
            if(h != 0 || v != 0)
            {
                moving = true;
                SelectedObj.transform.Translate(new Vector3(h * speed * Time.fixedDeltaTime, v * speed * Time.fixedDeltaTime, 0));
            }

            if (moving)
            {
                if (SelectedObj.GetComponent<Rigidbody2D>() != null)
                {
                    SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 0;
                }
            }
            else
            {
                if (SelectedObj.GetComponent<Rigidbody2D>() != null)
                {
                    SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 1;
                }
            }
        }
    }

    private void DetectObjects()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, size, 0);
        int i = 0;
        while (i < hits.Length)
        {
            Collider2D hit = hits[i];
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
                if (SelectedObj == ObjectsInField[j])
                {
                    if (SelectedObj.GetComponent<Rigidbody2D>() != null)
                    {
                        SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 1;
                        Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), false);
                        ToggleAnim(false);
                    }
                    SelectedObj = null;
                }
                ObjectsInField.RemoveAt(j);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos, size);
    }
}
