using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour, ISerializableMonoBehaviour
{
    public Player instance { get { return this; }  }
    public int Id { get; set; } = -1;
    public ActorType m_ActorType { get; } = ActorType.Player;
}

public interface ISerializableMonoBehaviour
{
    public int Id { set; get; }
    public Player instance { get; }
    public ActorType m_ActorType { get; }
}



public class XX
{
    Player pla;
    
    public ISerializableMonoBehaviour ssmono;
    public void aa()
    {
        ssmono = pla as ISerializableMonoBehaviour;
    }
}

public enum ActorType
{
    Player,
    Computer
}
public enum PlaneStyle
{
    Strike,
    Balance,
    Accuracy,
    Casual

}

public enum MoveableDimension
{
    Point,
    Line,
    Face
}
