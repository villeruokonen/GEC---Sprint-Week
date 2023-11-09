using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private struct PlayerInput
    {
        public float Horizontal
            => Input.GetAxisRaw("Horizontal");
        public float Vertical
            => Input.GetAxisRaw("Vertical");
        public bool WantsTornadoStart
            => Input.GetButtonDown("Fire1");
        public bool WantsTornadoStop
            => Input.GetButtonUp("Fire1");

        public bool IsMoving
            => Horizontal != 0 || Vertical != 0;
    }

    private readonly float _walkSpeed = 12;
    [SerializeField]
    private int _playerRotationAnglesPerSecond = 360;
    private Transform _playerModelT;
    
    private CharacterController _char;
    private Transform _cameraT;

    private bool _tornadoActive = false;

    [Header("Tornado")]
    [SerializeField]
    private int _tornadoAnglesPerSecond = 1200;
    private ParticleSystem[] _tornadoParticles;
    private Transform _tornadoModelT;
    private AudioSource _tornadoAudio;
    private readonly float _tornadoMaxRate = 1;
    private float _currentTornadoRate = 0;

    private Vector3 _tornadoMaxScale;

    private static GameObject _tornadoGroundMarkPrefab;

    private const float _tornadoGroundMarkRate = 0.1f;
    private float _tornadoGroundMarkTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _playerModelT = transform.Find("PlayerModel/Man");
        _tornadoModelT = transform.Find("PlayerModel/Tornado");
        _tornadoMaxScale = _tornadoModelT.localScale;
        _cameraT = Camera.main.transform;
        _tornadoParticles = _tornadoModelT.GetComponentsInChildren<ParticleSystem>();
        _tornadoAudio = _tornadoModelT.GetComponent<AudioSource>();
        _tornadoGroundMarkPrefab = Resources.Load<GameObject>("Prefabs/TornadoGroundMark");
        _char = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    void TryCreatePlayerGroundMark()
    {
        // Just using the tornado mark timer for now
        _tornadoGroundMarkTimer += Time.deltaTime * 1.8f;
        if (_tornadoGroundMarkTimer < _tornadoGroundMarkRate)
            return;

        _tornadoGroundMarkTimer = 0;

        var ray = new Ray(transform.position, Vector3.down);

        if (!Physics.Raycast(ray, out var hit, 100))
            return;

        Vector3 pos = hit.point + hit.normal * 0.08f;

        var groundMark = Instantiate(_tornadoGroundMarkPrefab, pos, Quaternion.identity);

        groundMark.transform.localScale *= Random.Range(0.4f, 0.8f);

        groundMark.transform.Rotate(Vector3.up, Random.Range(0, 360));

        groundMark.transform.up = hit.normal;
    }

    void TryCreateTornadoGroundMark()
    {
        _tornadoGroundMarkTimer += Time.deltaTime;
        if (_tornadoGroundMarkTimer < _tornadoGroundMarkRate)
            return;

        _tornadoGroundMarkTimer = 0;

        var ray = new Ray(transform.position, Vector3.down);

        if (!Physics.Raycast(ray, out var hit, 100))
            return;

        Vector3 pos = hit.point + hit.normal * 0.08f;

        var groundMark = Instantiate(_tornadoGroundMarkPrefab, pos, Quaternion.identity);

        groundMark.transform.localScale *= Random.Range(1f, 1.8f);

        groundMark.transform.Rotate(Vector3.up, Random.Range(0, 360));

        groundMark.transform.up = hit.normal;
    }

    void UpdateMovement()
    {
        var input = CheckInput();

        var desiredMove = new Vector3(input.Horizontal, 0, input.Vertical).normalized;

        if (input.WantsTornadoStart)
        {
            StartTornado();
        }
        else if (input.WantsTornadoStop)
        {
            StopTornado();
        }

        UpdateTornado();

        //Vector3 cameraForward = _cameraT.forward;

        desiredMove = Quaternion.Euler(0, _cameraT.eulerAngles.y, 0) * desiredMove;

        Vector3 actualMovement = desiredMove * _walkSpeed * Time.deltaTime;

        actualMovement.y = Physics.gravity.y * Time.deltaTime;

        if(input.IsMoving)
        {
            _playerModelT.Rotate(Vector3.up, _playerRotationAnglesPerSecond * Time.deltaTime);

            //if (!_tornadoActive)
            //    TryCreatePlayerGroundMark();
        }
        
        _char.Move(actualMovement);
    }

    PlayerInput CheckInput()
    {
        return new PlayerInput();
    }

    private void UpdateTornado()
    {
        float targetRate = _tornadoActive ? _tornadoMaxRate : 0;
        _currentTornadoRate = Mathf.Lerp(_currentTornadoRate, targetRate, 2 * Time.deltaTime);

        float rotationAngle = _currentTornadoRate * _tornadoAnglesPerSecond * Time.deltaTime;

        _tornadoModelT.Rotate(Vector3.up, rotationAngle, Space.World);
        _playerModelT.Rotate(Vector3.up, rotationAngle / 2, Space.World);

        _tornadoModelT.localScale = Vector3.Lerp(Vector3.zero, _tornadoMaxScale, _currentTornadoRate);

        _tornadoAudio.volume = _currentTornadoRate;

        _tornadoAudio.pitch = Mathf.Clamp(_currentTornadoRate, 0.8f, 1.1f);

        if (_tornadoAudio.volume > 0.05f && !_tornadoAudio.isPlaying)
            _tornadoAudio.Play();
        else if (_tornadoAudio.volume < 0.05f)
            _tornadoAudio.Stop();

        var mr = _tornadoModelT.GetComponent<MeshRenderer>();

        Color transp = new Color(1, 1, 1, 0);

        mr.material.color = Color.Lerp(transp, Color.white, _currentTornadoRate);

        mr.material.SetTextureOffset("_MainTex", new Vector2(0, Time.time * 0.5f));

        foreach(var ps in _tornadoParticles)
        {
            if (_currentTornadoRate > 0.1f)
                ps.Play();
            else
                ps.Stop();
        }

        if (_currentTornadoRate > 0.1f)
            TryCreateTornadoGroundMark();
    }

    private void StartTornado()
    {
        _tornadoActive = true;
    }

    private void StopTornado()
    {
        _tornadoActive = false;
    }
}