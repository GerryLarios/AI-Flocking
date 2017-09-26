using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	public Transform memberPrefab;
	public Transform enemyPrefab;

	public int numberOfMembers;
	public int numberOfEnemy;

	public List<Member> members;
	public List<Enemy> enemies;

	public float bounds;
	public float spawnRadius;

	// Use this for initialization
	void Start () {
		members = new List<Member> ();
		enemies = new List<Enemy> ();

		spawn (memberPrefab, numberOfMembers);
		spawn (enemyPrefab, numberOfEnemy);

		members.AddRange (FindObjectsOfType<Member> ());
		enemies.AddRange (FindObjectsOfType<Enemy> ());
	}

	void spawn(Transform prefab, int max){
		for (int i = 0; i < max; i++) {
			Instantiate (prefab, new Vector3 (Random.Range (-spawnRadius, spawnRadius), Random.Range (-spawnRadius, spawnRadius), 0), Quaternion.identity);
		}
	}

	public List<Member>GetNeighbors(Member member, float radius){
		List<Member> neighBorsFound = new List<Member> ();

		foreach(var otherMember in members){
			if (otherMember == member) {
				continue;
			}

			if (Vector3.Distance (member.position, otherMember.position) <= radius) {
				neighBorsFound.Add (otherMember);
			}
		}

		return neighBorsFound;
	}

	public List<Enemy> getEnemies(Member member, float radius){
		List<Enemy> returnEnemies = new List<Enemy> ();

		foreach (var enemy in enemies) {
			if (Vector3.Distance (member.position, enemy.position) <= radius) {
				returnEnemies.Add (enemy);
			}
		}
		return returnEnemies;
	}

	public void quitTest (){
		print ("Bye");
		Application.Quit();
	}
}
