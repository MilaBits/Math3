using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private int currentFrame = 0;
    private bool playing;

    private float fps = 10;

    private int index;

    public void Play(Vector2 position)
    {
        transform.position = position;
        animator.SetTrigger("Play");
        currentFrame = 0;
        playing = true;
    }
}