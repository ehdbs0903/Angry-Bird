using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Properties")]
    public float hp = 100f;
    public float damageMultiplier = 1f;
    public float minimumDamageThreshold = 5f;

    [Header("Effects")]
    public GameObject destroyEffect;
    public AudioClip hitSound;
    public AudioClip destroySound;

    private SpriteRenderer spriteRenderer;
    public Sprite sprite1;
    public Sprite sprite2;

    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;

        // �ּ� ��ݷ����� ������ ������ ����
        if (impactForce < minimumDamageThreshold) return;

        // ������ ���
        float damage = impactForce * damageMultiplier;
        TakeDamage(damage);

        // �浹 ����
        PlayHitSound();
    }

    void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            DestroyObstacle();
        }
        else if (hp <= 30)
        {
            spriteRenderer.sprite = sprite2;
        }
        else if (hp <= 70)
        {
            spriteRenderer.sprite = sprite1;
        }
    }

    void DestroyObstacle()
    {
        // �ı� ȿ�� ����
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // �ı� ���� ���
        if (destroySound != null)
        {
            AudioSource.PlayClipAtPoint(destroySound, transform.position);
        }

        Destroy(gameObject);
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}