using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Realtime;
using Cursor = UnityEngine.Cursor;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement")]
    [SerializeField] private int maxLife = 100;
    [SerializeField] private int currentLife = 100;
    public float jumpForce;
    public float speed; //Velocidad máxima del movimiento
    public float rotationSpeed = 5;

    [Header("Controls")]
    Rigidbody rig;
    Animator anim;
    [SerializeField] private int enemiesLeft;
    [SerializeField] GameObject bala;
    [SerializeField] GameObject posCanyon;
    [SerializeField] private TextMeshProUGUI txtVida;
    private bool started;
    [SerializeField] private ScrollRect panelJugadores;
    /*ExitGames.Client.Photon.Hashtable playerProperties;*/
    /*private ExitGames.Client.Photon.Hashtable _myCustomProperties = new ExitGames.Client.Photon.Hashtable ();
    public List<string> gamename = new List<string>();
    public Dictionary<string, int> playerList = new Dictionary<string, int>();*/





    // Start is called before the first frame update
    void Start()
    {
        txtVida = GetComponentInChildren<TextMeshProUGUI>();
        panelJugadores = GetComponentInChildren<ScrollRect>();
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //Si se pulsa teclas Horizontal o Vertical, movemos el personaje
            Move();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Disparo");
                Cursor.lockState = CursorLockMode.Locked;
                photonView.RPC("Shoot", RpcTarget.All);
                //Shoot();
            }
            if (Input.GetButtonUp("Cancel"))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                panelJugadores.gameObject.SetActive(true);
                //actualizar lista mientras esté activo
            }
            else
            {
                panelJugadores.gameObject.SetActive(false);
            }
        }
    }
    private void FixedUpdate()
    {
        Win();
    }

    /*public override void OnDisconnected()
    {
        Destroy(this);
    }*/

    [PunRPC]
    private void Shoot()
    {
        GameObject miBala = Instantiate(bala, posCanyon.transform.position, Quaternion.identity);
        miBala.transform.forward = posCanyon.transform.forward;
    }

    private void Move()
    {
        float movZ = Input.GetAxis("Vertical");
        float movX = Input.GetAxis("Horizontal");

        Vector3 velocidad = movZ * speed * transform.forward
            + transform.right * movX * speed
            + transform.up * rig.velocity.y;

        rig.velocity = velocidad;



        transform.Rotate(transform.up * Input.GetAxis("Mouse X")
            * rotationSpeed);

        velocidad = new Vector3(velocidad.x, 0, velocidad.z);

        anim.SetFloat("velocity", velocidad.magnitude);
    }

    private void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    [PunRPC]
    public void DamagePlayer(int quantity)
    {
        Debug.Log("Hit");
        currentLife -= quantity;
        
        if (currentLife <= 0)
        {
            Debug.Log("Game Over");
            Invoke(nameof(GameOver), .1f);
        }
        if (photonView.IsMine)
        {
            txtVida.text = "vida: " + currentLife + "/" + maxLife;
            HUDController.hudController.ActualizarVida(currentLife);
        }
            
    }

    
    void GameOver()
    {
        if (photonView.IsMine)
        {
            HUDController.hudController.ActualizarVida(currentLife);


            Camera.main.transform.SetParent(null);
            Cursor.lockState = CursorLockMode.None;
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadSceneAsync("Lose");
        }
        Destroy(gameObject);
    }

    void Win()
    {
        /*enemiesLeft = GameObject.FindGameObjectsWithTag("Player").Length;*/
        if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
        {
            if (photonView.IsMine)
            {
                Cursor.lockState = CursorLockMode.None;
                Debug.Log("Win");
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneManager.LoadScene("Win");
            }
        }
    }
    
}
