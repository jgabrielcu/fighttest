using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Componentes")]
    public Animator anim;
    public Transform puntoAtaque;
    public float rangoAtaque = 1.8f;
    public LayerMask capaEnemigo;

    [Header("Daño")]
    public float dañoHacha = 25f;
    public float dañoEscudo = 15f;
    public float tiempoEntreAtaques = 1.2f;

    [Header("Cooldown Escudo")]
    public float cooldownEscudo = 10f;
    private bool escudoDisponible = true;

    private bool puedeAtacar = true;
    private bool estaAtacando = false;

    void Update()
    {
        if (!puedeAtacar || estaAtacando) return;

        // --- Ataque con hacha ---
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int ataqueRandom = Random.Range(1, 4);
            string animacion = "attackAxe" + ataqueRandom;
            StartCoroutine(Ataque(animacion, dañoHacha, false));
        }

        // --- Ataque con escudo ---
        if (Input.GetKeyDown(KeyCode.Mouse1) && escudoDisponible)
        {
            StartCoroutine(AtaqueConEscudo());
        }
    }

    private IEnumerator AtaqueConEscudo()
    {
        escudoDisponible = false;
        yield return Ataque("attackShield", dañoEscudo, true);

        yield return new WaitForSeconds(cooldownEscudo);
        escudoDisponible = true;
    }

    private IEnumerator Ataque(string animacion, float daño, bool esEscudo)
    {
        puedeAtacar = false;
        estaAtacando = true;

        anim.SetBool(animacion, true);

        yield return new WaitForSeconds(0.4f);

        Collider[] enemigos = Physics.OverlapSphere(puntoAtaque.position, rangoAtaque, capaEnemigo);
        foreach (Collider enemigo in enemigos)
        {
            Enemy2 e = enemigo.GetComponent<Enemy2>();
            if (e != null)
            {
                e.RecibirDaño(daño);

                if (esEscudo)
                {
                    e.CancelarAtaquePorEscudo();
                    e.RecibirDaño(daño);
                }
                else
                {
                    e.RecibirDaño(daño);
                }
            }
            Duende d = enemigo.GetComponent<Duende>();
            if (d != null)
            {
                d.RecibirDaño(daño);

                if (esEscudo)
                {
                    d.RecibirDaño(daño);
                }
                else
                {
                    d.RecibirDaño(daño);
                }
            }
        }

        yield return new WaitForSeconds(tiempoEntreAtaques);

        anim.SetBool(animacion, false);

        puedeAtacar = true;
        estaAtacando = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
    }
}
