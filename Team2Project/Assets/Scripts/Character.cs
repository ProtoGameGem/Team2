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
    public GameObject Icon;

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

    // 염력
    public bool HasTelekinesisAbility = false;
    private bool TelekinesisOn = false;

    // 파쿠르
    public bool HasParkourAbility = false;
    private bool ShouldFallPakour = false;
    private bool ParkourOn = false;
    private float ParkourTime = 0f;
    private float ParkourJumpingTime = 0f;
    private bool EnoughFlyToParkour = false;

    //공유몽
    public bool SharingOn = false;

    private enum Ability {
        None, Telekinesis, Parkour
    }
    private Ability OriginalAbility = Ability.None;

    Animator Anim;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        actionState = ActionState.Idle;

        OriginalAbility = HasParkourAbility ? Ability.Parkour : OriginalAbility;
        OriginalAbility = HasTelekinesisAbility ? Ability.Telekinesis : OriginalAbility;
    }

    private void Update()
    {
        if (activeState == ActiveState.Manual)
        {
            //Debug.Log(rigidbody2D.velocity);
            MoveInputHandler();
            AbilityInputHandler();
        }
        VerticalHitCheck();
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
        if (HasTelekinesisAbility && !Flying)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TelekinesisOn = !TelekinesisOn;
                Telekinesis.SetActive(TelekinesisOn);
                Anim.SetBool("Walking", false);
                Anim.SetBool("Dashing", false);
                Anim.SetBool("Kinesis", TelekinesisOn);
                DashTime = 0f;
            }
        }
        if (HasParkourAbility && EnoughFlyToParkour)
        {
            if (!ShouldFallPakour)
            {
                if ((HitLeftWall && Dir == -1) || (HitRightWall && Dir == 1))
                {
                    if (!ParkourOn)
                    {
                        StopDash();
                        ParkourTime = 1f;
                        rigidbody2D.gravityScale = 0f;
                        h = v = 0;
                        Dir = Dir == 1 ? -1 : 1;
                        Flip();
                    }
                    ParkourOn = true;
                }
            }
        }

        if (ParkourTime > 0f)
        {
            ParkourTime -= Time.deltaTime;
            ParkourJumpingTime = 0f;
            rigidbody2D.velocity = Vector2.zero;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ParkourTime = 0f;
                ParkourOn = false;
                rigidbody2D.gravityScale = 1f;
                ParkourJumpingTime = 2f;
            }
            else if (ParkourTime <= 0f)
            {
                ParkourTime = 0f;
                ParkourOn = false;
                rigidbody2D.gravityScale = 1f;
                ShouldFallPakour = true;
                StopPakourJump();
            }
        }
    }

    public void SetOffAnim()
    {
        Anim.SetBool("Walking", false);
        Anim.SetBool("Dashing", false);
        Anim.SetBool("Kinesis", false);
    }

    public void AbilityOff(bool toogle)
    {
        if (HasTelekinesisAbility && !Flying)
        {
            TelekinesisOn = toogle;
            Telekinesis.SetActive(TelekinesisOn);
        }
        Anim.SetBool("Walking", false);
        Anim.SetBool("Dashing", false);
        Anim.SetBool("Kinesis", false);
    }

    private void MoveInputHandler()
    {
        if (TelekinesisOn) return;
        if (ParkourOn || ParkourJumpingTime > 0) return;

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
            Flip();
        }
    }

    private void Flip()
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
            }
        }
    }

    private bool FloorRayCast(Vector2 pos)
    {
        int floorMask = (1 << LayerMask.NameToLayer("Floor")) + (1 << LayerMask.NameToLayer("Obj")) + (1 << LayerMask.NameToLayer("Edge"));
        if (Physics2D.Raycast(pos, Vector2.down, 1f, floorMask))
        {
            FlyingBounce = false;
            Flying = false;
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            ShouldFallPakour = false;

            if (activeState == ActiveState.None)
            {
                rigidbody2D.velocity = new Vector2(0, 0);
            }
            return true;
        }
        return false;
    }

    private void VerticalHitCheck()
    {
        float gap = 0.2f;
        Vector2 pos = new Vector2(transform.position.x - gap, transform.position.y);
        bool Grounded = false;
        for (int i = 0; i < 3; i++)
        {
            if (FloorRayCast(pos + new Vector2(gap * i, 0)))
            {
                Grounded = true;
                break;
            }
            Debug.DrawRay(pos + new Vector2(gap * i, 0), Vector2.down * 1f, Color.cyan);
        }

        if (!Grounded)
        {
            Flying = true;
            if (InteractDreamRoll != null)
            {
                InteractDreamRoll.GetComponent<DreamRoll>().DisablePushMode();
            }
        }

        // 파쿠르 하기에 충분한 높이인지 체크
        int pakourFloorMask = (1 << LayerMask.NameToLayer("Floor")) + (1 << LayerMask.NameToLayer("Edge"));
        if (Physics2D.Raycast(transform.position, Vector2.down, 2f, pakourFloorMask))
        {
            EnoughFlyToParkour = false;
        }
        else
        {
            EnoughFlyToParkour = true;
        }
        Debug.DrawRay(transform.position, Vector2.down * 2f, Color.red);

        //머리위에 뭔가 충돌하면
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1), Vector2.up, 0.1f))
        {
            StopPakourJump();
        }
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 1), Vector2.up * 0.1f, Color.blue);
    }

    private void StopPakourJump()
    {
        ParkourTime = 0f;
        ParkourJumpingTime = 0f;
        ParkourOn = false;
        rigidbody2D.gravityScale = 1f;
        ShouldFallPakour = true;

        HorizontalFlyingSpeed = rigidbody2D.velocity.x;
        OriginalHorizontalFlyingSpeed = HorizontalFlyingSpeed;
    }

    private void StopDash()
    {
        DashDir = 0;
        DashTime = 0;
        PrevClickTime = CurClickTime = -1;
        PrevPressedKey = PressedKey = Key.None;
        actionState = (actionState & (~ActionState.Dash));
        Anim.SetBool("Dashing", false);
    }

    private void Move()
    {
        Anim.SetBool("Walking", false);
        if (FlyingBounce) h = 0;
        if (TelekinesisOn || ParkourOn)
        {
            rigidbody2D.velocity = Vector2.zero;
            return;
        }
        if (ParkourJumpingTime > 0)
        {
            ParkourJumpingTime -= Time.deltaTime * 3f;
            float speed = 35f * ParkourJumpingTime * Time.fixedDeltaTime;
            rigidbody2D.velocity = new Vector2(Dir * speed * 5f, speed * 7.5f);
            ShouldFallPakour = false;

            if (ParkourJumpingTime <= 0)
            {
                StopPakourJump();
            }
            return;
        }

        if (Boucning())
        {
            return;
        }
        if (!StartBounce())
        {
            if (ActionState.Dash == (actionState & ActionState.Dash))
            {
                float speed = DashSpeed * Multiplier * Time.fixedDeltaTime * DashTime;
                rigidbody2D.velocity = new Vector2(DashDir * speed, rigidbody2D.velocity.y);

                if (!Flying)
                {
                    DashTime -= (Time.deltaTime * 3);

                    if (DashTime < 0.1)
                    {
                        StopDash();
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
            rigidbody2D.velocity = new Vector2(HorizontalFlyingSpeed, jumpForce);
            Flying = true;
        }
    }

    private bool StartBounce()
    {
        if (HasParkourAbility) return false;
        if ((HitRightWall && h > 0.5) || (HitLeftWall && h < -0.5))
        {
            StopDash();
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

        if (tag == "Sharing")
        {
            SharingOn = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;

        if (tag == "Sharing")
        {
            SharingOn = false;
        }
    }

    public void ToogleSharedAbility(bool toogle)
    {
        if (OriginalAbility == Ability.Parkour)
        {
            HasTelekinesisAbility = toogle;
        }
        else if (OriginalAbility == Ability.Telekinesis)
        {
            HasParkourAbility = toogle;
        }
    }
}