using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGamage : MonoBehaviour
{
    public float damage = 25f;
    private bool canDamage = false;

    void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Golpeado por la oz!");
            // Lógica de daño al jugador:
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }

    public void EnableDamage() => canDamage = true;
    public void DisableDamage() => canDamage = false;
}
