using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour {

    [System.Serializable]
    public struct TunnelPrefab {
        public GameObject prefab;
        public int poolCount;
    }
    //The main camera that follows the ship
    public GameObject mainCamera;
    //The position of the camera relative to the ship
    public Vector3 cameraOffset;
    //Prefabs that can spawn (pooled at start)
    public TunnelPrefab[] tunnels;
    //Constant forwards velocity of the ship
    public float speed;
    //Strafing speed of the ship
    public float strafeSpeed;
    //Name of the tag an object has if it causes game over
    public string crashTag;
    //The number of tunnel objects loaded at once
    public int numTunnels;
    //The number of empty tunnels at the game start
    public int startTunnels;
    //Collider that prevents exiting the tunnel
    public GameObject tunnelCollider;
    //Acceleration of the ship
    public float acceleration;
    //The minimum amount of time before the game can restart after dying
    public float restartDelay;
    //The camera offset relative to the ship
    public float offset;

    new Rigidbody rigidbody;
    float tunnelLength = 10.0f;
    float latestTunnelPos;
    bool gameOver = false;
    bool started = false;

    //The UI component responsible for displaying the distance travelled
    public GameObject scoreUI;
    TextMeshPro scoreText;
    //The UI component responsible for displaying the current speed
    public GameObject speedUI;
    TextMeshPro speedText;
    //The UI component responsible for displaying the current speed
    public GameObject restartUI;
    TextMeshPro restartText;
    //Prefab for ship explosion
    public GameObject explosion;
    //Disabled spark particle effect
    public GameObject sparksLeft;
    //Disabled spark particle effect
    public GameObject sparksRight;

    float disableLeftIn = 0.0f;
    float disableRightIn = 0.0f;

    float desiredAngle;
    float currentAngle;
    //Degrees per second ship can rotate at
    public float turningSpeed;
    //Maximum angle ship can rotate to
    public float maxRotation;

    List<GameObject> activeTunnels;
    List<GameObject> inactiveTunnels;

    Transform cam;

    float gameOverTime;

    float initialPos;
    //How long before the game starts
    public float initialPause;
    //Multiplier to apply to speed when boosting
    public float boostMultiplier;
    //FOV when boosting
    public float boostFOV;
    //Standard FOV
    public float normalFOV;

    Camera camComponent;

    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        scoreText = scoreUI.GetComponent<TextMeshPro>();
        speedText = speedUI.GetComponent<TextMeshPro>();
        restartText = restartUI.GetComponent<TextMeshPro>();
        latestTunnelPos = -tunnelLength * 2.0f;
        //Create Object pool
        activeTunnels = new List<GameObject>();
        inactiveTunnels = new List<GameObject>();
        for (int i = 0; i < tunnels.Length; i++) {
            for (int j = 0; j < tunnels[i].poolCount; j++) {
                var t = Instantiate(tunnels[i].prefab);
                t.SetActive(false);
                inactiveTunnels.Add(t);
            }
        }
        //Create initial tunnel (made empty to give a chance to get ready)
        for(int i = 0; i < numTunnels; i++) {
            CreateTunnel(i < startTunnels);
        }
        desiredAngle = 0.0f;
        currentAngle = 0.0f;
        cam = mainCamera.transform;
        initialPos = transform.position.z;
        mainCamera.GetComponent<Camera>().fieldOfView = normalFOV;
        camComponent = mainCamera.GetComponent<Camera>();
    }

    void FixedUpdate() {
        bool boost = CrossPlatformInputManager.GetButton("Boost");
        //Apply boost FOV
        if (boost) {
            camComponent.fieldOfView = boostFOV;
        } else {
            camComponent.fieldOfView = normalFOV;
        }
        if (!started) {
            return;
        }
        if(initialPause > 0) {
            initialPause -= Time.fixedDeltaTime;
            if(initialPause < 0) {
                restartUI.SetActive(false);
            }
            restartText.text = (initialPause+0.5f).ToString("n0");
            return;
        }
        if (gameOver) {
            return;
        }
        //Keep infinite tunnel going
        if (transform.position.z - offset > latestTunnelPos - (tunnelLength * (numTunnels - 2))) {
            CreateTunnel();
        }
        //Accelerate if below desired speed
        float forwardSpeed = rigidbody.velocity.z;
        rigidbody.AddForce(new Vector3(0, 0, 1) * ((speed * (boost ? boostMultiplier : 1.0f)) - forwardSpeed), ForceMode.VelocityChange);
        //Apply strafe
        float strafe = CrossPlatformInputManager.GetAxis("Horizontal") * strafeSpeed;
        float rightSpeed = rigidbody.velocity.x;
        rigidbody.AddForce(new Vector3(1, 0, 0) * (strafe - rightSpeed), ForceMode.VelocityChange);
        //Apply vertical
        float vert = CrossPlatformInputManager.GetAxis("Vertical") * strafeSpeed;
        float vertSpeed = rigidbody.velocity.y;
        rigidbody.AddForce(new Vector3(0, 1, 0) * (vert - vertSpeed), ForceMode.VelocityChange);
        //Smooth rotation based on turn direction
        desiredAngle = strafe / strafeSpeed * -maxRotation;
        //Move in correct direction
        bool neg = currentAngle > desiredAngle;
        float mult = neg ? -1.0f : 1.0f;
        currentAngle += mult * turningSpeed;
        if(neg && currentAngle < desiredAngle) {
            currentAngle = desiredAngle;
        }else if (!neg && currentAngle > desiredAngle) {
            currentAngle = desiredAngle;
        }
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, currentAngle);
        //Make game harder by accelerating constantly
        speed += Time.fixedDeltaTime * acceleration;
        tunnelCollider.transform.position = new Vector3(0.0f, 0.0f, transform.position.z);
    }

    private void CreateTunnel(bool initial = false) {
        int index = Random.Range(0, inactiveTunnels.Count);
        if(initial) {
            index = 0;
        }
        GameObject tunnel = inactiveTunnels[index];
        //Move tunnel to correct container
        inactiveTunnels.RemoveAt(index);
        activeTunnels.Add(tunnel);
        //Disable oldest tunnel segment
        if (activeTunnels.Count > numTunnels) {
            GameObject oldest = activeTunnels[0];
            oldest.SetActive(false);
            activeTunnels.RemoveAt(0);
            inactiveTunnels.Add(oldest);
        }
        //Reposition and reactivate new tunnel
        tunnel.transform.position = new Vector3(0.0f, 0.0f, latestTunnelPos + tunnelLength);
        tunnel.SetActive(true);
        //Update tracker for tunnel position
        latestTunnelPos += tunnelLength;
    }

    private void Update() {
        if(!started) {
            if(Input.anyKeyDown) {
                started = true;
            } else {
                return;
            }
        }
        if (gameOver) {
            gameOverTime += Time.deltaTime;
            if (Input.anyKeyDown && gameOverTime > restartDelay) {
                SceneManager.LoadScene(0);
            }
        }
        cam.position = transform.position + cameraOffset;
        if(cam.position.y <= -4.7f) {
            cam.position = new Vector3(cam.position.x, -4.7f, cam.position.z);
        }
        if (cam.position.y >= 4.7f) {
            cam.position = new Vector3(cam.position.x, 4.7f, cam.position.z);
        }
        scoreText.text = "Distance: " + (transform.position.z-initialPos).ToString("n0") + "m";
        if (CrossPlatformInputManager.GetButton("Boost") && !gameOver) {
            speedText.text = "Speed: " + (speed * boostMultiplier).ToString("n0") + "m/s";
        } else {
            speedText.text = "Speed: " + speed.ToString("n0") + "m/s";
        }
        disableLeftIn -= Time.deltaTime;
        if (disableLeftIn <= 0) {
            disableLeftIn = 0;
            sparksLeft.SetActive(false);
        }
        disableRightIn -= Time.deltaTime;
        if (disableRightIn <= 0) {
            disableRightIn = 0;
            sparksRight.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.CompareTag(crashTag)) {
            GameOver();
        } else {
            ApplySparks(collision);
        }
    }

    private void OnCollisionStay(Collision collision) {
        ApplySparks(collision);
    }


    private void ApplySparks(Collision collision) {
        foreach (ContactPoint point in collision.contacts) {
            if (transform.position.x < point.point.x) {
                disableRightIn = 0.5f;
            } else {
                disableLeftIn = 0.5f;
            }
        }
        if (disableLeftIn > 0) {
            sparksLeft.SetActive(true);
        }
        if (disableRightIn > 0) {
            sparksRight.SetActive(true);
        }
    }

    private void GameOver() {
        gameOver = true;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        rigidbody.angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Instantiate(explosion, transform);
        restartUI.SetActive(true);
        restartText.text = "Press Any Key To Restart";
    }
}
