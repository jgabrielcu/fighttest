using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour
{
    [Header("Configuración general")]
    public KeyCode teclaRodar = KeyCode.LeftControl;
    public float cooldownRoll = 0.8f;
    public float distanciaRoll = 4f; 
    public float duracionRoll = 0.4f;

    private Animator anim;
    private bool puedeRodar = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(teclaRodar) && puedeRodar)
        {
            StartCoroutine(EjecutarRoll());
        }
    }

    private IEnumerator EjecutarRoll()
    {
        puedeRodar = false;
        anim.SetBool("roll", true);

        yield return new WaitForSeconds(0.3f);
        anim.SetBool("roll", false);

        yield return new WaitForSeconds(cooldownRoll);
        puedeRodar = true;
    }
}
