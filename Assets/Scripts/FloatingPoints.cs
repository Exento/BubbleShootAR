using System;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingPoints : MonoBehaviour
{
    
    public Material mat1;
    public Material mat2;
    public Material mat3;

    public int type = 0;
    public float speed = 0.2f;
    public float scaleFactor = 1.0f;

    //public PointManager pointManager;

    private int hp = 1;
    public float maxDistance = 2f;
    private Vector3 initialPosition;
    private int points;
    private Transform childTransform;


    void changeMat(MeshRenderer objMesh)
    {

        switch (hp)
        {
            case 1:
                objMesh.material = mat1;
                break;
            case 2:
                objMesh.material = mat2;
                break;
            case 3:
                objMesh.material = mat3;
                break;
            default:
                Debug.LogWarning($"ERROR: Bad material for hp: {hp}");
                break;
        }
    }

    public void setType(int t)
    {
        if (t > 2)
        {
            t = 2;
        }

        switch (t)
        {
            case 0:
                hp = 1;
                points = 20;
                childTransform = transform.Find("ball");
                break;
            case 1:
                hp = 2;
                points = 40;
                childTransform = transform.Find("cube");
                break;
            case 2:
                hp = 3;
                points = 60;
                childTransform = transform.Find("ring");
                break;
        }
        // If the child was found
        if (childTransform != null)
        {
            initialPosition = childTransform.position;
            changeMat(childTransform.GetComponent<MeshRenderer>());
            childTransform.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Error: Child not found. Type: {t}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"hit: {other.name}");
        if (other.gameObject.GetComponent<Projectile>() != null) 
        {
            Debug.Log("Projectile hit Bubble!");
            hp -= other.gameObject.GetComponent<Projectile>().dmg;
            other.gameObject.GetComponent<Projectile>().lowerHP();
            //play hit effect

            if (hp <= 0 )
            {
                PointManager.Instance.addPoints(points);
                PlayFieldAudio.Instance.AudioDestroy();
                Destroy(gameObject);
            }
            else
            {
                PlayFieldAudio.Instance.AudioHit();
                if (childTransform != null)
                {
                    changeMat(childTransform.GetComponent<MeshRenderer>());
                }
                else
                {
                    Debug.LogWarning("Child object's transform is not assigned, can't change material.");
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += Vector3.up * speed * scaleFactor * Time.deltaTime;
        float distance = Vector3.Distance(initialPosition, gameObject.transform.position);

        if (distance >= maxDistance)
        {
            Debug.Log("Child object has disappeared!");
            Destroy(gameObject);
        }
    }

}

