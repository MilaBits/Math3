using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    private Vector3 target;
    private Vector3 start;

    private float passedTime;
    [SerializeField]
    private float travelTime = .25f;

    public void Initialize(string text, Vector3 start, Vector3 target)
    {
        this.text.text = text;
        this.start = start;
        this.target = target;
    }

    private void Update()
    {
        passedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(start, target, passedTime / travelTime);

        if (passedTime > travelTime) Destroy(gameObject);
    }
}