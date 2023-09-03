using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

public class Shooter : MonoBehaviour
{
    public GameObject projectilePrefab; // The projectile prefab
    public Transform shootPoint; // The point from which the projectile will be shot
    private float shootForce = 1.0f; // Initial force applied to the projectile
    public Camera arCamera; // Reference to the AR Camera
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject arrow;
    public float shootAnimTime = 0.05f;
    private bool isReloading = false;
    public float reloadTime = 2f;
    public float gameScale = 1f;
    public GameArea gameArea;

    //STATS
    private int projectile_DMG = 1;
    private int projectile_HP = 1;
    private float projectile_Scale = 0.1f;
    private int shots = 1;

    //sound
    public AudioClip[] crossbowSounds;
    public AudioSource audioSource;

    private void Start()
    {
        skinnedMeshRenderer.SetBlendShapeWeight(0, 100f);

        //testing upgrades
        /*
        upMShot(1);
        upSize(300);
        upHP(4);
        upForce(100);
        upReload(60);
        */
    }

    void Update()
    {
        // Detect a single screen tap.
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                StartCoroutine(ShootAndReload());
                //Debug.Log("Tap Registered");
            }
        }

        // This will also allow you to shoot from the editor or a non-mobile build using the mouse.
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootAndReload());
            //Debug.Log("Left Click Registered");
        } 
    }

    private IEnumerator ShootAndReload()
    {

        if (!isReloading)
        {
            isReloading = true;

            // Smoothly set the blend shape weight to make it look "loaded"
            float startTime = Time.time;
            while (Time.time < startTime + shootAnimTime)
            {
                float t = (Time.time - startTime) / shootAnimTime;
                float weight = Mathf.Lerp(100f, 0f, t);
                skinnedMeshRenderer.SetBlendShapeWeight(0, weight);
                yield return null;
            }

            // Shoot the arrow
            Shoot();
            arrow.SetActive(false);

            // Smoothly reset the blend shape weight
            startTime = Time.time;
            while (Time.time < startTime + reloadTime)
            {
                float t = (Time.time - startTime) / reloadTime;
                float weight = Mathf.Lerp(0f, 100f, t);
                skinnedMeshRenderer.SetBlendShapeWeight(0, weight);
                yield return null;
            }

            isReloading = false;
            arrow.SetActive(true);
        }
        
    }

    void Shoot()
    {
        //Debug.Log("Starting Shot");

        PlayRandomCrossbowSound(); //player sound

        // Calculate the total spread angle, for example 30 degrees
        float totalSpreadAngle = 30f;

        // Calculate the individual angle step between each shot
        float angleStep = 0;
        if (shots > 1)
        {
            angleStep = totalSpreadAngle / (shots - 1);
        }

        // Start with negative half of the total spread angle or zero if only one shot
        float currentAngle = (shots > 1) ? -totalSpreadAngle / 2 : 0;

        for (int i = 0; i < shots; i++)
        {
            // Create a projectile instance at the shootPoint position
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

            // Send stats to the projectile
            projectile.GetComponent<Projectile>().setStats(projectile_DMG, projectile_HP, projectile_Scale, gameArea.corner1.transform.position, gameArea.corner2.transform.position);

            // Get the Rigidbody component from the instantiated projectile
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Calculate the shoot direction based on the phone's orientation and current angle
                Vector3 shootDirection = Quaternion.Euler(0, currentAngle, 0) * shootPoint.transform.forward;

                float finalForce = (shootForce * gameScale + 0.5f);

                // Apply force to the projectile
                rb.AddForce(shootDirection * finalForce, ForceMode.Impulse);
            }

            // Increment the angle for the next shot
            currentAngle += angleStep;
        }
    }



    public void upReload(int reload)
    {
        // Convert reload to float for more accurate division
        float reloadPercent = (float)reload / 100;

        // Reduce reload time
        reloadTime *= 1 - reloadPercent;

        // Check for a minimum reload time (for example, 0.1 seconds)
        if (reloadTime < 0.1f)
        {
            reloadTime = 0.1f;
        }

        Debug.Log($"Debug: Reload time got reduced to {reloadTime}");
    }

    public void upHP(int hp)
    {
        projectile_HP += hp;
        Debug.Log($"Debug: Arrow HP got upgraded to {projectile_HP}");
    }

    public void upSize(int size)
    {
        projectile_Scale *= 1 + ((float)size / 100);
        Debug.Log($"Debug: Size got upgraded to {projectile_Scale}");
    }

    public void upMShot(int shots)
    {
        this.shots += shots;
        Debug.Log($"Debug: Shots got upgraded to {this.shots}");
    }

    public void upForce(int force)
    {
        shootForce *= 1 + ((float)force / 100);
        Debug.Log($"Debug: Shoot force got upgraded to {shootForce}");
    }

    public void upDMG(int dmg)
    {
        projectile_DMG += dmg;
        Debug.Log($"Debug: Damage got upgraded to {projectile_DMG}");
    }

    void PlayRandomCrossbowSound()
    {
        // Choose a random clip
        int randomIndex = Random.Range(0, crossbowSounds.Length);

        // Play the clip
        audioSource.PlayOneShot(crossbowSounds[randomIndex]);
    }
}