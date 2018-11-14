using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Jumper : MonoBehaviour {

    public float jumpHeight = 8;
    public LayerMask playerMask;

    Collider2D ctrlCollider;
    ContactFilter2D filter = new ContactFilter2D();

    private void Start()
    {
        ctrlCollider = GetComponent<Collider2D>();
        filter.layerMask = playerMask;
    }

    private void Update()
    {
        Collider2D[] results = new Collider2D[5];
        ctrlCollider.OverlapCollider(filter, results);

        for (int i = 0; i < results.Length; i++) {

            if (results[i] == null)
                continue;

            Player player = results[i].GetComponent<Player>();

            if (player == null)
                continue;

            player.maxJumpHeight.AddModifier(Mathf.Abs(player.maxJumpHeight.GetBase() - jumpHeight));
            player.RecalculateMovementSettings();

            player.OnJumpInputDown();

            player.maxJumpHeight.RemoveModifier(Mathf.Abs(player.maxJumpHeight.GetBase() - jumpHeight));
            player.RecalculateMovementSettings();
        }
    }
}
