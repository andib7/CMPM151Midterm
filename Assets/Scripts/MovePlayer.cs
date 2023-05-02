﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//************** use UnityOSC namespace...
using UnityOSC;
//*************

public class MovePlayer : MonoBehaviour {

	public float speed;
	public Text countText;

	private Rigidbody rb;
	private int count;

//************* Need to setup this server dictionary...
Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
	//*************


	// Use this for initialization
	void Start () 
	{

		//************* Instantiate the OSC Handler...
		OSCHandler.Instance.Init();
		//OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", "ready");
		OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
		//*************

		//Application.runInBackground = true; //allows unity to update when not in focus

		rb = GetComponent<Rigidbody>();
		count = 0;
		setCountText();

		

		
	}
	

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//countText.text = "(x,y,z): (" + rb.position.x.ToString() + ", " + rb.position.y.ToString() + ", " + rb.position.z.ToString() + ")";



		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

		rb.AddForce(movement*speed);

		//************* Routine for receiving the OSC...
		OSCHandler.Instance.UpdateLogs();
		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;

		foreach (KeyValuePair<string, ServerLog> item in servers)
		{
			// If we have received at least one packet,
			// show the last received from the log in the Debug console
			if (item.Value.log.Count > 0)
			{
				int lastPacketIndex = item.Value.packets.Count - 1;

				//get address and data packet
				countText.text = item.Value.packets[lastPacketIndex].Address.ToString();
				countText.text += item.Value.packets[lastPacketIndex].Data[0].ToString();

			}
		}
		//*************
	}

	void OnTriggerEnter(Collider other) {

		string[] sounds = { "kick", "melody", "himat", "melody2", "snare", "bass", "clave"};


		if (other.gameObject.CompareTag ("Pick Up")) 
		{
			other.gameObject.SetActive (false);
			count = count + 1;
			setCountText ();

			if(count < 7)
            {
				OSCHandler.Instance.SendMessageToClient("pd", "/unity/"+sounds[count - 1],1);
			}
        }
	}

	void setCountText()
	{
		countText.text = "Count: " + count.ToString ();
		//************* Send the message to the client...
		//OSCHandler.Instance.SendMessageToClient("pd", "/unity/trigger", count);
		//*************
	}

}
