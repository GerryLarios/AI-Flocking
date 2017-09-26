using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberController : MonoBehaviour {

	public float maxFov = 180;
	public float maxAcceleration;
	public float maxVelocity;

	//Wander Variables
	public float wanderJitter;
	public float wanderRadius;
	public float wanderDistance;
	public float wanderPriority;
	//Cohesion Variables
	public float cohesionRadius;
	public float cohesionPriority;
	//Alingment Variables
	public float alingmentRadius;
	public float alingmentPriority;
	//Separation Variables
	public float separationRadius;
	public float separationPriority;
	//Avoidance Variables
	public float avoidanceRadius;
	public float avoidancePriority;


}
