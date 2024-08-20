using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMoss : MonoBehaviour
{
    public static int mossVel = Shader.PropertyToID("_Velocity");

    public Renderer rend;

    private Material mat;

    private Vector3 oldPos;

    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        mat = new Material(rend.material);

        rend.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.Lerp(velocity, oldPos - transform.position, Time.deltaTime * 2);

        mat.SetVector(mossVel, velocity);

        oldPos = transform.position;
    }

}
