using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Animations.Rigging;
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
    private Recoil recoil;
    private PostProcessVolume volume;
    private DepthOfField blur;
    [SerializeField]
    private float timeout;
    [SerializeField]
    [Space(10)]
    [Header("Shooting Properties")]
    public float Range;
    public GameObject BulletPf;
    public float maxAmmo;
    public Transform firePosition;
    public LayerMask layer = new LayerMask();
    public float ReloadTime;
    public float FireRate = 15f;
    private bool isReloading;
    private float CurrentAmmo;
    private float NextTimeToFire = 0f;
    public ParticleSystem Shell;
    public bool HadFired;
    private SoundManeger soundManeger;
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
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * Sencitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Sencitivity;
        xrot -= mouseY;
        xrot = Mathf.Clamp(xrot, -90f, 90);
        holder.localRotation = Quaternion.Euler(xrot, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);

        float zoomChange = 9;
        if (Input.GetButtonDown("Fire2"))
        {
            isScoped = !isScoped;
            recoil.aim = isScoped;
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
        CurrentAmmo--;
        Vector3 mousePos = Vector3.zero;
        Vector3 ScreenCenter = new Vector3(Screen.width / 2f, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            mousePos += hit.point;
            Vector3 aimDir = (mousePos - firePosition.position).normalized;
            Debug.DrawLine(firePosition.position, mousePos, Color.red, 5f);
            GameObject Bullet = Instantiate(BulletPf, firePosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Bullet bullet = Bullet.GetComponent<Bullet>();
            bullet.Initialized(bulletSpeed, Force);
            Destroy(Bullet, BulletLifeTime);
            EnemyAI enemyAI = hit.transform.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Debug.Log("Hit");
                enemyAI.TrunOnRagdoll();
                Rigidbody[] rbs = enemyAI.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in rbs)
                {
                    rb.AddForce(Force);
                }
            }
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
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
