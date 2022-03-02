using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
    public float StoredAmmo;
    public Camera ScopeCam;
    private float scrollWheel = 9f;
    private Camera cam;
    public Recoil recoil;
    private PostProcessVolume volume;
    private DepthOfField blur;
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
    [Space(10)]
    [Header("Camera Shake")]
    public float Magnitude;
    public float Roughness;
    public float FadeInTime;
    public float FadeOutTime;

    [Space(10)]
    [Header("Animation Rigging")]
    public MultiPositionConstraint ScopePositionRig;
    public MultiRotationConstraint ScopeRotationRig;
    public float DampTime;
    [Space(10)]
    [Header("Bullet Proaperties")]
    public float bulletSpeed;
    public float BulletLifeTime;
    [Space(10)]
    [Header("Gravity And Wind ")]
    public float Gravity;
    public float Wind;
    [Space(10)]
    [Header("Recoil")]
    public float Intensity;
    public float Duration;
    void Awake()
    {
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
        StoredAmmo = maxAmmo * 6f;
        string Storedammo = StoredAmmo.ToString();
        string current = CurrentAmmo.ToString();
        ammo.text = current + "\n" + Storedammo;
        recoil = Sniper.GetComponent<Recoil>();
    }
    void Update()
    {
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
        string Storedammo = StoredAmmo.ToString();
        string current = CurrentAmmo.ToString();
        ammo.text = current + "\n" + Storedammo;
    }
    void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * Sencitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Sencitivity;
        xrot -= mouseY;
        xrot = Mathf.Clamp(xrot, -90f, 90);
        holder.localRotation = Quaternion.Euler(xrot, 0f, 0f);
        player.Rotate(Vector3.up * mouseX);

    }
    public void ShellPlay ()
    {
        Shell.Play();
    }
    IEnumerator Shoot()
    {
        CurrentAmmo--;
        Vector3 mousePos = Vector3.zero;
        Vector3 ScreenCenter = new Vector3(Screen.width / 2f, Screen.height / 2);
        Ray ray = cam.ScreenPointToRay(ScreenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit, Range, layer))
        {
            mousePos += hit.point;
            mousePos += new Vector3(Wind, Gravity);
            Vector3 aimDir = (mousePos - firePosition.position).normalized;
            Debug.DrawLine(firePosition.position, mousePos, Color.red, 5f);
            GameObject Bullet = Instantiate(BulletPf, firePosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            Bullet bullet = Bullet.GetComponent<Bullet>();
            bullet.Initialized(bulletSpeed);
            Destroy(Bullet, BulletLifeTime);
        }
        animator.SetBool("Bolt",true);
        yield return new WaitForSeconds(1f - 0.25f);
        animator.SetBool("Bolt", false);
        yield return new WaitForSeconds(0.25f);
    }
    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(ReloadTime - 0.25f);
        animator.SetBool("Reload", false);
        yield return new WaitForSeconds(0.25f); 
        CurrentAmmo = maxAmmo;
        StoredAmmo -= maxAmmo;
        isReloading = false;
    }
}
