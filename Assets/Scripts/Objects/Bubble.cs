using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    const int EXPONENT_BASE = 2;
    const int RAND_RANGE = 2;
    public const int MAX_EXPONENT = 11;

    public ParticleSystem Particles;
    public ParticleSystem ExplosionParticles;
    public Transform Visual;
    public BubbleRow Row;
    public List<Bubble> Neighbours;
    public bool Validated;
    public bool CeilingBubble;

    [SerializeField] bool disableColliderLogic;
    [SerializeField] Collider2D collider;

    int bubbleValue;
    [SerializeField] int exponent;
    [SerializeField] bool isActive;
    [SerializeField] bool isAimTarget;
    [SerializeField] bool isConnected;

    public int PointValue { get { return (int)Mathf.Pow(EXPONENT_BASE, Exponent); } }

    public int Exponent
    {
        get { return exponent; }
        set
        {
            exponent = Mathf.Clamp(value, 0, MAX_EXPONENT);
            IsActive = exponent != 0;
            IsConnected = exponent != 0;
            ExponentChanged?.Invoke(this);
        }
    }


    public bool IsActive
    {
        get { return isActive;}
        private set
        {
            isActive = value;
            IsActiveChanged?.Invoke(this);
            if (isActive)
                Visual.localPosition = Vector3.zero;

            if(!disableColliderLogic)
                collider.enabled = isActive;
        }
    }

    public bool IsAimTarget
    {
        get { return isAimTarget;}
        set
        {
            isAimTarget = value;
            IsAimTargetChanged?.Invoke(this);
        }
    }

    public bool IsConnected
    {
        get { return isConnected; }
        set
        {
            Validated = true;
            isConnected = value;
            if (!isConnected && IsActive)
            {
                collider.enabled = false;
                DetacheBubble();
            }
        }
    }

    public event Action<Bubble> ExponentChanged;
    public event Action<Bubble> IsActiveChanged;
    public event Action<Bubble> IsAimTargetChanged;

    public IVisualize<(Bubble, Action)> fallingVisual;

    public void BaseConnectionCheck()
    {
        List<Bubble> visited = new();
        List<Bubble> queue = new()
        {
            this
        };
        List<Bubble> next = new();

        while (queue.Count > 0)
        {
            next.Clear();
            for (int i = 0; i < queue.Count; i++)
            {

                if (queue[i].Row.Index == 0)
                {
                    IsConnected = true;
                    return;
                }

                visited.Add(queue[i]);
                AddNeighbours(queue[i]);
            }

            queue.Clear();
            queue.AddRange(next);
        }

        IsConnected = false;

        void AddNeighbours(Bubble bubble)
        {
            for (int i = 0; i < bubble.Neighbours.Count; i++)
            {
                Bubble n = bubble.Neighbours[i];
                if (visited.Contains(n) || queue.Contains(n) || next.Contains(n) || !n.IsActive)
                    continue;

                next.Add(n);
            }
        }

    }

    public void ActiveColliderCheck(bool ignoreInactive = true)
    {
        bool hittable = false;
        foreach (Bubble bubble in Neighbours)
        {
            if (!IsActive && ignoreInactive)
                break;
            else if(!bubble.IsActive)
            {
                hittable = true;
                break;
            }
        }

        if(!disableColliderLogic)
            collider.enabled = hittable;

    }

    public void SetRandomExponent()
    {
        int max = BubbleManager.acc.GetFieldAverage() + UnityEngine.Random.Range(0, RAND_RANGE);
        max = Mathf.Clamp(max, 1, MAX_EXPONENT - 1);
        Exponent = UnityEngine.Random.Range(1, max + 1);
    }

    public void DetacheBubble()
    {
        Bubble target = this;

        StartCoroutine(fallingVisual.Visualize((this, () =>
        {
            GameData.Score += PointValue;
            Exponent = 0;
        }
        )));

    }

    private void Awake()
    {
        isConnected = true;
    }
}
