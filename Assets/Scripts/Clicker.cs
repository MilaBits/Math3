using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{
    [SerializeField]
    private float clickInterval;

    private float time;

    [SerializeField]
    private GameInput gameInput;

    void Update()
    {
        if (time >= clickInterval)
        {
            gameInput.HandleClick(true);
            time = 0;
            return;
        }
        time += Time.deltaTime;
    }
}
