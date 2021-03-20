using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSet : MonoBehaviour
{
    [SerializeField] PlatformButton Button;
    [SerializeField] GameObject Platform;
    [SerializeField] float Speed = 10;
    [SerializeField] bool LoopOn = false;
    [SerializeField] Transform[] Waypoints;
    [SerializeField] float RestTime;
    [SerializeField] bool ForRide;

    int targetWaypointIdx = 0;
    bool SetOnce = false;
    Vector3 CurDir;
    float CurRestTime = 1f;

    private void Update()
    {
        if (Button.ButtonPressed)
        {
            if (!SetOnce)
            {
                SetOnce = true;
            }
            MovePlatform();
        }
        else if (!Button.ButtonPressed)
        {
            SetOnce = false;
        }
    }

    private void MovePlatform()
    {
        Vector3 to = Waypoints[targetWaypointIdx].position;
        if (CurRestTime == 0)
        {
            Platform.transform.position = Vector3.MoveTowards(Platform.transform.position, to, Speed * Time.deltaTime);
        }

        if (Vector3.Distance(to, Platform.transform.position) < 0.001f)
        {
            targetWaypointIdx = (targetWaypointIdx + 1) % Waypoints.Length;
            if (targetWaypointIdx == 0)
            {
                if (LoopOn)
                {
                    CurRestTime = RestTime;
                }
                else
                {
                    CurRestTime = -1f;
                }
            }
        }

        if (CurRestTime > 0)
        {
            CurRestTime -= Time.deltaTime;
            if (CurRestTime < 0)
                CurRestTime = 0f;
        }

        Platform.transform.Translate(CurDir * Speed * Time.deltaTime);
    }

   

}
