using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    bool IsCreditMenuOn = false;
    bool IsTutorialPanelOn = false;
    bool Pressed = false;

    [SerializeField] GameObject CreditPanel;
    [SerializeField] GameObject TutorialPaenl;

    private void Start()
    {
        CreditPanel.SetActive(false);
        TutorialPaenl.SetActive(false);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("game_map1");
    }

    public void OpenCreaditMenu()
    {
        CreditPanel.SetActive(true);
        IsCreditMenuOn = true;
    }

    public void OpenTutorialMenu()
    {
        TutorialPaenl.SetActive(true);
        IsTutorialPanelOn = true;
    }


    private void Update()
    {
        if (Pressed)
        {
            if (Input.GetMouseButton(0))
            {
                Pressed = false;
            }
        }

        if (!IsCreditMenuOn && !IsTutorialPanelOn)
        {
            if (Input.GetMouseButton(0))
            {
                Pressed = true;
            }
        }

        if (!Pressed)
        {
            if (IsCreditMenuOn)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    IsCreditMenuOn = false;
                    CreditPanel.SetActive(false);
                }
            }
            else if (IsTutorialPanelOn)
            {
                IsTutorialPanelOn = false;
                TutorialPaenl.SetActive(false);
            }
        }
    }
}
