using UnityEngine;

public class FloatingMovement : MonoBehaviour
{
    [SerializeField] private float intensity = 1f; //흔들림 강도
    [SerializeField] private float time = 1f; //(왕복하는데 걸리는 시간) * (0.5)
    private float curTime = 0;

    private void Start()
    {
        InvokeRepeating("Floating", 0.01f, (0.01f / time));
    }

    private void OnEnable()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            transform.position = transform.position + new Vector3(0, 0.2f, 0);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            CancelInvoke("Floating");
            InvokeRepeating("Floating", 0.01f, (0.01f / time));
        }
    }

    private void OnDisable()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            CancelInvoke("Floating");
        }
    }

    //상하이동 함수
    private void Floating()
    {
        if (GetComponent<Rigidbody2D>() != null)
        {
            float h = Input.GetAxis("Horizontal");
            if (h != 0)
            {
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        //시간을 x로 위치를 y로 보고 Sin그래프를 이용
        if (curTime < 1)
        {
            curTime += 0.01f / time;
            if (curTime >= 1)
                curTime = 0;
        }
        float y = Mathf.Sin(curTime * 2 * Mathf.PI);
        transform.position = new Vector3(transform.position.x, transform.position.y + y * intensity, transform.position.z);
    }
}
