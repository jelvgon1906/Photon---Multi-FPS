using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;


public class CtrlConexion : MonoBehaviourPunCallbacks
{
    #region Constantes
    //Constantes de definición de equipos
    public const int SIN_EQUIPO = 0;
    public const int ROJO = 1;
    public const int AZUL = 2;
    #endregion

    #region Variables privadas

    ExitGames.Client.Photon.Hashtable playerProperties;

    #endregion


    #region Variables publicas
    public GameObject panelInicio; //Panel de inicio del juego
    public GameObject panelBienvenida; //Panel de bienvenida del juego
    public GameObject panelCreacionSala; //Panel para creación de una sala
    public GameObject panelSala; //Panel con los jugadores conectados
    public GameObject panelConexionSala; //Panel para unirse a una sala
    public GameObject panelSeleccionEquipo; //Panel para seleccionar el equipo del jugador

    public TMP_InputField inputNickname; //Entrada con el nombre de usuario
    public TextMeshProUGUI txtEstado;  //Contiene la última salida por pantalla en estado
    public TextMeshProUGUI txtInfoUser;  //Contiene información sobre el usuario

    public TMP_InputField inputNombreSala; //Nombre de la sala a crear
    public TMP_InputField inputMinJug;  //Mínimo número de jugadores de la sala creada
    public TMP_InputField inputMaxJug;  //Máximo número de jugadores en la partida

    public TMP_InputField inputNombreSalaJoin; //Nombre de la sala a unirse

    public TextMeshProUGUI txtNombreSalaPanelSala; //Etiqueta donde aparece el nombre de la sala
    public TextMeshProUGUI txtCapacidadPanelSala; //Etiqueta con la ocupación actual de la sala

    public Button botonConectar; //Instancia del boton Conectar
    public Button botonComenzarJuego; //Instancia del boton Conectar

    public GameObject elemJugador; //Cada uno de los botones que representa un jugador en la lista de sala
    public GameObject contenedorJugadores; //Contenedor que mantiene la lista de jugadores

    
    #endregion

    private void Start()
    {
        if (panelInicio != null) { CambiarPanel(panelInicio); }
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(this.gameObject);

        /*contenedorJugadores = GameObject.FindWithTag("contenedorJugadores");*/
    }


    #region Eventos para botones
    /// <summary>
    /// Método que se ejecuta al pulsar el botón de Conexión a Photon
    /// Comprueba si el nombre de usuario es correcto y realiza la conexión
    /// </summary>
    public void OnClickConectarAServidor()
    {
        //Comprobamos si el nombre de usuario es correcto
        if (!(string.IsNullOrWhiteSpace(inputNickname.text) ||
            string.IsNullOrEmpty(inputNickname.text)))
        {
            //Comprobamos si no estamos ya conectados a Photon
            if (!(PhotonNetwork.IsConnected))
            {
                //Deshabilitamos el botón para evitar doble pulsación
                botonConectar.interactable = false;
                //Conectamos a Photon con la configuración de usuario
                PhotonNetwork.ConnectUsingSettings(); 
                
                CambiarEstado("Conectando...");
            }
            else
            {
                //Indicar que ya estamos conectados
                CambiarEstado("Ya está conectado a Photon");
            }
        } else
        {
            //Indicar que el nombre no es correcto
            CambiarEstado("Nombre de usuario incorrecto");
        }

        
        

    }

    /// <summary>
    /// Método que se lanza al pulsar el botón Crear Sala del 
    /// menú de bienvenida. Cambia al panel CreacionSala
    /// </summary>
    public void OnClickIrACrearSala()
    {
        CambiarPanel(panelCreacionSala);
    }
    public void OnClickIrAConectarASala()
    {
        CambiarEstado("Conexión a una sala existente");
        CambiarPanel(panelConexionSala); 
        
    }

    /// <summary>
    /// Método que se lanza al pulsar el botón 
    /// Crear Sala del panel Creación de sala.
    /// Este método comprueba que el nombre de la sala es válido
    /// y que los valores para el número de jugadores son correctos
    /// </summary>
    public void OnClickCrearSala()
    {
        //Empezamos comprobando el nombre de sala. 
        //Si es válido, comprobamos los valores para el número de jugadores.
        //Sólo entonces, se crea la sala con los valores indicados.

        int min, max;
        min = int.Parse(inputMinJug.text);
        max = int.Parse(inputMaxJug.text);

        if (!(string.IsNullOrWhiteSpace(inputNombreSala.text) ||
            string.IsNullOrEmpty(inputNombreSala.text)))
        {
            if (min>0 && max >= min)
            {
                RoomOptions opcionesSala = new RoomOptions();
                opcionesSala.MaxPlayers = (byte) max;
                opcionesSala.IsVisible = true;
                //opcionesSala.IsOpen = false;

                PhotonNetwork.CreateRoom(inputNombreSala.text, opcionesSala, TypedLobby.Default);
            }
            else
            {
                CambiarEstado("Número de jugadores no válido");
            }
        }
        else
        {
            CambiarEstado("Nombre de sala incorrecto");
        }

    }

    /// <summary>
    /// Método que comprueba el nombre de la sala 
    /// para conectarse a ella si existe.
    /// Lanza el método JoinRoom de PUN
    /// </summary>
    public void OnClickUnirseASala()
    {
        if (!string.IsNullOrEmpty(inputNombreSalaJoin.text))
        {
            PhotonNetwork.JoinRoom(inputNombreSalaJoin.text);
        }
        else
        {
            CambiarEstado("Introduzca un nombre correcto para la sala");
        }
    }

    /// <summary>
    /// Este método sacará al jugador de la sala en la que se encuentre
    /// y se irá al panel de bienvenida
    /// </summary>
    public void OnClickSalirYVolver()
    {
        PhotonNetwork.LeaveRoom();
        CambiarEstado(PhotonNetwork.NickName + " abandona la sala");
        CambiarPanel(panelBienvenida);
    }
    /// <summary>
    /// Este método cargará de nuevo el panel de bienvenida
    /// </summary>
    public void OnClickVolver()
    {
        CambiarEstado(PhotonNetwork.NickName + ", escoja crear o unirse a una sala");
        CambiarPanel(panelBienvenida);

    }
    /// <summary>
    /// Este método desconecta de Photon y sale a Intro
    /// </summary>
    public void OnClickDesconectar()
    {
        PhotonNetwork.Disconnect();
        CambiarEstado("Desconectando...");
    }
    #endregion

    public void OnClickComenzarJuego()
    {
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Asigna al local player un valor en un id "equipo"
    /// de la tabla Hash, con el equipo seleccionado
    /// </summary>
    public void OnClickEquipoRojo()
    {
        SeleccionEquipo(ROJO);

    }

    public void OnClickEquipoAzul()
    {
        SeleccionEquipo(AZUL);

    }


    public void OnClickSeleccionarEquipo()
    {
        CambiarPanel(panelSeleccionEquipo);
    }




    #region Eventos propios de Photon
    public override void OnConnected()
    {
        //base.OnConnected();
        CambiarEstado("Conectado a Photon");
        PhotonNetwork.NickName = inputNickname.text;
        txtInfoUser.text = PhotonNetwork.NickName;
        //Cambiamos a panelBienvenida
        CambiarPanel(panelBienvenida);

        playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add("equipo", SIN_EQUIPO); //Valor 0 indica SIN_EQUIPO
        playerProperties.Add("health", 101);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        CambiarEstado("Usuario desconectado: " + cause.ToString());
        CambiarPanel(panelInicio);
        txtInfoUser.text = "No user";
        botonConectar.interactable = true;
    }

    public override void OnCreatedRoom()
    {
        string mensaje = PhotonNetwork.NickName
            + " se ha conectado a "
            + PhotonNetwork.CurrentRoom.Name;
        CambiarEstado(mensaje);
        CambiarPanel(panelSala);
        ActualizarPanelJugadores();
    }

    public override void OnJoinedRoom()
    {
        string mensaje = PhotonNetwork.NickName
        + " se ha unido a "
        + PhotonNetwork.CurrentRoom.Name;
        CambiarEstado(mensaje);
        CambiarPanel(panelSala);
        ActualizarPanelJugadores();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        CambiarEstado("Error al crear sala: " + message);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        CambiarEstado("No ha sido posible conectar a la sala: " + message);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ActualizarPanelJugadores();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ActualizarPanelJugadores();
    }

    #endregion


    #region Métodos privados
    /// <summary>
    /// Método que cambiará el mensaje de Estado 
    /// de los paneles de introducción al juego
    /// </summary>
    /// <param name="texto">Nuevo mensaje a colocar</param>
    private void CambiarEstado (string texto)
    {
        txtEstado.text = texto;
    }


    /// <summary>
    /// Permite la navegación entre paneles de una forma cómoda, 
    /// cambiando al panel indicado como parámetro. 
    /// Para ello, desactiva todos y activa el que se indica.
    /// </summary>
    /// <param name="panelObjetivo">Panel a activar</param>
    private void CambiarPanel (GameObject panelObjetivo)
    {
        panelBienvenida.SetActive(false);
        panelInicio.SetActive(false);
        panelCreacionSala.SetActive(false);
        panelSala.SetActive(false);
        panelConexionSala.SetActive(false);
        panelSeleccionEquipo.SetActive(false);

        panelObjetivo.SetActive(true);
    }

    /// <summary>
    /// Este método revisa la lista de jugadores de la sala, 
    /// y actualiza en el Panel de Sala, toda la información
    /// para que sea visible por el usuario.
    /// Crea tantos botones como jugadores en la sala y 
    /// representa su información en cada uno de ellos
    /// </summary>
    private void ActualizarPanelJugadores()
    {
        //Actualizaci�n del nombre de sala y su capacidad
        txtNombreSalaPanelSala.text = PhotonNetwork.CurrentRoom.Name;
        txtCapacidadPanelSala.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        //Eliminamos todos los botones para empezar desde cero en cada actualización
        while (contenedorJugadores.transform.childCount > 0)
        {
            if (panelInicio != null)
            {
                DestroyImmediate(contenedorJugadores.transform.GetChild(0).gameObject);
            }
               
        }


        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            //Instanciamos un nuevo boton y lo colgamos del contenedor
            GameObject nuevoElemento = Instantiate(elemJugador);
            nuevoElemento.transform.SetParent(contenedorJugadores.transform, false);
            //Localizamos sus etiquetas y las actualizamos
            nuevoElemento.transform.Find("txtNombreJugador").GetComponent<TextMeshProUGUI>().text = jugador.NickName;
            //Equipo del jugador            
            object equipoJugador = jugador.CustomProperties["equipo"];
            string equipo = "";
            switch ((int)equipoJugador)
            {
                case SIN_EQUIPO:
                    equipo = "Sin equipo";
                    break;
                case ROJO:
                    equipo = "Rojo";
                    break;
                case AZUL:
                    equipo = "Azul";
                    break;
            }
            
            nuevoElemento.transform.Find("txtEquipo").GetComponent<TextMeshProUGUI>().text = equipo;

        }


        //Activaci�n de bot�n Comenzar Juego si el n�mero m�nimo de jugadores est� en la sala
        if (PhotonNetwork.CurrentRoom.PlayerCount >= int.Parse(inputMinJug.text)
            && PhotonNetwork.IsMasterClient)
        {
            botonComenzarJuego.gameObject.SetActive(true);
        }
        else
        {
            botonComenzarJuego.gameObject.SetActive(false);
        }
    
    }

    private void SeleccionEquipo(int equipo)
    {
        playerProperties["equipo"] = equipo;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        CambiarEstado("Equipo seleccionado: " + (equipo == ROJO ? "rojo" : "azul"));
        CambiarPanel(panelBienvenida);
    }

    



    #endregion
}
