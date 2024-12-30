using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    public AudioClip ThemeSound;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource != null && ThemeSound != null)
        {
            _audioSource.clip = ThemeSound;
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
