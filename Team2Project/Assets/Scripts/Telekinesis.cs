using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] List<GameObject> ObjectsInField = new List<GameObject>();
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
            TooglePhysicsSetting(false);
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

    private void TooglePhysicsSetting(bool toogle)
    {
        if (SelectedObj != null)
        {
            // 염력으로 움질일 때
            if (toogle)
            {
                SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 0;
                SelectedObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                SelectedObj.GetComponent<Rigidbody2D>().gravityScale = 1;
                if (SelectedObj.gameObject.tag == "NonePushableObject")
                    SelectedObj.GetComponent<Rigidbody2D>().mass = 1000f;
                SelectedObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                SelectedObj.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            }
        }
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
                            TooglePhysicsSetting(false);
                            Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), false);
                            ToggleAnim(false);
                        }
                        // 처음 물건이 선택되는 시점
                        SelectedObj = hit.collider.gameObject;
                        TooglePhysicsSetting(true);
                        Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), true);
                        SelectedObj.transform.rotation = Quaternion.Euler(0, 0, 0);
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
                Vector2 movement = new Vector2(h, v).normalized;
                float speed = 200f;
                SelectedObj.GetComponent<Rigidbody2D>().velocity = new Vector2(movement.x * speed * Time.fixedDeltaTime, movement.y * speed * Time.fixedDeltaTime);
            }

            if (moving)
            {
                if (SelectedObj.GetComponent<Rigidbody2D>() != null)
                {
                    TooglePhysicsSetting(true);
                }
            }
            else
            {
                if (SelectedObj.GetComponent<Rigidbody2D>() != null)
                {
                    TooglePhysicsSetting(false);
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
                        Physics2D.IgnoreLayerCollision(SelectedObj.layer, LayerMask.NameToLayer("Player"), false);
                        TooglePhysicsSetting(false);
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
