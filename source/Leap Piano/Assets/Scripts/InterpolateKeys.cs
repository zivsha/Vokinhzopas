using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using AssemblyCSharp;
using System;

public class InterpolateKeys : MonoBehaviour
{	
	/***********************************************************************************
	 * 
	 * 								Environment Variables
	 * 
	/***********************************************************************************/

	public static float SmoothDown = 20f;
	public static float SmoothUp= 5f;

	public static float cameraSmoothness = 1f;
	public static float cameraTranslateStep = 1f;

	public static float RotateBlack = 0.05f;
	public static float RotateWhite = 4.0f;

	public static float TranslateBlack = -0.0025f;
	public static float TranslateWhite = 0.03f;

	public static DateTime LastTimePlayed;

	Color KeyDownColorWhite = new Color (0.5f, 0, 0, 1f);
	Color KeyDownColorBlack = new Color (0.25f, 0, 0, 1f);//Dark Red
	
	static bool IsHandVisible;
	static bool MouseEnabled;

	/***********************************************************************************
	 * 
	 * 								Member fields
	 * 
	/***********************************************************************************/

	private Vector3 m_startPos;
	private Vector3 m_targetPosition;

	private Quaternion m_startRot;
	private Quaternion m_targetRotation;

	private bool m_isKeyCurrentlyPressed;
	private bool m_isWhiteKey;

	Color m_targetColor;
	Color KeyDownColor;

	private Color m_keyColor;
	private System.DateTime m_prevCollisionTime;
	
	void Start () 
	{
		m_isWhiteKey = transform.parent.gameObject.name == "White_Keys";
		m_keyColor = renderer.material.color;
		m_targetColor = m_keyColor;

		m_startPos = transform.localPosition;
		m_targetPosition = m_startPos;

		m_startRot = transform.localRotation;
		m_targetRotation = m_startRot;

		m_prevCollisionTime = System.DateTime.Now.Subtract (new System.TimeSpan (0, 0, 2));
		MouseEnabled = false;
		m_isKeyCurrentlyPressed = false;

		LastTimePlayed = DateTime.Now.Subtract (new TimeSpan (0, 0, 10));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!IsHandVisible && !MouseEnabled)
			RestoreKeys();

		InterpolateKeyMovement ();
	}

	void RestoreKeys()
	{
		m_isKeyCurrentlyPressed = false;
		m_targetPosition = m_startPos;
		m_targetRotation = m_startRot;
		m_targetColor = m_keyColor;
	}

	void InterpolateKeyMovement ()
	{
		float smooth = m_isKeyCurrentlyPressed ? SmoothDown : SmoothUp;
		transform.localPosition = Vector3.Lerp(transform.localPosition, m_targetPosition, smooth * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, m_targetRotation, smooth * Time.deltaTime);			
		renderer.material.color = Color.Lerp(renderer.material.color ,m_targetColor, smooth * Time.deltaTime);
	}
	
	/***********************************************************************************
	 * 
	 * 								Events Handlers
	 * 
	/***********************************************************************************/

	void OnCollisionEnter(Collision other)
	{
		if(System.DateTime.Now.Subtract(m_prevCollisionTime).TotalMilliseconds < 500)
		   return;

		if(!other.transform.gameObject.name.StartsWith("Mesh"))
		{
			OnKeyDown();
		}
	}

	void OnKeyDown()
	{
		LastTimePlayed = DateTime.Now;
		m_prevCollisionTime = System.DateTime.Now;
		m_targetColor = m_isWhiteKey ? KeyDownColorWhite : KeyDownColorBlack;;
		m_isKeyCurrentlyPressed = true;
		float translateBy = m_isWhiteKey ? TranslateWhite : TranslateBlack;
		m_targetPosition = m_startPos + new Vector3 (0, translateBy, 0);
		m_targetRotation = Quaternion.Euler(0.0f, 0.0f, m_isWhiteKey ? RotateWhite : RotateBlack);
		audio.Play ();
	}

	void OnCollisionExit(Collision other)
	{
		if (!other.transform.gameObject.name.StartsWith ("Mesh"))
		{
			StartCoroutine(WaitAndRestore(100));
		}
	} 
	
	/***********************************************************************************
	 * 
	 * 								Auxiliary Methods
	 * 
	/***********************************************************************************/
	
	IEnumerator WaitAndRestore(float timeInMilliseconds)
	{
		var stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();
		while (stopwatch.ElapsedMilliseconds < timeInMilliseconds)
		{
			yield return null;
		}
		stopwatch.Stop();
		RestoreKeys();
	}

	/***********************************************************************************
	 * 
	 * 								Static Methods
	 * 
	/***********************************************************************************/

	public static void OnEnableMouse(bool mouseEnabled)
	{
		MouseEnabled = mouseEnabled;
		//Debug.Log (System.DateTime.Now + ":Mouse Controller is now " + (mouseEnabled ? "[enabled]" : "[disabled]"));
	}

	public static void UpdateHands (int num_hands)
	{
		IsHandVisible = num_hands > 0;
	}
	

	/***********************************************************************************
	 * 
	 * 								Mouse controll
	 * 
	/***********************************************************************************/
	
	void OnMouseEnter()
	{
		if (Input.GetMouseButton (0))
		{
			OnMouseDown ();
		}
	}
	
    void OnMouseExit()
	{
		OnMouseUp ();
	}
	
	void OnMouseDown()
	{
		if (!MouseEnabled)
			return;

		if(System.DateTime.Now.Subtract(m_prevCollisionTime).TotalMilliseconds < 500)
			return;

		OnKeyDown ();
	}

	void OnMouseUp ()
	{
		StartCoroutine(WaitAndRestore(100));
	}
}
