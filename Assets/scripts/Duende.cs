using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duende : MonoBehaviour
{
    [Header("Componentes")]
    public Animator anim;
    public Transform objetivo;

    [Header("Parámetros generales")]
    public float velocidad = 3f;
    public float rangoDeteccion = 10f;
    public float rangoAtaque = 2f;
    public float vida = 60f;
    public float daño = 10f;

    [Header("Cooldowns")]
    public float cooldownAtaqueBasico = 2.5f;

    [Header("Ataques especiales")]
    public float dañoSalto = 20f;
    public float cooldownSalto = 20f;
    public float rangoSalto = 6f;
    public float fuerzaSalto = 8f;
    public float radioImpacto = 2.5f;

    private bool estaMuerto = false;
    private bool puedeAtacar = true;
    private bool puedeSaltar = true;
    private bool enSalto = false;
    private bool enAtaque = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim.SetBool("idle", true);
    }

    void Update()
    {
        if (estaMuerto || objetivo == null || vida <= 0 || enSalto || enAtaque) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (puedeSaltar && distancia <= rangoSalto && distancia > rangoAtaque)
        {
            StartCoroutine(AtaqueSalto());
        }
        else if (puedeAtacar && distancia <= rangoAtaque)
        {
            MirarJugador();
            StartCoroutine(AtaqueBasico());
        }
        else if (distancia <= rangoDeteccion)
        {
            PerseguirJugador(distancia);
        }
        else
        {
            anim.SetBool("walk", false);
            anim.SetBool("idle", true);
        }
    }

    void PerseguirJugador(float distancia)
    {
        if (distancia > rangoAtaque * 0.8f)
        {
            anim.SetBool("idle", false);
            anim.SetBool("walk", true);

            Vector3 dir = (objetivo.position - transform.position).normalized;
            dir.y = 0;
            transform.position += dir * velocidad * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            anim.SetBool("walk", false);
            MirarJugador();
        }
    }

    void MirarJugador()
    {
        Vector3 direccion = (objetivo.position - transform.position);
        direccion.y = 0;
        if (direccion.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    IEnumerator AtaqueBasico()
    {
        puedeAtacar = false;
        enAtaque = true;

        anim.SetBool("walk", false);
        anim.SetBool("attack", true);

        yield return new WaitForSeconds(1f);

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia <= rangoAtaque)
        {
            Salud saludJugador = objetivo.GetComponent<Salud>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDaño(daño);
            }    
        }

        yield return new WaitForSeconds(0.5f);

        anim.SetBool("attack", false);
        anim.SetBool("idle", true);
        enAtaque = false;

        yield return new WaitForSeconds(cooldownAtaqueBasico);
        puedeAtacar = true;
    }

    IEnumerator AtaqueSalto()
    {
        puedeSaltar = false;
        enSalto = true;

        anim.SetBool("jumpAttack", true);
        anim.SetBool("walk", false);
        anim.SetBool("idle", false);

        yield return new WaitForSeconds(0.3f);

        Vector3 dir = (objetivo.position - transform.position).normalized;
        rb.velocity = new Vector3(dir.x * 5f, fuerzaSalto, dir.z * 5f);

        yield return new WaitForSeconds(1.0f);

        // daño por impacto
        Collider[] colisiones = Physics.OverlapSphere(transform.position, radioImpacto);
        foreach (Collider c in colisiones)
        {
            if (c.transform == objetivo)
            {
                Salud saludJugador = c.GetComponent<Salud>();
                if (saludJugador != null)
                {
                    saludJugador.RecibirDaño(dañoSalto);
                }
            }
        }

        anim.SetBool("jumpAttack", false);
        anim.SetBool("walk", false);
        anim.SetBool("idle", true);

        yield return new WaitForSeconds(0.5f);
        enSalto = false;

        yield return new WaitForSeconds(cooldownSalto);
        puedeSaltar = true;
    }

    public void RecibirDaño(float cantidad)
    {
        if (vida <= 0) return;
        vida -= cantidad;

        if (vida <= 0)
        {
            Muerte();
        }
    }

    void Muerte()
    {
        if (estaMuerto) return;
        estaMuerto = true;
        
        StopAllCoroutines();

        rb.isKinematic = true;
        anim.SetBool("idle", false);
        anim.SetBool("walk", false);
        anim.SetBool("attack", false);
        anim.SetBool("jumpAttack", false);
        anim.SetBool("die", true);

        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangoSalto);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radioImpacto);

    }
}
