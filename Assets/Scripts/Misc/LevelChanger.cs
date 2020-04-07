using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    
    private Animator _Animator;
    private int LevelToLoad;

    private void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    public void FadeToLevel(int index)
    {
        LevelToLoad = index;
        _Animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
}
