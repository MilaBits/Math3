using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
	[SerializeField]
	private TileGrid grid;
	
	[SerializeField]
	private TextMeshProUGUI timeText;
	
	private bool running = false;
	
	float remainingTime;
	
	protected void Start()
	{
		grid.GameRules.NextAnswerEvent.AddListener(AddBonusTime);
	}
	
	public void StartTimer(){
		remainingTime = grid.GameRules.GameTime;
		running = true;
	}
	
	public void StopTimer(){
		running = false;
	}
	
	public void AddBonusTime(){
		remainingTime += grid.GameRules.ScoreBonusTime;
		Debug.Log("Time Added!");
	}

    void Update()
    {
	    if (running) {
	    	remainingTime -= Time.deltaTime * 1000;
	    	
	    	int minutes = (int) ((remainingTime / (1000*60)) % 60);
	    	int seconds = (int) (remainingTime / 1000) % 60;
	    	timeText.text = string.Format("{0}:{1}", minutes, seconds);
	    }
    }
}
