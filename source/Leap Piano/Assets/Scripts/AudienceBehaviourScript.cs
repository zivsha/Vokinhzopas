using UnityEngine;
using System.Collections;
using System;
using System.Timers;

public class AudienceBehaviourScript : MonoBehaviour
{
	/* "celebration", "celebration2", "celebration3",,*/
	string[] animations = {"applause",  "applause2"};
	bool playing;

	// Use this for initialization
	void Start ()
	{
		if(InterpolateKeys.LastTimePlayed == DateTime.MinValue)
		{
			InterpolateKeys.LastTimePlayed = DateTime.Now.Subtract (new TimeSpan (0, 0, 5));
		}
	}

	void Update ()
	{
		if(!playing)
			StartCoroutine(Play());
	}
	
	IEnumerator Play()
	{ 
		playing = true;  
		//Debug.Log("DateTime.Now = " + DateTime.Now + " LastTimePlayed = " + OnTouch.LastTimePlayed );
		DateTime now = DateTime.Now;
		//Wait for a random time
		float randWaitTime = UnityEngine.Random.Range (0, 1000)/1000f;
		yield return new WaitForSeconds(randWaitTime);

		//Skip animation change in a probability of 1/3
		int randReturn = UnityEngine.Random.Range (0, 2);
		if (randReturn != 0)
		{
			//If piano was played in last 3 seconds, applause		
			if(now.Subtract(InterpolateKeys.LastTimePlayed).TotalMilliseconds < 1500)
			{
				gameObject.animation.wrapMode = WrapMode.Once;
				int randomIndex = UnityEngine.Random.Range (0, animations.Length);
				gameObject.animation.Play(animations[randomIndex]);
			}
			else
			{
				gameObject.animation.wrapMode = WrapMode.Once;
				gameObject.animation.Play("idle");
			}

			yield return new WaitForSeconds(gameObject.animation.clip.length);
		}
		playing = false;
	}
}