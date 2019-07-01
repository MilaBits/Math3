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

    [SerializeField]
    private bool autoclick;

    public void Click(Vector2 position)
    {
        gameInput.HandleClick(position);
    }

    void Update()
    {
        if (autoclick)
        {
            if (time >= clickInterval)
            {
                gameInput.HandleClick(new Vector3(Random.value, Random.value));
                time = 0;
                return;
            }

            time += Time.deltaTime;
        }
    }
}