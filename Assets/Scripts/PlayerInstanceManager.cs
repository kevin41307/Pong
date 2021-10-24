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
    public event System.Action<int, GameObject> OnInstantiatePlayer;
    public EnclosureArea2D p1MoveableArea;
    public EnclosureArea2D p2MoveableArea; // 2p dont know how to control...


    public PlayerControl strikeInstance;
    public PlayerControl balanceInstance;
    public PlayerControl accuracyInstance;
    public PlayerControl casualInstance;

    private Vector3 position;
    [HideInInspector]
    public GameObject currentPlayerGameObject = null;
    private GameObject newPlayerGameObject = null;

    private void Start()
    {
        currentPlayerGameObject = NewPlayer(PlaneStyle.Strike);
    }

    public GameObject NewPlayer(PlaneStyle planeStyle, bool active = true)
    {
        newPlayerGameObject = Generate(planeStyle);
        AvatarCard card = new AvatarCard(newPlayerGameObject);
        newPlayerGameObject.SetActive(active);
        currentPlayerGameObject = newPlayerGameObject;
        
        OnInstantiatePlayer?.Invoke( 1, currentPlayerGameObject);
        return currentPlayerGameObject;
    }
    public GameObject NewPlayerAtLastTransform(PlaneStyle planeStyle, bool active = true)
    {
        newPlayerGameObject = Generate(planeStyle);

        AvatarCard card = AvatarCard.FindSpecifiedCard(currentPlayerGameObject);
        if(card != null)
            card.avatar = newPlayerGameObject;

        CopyTransform();
        currentPlayerGameObject.SetActive(false);
        Destroy(currentPlayerGameObject, Time.deltaTime); // destroy at next frame.
        newPlayerGameObject.SetActive(active);
        currentPlayerGameObject = newPlayerGameObject;
        OnInstantiatePlayer?.Invoke(2, currentPlayerGameObject);
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
                Debug.Log("PlayerInstanceManager:Generate() dont has this type" + planeStyle);
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

public class AvatarCard
{
    public GameObject avatar;
    public string avatarName;
    public int avatarID;

    private string avatarDefaultName = "Player";
    private static int avatarIDCount = 1;
    private static List<AvatarCard> avatarCards = new List<AvatarCard>();
    public AvatarCard(GameObject _avatar, string _avatarName = "Player")
    {
        avatar = _avatar;
        avatarName = _avatarName;
        avatarID = avatarIDCount++;
        avatarCards.Add(this);
        WalkCards();
    }

    public static void WalkCards()
    {
        for (int i = 0; i < avatarCards.Count; i++)
        {
            Debug.Log(avatarCards[i].avatarName + " " + avatarCards[i].avatarID + " " + avatarCards[i].avatar);
        }
    }
    public static AvatarCard FindSpecifiedCard(GameObject target)
    {
        for (int i = 0; i < avatarCards.Count; i++)
        {
            if (avatarCards[i].avatar == target)
            {
                return avatarCards[i];
            }
        }
#if UNITY_EDITOR
        Debug.Log("Cannot find specified card with " + target + ".");
#endif
        return null;
    }

    public static AvatarCard FindSpecifiedCard(int avatarID)
    {
        for (int i = 0; i < avatarCards.Count; i++)
        {
            if(avatarCards[i].avatarID == avatarID)
            {
                return avatarCards[i];
            }
        }
#if UNITY_EDITOR
        Debug.Log("Cannot find specified card with " + avatarID + ".");
#endif
        return null;
    }
}
