using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ARButton : MonoBehaviour
{
    public UnityEvent myEvent;
    public AudioClip buttonSound;
    private Animator buttonAnimator;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        buttonAnimator = GetComponent<Animator>();
    }

    public void Pressed()
        {
            myEvent.Invoke();
            buttonAnimator.SetTrigger("pressed");
            audioSource.PlayOneShot(buttonSound);
        }

    }
