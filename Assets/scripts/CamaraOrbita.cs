using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraOrbita : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float sensitivityX = 200f;
    public float sensitivityY = 100f;
    public float minYAngle = -30f;
    public float maxYAngle = 70f;
    public float followSpeed = 10f;
    public float rollFollowSpeed = 25f;

    private float rotX = 0f;
    private float rotY = 0f;

    private Personaje personaje;

    private void Start()
    {
        personaje = target.GetComponentInParent<Personaje>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        //float velocidadActual = (personaje != null && personaje.EstaRodando()) ? rollFollowSpeed : followSpeed;

        Transform personajeTransform = personaje.transform;
        //target.position = Vector3.Lerp(target.position, personajeTransform.position, velocidadActual * Time.deltaTime);


        rotX += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        rotY -= Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
        rotY = Mathf.Clamp(rotY, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        transform.position = target.position + rotation * offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}