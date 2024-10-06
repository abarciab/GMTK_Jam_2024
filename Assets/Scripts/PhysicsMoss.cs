using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMoss : MonoBehaviour
{
    public static int mossVel = Shader.PropertyToID("_Velocity");

    public List<Renderer> rend;

    private Material mat;

    private Vector3 oldPos;

    private Vector3 velocity;

    public List<Mesh> meshes;

    // Start is called before the first frame update
    void Start()
    {
        mat = new Material(rend[0].material);

        foreach (Renderer r in rend)
        {
            r.material = mat;

            r.GetComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Count)];
        }


    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.Lerp(velocity, oldPos - transform.position, Time.deltaTime * 2f);

        mat.SetVector(mossVel, velocity);

        oldPos = transform.position;
    }

}
