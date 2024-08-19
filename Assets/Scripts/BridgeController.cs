using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float _secondsToOpen = 5;

    [Header("parts")]
    [SerializeField] private Transform _deck;
    [SerializeField] private Transform _sideWallParent;
    [SerializeField] private GameObject _sideWallPrefab;
    [SerializeField] private GameObject _sideWallWithRopePrefab;
    [SerializeField] private float _sideWallWidth;

    [Header("Sounds")]
    [SerializeField] private Sound _sideWallPlaceSound;

    private Transform _start;
    private Transform _end;

    private bool _animating;
    private Vector3 _middle => Vector3.Lerp(_start.position, _end.position, 0.5f);

    private void OnEnable()
    {
        if (!_sideWallPlaceSound.Instantialized) _sideWallPlaceSound = Instantiate(_sideWallPlaceSound);
    }

    [ButtonMethod]
    void Update()
    {
        if (_animating || !_start || !_end) return;

        transform.position = _middle;
        transform.LookAt(_end.position);

        var dist = Vector3.Distance(_start.position, _end.position);

        var scale = _deck.localScale;
        scale.z = dist;
        _deck.localScale = scale;
    }

    [ButtonMethod]
    private IEnumerator PlaceSideWalls(float time)
    {
        float waitTime = time/4;
        time -= waitTime;
        yield return new WaitForSeconds(waitTime);

        var totalDist = Vector3.Distance(_start.position, _end.position);


        int sideWallCount = Mathf.FloorToInt(totalDist / _sideWallWidth);
        int ropeIndex = Random.Range(0, sideWallCount);
        for (int i = 0; i < sideWallCount; i++) {
            var newSideWall = Instantiate(i == ropeIndex ? _sideWallWithRopePrefab : _sideWallPrefab, _sideWallParent);
            newSideWall.transform.localRotation = Quaternion.identity;
            var pos = new Vector3(_deck.transform.localScale.x / 2, 0, i * _sideWallWidth - (totalDist-_sideWallWidth)/2);
            newSideWall.transform.localPosition = pos;

            newSideWall = Instantiate(_sideWallPrefab, _sideWallParent);
            newSideWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
            pos = new Vector3(-_deck.transform.localScale.x/2, 0, i * _sideWallWidth - (totalDist - _sideWallWidth) / 2);
            newSideWall.transform.localPosition = pos;

            yield return new WaitForSeconds(time / sideWallCount);
            _sideWallPlaceSound.Play(transform);
        }
    }

    public void Initialize(Transform start, Transform end)
    {
        _start = start;
        _end = end;
        RotateEnds(_start, _end);


        StartCoroutine(Animate());
        StartCoroutine(PlaceSideWalls(_secondsToOpen));
    }

    void RotateEnds(Transform start, Transform end)
    {
        Vector3 dirSE = end.position - start.position;
        Vector3 dirES = start.position - end.position;

        dirSE.y = 0;
        dirES.y = 0;

        dirSE.Normalize();
        dirES.Normalize();

        Quaternion startRot = Quaternion.LookRotation(dirSE);
        Quaternion endRot = Quaternion.LookRotation(dirES);

        start.rotation = Quaternion.Euler(0, startRot.eulerAngles.y, 0);
        end.rotation = Quaternion.Euler(0, endRot.eulerAngles.y, 0);
    }

    private IEnumerator Animate()
    {
        _animating = true;
        float timePassed = 0;
        float deckTime = _secondsToOpen / 4;
        while (timePassed < deckTime) {
            float progress = timePassed / deckTime;
            transform.position = Vector3.Lerp(_start.position, _end.position, progress/2);
            float dist = Vector3.Distance(transform.position, _start.position) * 2;
            transform.LookAt(_end.position);
            var scale = _deck.localScale;
            scale.z = dist;
            _deck.localScale = scale;

            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }

        _animating = false;
    }
}
