using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    GameObject miPersonaje;
    
    // Start is called before the first frame update
    void Start()
    {

        if (PhotonNetwork.IsConnected)
        {
            
            //Instanciamos el personaje
            object valorHash = PhotonNetwork.LocalPlayer.CustomProperties["equipo"];
            int equipo = (int)valorHash;
            Debug.Log("equipo seleccionado " + equipo);
            if (equipo == 2)
            {
                miPersonaje = PhotonNetwork.Instantiate("SwatBlue", new Vector3(0, 0, 0), Quaternion.identity);
            }
            else
            {
                miPersonaje = PhotonNetwork.Instantiate("SwatRed", new Vector3(0, 0, 0), Quaternion.identity);
            }
            //Colocamos la cámara
            Camera.main.transform.SetParent(miPersonaje.transform);

            //Movemos al personaje a una posición aleatoria
            float valorPos = Random.Range(-3, 3);
            miPersonaje.transform.position = new Vector3(valorPos, 0, valorPos);
        }
        
    }
}