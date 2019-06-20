using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;
	
	private Vector3 target;
	private Vector3 start;
    
	public void Init(string text, Vector3 start, Vector3 target){
		this.text.text = text;
		this.start = start;
		this.target = target;
	}
}
