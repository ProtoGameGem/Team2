using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Key
{
    None = 1,
    Left = 2,
    Right = 4
}

public class Character : MonoBehaviour, ICharacter
{
    ActiveState activeState;
    ActiveState ICharacter.ActiveState
    {
        get { return activeState; }
        set { activeState = value; }
    }

    ActionState actionState;
    ActionState ICharacter.ActionState
    {
        get { return actionState; }
        set { actionState = value; }
    }

    Key PressedKey = Key.None;
    Key PrevPressedKey = Key.None;

    [SerializeField] private float Multiplier = 10;
    [SerializeField] private float Speed = 7;
    [SerializeField] private float MomentumSpeed = 8;
    [SerializeField] private float DashSpeed = 40;
    [SerializeField] private float JumpForce = 25;
    private float HorizontalFlyingSpeed = 0;
    private float OriginalHorizontalFlyingSpeed = 0;
    public bool Flying = false;

    private Rigidbody2D rigidbody2D;

    float ClickDelay = 0.5f;
    float DashTime = 0;
    float PrevClickTime = -1;
    float CurClickTime = -1;

    float v = 0;
    float h = 0;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        actionState = ActionState.Idle;
    }

    private void Update()
    {
        if (activeState == ActiveState.Manual)
        {
            InputHandler();
        }
    }

    private void FixedUpdate()
    {
        if (activeState == ActiveState.Manual)
        {
            Move();
        }
    }

    private void InputHandler()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        // 좌우 키 눌렸을 때
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SideKeyClicked(Key.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SideKeyClicked(Key.Right);
        }

        if (!Flying && Input.GetKeyDown(KeyCode.UpArrow))
        {
            actionState = (actionState | ActionState.Jump);
        }
    }

    private void SideKeyClicked(Key key)
    {
        actionState = (actionState | ActionState.Move);

        if ((actionState & ActionState.Dash) == ActionState.Dash) return;

        PrevPressedKey = PressedKey;
        PressedKey = key;

        PrevClickTime = CurClickTime;
        CurClickTime = Time.time;

        if (PrevClickTime != -1 && PrevPressedKey == PressedKey)
        {
            if (CurClickTime - PrevClickTime <= ClickDelay)
            {
                actionState = (actionState | ActionState.Dash);
                DashTime = 1;
                //Debug.Log("Dash");
            }
        }
    }


    private void Move()
    {
        // 공중 이동
        if (Flying)
        {
            rigidbody2D.velocity = new Vector2(HorizontalFlyingSpeed, rigidbody2D.velocity.y);
            float speed = MomentumSpeed * h * Multiplier * Time.fixedDeltaTime;
            float range = Mathf.Abs(OriginalHorizontalFlyingSpeed);
            HorizontalFlyingSpeed = Mathf.Clamp(HorizontalFlyingSpeed + speed, -range, range);
        }
        // 땅에서 대쉬 이동
        else if (ActionState.Dash == (actionState & ActionState.Dash))
        {
            float speed = DashSpeed * Multiplier * Time.fixedDeltaTime * DashTime;
            rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
            DashTime -= (Time.deltaTime * 2);
            if (DashTime < 0.1)
            {
                DashTime = 0;
                PrevClickTime = CurClickTime = -1;
                PrevPressedKey = PressedKey = Key.None;
                actionState = (actionState & (~ActionState.Dash));
                if (ActionState.Move == (actionState & ActionState.Move))
                {
                    rigidbody2D.velocity = new Vector2(Speed * Multiplier * Time.fixedDeltaTime, rigidbody2D.velocity.y);
                }
            }
        }
        // 땅에서 좌우 이동
        else if (ActionState.Move == (actionState & ActionState.Move))
        {
            float speed = Speed * Multiplier * Time.fixedDeltaTime;
            rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
        }
        // 점프 시작 설정
        if (ActionState.Jump == (actionState & ActionState.Jump))
        {
            actionState = (actionState & (~ActionState.Jump));
            float jumpForce = JumpForce * Multiplier * Time.fixedDeltaTime;
            HorizontalFlyingSpeed = rigidbody2D.velocity.x;
            OriginalHorizontalFlyingSpeed = HorizontalFlyingSpeed;
            Debug.Log(HorizontalFlyingSpeed);
            rigidbody2D.velocity = new Vector2(HorizontalFlyingSpeed, jumpForce);
            Flying = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            Flying = false;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            if (activeState == ActiveState.None)
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
        }
    }
}