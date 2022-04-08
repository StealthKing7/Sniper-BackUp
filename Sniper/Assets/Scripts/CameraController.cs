using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    private GameObject Sniper;
    private bool isScoped = false;
    private float xrot = 0;
    private float ScopeWeight;
    private float Sencitivity;
    public float AimSencitivity;
    public float NormalSencitivity;
    public Transform player;
    public Transform holder;
    public GameObject crosshair;
    private Animator animator;
    public Text ammo;
    public Camera ScopeCam;
    private float scrollWheel = 9f;
    private Camera cam;
    private float framerate;
    private float timer; 
    private Recoil recoil;
    private PostProcessVolume volume;
    private DepthOfField blur;
    private float mouseX;
    private float mouseY;
    [SerializeField]
    private GameObject Glass;
    [SerializeField]
    private GameObject ScopeGlass;
    [SerializeField]
    private Transform weapon;
    [SerializeField]
    private Text fpscounter;
    [SerializeField]
    [Space(10)]
    [Header("Shooting Properties")]
    public float Range;
    public GameObject BulletPf;
    public float maxAmmo;
    public Transform firePosition;
    public LayerMask layer = new LayerMask();
    [SerializeField]
    private ParticleSystem muzzleFlash; 
    public float ReloadTime;
    public float FireRate = 15f;
    [SerializeField]
    private Vector3 NonScopeOffset;
    [SerializeField]
    private Vector3 ScopeOffset;
    private bool isReloading;
    private float CurrentAmmo;
    private float NextTimeToFire = 0f;
    public ParticleSystem Shell;
    [HideInInspector]
    public bool HadFired;
    private SoundManeger soundManeger;
    [Space(10)]
    [Header("Weapon Sway")]
    [SerializeField]
    private float SwayMultiplier;
    [SerializeField]
    private float SwayDampTime;
    [SerializeField]
    private float BreathSwayX;
    [SerializeField]
    private float BreathSwayY;
    [Space(10)]
    [Header("Animation Rigging")]
    public MultiPositionConstraint ScopePositionRig;
    public MultiRotationConstraint ScopeRotationRig;
    public float DampTime;
    [Space(10)]
    [Header("Bullet Proaperties")]
    [SerializeField]
    public float bulletSpeed;
    [SerializeField]
    public float BulletLifeTime;
    [SerializeField]
    private Vector3 Force;

    void Awake()
    {
        Application.targetFrameRate = 60;
        soundManeger = FindObjectOfType<SoundManeger>();
        Instance = this;
        cam = GetComponent<Camera>();
        animator = GetComponent<Animator>();
        volume = GetComponent<PostProcessVolume>();
        blur = volume.profile.GetSetting<DepthOfField>();
    }
    void Start()
    {
        Sencitivity = NormalSencitivity;
        Sniper = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        CurrentAmmo = maxAmmo;
        ammo.text = CurrentAmmo.ToString();
        recoil = Sniper.GetComponent<Recoil>();
    }
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * Sencitivity;
        mouseY = Input.GetAxis("Mouse Y") * Sencitivity;
        xrot -= mouseY;
        xrot = Mathf.Clamp(xrot, -90f, 90);
        holder.localRotation = Quaternion.Euler(xrot, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);
        float SwaymouseX = Input.GetAxisRaw("Mouse X") * SwayMultiplier;
        float SwaymouseY = Input.GetAxisRaw("Mouse Y") * SwayMultiplier;
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion TargetRot = rotationX * rotationY;
        weapon.localRotation = Quaternion.Slerp(weapon.localRotation, TargetRot, SwayDampTime);
        float zoomChange = 9;
        if (Input.GetButtonDown("Fire2"))
        {
            isScoped = !isScoped;
            recoil.aim = isScoped;
            StartCoroutine(ChangeGlass());
        }

        if(Input.mouseScrollDelta.y > 0)
        {
            scrollWheel -= zoomChange * DampTime;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            scrollWheel += zoomChange * DampTime;
        }
        
        if (Input.GetButtonDown("Fire1") && CurrentAmmo > 0 && isReloading == false && Time.time >= NextTimeToFire)
        {
            recoil.Fire();
            NextTimeToFire = Time.time + 1f / FireRate;
            StartCoroutine(Shoot());

        }
        if(CurrentAmmo == 0 || (Input.GetKeyDown(KeyCode.R) && CurrentAmmo < maxAmmo))
        {
            ScopeWeight = 0;
            StartCoroutine(Reload());
        }
        if (timer > 1f)
        {
            framerate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
        string fps = framerate + " fps";
        fpscounter.text = fps;
        if (Input.GetKey(KeyCode.A))
        {
            SceneManager.LoadScene(0);
        }
    }

    private IEnumerator ChangeGlass()
    {
        if (isScoped)
        {
            yield return new WaitForSeconds(0.0375f);
            ScopeGlass.SetActive(true);
            Glass.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.0375f);
            Glass.SetActive(true);
            ScopeGlass.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        scrollWheel = Mathf.Clamp(scrollWheel, 4, 10);
        if (isScoped)
        {
            ScopeWeight = 1;
            ScopePositionRig.weight = Mathf.Lerp(ScopePositionRig.weight, ScopeWeight, DampTime);
            ScopeRotationRig.weight = Mathf.Lerp(ScopeRotationRig.weight, ScopeWeight, DampTime);
            blur.focalLength.value = Mathf.Lerp(blur.focalLength.value, 100f, 1);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, DampTime);
            ScopeCam.fieldOfView = scrollWheel;
            crosshair.SetActive(false);
            Sencitivity = AimSencitivity;
        }
        else
        {
            ScopeWeight = 0;
            ScopePositionRig.weight = Mathf.Lerp(ScopePositionRig.weight, ScopeWeight, DampTime);
            ScopeRotationRig.weight = Mathf.Lerp(ScopeRotationRig.weight, ScopeWeight, DampTime);
            blur.focalLength.value = Mathf.Lerp(blur.focalLength.value, 1f, 1);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, DampTime);
            ScopeCam.fieldOfView = scrollWheel;
            crosshair.SetActive(true);
            Sencitivity = NormalSencitivity;
        }
    }
    public void ShellPlay ()
    {
        Shell.Play();
    }
    IEnumerator Shoot()
    {
        HadFired = true;
        if (!isScoped)
        {
            muzzleFlash.Play();
        }
        CurrentAmmo--;
        soundManeger.Play("Shoot");
        Vector3 mousePos = Vector3.zero;
        Vector3 ScreenCenter = new Vector3(Screen.width / 2f, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            mousePos = hit.point;
            Vector3 aimDir = (mousePos - firePosition.position).normalized;
            Debug.DrawLine(firePosition.position, mousePos, Color.red, 5f);
            GameObject bullet = Instantiate(BulletPf, firePosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Bullet bulletsc = bullet.GetComponent<Bullet>();
            bulletsc.Initialized(bulletSpeed);
            Destroy(bullet, BulletLifeTime);
        }
        animator.SetBool("Bolt",true);
        yield return new WaitForSeconds(0.5f - 0.25f);
        animator.SetBool("Bolt", false);
        yield return new WaitForSeconds(0.25f);
    }
    void BoltUp()
    {
        soundManeger.Play("Bolt up");
    }
    void BoltBack()
    {
        soundManeger.Play("Bolt Back");
    }
    void BoltForward()
    {
        soundManeger.Play("Bolt Forward");
    }
    void BoltDown()
    {
        soundManeger.Play("Bolt Down");
    }
    void MagzineOut()
    {
        soundManeger.Play("Magine Out");
    }
    void MagzineIn()
    {
        soundManeger.Play("Magine In");
    }
    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(ReloadTime - 0.25f);
        animator.SetBool("Reload", false);
        yield return new WaitForSeconds(0.25f); 
        CurrentAmmo = maxAmmo;
        isReloading = false;
    }

}
