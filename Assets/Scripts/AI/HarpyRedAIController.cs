﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpyRedAIController : MonoBehaviour {

	private Animator animator;
	public CharacterController control;
	public GameObject player;

	public int followRange, dashRange;
	public float height, moveSpeed, turnSpeed, maxDashTime, dashCooldown = 10;
	private float distance, currentSpeed, currentDashTime, dashCooldownCounter;
	private bool inRange;
	private Vector3 moveVector, playerXZPosition, harpyXZPosition;
	private enum HarpyRedAction {Neutral, Following, Dashing}
	private HarpyRedAction harpyRedAction;
	

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponentInChildren<Animator>();
		control = gameObject.GetComponent<CharacterController>();
		player = GameObject.FindWithTag("Player");
		currentSpeed = moveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		distance = Vector3.Distance(transform.position, player.transform.position);
		inRange = distance < followRange;
		moveVector.y = (height - transform.position.y)*Time.deltaTime; //slowly move to default height

		if(harpyRedAction == HarpyRedAction.Neutral && inRange){
			animator.SetInteger("flying", 0);
			harpyRedAction = HarpyRedAction.Following;
		}
		if(inRange){
			if(harpyRedAction == HarpyRedAction.Following){
				animator.SetInteger("flying", 0);
				moveVector = transform.TransformDirection(Vector3.forward) * currentSpeed * Time.deltaTime;
				harpyXZPosition = new Vector3(transform.position.x, 0, transform.position.z);
				playerXZPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
				transform.rotation = Quaternion.Lerp(transform.rotation, 
					Quaternion.LookRotation(playerXZPosition - harpyXZPosition), turnSpeed * Time.deltaTime);
						moveVector.y = (height - transform.position.y)*Time.deltaTime; 
				if(distance < 2){ 
					moveVector = Vector3.zero;
				}				
				
				if(distance < dashRange && Time.time > dashCooldownCounter){
					harpyRedAction = HarpyRedAction.Dashing;
					currentDashTime = 0;
					currentSpeed += 40;
				}
			}
			else if(harpyRedAction == HarpyRedAction.Dashing){
				moveVector = transform.TransformDirection(Vector3.forward) * currentSpeed * Time.deltaTime;
				currentDashTime += 1;
				
				if(currentDashTime > maxDashTime){
					animator.SetInteger("flying", 0);
					currentSpeed -= 40;
					dashCooldownCounter = Time.time + dashCooldown;
					harpyRedAction = HarpyRedAction.Following;
				}
				else if(currentDashTime+(maxDashTime*0.2)> maxDashTime){
					animator.SetInteger("flying", 3);
				}
				else if(currentDashTime < 20){
					animator.SetInteger("flying", 1);
					moveVector = Vector3.zero;
					harpyXZPosition = new Vector3(transform.position.x, 0, transform.position.z);
					playerXZPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
					transform.rotation = Quaternion.Lerp(transform.rotation, 
						Quaternion.LookRotation(playerXZPosition - harpyXZPosition), turnSpeed * Time.deltaTime);
				}
				else if(currentDashTime < maxDashTime){
					animator.SetInteger("flying", 2);
					moveVector.y = ((player.transform.position.y-5) - transform.position.y) * Time.deltaTime;
				}
			}
		}
		else if(!inRange){
			animator.SetInteger("flying", 0);
			currentSpeed = moveSpeed;
			harpyRedAction = HarpyRedAction.Neutral;
			moveVector = Vector3.zero;
		}
		control.Move(moveVector);
	}
}
