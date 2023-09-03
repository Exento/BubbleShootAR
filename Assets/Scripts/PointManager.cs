using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PointManager : MonoBehaviour
{
    private int points = 0;
    public Button b1;
    public Button b2;
    public Button b3;

    public TMP_Text balance;

    public static PointManager Instance;
    public Transform cameraTransform;  // Reference to the main camera transform
    public Transform targetCanvas;     // Reference to the target canvas transform

    public AudioClip buySound;
    public AudioClip negativeSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        balance.text = "test";
    }


    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public int cost;
        public int level = 0;

        public Upgrade(string name, int cost)
        {
            this.name = name;
            this.cost = cost;
        }
    }

    public List<Upgrade> upgrades;
    public List<Upgrade> selectedUpgrades = new List<Upgrade>(3);
    public Shooter shooter;

    void Start()
    {
        UpdateBalance();

        // Initialize upgrades
        upgrades = new List<Upgrade>
        {
            new Upgrade("+30% Reload", 300),
            new Upgrade("+1 Piercing", 500),
            new Upgrade("+40% ArrowSize", 100),
            new Upgrade("+1 Multishot", 700),
            new Upgrade("+50% FlySpeed", 200),
            new Upgrade("+1 Damage", 300)
        };

        //gameObject.SetActive(false);
        b1.transform.parent.gameObject.SetActive(false);
        Debug.Log($"Deactivated {b1.transform.parent.gameObject.name}");
    }

    private void Update()
    {
        // Rotate the canvas to face the camera
        FaceCamera();
    }

    private void FaceCamera()
    {
        if (targetCanvas == null) return;

        // Create a new rotation based on the relative position of the camera and the canvas
        Vector3 relativePos = cameraTransform.position - targetCanvas.position;

        // Reset the y component to zero so that the canvas only rotates around its y-axis
        relativePos.y = 0;

        // Calculate the rotation using Quaternion.LookRotation
        Quaternion rotation = Quaternion.LookRotation(-relativePos);

        // Apply rotation to the target canvas
        targetCanvas.rotation = rotation;
    }

    void SelectRandomUpgrades()
    {
        selectedUpgrades.Clear();

        List<Upgrade> availableUpgrades = new List<Upgrade>(upgrades);

        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, availableUpgrades.Count);
            selectedUpgrades.Add(availableUpgrades[index]);
            availableUpgrades.RemoveAt(index);
        }
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI");

        UpdateButton(b1, selectedUpgrades[0], points);
        UpdateButton(b2, selectedUpgrades[1], points);
        UpdateButton(b3, selectedUpgrades[2], points);

        // Clear old listeners - quick fix for multiple listeners in later rounds
        b1.onClick.RemoveAllListeners();
        b2.onClick.RemoveAllListeners();
        b3.onClick.RemoveAllListeners();

        // Add new listeners
        b1.onClick.AddListener(() => BuyUpgrade(selectedUpgrades[0].name));
        b2.onClick.AddListener(() => BuyUpgrade(selectedUpgrades[1].name));
        b3.onClick.AddListener(() => BuyUpgrade(selectedUpgrades[2].name));

        b1.transform.parent.gameObject.SetActive(true);
    }

    private void UpdateButton(Button button, Upgrade upgrade, int playerPoints)
    {
        TMP_Text textComponent = button.GetComponentInChildren<TMP_Text>();
        textComponent.text = $"{upgrade.name} ({upgrade.cost}$)";
        textComponent.color = (upgrade.cost > playerPoints) ? Color.red : Color.black;
    }

    public void BuyUpgrade(string upgradeName)
    {
        // Your existing code for buying an upgrade
        Upgrade selected = selectedUpgrades.Find(upg => upg.name == upgradeName);
        if (selected != null && points >= selected.cost)
        {

            points -= selected.cost;

            // Code to deduct player's money and update the upgrade
            selected.level++;
            selected.cost = (int)(selected.cost * 2f);

            switch (selected.name) 
            {
                case "+30% Reload":
                    shooter.upReload(30);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                case "+1 Piercing":
                    shooter.upHP(1);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                case "+40% ArrowSize":
                    shooter.upSize(40);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                case "+1 Multishot":
                    shooter.upMShot(1);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                case "+50% FlySpeed":
                    shooter.upForce(50);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                case "+1 Damage":
                    shooter.upDMG(1);
                    Debug.Log($"Debug: Player bought {selected.name}");
                    break;
                default:
                    Debug.LogWarning($"ERROR: Bad Update: {selected.name}");
                    break;
            }

            audioSource.PlayOneShot(buySound);
            UpdateBalance();
        }else
        {
            audioSource.PlayOneShot(negativeSound);
        }

        // Update the UI again to reflect the new levels and costs
        UpdateUI();
        b1.transform.parent.gameObject.SetActive(false);
        gameObject.GetComponent<GameArea>().continueGame();
    }

    public void UpdateBalance()
    {
        balance.text = $"{points}\nPoints";
        Debug.Log("Balance updated to: " + points);
    }


    public void addPoints(int amount)
    {
        #if UNITY_EDITOR

        amount *= 10;
        #endif

        points += amount;
        UpdateBalance();
        Debug.Log($"Player now has {points} Points");
    }

    public int getPoints(int amount)
    {
        return points;
    }

    public void shop()
    {
        // Select random upgrades to offer
        SelectRandomUpgrades();
        UpdateUI();
    }
}
