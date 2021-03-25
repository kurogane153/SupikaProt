using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    #region Singleton
    private static ReticleController instance;

    public static ReticleController Instance
    {
        get
        {
            if (instance == null) {
                instance = (ReticleController)FindObjectOfType(typeof(ReticleController));

                if (instance == null) {
                    Debug.LogError(typeof(ReticleController) + "is nothing");
                }
            }

            return instance;
        }
    }
    #endregion
    [Header("サウンド系")]
    [SerializeField] private SoundPlayer _soundPlayer;
    [SerializeField] private AudioClip _se_LockOn;

    [Space(10)]
    [SerializeField] private GameObject _lockedOnReticlePrefab;
    [SerializeField] private Color _lockOnColor;
    [SerializeField] private float _speedX = 1f;
    [SerializeField] private float _speedY = 1f;
    [SerializeField] private float _aimAssistDegreeX = 3;
    [SerializeField] private float _aimAssistDegreeY = 3;
    [SerializeField] private int _generateReticleMax = 3;

    [Header("ロックオンボタンを押してロックオンする"), Space(10)]
    [SerializeField] public bool _lockOnTargetOnLockOnButtonDown;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private RectTransform rectTransform;
    private Image image;
    private Color defaultColor;
    private bool isBeforeTargeting;
    private int generatedReticleCount;

    public int GeneratedReticleCount
    {
        get { return generatedReticleCount; }

        set { generatedReticleCount = value; 
            if(generatedReticleCount < 0) {
                generatedReticleCount = 0;
            }
            LockedOnCountMarkScript.LockedOnCountCheck();
        }
    }

    public int GenerateReticleMax
    {
        get { return _generateReticleMax; }
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    private void Awake()
    {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        defaultColor = image.color;

        if (_soundPlayer == null) {
            if ((_soundPlayer = GetComponentInChildren<SoundPlayer>()) == null) {
                Debug.LogError(gameObject.name + "の" + nameof(_soundPlayer) + "が空です");
            }
            Debug.Log(gameObject.name + "は、子要素にアタッチされているAudioSourceを自動的に" + nameof(_soundPlayer) + "にアタッチしました");
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void MoveReticle(float x, float y, Transform target = null)
    {   
        if (target && (Mathf.Abs(x) < 0.2f) && (Mathf.Abs(y)) < 0.2f) {
            TargetLockOnMove(target);
        }

        float newX = x * _speedX;
        float newY = y * _speedY;
        bool isNowTargeting = false;

        if (target) {
            image.color = _lockOnColor;
            newX /= _aimAssistDegreeX;
            newY /= _aimAssistDegreeY;

            if (_lockOnTargetOnLockOnButtonDown && Input.GetButtonDown("LockOn")) {
                isNowTargeting = true;
            } else if(!_lockOnTargetOnLockOnButtonDown){
                isNowTargeting = true;
            }

            

            if(isNowTargeting && !isBeforeTargeting) {
                AsteroidScript asteroid = target.GetComponent<AsteroidScript>();

                if (!asteroid.IsLockedOn && generatedReticleCount < _generateReticleMax ) {
                    asteroid.IsLockedOn = true;

                    GameObject newLockonReticle = Instantiate(_lockedOnReticlePrefab, transform.position, Quaternion.identity);
                    LockedOnReticle lockedOnReticle = newLockonReticle.GetComponent<LockedOnReticle>();

                    lockedOnReticle.InstantiateSettings(canvas, target);
                    GeneratedReticleCount += 1;

                    _soundPlayer.PlaySE(_se_LockOn);
                }
            }

        } else {
            image.color = defaultColor;
            isNowTargeting = false;
        }

        Vector3 movePos = new Vector3(newX, newY);

        if(!IsNewScreenPositionScreenOutsite(rectTransform.position + movePos)) {
            rectTransform.position += movePos;
        }

        isBeforeTargeting = isNowTargeting;

    }

    private bool IsNewScreenPositionScreenOutsite(Vector3 pos)
    {
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.ScreenToViewportPoint(pos))){
            return false;
        }
        return true;
    }

    private void TargetLockOnMove(Transform target)
    {
        Vector3 newPoint = Camera.main.WorldToScreenPoint(target.position);
        Rect rect = new Rect(0, 0, 1, 1);

        if (rect.Contains(Camera.main.WorldToViewportPoint(target.position))) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, newPoint, canvas.worldCamera, out Vector2 newPos);
            rectTransform.localPosition = newPos;
        }
        
    }

    public Vector2 GetReticlePos()
    {
        return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rectTransform.position);
    }
}
