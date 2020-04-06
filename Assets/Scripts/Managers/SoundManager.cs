using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Backgound")] 
    public AudioSource AudioSource;
    public AudioClip sadBackground;
    public AudioClip introBackground;
    public AudioClip persecutionBackground;
    
    [SerializeField]
    private AudioClip[] soundLibrary;
    
	void Awake()
	{
		if (SoundManager.instance == null)
		{
			SoundManager.instance = this;
		}
		else if (SoundManager.instance != this)
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		AudioSource = GetComponent<AudioSource>();
	}

	public void PlaySound(string soundName)
	{
		foreach (AudioClip sound in soundLibrary) {
			if (sound.name == soundName)
			{
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.PlayOneShot(sound);
			}
		}
	}
	
	public void PlayBackground(AudioClip backgroundClip)
	{
		AudioSource.Stop();
		float volumen = 0.3f;
		AudioSource.volume = volumen;
		AudioSource.clip = null;
		AudioSource.clip = backgroundClip;
		AudioSource.Play();
	}


}
