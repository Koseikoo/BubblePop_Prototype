using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

using UnityEngine;

public enum CollisionTags
{
    Wall,
    Bubble,
    Ceiling
}

public class BubbleShooter : MonoBehaviour
{
    const float RAY_LENGTH = 100;
    const int MAX_BOUNCES = 5;
    const float CANCEL_SHOT_THRESHOLD = 50;

    Vector2 currentTouchPosition;
    Vector2 startTouchPosition;

    Vector2 shootDirection;
    Bubble aimedBubble;
    Bubble hitBubble;

    [SerializeField] List<Vector2> RawShootingPath = new();
    [SerializeField] List<Vector2> PinnedShootingPath = new();

    IVisualize<Bubble> impactVisual;
    IVisualize<List<Vector2>> shootingVisual;
    IVisualize<(Action shooter, Action reserve)> reloadVisuals;
    IEvent<int> fieldImpactVisual;

    Vector2 ShootDirection
    {
        get { return shootDirection; }
        set
        {
            shootDirection = value;
            ShootDirectionChanged?.Invoke(RawShootingPath);
        }
    }

    Bubble AimedBubble
    {
        get { return aimedBubble; }
        set
        {
            if (aimedBubble != null)
                aimedBubble.IsAimTarget = false;

            aimedBubble = value;
            GameData.Aiming = aimedBubble != null;

            if (aimedBubble != null)
                aimedBubble.IsAimTarget = true;
        }
    }

    Bubble HitBubble
    {
        get { return hitBubble; }
        set
        {
            if (hitBubble != null)
                hitBubble.Visual.localScale = .9f * Vector3.one;
            hitBubble = value;

            if(hitBubble != null)
                hitBubble.Visual.localScale = Vector3.one;
        }
    }

    public event Action<List<Vector2>> ShootDirectionChanged;
    public event Action<Bubble> ImpactEvent;

    private void Awake()
    {
        shootingVisual = GetComponent<ShootingVisual>();
        reloadVisuals = GetComponent<ReloadVisuals>();
        impactVisual = GetComponent<DeepImpactVisual>();
        fieldImpactVisual = GetComponent<FieldImpactReaction>();
    }

    private void Start()
    {
        startTouchPosition = GameManager.acc.cam.WorldToScreenPoint(BubbleManager.acc.ShootBubble.transform.position);
    }

    public void ProcessShootingInput(Touch touch)
    {
        if (GameData.ActiveShot || GameData.ActiveRowPush)
            return;


        switch (touch.phase)
        {
            case TouchPhase.Began:
                GameData.Aiming = true;
                break;

            case TouchPhase.Moved or TouchPhase.Stationary:
                currentTouchPosition = touch.position;
                if (Vector2.Distance(startTouchPosition, currentTouchPosition) < CANCEL_SHOT_THRESHOLD)
                {
                    AimedBubble = null;
                    RawShootingPath.Clear();
                    PinnedShootingPath.Clear();
                    ShootDirection = default;
                    return;
                }

                ShootDirection = (currentTouchPosition - startTouchPosition).normalized;
                CalculateBubblePath(ShootDirection);
                break;

            case TouchPhase.Ended or TouchPhase.Canceled:
                StartCoroutine(ShootBubble());
                GameData.Aiming = false;
                HitBubble = null;
                break;
        }
    }

    void CalculateBubblePath(Vector2 inputDirection)
    {
        HitBubble = null;
        RawShootingPath.Clear();
        RawShootingPath.Add(BubbleManager.acc.ShootBubble.transform.position);
        RaycastHit2D hit = default;

        if (currentTouchPosition.y <= startTouchPosition.y)
            return;

        for (int i = 0; i < MAX_BOUNCES; i++)
        {
            hit = Physics2D.Raycast(RawShootingPath[^1] + (.01f * inputDirection), inputDirection, RAY_LENGTH);

            if (hit.collider == null)
                return;

            RawShootingPath.Add(hit.point);
            inputDirection = Vector2.Reflect(inputDirection, hit.normal);

            if (!hit.transform.CompareTag(CollisionTags.Wall.ToString()))
                break;
        }

        if (hit.transform.CompareTag(CollisionTags.Bubble.ToString()))
        {
            HitBubble = hit.transform.GetComponent<Bubble>();
            GetTargetFromBubble(hit.point, HitBubble);
        }
        else if (hit.transform.CompareTag(CollisionTags.Ceiling.ToString()))
        {
            GetTargetFromCeiling(hit.point);
        }
        else
        {
            AimedBubble = null;
        }
    }

    void GetTargetFromBubble(Vector3 hitPoint, Bubble hitBubble)
    {

        List<Bubble> validBubbles = new();
        foreach (Bubble neighbour in hitBubble.Neighbours)
        {
            Vector2 impactDir = (hitBubble.transform.position - hitPoint).normalized;
            Vector2 neighbourDir = (hitBubble.transform.position - neighbour.transform.position).normalized;
            float dot = Vector2.Dot(impactDir, neighbourDir);

            if (dot < 0 || neighbour.IsActive)
                continue;

            validBubbles.Add(neighbour);
        }

        Bubble closestBubble = null;
        foreach (Bubble bubble in validBubbles)
        {
            if(closestBubble == null)
                closestBubble = bubble;
            else if (Vector3.Distance(hitPoint, bubble.transform.position) <
                     Vector3.Distance(hitPoint, closestBubble.transform.position))
                closestBubble = bubble;
        }

        AimedBubble = closestBubble;
    }

    void GetTargetFromCeiling(Vector3 hitPoint)
    {
        //toCheck.AddRange(BubbleManager.acc.FieldRows[0].Bubbles);
        List<Bubble> possibleAimTargets = new();
        possibleAimTargets.AddRange(BubbleManager.acc.FieldRows[0].Bubbles);

        Bubble closestBubble = null;
        foreach (Bubble bubble in possibleAimTargets)
        {
            if (bubble.IsActive)
                continue;
            else if (closestBubble == null)
            {
                closestBubble = bubble;
                continue;
            }

            if(Vector3.Distance(hitPoint, bubble.transform.position) <
               Vector3.Distance(hitPoint, closestBubble.transform.position))
                closestBubble = bubble;
        }

        AimedBubble = closestBubble;
        
    }

    IEnumerator ShootBubble()
    {
        if (AimedBubble == null)
        {
            RawShootingPath.Clear();
            PinnedShootingPath.Clear();
            ShootDirection = default;
            yield break;
        }

        GameData.ActiveShot = true;

        PinnedShootingPath = new(RawShootingPath);
        PinnedShootingPath[^1] = AimedBubble.transform.position;
        RawShootingPath.Clear();
        ShootDirection = default;
        yield return StartCoroutine(shootingVisual.Visualize(PinnedShootingPath));
        PinnedShootingPath.Clear();


        Bubble bubble = AimedBubble;
        AimedBubble = null;

        bubble.IsAimTarget = false;
        bubble.Exponent = BubbleManager.acc.ShootBubble.Exponent;
        StartCoroutine(impactVisual.Visualize(bubble));
        fieldImpactVisual.TriggerEvent(bubble.Exponent);
        BubbleManager.acc.FieldStructureUpdate(false);

        yield return StartCoroutine(BubbleManager.acc.Merger.BubbleMerge(bubble));
        yield return StartCoroutine(reloadVisuals.Visualize((
            () => BubbleManager.acc.ShootBubble.Exponent = BubbleManager.acc.ReserveBubble.Exponent,
            () => BubbleManager.acc.ReserveBubble.SetRandomExponent()
            )));

        GameData.ActiveShot = false;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < RawShootingPath.Count - 1; i++)
        {
            Gizmos.DrawLine(RawShootingPath[i], RawShootingPath[i + 1]);
        }
    }
}
