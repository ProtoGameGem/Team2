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
    private Character Character;
    private int EnabledCharacterIdx = 0;
    private int CollectedTaroNumber = 0;

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
    }

    private void Update()
    {
        SwitchCharacter();
    }

    public void CollectTaro()
    {
        CollectedTaroNumber++;
        if (TaroNumText != null)
        {
            TaroNumText.text = (": " + CollectedTaroNumber.ToString());
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

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
