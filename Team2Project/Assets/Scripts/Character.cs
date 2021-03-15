using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Key
{
    None = 1, 
    Left = 2,
    Right = 4
}

public class Character : MonoBehaviour
{
    ICharacterBehaviour characterBehaviour;

    ActiveState CurrentActiveState = ActiveState.None;
    ActionState CurrentActionState = ActionState.Idle;
    Key PressedKey = Key.None;
    Key PrevPressedKey = Key.None;

    [SerializeField] private float Multiplier = 1;
    [SerializeField] private float Speed = 8;
    [SerializeField] private float DashSpeed = 50;

    private Rigidbody2D rigidbody2D;

    int Clicked = 0;
    float ClickTime = 0;
    float ClickDelay = 0.5f;
    float DashTime = 0;

    float v = 0;
    float h = 0;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        InputHandler();
        Move();
    }

    private void InputHandler()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        if (CurrentActionState != ActionState.Dash)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CurrentActionState = ActionState.Move;
                PrevPressedKey = PressedKey;
                PressedKey = Key.Left;
                Clicked++;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CurrentActionState = ActionState.Move;
                PrevPressedKey = PressedKey;
                PressedKey = Key.Right;
                Clicked++;
            }

            if (Clicked > 0)
            {
                ClickTime += Time.deltaTime;
            }
            if (Clicked > 1)
            {
                if (ClickTime <= ClickDelay && PrevPressedKey == PressedKey)
                {
                    CurrentActionState = ActionState.Dash;
                    DashTime = 1;
                }
                ClickTime = 0;
                Clicked = 0;
            }
        }
    }

    private void Move()
    {    
        if (CurrentActionState == ActionState.Dash)
        {
            float speed = DashSpeed * Multiplier * Time.fixedDeltaTime * DashTime;
            rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
            DashTime -= Time.deltaTime;
            if (DashTime < 0.1)
            {
                DashTime = 0;
                CurrentActionState -= ActionState.Dash;
            }
        }
        else
        {
            float speed = Speed * Multiplier * Time.fixedDeltaTime;
            rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
        }
    }
}
