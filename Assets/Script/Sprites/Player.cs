using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    const int RUN_SPEED = 1;
	void Start()
	{
	}

	void Update()
	{
	}

    public Animator animator{ get { return GetComponent<Animator>(); }}
}
