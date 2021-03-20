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

    private float DashDir = 0;
    public float Dir = 0;
    private float HorizontalFlyingSpeed = 0;
    private float OriginalHorizontalFlyingSpeed = 0;
    public bool Flying = false;
    public GameObject InteractDreamRoll = null;
    public bool HitLeftWall = false;
    public bool HitRightWall = false;
    public bool Pushing = false;

    private Rigidbody2D rigidbody2D;

    float ClickDelay = 0.5f;
    float DashTime = 0;
    float PrevClickTime = -1;
    float CurClickTime = -1;
    float BounceTime = 0;
    bool FlyingBounce = false;

    float v = 0;
    float h = 0;

    [SerializeField] private GameObject Telekinesis;

    public bool HasTelekinesisAbility = false;
    public bool HasParkourAbility = false;

    private bool TelekinesisOn = false;

    Animator Anim;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        actionState = ActionState.Idle;
    }

    private void Update()
    {
        if (activeState == ActiveState.Manual)
        {
            MoveInputHandler();
            AbilityInputHandler();
        }
    }

    private void FixedUpdate()
    {
        if (activeState == ActiveState.Manual)
        {
            Move();
        }
    }

    private void AbilityInputHandler()
    {
        if(HasTelekinesisAbility && !Flying)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TelekinesisOn = !TelekinesisOn;
                Telekinesis.SetActive(TelekinesisOn);
                Anim.SetBool("Walking", false);
            }
        }
    }

    public void AbilityOff(bool toogle)
    {
        if (HasTelekinesisAbility && !Flying)
        {
            TelekinesisOn = toogle;
            Telekinesis.SetActive(TelekinesisOn);
            Anim.SetBool("Walking", TelekinesisOn);
        }
    }

    private void MoveInputHandler()
    {
        if (TelekinesisOn) return;

        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        Dir = h < 0 ? -1 : h > 0 ? 1 : Dir;

        // 좌우 키 눌렸을 때
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SideKeyClicked(Key.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SideKeyClicked(Key.Right);
        }

        if (!Flying && Input.GetKeyDown(KeyCode.UpArrow) && !Pushing)
        {
            actionState = (actionState | ActionState.Jump);
        }

        // 밀기
        if (!Flying && InteractDreamRoll != null && Input.GetKeyDown(KeyCode.Q))
        {
            DreamRoll dreamRoll = InteractDreamRoll.GetComponent<DreamRoll>();
            if (dreamRoll != null)
            {
                if (dreamRoll.Pushed)
                {
                    dreamRoll.DisablePushMode();
                }
                else
                {
                    dreamRoll.EnablePushMode(gameObject);
                }
            }
        }
        if (!Pushing)
        {
            Vector3 scale = transform.localScale;
            if (Dir > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
            }
            else if (Dir < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
            }
        }
    }


    private void SideKeyClicked(Key key)
    {
        if (((actionState & ActionState.Dash) == ActionState.Dash) || Pushing) return;

        actionState = (actionState | ActionState.Move);

        PrevPressedKey = PressedKey;
        PressedKey = key;

        PrevClickTime = CurClickTime;
        CurClickTime = Time.time;

        // 대쉬
        if (PrevClickTime != -1 && PrevPressedKey == PressedKey)
        {
            if (CurClickTime - PrevClickTime <= ClickDelay)
            {
                actionState = (actionState | ActionState.Dash);
                DashTime = 1;
                DashDir = h < 0 ? -1 : 1;
                Anim.SetBool("Dashing", true);
                //Debug.Log("Dash");
            }
        }
    }


    private void Move()
    {
        Anim.SetBool("Walking", false);
        if (FlyingBounce) h = 0;
        if (TelekinesisOn)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }
        if (Boucning())
        {
            return;
        }
        if (!StartBounce())
        {
            // 땅에서 대쉬 이동
            if (ActionState.Dash == (actionState & ActionState.Dash))
            {
                float speed = DashSpeed * Multiplier * Time.fixedDeltaTime * DashTime;
                rigidbody2D.velocity = new Vector2(DashDir * speed, rigidbody2D.velocity.y);

                if (!Flying)
                {
                    DashTime -= (Time.deltaTime * 3);

                    if (DashTime < 0.1)
                    {
                        DashDir = 0;
                        DashTime = 0;
                        PrevClickTime = CurClickTime = -1;
                        PrevPressedKey = PressedKey = Key.None;
                        actionState = (actionState & (~ActionState.Dash));
                        Anim.SetBool("Dashing", false);
                    }
                }
                else
                {
                    if (DashTime > 0.5)
                    {
                        DashTime -= (Time.deltaTime * 3);
                    }
                }
            }
            // 공중 이동
            else if (Flying)
            {
                rigidbody2D.velocity = new Vector2(HorizontalFlyingSpeed, rigidbody2D.velocity.y);
                float speed = MomentumSpeed * h * Multiplier * Time.fixedDeltaTime;
                float range = Mathf.Abs(OriginalHorizontalFlyingSpeed);
                range = range == 0 ? (Speed * Multiplier * Time.fixedDeltaTime) : range;
                HorizontalFlyingSpeed = Mathf.Clamp(HorizontalFlyingSpeed + speed, -range, range);
            }
            // 땅에서 좌우 이동
            else if (ActionState.Move == (actionState & ActionState.Move))
            {
                float speed = Speed * Multiplier * Time.fixedDeltaTime;
                rigidbody2D.velocity = new Vector2(h * speed, rigidbody2D.velocity.y);
                if (h != 0)
                {
                    Anim.SetBool("Walking", true);
                }
            }
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

    private bool StartBounce()
    {
        if ((HitRightWall && h > 0.5) || (HitLeftWall && h < -0.5))
        {
            FlyingBounce = true;
            rigidbody2D.AddForce(new Vector2(0, 175 * Time.deltaTime), ForceMode2D.Impulse);
            BounceTime = 1f;
            return true;
        }
        return false;
    }

    private bool Boucning()
    {
        if (BounceTime > 0)
        {
            if (FlyingBounce && BounceTime < 0.5f)
            {
                return true;
            }
            BounceTime -= Time.deltaTime * 2f;

            if (BounceTime < 0)
            {
                BounceTime = 0;
                rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
                return false;
            }
            else
            {
                float power = 100 * -Dir;
                rigidbody2D.velocity = new Vector2(BounceTime * power * Time.fixedDeltaTime, rigidbody2D.velocity.y);
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Floor" || tag == "PushableObject" || tag == "NonePushableObject")
        {
            FlyingBounce = false;
            Flying = false;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            if (activeState == ActiveState.None)
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Floor")
        {
            FlyingBounce = false;
            Flying = false;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

            if (activeState == ActiveState.None)
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Floor")
        {
            Flying = true;
            if (InteractDreamRoll != null)
            {
                InteractDreamRoll.GetComponent<DreamRoll>().DisablePushMode();
            }
        }
    }
}