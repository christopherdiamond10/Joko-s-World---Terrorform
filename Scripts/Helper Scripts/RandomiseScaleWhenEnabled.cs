using UnityEngine;
using System.Collections;

public class RandomiseScaleWhenEnabled : MonoBehaviour
{
	public float m_fMinScale = 0.005f;
	public float m_fMaxScale = 0.01f;
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnEnable()
	{
		Vector3 Scale = transform.localScale;
		Scale.x = Random.Range(m_fMinScale, m_fMaxScale);
		Scale.y = Scale.x;
		transform.localScale = Scale;
	}
}
