﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInput : MonoBehaviour {
    public TileGrid Grid;

    public LayerMask TileMask;
    public LayerMask BackgroundMask;

    public float SlideDuration = 0.5f;

    private bool lockedInput;
    private float elapsedTime;

    void Update() {
        if (Input.GetMouseButtonDown(0) && !lockedInput) {
            // Lock input untill animations done
            lockedInput = true;
            elapsedTime = 0;

            // Get click position
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos2D = new Vector2(clickPos.x, clickPos.y);

            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            // Make sure player taps outside the grid
            if (hit.collider != null && BackgroundMask == (BackgroundMask | (1 << hit.transform.gameObject.layer))) {
                clickPos = hit.point;
            }
            else {
                return;
            }

            // Get closest tile to click
            float closestDistance = 100f;
            Tile closestTile = Grid.tiles[0, 0];
            foreach (Tile tile in Grid.tiles) {
                float distance = Vector2.Distance(tile.transform.position, clickPos);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestTile = tile;
                }
            }

            // Calculate in which direction tiles should slide
            Vector3 directionCalcVector = closestTile.transform.position - clickPos;
            Vector3 slideDirection = CalculateSlideDirection(directionCalcVector);

            // Grab tiles to slide
            float raycastDistance = 0f;
            if (slideDirection == Vector3.up || slideDirection == Vector3.down) {
                raycastDistance = Grid.dimensions.y * Grid.spacing;
            }
            else {
                raycastDistance = Grid.dimensions.x * Grid.spacing;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(
                closestTile.transform.position + -slideDirection * raycastDistance, slideDirection, raycastDistance * 2,
                TileMask);

            // Move last tile to front
            Vector3 loopPos = closestTile.transform.position;
            hits[hits.Length - 1].transform.position += new Vector3(0, 0, -5);
            hits[hits.Length - 1].transform.GetComponent<Tile>().Slide(loopPos,
                SlideDuration);

            // Move rest of tiles one space
            for (int i = 0; i < hits.Length - 1; i++) {
                Tile tile = hits[i].transform.GetComponent<Tile>();
                tile.Slide(tile.transform.position + slideDirection * Grid.spacing, SlideDuration);
            }
        }

        // Count for input lock
        elapsedTime += Time.deltaTime;

        // Unlock input when the sliding finishes
        if (lockedInput && elapsedTime > SlideDuration) {
            lockedInput = false;
            Debug.Log("unlocked");
        }
    }

    /// <summary>
    /// Turns a direction vector into a 90 degree direction vector, returns the direction *opposite* from it, the direction the tiles will slide in.
    /// </summary>
    /// <param name="directionCalcVector">Any direction</param>
    /// <returns>Direction the tiles should slide in</returns>
    private Vector2 CalculateSlideDirection(Vector3 directionCalcVector) {
        if (directionCalcVector.x > -0.4 && directionCalcVector.x < 0.4 &&
            directionCalcVector.y < 0.0) {
            Debug.Log("up");
            return Vector2.down;
        }

        if (directionCalcVector.x < 0.0 &&
            directionCalcVector.y > -0.4 && directionCalcVector.y < 0.4) {
            Debug.Log("right");
            return Vector2.left;
        }

        if (directionCalcVector.x > -0.4 && directionCalcVector.x < 0.4 &&
            directionCalcVector.y > 0.0) {
            Debug.Log("down");
            return Vector2.up;
        }

        if (directionCalcVector.x > 0.0 &&
            directionCalcVector.y > -0.4 && directionCalcVector.y < 0.4) {
            Debug.Log("left");
            return Vector2.right;
        }

        return Vector2.zero;
    }
}