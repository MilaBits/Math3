using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        //TODO: make background clickable during tutorial
        Debug.DrawLine(position, position + new Vector2(1, 0), Color.red, 1f);
        Debug.DrawLine(position, position + new Vector2(0, 1), Color.red, 1f);
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