using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public class Main : MonoBehaviour
{
	// Use this for initialization
	bool showQuitButton = false;
	bool enableMouse = false;
	Texture2D enableMouseButtonTexute;
	Texture2D disableMouseButtonTexute;
	DateTime prevTimeButtonAppeared;
	Texture2D quitButton;

	float ShiftCameraSmooth = 1f;
	public static Vector3 CameraTargetPos;

	void Start ()
	{
		disableMouseButtonTexute = Resources.Load("noMouse") as Texture2D;
		enableMouseButtonTexute = Resources.Load("useMouse") as Texture2D;
		quitButton = Resources.Load("QuitProg") as Texture2D;
		prevTimeButtonAppeared = DateTime.Now.Subtract(new TimeSpan(0,0,10));
		CameraTargetPos = Camera.main.transform.position;
	}
	
	void Update ()
	{	
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			showQuitButton = !showQuitButton;
		}

		ShiftCameraByKeyboard ();
	}

	void ShiftCameraByKeyboard ()
	{
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CameraTargetPos, ShiftCameraSmooth * Time.deltaTime);
	}

	void OnGUI ()
	{
		GUI.backgroundColor = new Color(0,0,0,0);

		if(Input.GetAxis("Mouse X") != 0)
		{
			prevTimeButtonAppeared = DateTime.Now;
		}

		if(DateTime.Now.Subtract(prevTimeButtonAppeared).TotalMilliseconds < 1500)
		{
			if (GUI.Button (new Rect (Screen.width * 0.9f, Screen.height * 0.05f, 128, 128), enableMouse ? enableMouseButtonTexute : disableMouseButtonTexute)) 
			{
				enableMouse = !enableMouse;
				InterpolateKeys.OnEnableMouse(enableMouse);
			}
		}
		if(showQuitButton)
		{
			if(GUI.Button(new Rect((Screen.width) /2 - 64, 3*(Screen.height)/4, 128, 128), quitButton))
			{
				Application.Quit();        
			}
		}

		Event e = Event.current;
		if (e.isKey)
		{
			CameraTargetPos = Camera.main.transform.position;
			if(e.keyCode == KeyCode.RightArrow)
			{
				AssemblyCSharp.Tools.ShiftView(AssemblyCSharp.Tools.Direction.Right);
			}
			else if(e.keyCode == KeyCode.LeftArrow)
			{
				AssemblyCSharp.Tools.ShiftView(AssemblyCSharp.Tools.Direction.Left);
			}
			//Debug.Log ("###############################################");
			//Debug.Log ("Move HandController to: " + HandController.HandTargetPosition);
			//Debug.Log ("Move Camera to: " + Camera.main.transform.position);
			//Debug.Log ("###############################################/n/n");
		}
	}
}
