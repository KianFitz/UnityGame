using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour {
    [SerializeField] private float accuracy;
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private int crosshairWidth = 5;
    [SerializeField] private int crosshairHeight = 5;
    [SerializeField] private int crosshairMinSpacing = 50;
    [SerializeField] private int crosshairMaxSpacing = 100;
    [SerializeField] private int crosshairSpeed = 2;

    private float currentSpacing = 0;
    private PlayerController controller;
    private Vector2 restPos;
    private RectTransform crosshair;


    void Start() {
        GenerateCrosshairVertical((int)Direction.UP);
        GenerateCrosshairVertical((int)Direction.DOWN);
        GenerateCrosshairHorizontal((int)Direction.LEFT);
        GenerateCrosshairHorizontal((int)Direction.RIGHT);
        restPos = new Vector2(crosshairMinSpacing, crosshairMinSpacing);
        controller = transform.root.GetComponent<PlayerController>();
        crosshair = GetComponent<RectTransform>();
        crosshair.sizeDelta = new Vector2(restPos.x, restPos.y);
    }

    void Update() {

        // accuracy = Mathf.Max(Mathf.Abs(controller.moveDirection.x), Mathf.Abs(controller.gravityVector.y), Mathf.Abs(controller.moveDirection.z));

        //crosshair.sizeDelta = new Vector2(restPos.x + accuracy * 10, restPos.y + accuracy * 10);
    }

    private void GenerateCrosshairHorizontal(int position) {
        transform.GetChild(position).GetComponent<RectTransform>().sizeDelta = new Vector2(crosshairHeight, crosshairWidth);
        transform.GetChild(position).GetComponent<Image>().color = crosshairColor;
    }

    private void GenerateCrosshairVertical(int position) {
        transform.GetChild(position).GetComponent<RectTransform>().sizeDelta = new Vector2(crosshairWidth, crosshairHeight);
        transform.GetChild(position).GetComponent<Image>().color = crosshairColor;
    }

    private enum Direction {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }
}
