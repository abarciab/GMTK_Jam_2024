using UnityEngine;

public class GuardianMessagesRoom : MonoBehaviour
{
    [SerializeField] private Transform _cameraParent;
    [SerializeField] private Transform _cameraPosTarget;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private int _burstAmount = 10;

    void Update()
    {
        _cameraParent.transform.SetPositionAndRotation(_cameraPosTarget.position, _cameraPosTarget.rotation);
    }

    public void Activate()
    {
        _particles.Emit(_burstAmount);
    }
}
