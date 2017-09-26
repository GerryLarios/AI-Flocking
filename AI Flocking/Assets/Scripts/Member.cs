using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Member : MonoBehaviour {

	public Vector3 position;
	public Vector3 velocity;
	public Vector3 acceleration;

	public Level level;
	public MemberController conf;

	Vector3 wanderTarget;

	public GameObject particle;

	void Start(){
		level = FindObjectOfType<Level> ();
		conf = FindObjectOfType<MemberController> ();

		position = transform.position;
		velocity = new Vector3 (Random.Range (-3, 3), Random.Range (-3, 3), 0);
	}

	void Update(){
		acceleration = combine ();
		acceleration = Vector3.ClampMagnitude (acceleration, conf.maxAcceleration);
		velocity = velocity + acceleration * Time.deltaTime;
		velocity = Vector3.ClampMagnitude (velocity, conf.maxVelocity);
		position = position + velocity * Time.deltaTime;
		wrapAround (ref position, -level.bounds, level.bounds);
		transform.position = position;

	}

	protected Vector3 wander(){
		float jitter = conf.wanderJitter * Time.deltaTime;
		wanderTarget += new Vector3 (randomBinomial () * jitter, randomBinomial () * jitter, 0);
		wanderTarget = wanderTarget.normalized;
		wanderTarget *= conf.wanderRadius;
		Vector3 targetInLocalSpace = wanderTarget + new Vector3 (conf.wanderDistance, conf.wanderDistance, 0);
		Vector3 targetInWorldSpace = transform.TransformPoint (targetInLocalSpace);
		targetInWorldSpace -= this.position;

		return targetInWorldSpace.normalized;
	}

	Vector3 cohesion(){
		Vector3 cohesionVector = new Vector3 ();
		int countMembers = 0;

		var neighbors = level.GetNeighbors (this, conf.cohesionRadius);
		if (neighbors.Count == 0) {
			return cohesionVector;
		}

		foreach (var member in neighbors) {
			if (isInFov (member.position)) {
				cohesionVector += member.position;
				countMembers++;
			}
		}

		if (countMembers == 0) {
			return cohesionVector;
		}

		cohesionVector /= countMembers;
		cohesionVector = cohesionVector - this.position;
		cohesionVector = Vector3.Normalize (cohesionVector);

		return cohesionVector;
	}

	Vector3 alignment(){
		Vector3 alignVector = new Vector3 ();
		var members = level.GetNeighbors (this, conf.alingmentRadius);
		if (members.Count == 0) {
			return alignVector;
		}

		foreach (var member in members) {
			if (isInFov (member.position)) {
				alignVector += member.velocity;
			}
		}

		return alignVector.normalized;
	}

	Vector3 separation(){
		Vector3 separateVector = new Vector3 ();
		var members = level.GetNeighbors (this, conf.separationRadius);
		if (members.Count == 0) {
			return separateVector;
		}

		foreach (var member in members) {
			if (isInFov (member.position)) {
				Vector3 movingTowards = this.position - member.position;
				if(movingTowards.magnitude > 0){
					separateVector += movingTowards.normalized / movingTowards.magnitude;
				}
			}
		}

		return separateVector.normalized;
	}

	Vector3 avoidance(){
		Vector3 avoidVector = new Vector3 ();
		var enemyList = level.getEnemies (this, conf.avoidanceRadius);
		if (enemyList.Count == 0) {
			return avoidVector;
		}

		foreach(var enemy in enemyList){
			avoidVector += runAway (enemy.position);
		}
		return avoidVector;
	}

	Vector3 runAway(Vector3 target){
		Vector3 neededVelocity = (position - target).normalized * conf.maxVelocity;
		return neededVelocity - velocity;
	}

	virtual protected Vector3 combine(){
		Vector3 finalVec = conf.cohesionPriority * cohesion() + 
			conf.wanderPriority * wander() + 
			conf.alingmentPriority * alignment() +
			conf.separationPriority * separation() +
			conf.avoidancePriority * avoidance();
		return finalVec;
	}

	void wrapAround(ref Vector3 vector, float min, float max){
		vector.x = getWrapAround (vector.x, min, max);
		vector.y = getWrapAround (vector.y, min, max);
		vector.z = getWrapAround (vector.z, min, max);
	}

	float getWrapAround(float value, float min, float max){
		
		if (value > max) {
			value = min;
		} else if(value < min) {
			value = max;
		}

		return value;
	}

	float randomBinomial(){
		return Random.Range (0f, 1f) - Random.Range (0f, 1f);
	}

	bool isInFov(Vector3 vec){
		return Vector3.Angle (this.velocity, vec - this.position) <= conf.maxFov;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy") {
			Instantiate (particle, this.transform.position, this.transform.rotation);
			Destroy (this.gameObject);
		}
	}
}
