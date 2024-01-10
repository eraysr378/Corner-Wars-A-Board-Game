using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip moveSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip startSound;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMoveSound()
    {
        audioSource.clip = moveSound;
        audioSource.Play();
    }
    public void PlayLoseSound()
    {
        audioSource.clip = loseSound;
        audioSource.Play();
    }
    public void PlayWinSound()
    {
        audioSource.clip = winSound;
        audioSource.Play();
    }
    public IEnumerator PlayGameStartSound()
    {
        audioSource.pitch = 1;
        audioSource.clip = startSound;
        audioSource.Play();
        yield return new WaitForSeconds(1f);
        audioSource.pitch = 2;

    }
}
