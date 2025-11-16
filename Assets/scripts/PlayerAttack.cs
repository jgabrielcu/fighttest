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

    // --- Control de secuencia ---
    private int contadorGolpes = 0;
    private int comboActual = 1;

    void Update()
    {
        if (!puedeAtacar || estaAtacando) return;

        // --- Ataque con hacha ---
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(SistemaAtaque());
        }

        // --- Ataque con escudo ---
        if (Input.GetKeyDown(KeyCode.Mouse1) && escudoDisponible)
        {
            StartCoroutine(AtaqueConEscudo());
        }
    }

    private IEnumerator SistemaAtaque()
    {
        puedeAtacar = false;
        estaAtacando = true;

        // Aumenta el contador de golpes
        contadorGolpes++;

        // Si ha hecho menos golpes  ataques básicos
        if (contadorGolpes < 2)
        {
            int ataqueRandom = Random.Range(1, 4); // atack1, atack2, atack3
            string animacion = "attackAxe" + ataqueRandom;
            anim.SetBool("attackAxe1", false);
            anim.SetBool("attackAxe2", false);
            anim.SetBool("attackAxe3", false);

            // Activar la animación correspondiente
            anim.SetBool(animacion, true);
            yield return Ataque(animacion, dañoHacha, false);
        }
        else
        {
            // --- Ejecutar combo ---
            if (comboActual == 1)
            {
                anim.SetInteger("Combo", 1);  // Usamos SetInteger para combos
                comboActual = 2; // Siguiente vez será Attack2
            }
            else
            {
                anim.SetInteger("Combo", 2);  // Usamos SetInteger para combos
                comboActual = 1; // Siguiente vez será Attack1
            }

            yield return Ataque("ComboAttack", dañoHacha + 10, false);

            // Muy importante aplicar
            anim.SetInteger("Combo", 0);

            contadorGolpes = 0; // reiniciamos el contador
        }

        puedeAtacar = true;
        estaAtacando = false;
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
        yield return new WaitForSeconds(0.20f);

        Collider[] enemigos = Physics.OverlapSphere(puntoAtaque.position, rangoAtaque, capaEnemigo);
        foreach (Collider enemigo in enemigos)
        {
            Enemy2 e = enemigo.GetComponent<Enemy2>();
            Duende d = enemigo.GetComponent<Duende>();

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

        if (animacion.StartsWith("attackAxe"))
            anim.SetBool(animacion, false);

        if (animacion == "attackShield")
            anim.SetBool("attackShield", false);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
    }
}
