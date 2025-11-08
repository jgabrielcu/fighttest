using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2 : MonoBehaviour
{
    [Header("Componentes")]
    public Animator anim;
    public Transform objetivo;

    [Header("Parámetros")]
    public float velocidad = 3.5f; 
    public float rangoDeteccion = 6f; 
    public float distanciaExtra = 2f; 
    public float rangoAtaque = 2f; 
    public float daño;

    [Header("Vida")]
    public float vida = 100f;
    private float vidaMaxima;

    [Header("Rugido")] 
    private float tiempoRugido; 
    public float cooldownRugido = 90f;

    [Header("Invocación de Duendes")]
    public GameObject prefabDuende;
    public Transform[] puntosInvocacion;
    public float tiempoEntreInvocaciones = 70f;
    public int cantidadDuendes = 2;
    public float spawnDelay = 2.0f;

    private bool puedeInvocar = true;
    private bool invocando = false;

    private bool persiguiendo; 
    private bool atacando; 
    private bool rugiendo; 
    private float distancia; 

    private bool puedeRotar = true;
    private bool interrumpido = false;
    private bool estaAtacando = false;
    public float tiempoEntreAtaques = 2f;

    [Header("Efectos visuales")]
    public GameObject efectoGolpe;
    public Transform puntoImpacto;

    [Header("Aturdimiento tras golpe con escudo")]
    public float tiempoAturdido = 1.8f;
    private bool estaAturdido = false;


    private void Start()
    {
        vidaMaxima = vida;
    }

    private void Update()
    {
        if (objetivo == null || interrumpido) return;

        distancia = Vector3.Distance(transform.position, objetivo.position);

        // --- Determinar estado ---

        if (distancia < rangoDeteccion && distancia > rangoAtaque)
        {
            persiguiendo = true;
            atacando = false;
        }
        else if (distancia <= rangoAtaque)
        {
            persiguiendo = false;
            atacando = true;
        }
        else if (distancia > rangoDeteccion + distanciaExtra)
        {
            persiguiendo = false;
            atacando = false;
        }

        // --- Movimiento y rotación ---
        if (!estaAtacando && !invocando && !estaAturdido)
        {
            if (puedeRotar && !atacando)
            {
                Vector3 direccion = (objetivo.position - transform.position).normalized;

                direccion.y = 0;

                if (direccion != Vector3.zero)
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direccion), Time.deltaTime * 5f);
            }

            if (persiguiendo)
            {
                // Si está herido, se mueve más lento
                float velocidadActual = (vida <= 20f) ? velocidad * 0.6f : velocidad;
                transform.position = Vector3.MoveTowards(transform.position, objetivo.position, velocidadActual * Time.deltaTime);

                anim.SetBool("attack", false);
                anim.SetBool("roar", false);

                // Cambiar entre caminar normal y caminar herido
                if (vida <= 20f)
                {
                    anim.SetBool("walk", false);
                    anim.SetBool("wounded", true);
                }
                else
                {
                    anim.SetBool("walk", true);
                    anim.SetBool("wounded", false);
                }
            }

            // --- Movimiento normal o herido ---

            else
            {
                anim.SetBool("walk", false);
                anim.SetBool("wounded", false);
            }
        }
        else
        {
            anim.SetBool("walk", false);
        }

            // --- Ataque ---
        if (atacando && !estaAtacando && !estaAturdido)
        {
            StartCoroutine(RealizarAtaque());
        }

            // --- Rugido (curación pasiva cada cierto tiempo) ---
        if (distancia > rangoAtaque + 1f && Time.time >= tiempoRugido && !rugiendo && !estaAtacando)
        {
            StartCoroutine(Rugido());
        }

            // --- Invocación de duendes ---
        if (!invocando && puedeInvocar && vida > 0 && !estaAtacando && !estaAturdido && !rugiendo)
        {
            StartCoroutine(InvocarDuendes());
        }
    }

    //--------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------

    private IEnumerator RealizarAtaque()
    {
        estaAtacando = true;
        atacando = true;
        persiguiendo = false;
        puedeRotar = false;

        anim.SetBool("attack", true);

        yield return new WaitForSeconds(1.5f);

        anim.SetBool("attack", false);

        yield return new WaitForSeconds(tiempoEntreAtaques);

        estaAtacando = false;
        atacando = false;
        puedeRotar = true;
    }

    private void OnDrawGizmos() 
    { 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion); 
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, rangoAtaque); 
    }

    public void Ataque()
    {
        if (objetivo == null) return;

        float distanciaActual = Vector3.Distance(transform.position, objetivo.position);

        if (distanciaActual <= rangoAtaque)
        {
            Salud saludJugador = objetivo.GetComponent<Salud>();
            if (saludJugador != null)
            {
                // Si el enemigo está al 20% o menos de vida, aumenta el daño
                float porcentajeVida = vida / vidaMaxima;
                float dañoFinal = daño;

                if (porcentajeVida <= 0.2f)
                {
                    dañoFinal *= 1.5f; // +50% de daño, cambiarlo a 2f es el doble
                }

                saludJugador.RecibirDaño(daño);

                if (efectoGolpe != null && puntoImpacto != null)
                {
                    Instantiate(efectoGolpe, puntoImpacto.position, Quaternion.identity);
                }
            }
        }
    }

    public void RecibirDaño(float cantidad)
    {
        vida -= cantidad;

        if (vida <= 0f)
        {
            Muerte();
            return;
        }
    }

    public void InterrumpirAtaqueConEscudo()
    {
        if (interrumpido) return;

        StartCoroutine(AnimacionInterrupcion());
    }

    private IEnumerator AnimacionInterrupcion()
    {
        interrumpido = true;
        anim.SetBool("attack", false);
        anim.SetBool("walk", false);
        anim.SetBool("wounded", false);
        anim.SetBool("roar", false);
        anim.Play("gethit", 0, 0f);
        puedeRotar = false;
        estaAtacando = false;

        yield return new WaitForSeconds(1.5f);

        interrumpido = false;
        puedeRotar = true;
    }

    private void Muerte()
    {
        anim.SetBool("walk", false);
        anim.SetBool("attack", false);
        anim.SetBool("wounded", false);
        anim.SetBool("roar", false);
        anim.SetBool("die", true);
        Destroy(gameObject, 3f);
    }

    private IEnumerator Rugido()
    {
        rugiendo = true;
        anim.SetBool("roar", true);
        puedeRotar = false;
        persiguiendo = false;
        atacando = false;

        float duracionRugido = 6.7f;
        float tiempoInicio = Time.time;
        float intervaloCuracion = 3f; // cada x segundos cura un poco

        while (Time.time < tiempoInicio + duracionRugido)
        {
            vida = Mathf.Min(vida + 2f, vidaMaxima); // cura puntos
            yield return new WaitForSeconds(intervaloCuracion);
        }

        anim.SetBool("roar", false);
        puedeRotar = true;
        rugiendo = false;
        // Reiniciar cooldown
        tiempoRugido = Time.time + 20f; // cada 20 segundos
    }

    public void CancelarAtaquePorEscudo()
    {
        if (estaAturdido) return;

        StopAllCoroutines();

        anim.SetBool("attack", false);
        anim.SetBool("roar", false);
        anim.SetBool("walk", false);
        anim.SetBool("wounded", false);

        anim.SetBool("gethit", true);

        StartCoroutine(AturdirPorEscudo());
    }

    private IEnumerator AturdirPorEscudo()
    {
        estaAturdido = true;
        puedeRotar = false;
        estaAtacando = false;
        persiguiendo = false;

        yield return new WaitForSeconds(0.6f);
        anim.SetBool("gethit", false);

        yield return new WaitForSeconds(tiempoAturdido);

        estaAturdido = false;
        puedeRotar = true;
        interrumpido = false;

        anim.SetBool("walk", false);
        anim.SetBool("wounded", false);
        anim.SetBool("attack", false);
    }

    private IEnumerator InvocarDuendes()
    {
        if (prefabDuende == null || puntosInvocacion == null || puntosInvocacion.Length == 0)
        {
            puedeInvocar = false;
            yield break;
        }

        invocando = true;
        puedeInvocar = false;

        anim.SetBool("spellcast", true);
        puedeRotar = false;
        persiguiendo = false;
        atacando = false;

        yield return new WaitForSeconds(5.5f);
        Transform playerTransform = null;
        if (objetivo != null) playerTransform = objetivo;
        else
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        // --- Invocar ---
        for (int i = 0; i < cantidadDuendes; i++)
        {
            Transform punto = puntosInvocacion[Random.Range(0, puntosInvocacion.Length)];
            GameObject go = Instantiate(prefabDuende, punto.position, punto.rotation);

            Duende du = go.GetComponent<Duende>();
            if (du != null && objetivo != null)
            {
                if (playerTransform != null)
                    du.objetivo = playerTransform;
                else if (objetivo != null)
                    du.objetivo = objetivo;
            }
        }

        yield return new WaitForSeconds(2.5f);

        anim.SetBool("spellcast", false);
        puedeRotar = true;
        invocando = false;

        yield return new WaitForSeconds(tiempoEntreInvocaciones);
        puedeInvocar = true;
    }
}

