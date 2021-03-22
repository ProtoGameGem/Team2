using UnityEngine;

public class PlatformSet : MonoBehaviour
{
    [SerializeField] PlatformButton Button;
    [SerializeField] GameObject Platform;
    [SerializeField] float Speed = 10;
    [SerializeField] Transform from;
    [SerializeField] Transform to;

    private void Update()
    {
        if (Button.ButtonPressed)
        {
            Platform.transform.position = Vector3.MoveTowards(Platform.transform.position, to.position, Speed * Time.deltaTime);
        }
        else
        {
            Platform.transform.position = Vector3.MoveTowards(Platform.transform.position, from.position, Speed * Time.deltaTime);
        }
    }
}
