using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamRoll : MonoBehaviour
{
    public bool Pushed = false;
    GameObject PushCharacter = null;
    float Gap = 0;
    float FallTime = 0;
    float RotSpeed = 80;

    public void EnablePushMode(GameObject character)
    {
        PushCharacter = character;
        Gap = PushCharacter.transform.position.x - transform.position.x;
        GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
        Pushed = true;
    }

    public void DisablePushMode()
    {
        if (PushCharacter != null)
        {
            PushCharacter.GetComponent<Character>().InteractDreamRoll = null;
            PushCharacter = null;
        }
        Pushed = false;
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
    }

    private void Update()
    {
        if (PushCharacter != null)
        {
            float h = Input.GetAxis("Horizontal");
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

            if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                Vector2 velocity = new Vector2(0, rigidbody2D.velocity.y);
                rigidbody2D.velocity = velocity;
                PushCharacter.GetComponent<Rigidbody2D>().velocity = velocity;
            }

            transform.position = new Vector3(PushCharacter.transform.position.x - Gap, transform.position.y, transform.position.z);
            transform.Rotate(0, 0, RotSpeed * -h * Time.deltaTime);
            
            if (rigidbody2D.velocity.y < -0.3)
            {
                FallTime += Time.deltaTime;
                if (FallTime > 0.5f)
                {
                    FallTime = 0;
                    DisablePushMode();
                }
            }
        }
    }
}
