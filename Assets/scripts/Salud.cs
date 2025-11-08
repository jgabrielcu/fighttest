using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salud : MonoBehaviour
{
    public float salud;
    public float saludMax;

    [Header("Interfaz")]
    public CanvasGroup Rojo;

    [Header("Muerto")]
    public GameObject Muerto;

    private void Update()
    {
        if (Rojo.alpha > 0)
        {
            Rojo.alpha -= Time.deltaTime;
        }
    }

    public void RecibirCura(float cura)
    {
        salud += cura;

        if (salud > saludMax)
        {
            salud = saludMax;
        }
    }

    public void RecibirDaño(float daño)
    {
        Personaje personaje = GetComponent<Personaje>();
        if (personaje != null && personaje.EstaBloqueando())
        {
            daño *= 0.2f; // 0 = bloquea todo el daño, 0.3f = 70% bloqueado
        }

        salud -= daño;
        if (daño > 0) // solo mostrar el rojo si recibió daño real
            Rojo.alpha = 1;


        if (salud <= 0)
        {
            Instantiate(Muerto);
            Destroy(gameObject);
        }
    }
}
