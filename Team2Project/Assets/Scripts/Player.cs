using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private List<Character> Characters = new List<Character>();
    [SerializeField] private List<GameObject> VirtualCam = new List<GameObject>();
    private bool AimFirstPlayer = true;
    private Character Character;
    private int EnabledCharacterIdx = 0;
    private Vector3 CamOffset = new Vector3(0, 3, -12);


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
}
