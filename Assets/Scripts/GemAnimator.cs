using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemAnimator : MonoBehaviour
{
    public Renderer distortionRend, gemRend;

    public Material myMat, gemMat;

    public ParticleSystem idlePS1, idlePS2, pickupParticles;

    //public AudioSource pickupSound, towerFinishSound;
    //[SerializeField] private Sound PickupSound, TowerFinishSound

    // Start is called before the first frame update
    void Start()
    {
        myMat = new Material(distortionRend.material);

        distortionRend.material = myMat;



        gemMat = new Material(gemRend.material);

        gemRend.material = gemMat;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{

        //}


        //if (Input.GetKeyDown(KeyCode.N))
        //{

        //    gemMat.SetFloat("_isActive", 0);

        //    distortionRend.gameObject.SetActive(false);

        //    pickupParticles.gameObject.SetActive(false);

        //    idlePS1.gameObject.SetActive(false);

        //    idlePS2.gameObject.SetActive(false);
        //}
    }

    public void PickUpGem()
    {
        distortionRend.gameObject.SetActive(true);

        myMat.SetFloat("_Ping", Time.time);

        gemMat.SetFloat("_Ping", Time.time);

        gemMat.SetFloat("_isActive", 1);

        pickupParticles.gameObject.SetActive(true);

        idlePS1.gameObject.SetActive(true);

        idlePS2.gameObject.SetActive(true);

        GameManager.i.CompleteTower();

        //pickupSound.Play();

        //towerFinishSound.Play();
    }
}
