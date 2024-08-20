using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    [SerializeField] private Transform _lookTarget;

    public void MovePlayerHere()
    {
        GameManager.i.Player.transform.position = transform.position;
    }

    public void SetLook()
    {
        StartCoroutine(waitThenSetLook());
    }

    private IEnumerator waitThenSetLook()
    {
        yield return new WaitForSeconds(0.05f); 
        GameManager.i.Camera.LookAt(_lookTarget);

        var player = GameManager.i.Player.transform;
        player.transform.LookAt(_lookTarget);
        var euler = player.transform.eulerAngles;
        euler.x = euler.z = 0;
        player.transform.eulerAngles = euler;
    }
}
