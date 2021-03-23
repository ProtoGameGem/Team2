using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Character> Characters = new List<Character>();
    [SerializeField] private List<GameObject> VirtualCam = new List<GameObject>();
    private bool AimFirstPlayer = true;
    public Character Character;
    private int EnabledCharacterIdx = 0;
    private int CollectedTaroNumber = 0;
    [SerializeField] int GoalTaroNumber = 3;
    [SerializeField] Text ClearTextObj;
    [SerializeField] GameObject ClearPanelObj;
    [SerializeField] string ClearTextString;
    [SerializeField] string NextSceneName;

    public Text TaroNumText;

    public static Player instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Start()
    {
        // EnabledCharacterIdx 캐릭터를 제외한 모든 캐릭터 컨트롤 비활성화
        for (int i = 0; i < Characters.Count; i++)
        {
            Characters[i].ConvertToDisableState();
        }
        // 캐릭터 캐싱
        Character = Characters[EnabledCharacterIdx];
        Character.ConvertToManualState();
        ClearPanelObj.SetActive(false);
    }

    private void Update()
    {
        SwitchCharacter();
        SharingAbilityHandler();
    }

    public void CollectTaro()
    {
        CollectedTaroNumber++;
        if (TaroNumText != null)
        {
            TaroNumText.text = (": " + CollectedTaroNumber.ToString());
        }

        if (CollectedTaroNumber >= GoalTaroNumber)
        {
            Clear();
        }
    }

    private void SwitchCharacter()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Character OldCharacter = Characters[EnabledCharacterIdx];
            OldCharacter.ConvertToDisableState();
            if (!OldCharacter.Flying)
            {
                OldCharacter.GetComponent<Rigidbody2D>().velocity = new Vector2(0, OldCharacter.GetComponent<Rigidbody2D>().velocity.y);            
                OldCharacter.AbilityOff(false);
            }
            EnabledCharacterIdx = (EnabledCharacterIdx + 1) % Characters.Count;
            // 캐릭터 캐싱
            Character = Characters[EnabledCharacterIdx];
            Character.ConvertToManualState();

            if (AimFirstPlayer)
            {
                VirtualCam[0].SetActive(false);
                VirtualCam[1].SetActive(true);
            }
            else
            {
                VirtualCam[0].SetActive(true);
                VirtualCam[1].SetActive(false);
            }
            AimFirstPlayer =! AimFirstPlayer;
        }
    }

    public void SharingAbilityHandler()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            Character CurCharacter = Characters[i];
            Character NextCharacter = Characters[((i + 1) % Characters.Count)];
            if (CurCharacter.SharingOn && !NextCharacter.Icon.activeSelf)
            {
                if (NextCharacter.InteractDreamRoll != null)
                {
                    DreamRoll dreamRoll = NextCharacter.InteractDreamRoll.GetComponent<DreamRoll>();
                    if (dreamRoll != null)
                    {
                        if (dreamRoll.Pushed)
                        {
                            dreamRoll.DisablePushMode();
                            NextCharacter.GetComponent<Animator>().SetBool("Pushing", false);
                        }
                    }
                }
                NextCharacter.ToogleSharedAbility(true);
                NextCharacter.Icon.SetActive(true);
            }
            else if (!CurCharacter.SharingOn)
            {
                NextCharacter.ToogleSharedAbility(false);
                NextCharacter.Icon.SetActive(false);
            }
        }   
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void Clear()
    {
        ClearTextObj.text = ClearTextString;
        ClearPanelObj.SetActive(true);
        StartCoroutine(MoveToNextScene());
    }

    IEnumerator MoveToNextScene()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene(NextSceneName);
    }
}
