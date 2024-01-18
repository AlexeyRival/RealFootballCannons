using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CannonPlayer : NetworkBehaviour
{
    //управление
    private Vector2 mouse;
    private float y, z;
    private float startY;

    //сетева€ часть
    private bool isStarted;
    [SyncVar]
    private int playerID;

    //префаб м€ча
    public GameObject ball;

    //цвет
    public Color color;

    //анимаци€ камеры
    private Camera cam;
    private bool camAnim;
    private Vector3 camPoint;

    //сила выстрела
    private float power;
    private Slider powerSlider;

    //выстрел
    [Command]
    private void CmdShot(float power) 
    {
        if (GameObject.FindGameObjectsWithTag("Ball").Length > 20) { return; }//проверка на максимум м€чей
        
        //м€ч
        GameObject ob = Instantiate(ball, transform.position - transform.right * 2, transform.rotation);

        ob.GetComponent<Rigidbody>().AddRelativeForce(-50*power, 0, 0, ForceMode.Impulse);
        ob.transform.name = new string('0', playerID);//зачем парсить строку, когда можно делать еЄ разной длины?

        //покраска м€ча в цвет игрока
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_PlayerColor", color);
        ob.GetComponent<Renderer>().SetPropertyBlock(block);

        Destroy(ob, 15f);

        NetworkServer.Spawn(ob);
    }

    //присвоение номера
    [Command]
    private void CmdSetID(int id) 
    {
        playerID = id;
    }
    
    //кастомизаци€
    [Command]
    private void CmdSetColor(Color color)
    {
        SetColorRPC(color);
    }
    [ClientRpc]
    private void SetColorRPC(Color color)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_PlayerColor", color);
        GetComponent<Renderer>().SetPropertyBlock(block);

        GameObject.Find("Goal " + playerID).GetComponent<Renderer>().SetPropertyBlock(block);//единоразово перекрасить ворота
        this.color = color;
    }

    //подключение к серверу
    private void Start()
    {
        if (!isLocalPlayer) return;

        int id = GameObject.FindGameObjectsWithTag("Player").Length;
        CmdSetID(id);
        transform.position = transform.position + new Vector3(-2 * id, 0, 0);
        
        if (id % 2 == 0)//чтобы в лобби было поживее
        {
            startY = 180;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;
        powerSlider = GameObject.Find("Canvas").transform.Find("Power").GetComponent<Slider>();
    }
    //старт матча
    private void StartMatch()
    {
        isStarted = true;
        Transform spawnpoint = GameObject.Find("SpawnPoint "+playerID).transform;
        transform.position = spawnpoint.position + spawnpoint.transform.forward * 3;
        startY = spawnpoint.rotation.eulerAngles.y + 90;
        camAnim = true;
        camPoint = spawnpoint.position + Vector3.up*2;
        powerSlider.gameObject.SetActive(true);
    }


    void Update()
    {
        if (!isLocalPlayer) return;

        //управление пушкой
        mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        z = Mathf.Clamp(z + mouse.y, -70, 70);
        y = Mathf.Clamp(y + mouse.x, -80, 80);
        transform.rotation = Quaternion.Euler(0, y + startY, z);

        if (!isStarted) //лобби
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) { CmdSetColor(Color.blue); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { CmdSetColor(Color.red); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { CmdSetColor(Color.green); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { CmdSetColor(Color.yellow); }
            if (LobbyTimer.isStarted)
            {
                StartMatch();
            }
        }

        if (isStarted) //стрельба
        {
            if (Input.GetKey(KeyCode.Mouse0)) 
            {
                power = Mathf.Clamp01(power + Time.deltaTime * (1-power));
                powerSlider.value = power;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0)) 
            {
                CmdShot(power);
                power = 0;
                powerSlider.value = 0;
            }
        }

        //€ не стал делать false, чтобы код был чуть более €вно разделЄн на блоки.


        if (camAnim) //плавное перемещение камеры в точку над пушкой
        {
            cam.transform.position = Vector3.Slerp(cam.transform.position, camPoint, Time.deltaTime * 2);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(15, startY - 90, 0), Time.deltaTime * 2);
            if ((cam.transform.position - camPoint).sqrMagnitude < .25f) { camAnim = false; }
        }

    }
}
