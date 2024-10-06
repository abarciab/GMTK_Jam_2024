using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerAnimator : MonoBehaviour
{
    public AnimationCurve curve;

    private SkinnedMeshRenderer smr;

    private float animationTime = 0;

    private float animationTotalTime = 0.68f;

    public ParticleSystem ps;
    
    // Start is called before the first frame update
    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
    }



    public void PlayBounce()
    {
        StopAllCoroutines();
        StartCoroutine(Bounce());
    }

    private IEnumerator Bounce()
    {
        float timeSincePlayed = 0;

        ps.Play();

        while (timeSincePlayed < animationTotalTime)
        {
            timeSincePlayed += Time.deltaTime;

            animationTime = timeSincePlayed;

            smr.SetBlendShapeWeight(0, curve.Evaluate(animationTime));

            yield return null;
        }

        yield return null;
    }
}
