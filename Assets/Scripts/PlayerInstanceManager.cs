using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[DefaultExecutionOrder(-1)]
public class PlayerInstanceManager : MonoBehaviourSingletonPersistent<PlayerInstanceManager>
{
    /*
    public delegate void PlaneStyleChange(Player player, PlaneStyle nextStyle);
    public event PlaneStyleChange OnPlaneStyleChange;
    */
    public event System.Action<GameObject> OnInstantiatePlayer;
    public EnclosureArea2D p1MoveableArea;
    public EnclosureArea2D p2MoveableArea; // 2p dont know how to control...


    public PlayerControl strikeInstance;
    public PlayerControl balanceInstance;
    public PlayerControl accuracyInstance;
    public PlayerControl casualInstance;

    private Vector3 position;
    GameObject currentPlayerGameObject = null;
    GameObject newPlayerGameObject = null;
    public override void Awake()
    {
        base.Awake();
       

    }
    private void Start()
    {
        currentPlayerGameObject = NewPlayer(PlaneStyle.Strike);
    }

    public GameObject NewPlayer(PlaneStyle planeStyle, bool active = true)
    {
        newPlayerGameObject = Generate(planeStyle);
        newPlayerGameObject.SetActive(active);
        currentPlayerGameObject = newPlayerGameObject;
        OnInstantiatePlayer?.Invoke(currentPlayerGameObject);
        return currentPlayerGameObject;
    }
    public GameObject NewPlayerAtLastTransform(PlaneStyle planeStyle, bool active = true)
    {
        newPlayerGameObject = Generate(planeStyle);
        CopyTransform();
        currentPlayerGameObject.SetActive(false);
        Destroy(currentPlayerGameObject, Time.deltaTime); // destroy at next frame.
        newPlayerGameObject.SetActive(active);
        currentPlayerGameObject = newPlayerGameObject;
        OnInstantiatePlayer?.Invoke(currentPlayerGameObject);
        return currentPlayerGameObject;
    }
    private GameObject Generate(PlaneStyle planeStyle)
    {
        GameObject newPlayerGo = null;
        switch (planeStyle)
        {
            case PlaneStyle.Strike:
                newPlayerGo = Instantiate(strikeInstance).gameObject;
                break;
            case PlaneStyle.Balance:
                newPlayerGo = Instantiate(balanceInstance).gameObject;
                break;
            case PlaneStyle.Accuracy:
                newPlayerGo = Instantiate(accuracyInstance).gameObject;
                break;
            case PlaneStyle.Casual:
                newPlayerGo = Instantiate(casualInstance).gameObject;
                break;
            default:
                Debug.Log("In PlayerInstanceManager Generate dont has this type" + planeStyle);
                break;
        }
        return newPlayerGo;
    }
    private void CopyTransform()
    {
        Vector3 pos = currentPlayerGameObject.transform.position;
        Vector3 scale = currentPlayerGameObject.transform.localScale;
        Quaternion quater = currentPlayerGameObject.transform.rotation;
        newPlayerGameObject.transform.position = pos;
        newPlayerGameObject.transform.localScale = scale;
        newPlayerGameObject.transform.rotation = quater;

    }



}
