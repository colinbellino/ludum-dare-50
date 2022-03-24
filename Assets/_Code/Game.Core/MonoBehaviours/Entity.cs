using System;
using FMODUnity;
using UnityEngine;

namespace Game.Core
{
	public class Entity : MonoBehaviour
	{
		[SerializeField] public Animator Animator;
		[SerializeField] public SpriteRenderer SpriteRenderer;
		[SerializeField] public AudioSource AudioSource;

		[Header("Audio")]
		[SerializeField] public EventReference[] SoundWalk;

		[HideInInspector] public bool Dead;
		[HideInInspector] public ClipLength AnimationClipLength;
	}

	[Serializable]
	public class ClipLength : SerializableDictionary<string, float> { }
}
