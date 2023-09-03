using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int dmg = 1; //DMG to bubbles
    public int hp = 1; //Bubbles the projectile survives
    public float timeToLive = 1f; // time in seconds before the projectile is automatically destroyed
    public Vector3 pointA;
    public Vector3 pointB;

    private float timeAlive;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Ensure the arrow is moving to update its direction
        if (rb.velocity != Vector3.zero)
        {
            // Point the arrow in the direction it is moving
            transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
        }
    }

    void Update()
    {
        // Increment how long this projectile has been alive
        timeAlive += Time.deltaTime;

        // Destroy this projectile if its time to live has passed
        if (timeAlive >= timeToLive)
        {
            Destroy(gameObject);
        }
    }

    public void setStats(int dmg, int hp, float scale, Vector3 a, Vector3 b)
    {
        this.dmg = dmg;
        this.hp = hp;
        transform.localScale = new Vector3(scale, scale, scale);
        pointA = a;
        pointB = b;
    }

    public void lowerHP()
    {
        hp -= 1;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the GameObject the projectile collided with has the AR Plane tag or layer
        if (other.gameObject.tag == "ARPlane")
        {
            // Check if the projectile is between the two points
            if (IsBetweenPoints(transform.position, pointA, pointB))
            {
                // Ignore this collision
                return;
            }

            // Otherwise, make the arrow stick into the plane
            rb.isKinematic = true;
        }
    }

    bool IsBetweenPoints(Vector3 point, Vector3 pointA, Vector3 pointB)
    {
        // Implement logic here to determine if 'point' lies between 'pointA' and 'pointB'
        // This can be simple or complex depending on your specific requirements

        // Example: check if 'point' is between 'pointA' and 'pointB' along the X axis
        return point.x >= Mathf.Min(pointA.x, pointB.x) && point.x <= Mathf.Max(pointA.x, pointB.x);
    }

}
