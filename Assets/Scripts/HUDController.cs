using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviourPunCallbacks
{
    [SerializeField] private int Health = 100;
    public static HUDController hudController;
    ExitGames.Client.Photon.Hashtable infoPlayer;
    public GameObject contenedor;
    public GameObject playerPrefabButton;
    private TextMeshProUGUI txtNick;
    private void Start()
    {
        hudController = this;
        infoPlayer = PhotonNetwork.LocalPlayer.CustomProperties;
        ActualizarVida(Health);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ActualizarJugadores(); 
        }
    }
    public void ActualizarVida(int health)
    {
        infoPlayer["health"] = health;
        if (photonView.IsMine)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(infoPlayer);
        }
    }

    private void ActualizarJugadores()
    {
        
        //Eliminamos todos los botones para empezar desde cero en cada actualización
        while (contenedor.transform.childCount > 0)
        {
            DestroyImmediate(contenedor.transform.GetChild(0).gameObject);
        }


        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            object tmp = jugador.CustomProperties["health"];

            //Instanciamos un nuevo boton y lo colgamos del contenedor
            GameObject nuevoElemento = Instantiate(playerPrefabButton);
            nuevoElemento.transform.SetParent(contenedor.transform, false);
            //Localizamos sus etiquetas y las actualizamos
            nuevoElemento.transform.Find("txtNombreJugador").GetComponent<TextMeshProUGUI>().text = jugador.NickName;
            nuevoElemento.transform.Find("txtHealth").GetComponent<TextMeshProUGUI>().text = tmp.ToString();
        }
    }
}
    
