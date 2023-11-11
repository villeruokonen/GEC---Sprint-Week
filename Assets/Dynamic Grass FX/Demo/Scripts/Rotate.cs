using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 Angle = Vector3.one;

	void Update ()
	{
		transform.Rotate(Angle * 24 * Time.deltaTime);
	}
}
