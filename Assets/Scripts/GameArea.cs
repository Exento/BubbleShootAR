using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;

public class GameArea : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    Vector3 positionP1 = Vector3.zero;
    Vector3 positionP2 = Vector3.zero;

    // Spielfeld-GameObject
    public GameObject gameFieldPosition;
    private GameObject gameField;
    public GameObject pointPrefab;
    public GameObject cornerPrefab;
    public PointManager pointMaster;
    public GameObject shootPoint;
    public Shooter shooter;

    private float RoundTime = 60f;
    private float BubblesPerRound = 10;
    private bool gameStarted = false;
    private int currentRound = 1;
    public GameObject corner1;
    public GameObject corner2;
    public ARPlaneManager planeManager;

    public float scaleFactor = 1f;

    [SerializeField]
    private AudioClip positive;
    [SerializeField]
    private AudioClip stoneSlide;
    [SerializeField]
    private AudioClip audioFin;
    [SerializeField]
    private AudioClip audioButton;

    private List<GameObject> bubbles = new List<GameObject>();

    #if UNITY_EDITOR
        private Vector3 editorPositionP1 = new Vector3(0, 0, 1); // Default test values
        private Vector3 editorPositionP2 = new Vector3(1, 0, 2);
    #endif

    private void Start()
    {
        shootPoint.gameObject.SetActive(false);
        gameField = gameFieldPosition.transform.GetChild(0).gameObject;

        #if UNITY_EDITOR

        //DEBUG
        RoundTime = RoundTime / 4;
        Debug.Log("Setting editor positions");
            Debug.Log($"Debug: Editor Pos1: {editorPositionP1}");
            Debug.Log($"Debug: Editor Pos2: {editorPositionP2}");

            SetTower("p1", editorPositionP1);
            SetTower("p2", editorPositionP2);

            //UpdateGameField(positionP1, positionP2);
        #endif

    }

    void OnEnable() => m_TrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Start the coroutine to wait a bit before setting the tower's position
            StartCoroutine(WaitForARUpdateAndSetTower(newImage));

            gameObject.GetComponent<AudioSource>().PlayOneShot(positive);
        }
    }

    private IEnumerator WaitForARUpdateAndSetTower(ARTrackedImage newImage)
    {
        // Wait for 0.5 seconds (or however long you find works for you)
        yield return new WaitForSeconds(0.5f);

        // Now set the tower's position
        SetTower(newImage.referenceImage.name, newImage.transform.position);
        Debug.Log($"Debug: Found new TrackedImage! Name: {newImage.referenceImage.name}, Position: {newImage.transform.position}");
    }

    private void SetTower(string trackedImageName,Vector3 pos)
    {
        Debug.Log($"Debug: Got position for corner {trackedImageName}: {pos}");
        switch (trackedImageName)
        {
            case "p1":  positionP1 = pos; break;
            case "p2": positionP2 = pos; break;
            default: Debug.LogWarning($"Debug: Incorrect Corner Name: '{trackedImageName}'"); ; break;
        }

        if (positionP1 != Vector3.zero && positionP2 != Vector3.zero)
        {
            corner1 = Instantiate(cornerPrefab, positionP1, Quaternion.identity);
            corner2 = Instantiate(cornerPrefab, positionP2, Quaternion.identity);

            Debug.Log($"Debug: All corners set! P1:{positionP1}, P2:{positionP2}");
            UpdateGameField(positionP1, positionP2);

            // Start the game only if it hasn't already been started
            if (!gameStarted)
            {
                gameStarted = true;
                StartCoroutine(StartGame());
            }
        }
        else
        {
            Debug.Log($"Debug: waiting for more corners! P1:{positionP1}, P2:{positionP2}");
        }


    }


    // Methode, um das Spielfeld zu erzeugen oder zu aktualisieren
    void UpdateGameField(Vector3 p1, Vector3 p2)
    {
        Debug.Log("Debug: Placing GameField!");

        // Calculate the midpoint between p1 and p2
        Vector3 center = (p1 + p2) / 2;

        // Calculate the distance between p1 and p2
        float distance = Vector3.Distance(p1, p2);

        // The scaling factor should be such that the diagonal of the square is equal to the distance between p1 and p2.
        // Since the diagonal of a square = side_length * sqrt(2), the side_length = diagonal / sqrt(2).
        scaleFactor = distance / Mathf.Sqrt(2);  // Assuming your original game field size is 2m x 2m

        shooter.gameScale = scaleFactor;

        // Set the position and scale of the game field
        gameFieldPosition.transform.position = center;
        gameFieldPosition.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor); // Keeping height as 1

        Debug.Log("Debug: Rescaling Towers!");
        //scale corners
        corner1.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        corner2.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        //set gravity accoridng to the scale
        // Set gravity to half its normal strength.
        Physics.gravity = new Vector3(0, -1f * scaleFactor, 0);

        // Set the orientation so that one corner points towards p1 and the opposite corner points towards p2
        // We need to rotate the square 45 degrees around the up axis to align its corners with p1 and p2
        Vector3 direction = (p2 - p1).normalized;
        gameFieldPosition.transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, 45f, 0);


        Debug.Log("Debug: Activating GameField!");
        gameField.SetActive(true);
        gameObject.GetComponent<AudioSource>().PlayOneShot(stoneSlide);
        gameField.GetComponent<Animator>().SetTrigger("start");
        Debug.Log("Debug: GameField set!");

        /*
        // Disable plane detection
        planeManager.enabled = false;

        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
            //plane.GetComponent<MeshRenderer>().enabled = false;
            //plane.GetComponent<LineRenderer>().enabled = false;

            //plane.GetComponent<MeshRenderer>().enabled = false;
        }
        */

        gameStarted = true;
        StartCoroutine(StartGame());
    }

    public void continueGame()
    {
        if (gameStarted == false)
        {
            gameObject.GetComponent<AudioSource>().PlayOneShot(audioButton);
            gameStarted = true;
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        Debug.Log("Debug: Starting GameLoop!");
        gameField.GetComponent<Animator>().SetTrigger("toggle");
        yield return new WaitForSeconds(2);
        BubblesPerRound += currentRound * 2.0f;
        

        float spawnTimer = 0f;
        float nextSpawnTime = 0f;
        float spawnRate = RoundTime / BubblesPerRound;  // Base time between spawns
        float spawnRateRandomness = 0.2f * spawnRate;  // Random variance in spawn time

        float difficulty = 0.05f * currentRound;
        

        shootPoint.gameObject.SetActive(true);


        // Compute the corners and vectors representing the sides of the square
        Vector3 bottomLeft = positionP1;
        Vector3 bottomRight = new Vector3(positionP2.x, positionP1.y, positionP1.z);
        Vector3 topLeft = new Vector3(positionP1.x, positionP1.y, positionP2.z);

        Vector3 rightVector = bottomRight - bottomLeft;
        Vector3 upVector = topLeft - bottomLeft;

        while (spawnTimer < RoundTime)
        {
            if (spawnTimer >= nextSpawnTime)
            {
                // Get type of bubble
                int type = 0;

                // Randomly upgrade the type
                while (Random.Range(0f, 1f) < difficulty && type <= 2)
                {
                    type += 1;
                }


                // Generate random coefficients to find a point within the square
                float randomRight = Random.Range(0.07f, 0.93f);
                float randomUp = Random.Range(0.07f, 0.93f);

                // Compute the random position within the square
                Vector3 randomPosition = bottomLeft + rightVector * randomRight + upVector * randomUp;
                randomPosition.y -= 0.5f * scaleFactor;  // Move the spawn position down

                GameObject newBubble = Instantiate(pointPrefab, randomPosition, Quaternion.identity);
                newBubble.transform.localScale = new Vector3(scaleFactor * 1.5f, scaleFactor * 1.5f, scaleFactor * 1.5f);
                newBubble.GetComponent<FloatingPoints>().maxDistance *= scaleFactor;
                newBubble.GetComponent<FloatingPoints>().setType(type);
                newBubble.GetComponent<FloatingPoints>().scaleFactor = scaleFactor;

                bubbles.Add(newBubble);

                // Calculate next spawn time
                nextSpawnTime += spawnRate + Random.Range(-spawnRateRandomness, spawnRateRandomness);
            }

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        gameField.GetComponent<Animator>().SetTrigger("toggle");
        Debug.Log("Debug: Round Over!");
        currentRound++;

        // Destroy remaining Bubbles
        foreach (GameObject bubble in bubbles)
        {
            if (bubble != null)
            {
                Destroy(bubble);
            }
        }
        bubbles.Clear();
        gameObject.GetComponent<AudioSource>().PlayOneShot(audioFin);
        shootPoint.gameObject.SetActive(false);
        gameStarted = false;
        pointMaster.shop();
    }
}
