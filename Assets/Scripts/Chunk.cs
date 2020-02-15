using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public bool isEmpty = false;
    public static readonly int chunkSize = 16;
    private static readonly int tResolution = 16;
    public MapManager mapManager { private get; set; }
    public bool doUpdate { private get; set; }
    public int chunkX { get; private set; }
    public int chunkY { get; private set; }
    public int chunkZ { get; private set; }

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
    public Mesh mesh { get; private set; }
    private MeshCollider col;

    private bool[,,] blocks;
    private int faceCount;
    private float tUnit;

    private void Start() {
        Init();
    }

    private void Update() {
        if (doUpdate) {
            GenerateMesh();
            UpdateMesh();
            doUpdate = false;
        }
    }

    private void Init() {
        chunkX = (int)transform.position.x;
        chunkY = (int)transform.position.y;
        chunkZ = (int)transform.position.z;
        tUnit = 1f / tResolution;
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();

        blocks = new bool[chunkSize, chunkSize, chunkSize];
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {
                for (int y = 0; y < chunkSize; y++) {
                    blocks[x, y, z] = true;
                }
            }
        }
    }

    public bool Block(int x, int y, int z) {
        if (x >= chunkSize || x < 0 || y >= chunkSize || y < 0 || z >= chunkSize || z < 0) {
            int xPos = Mathf.FloorToInt(chunkX / chunkSize);
            int yPos = Mathf.FloorToInt(chunkY / chunkSize);
            int zPos = Mathf.FloorToInt(chunkZ / chunkSize);

            if (x >= chunkSize) {
                xPos += 1;
                x = 0;
            } else if (x < 0) {
                xPos -= 1;
                x = chunkSize - 1;
            }

            if (y >= chunkSize) {
                yPos += 1;
                y = 0;
            } else if (y < 0) {
                yPos -= 1;
                y = chunkSize - 1;
            }

            if (z >= chunkSize) {
                zPos += 1;
                z = 0;
            } else if (z < 0) {
                zPos -= 1;
                z = chunkSize - 1;
            }

            if (xPos >= 0 && xPos < MapManager.mapWidth && yPos >= 0 && yPos < MapManager.mapHeight && zPos >= 0 && zPos < MapManager.mapWidth)
                return mapManager.chunks[xPos, yPos, zPos].Block(x, y, z);
            else return true;
        }
        return blocks[x, y, z];
    }

    public void SetBlock(int x, int y, int z, bool b) {
        if (x < chunkSize && x >= 0 && y < chunkSize && y >= 0 && z < chunkSize && z >= 0) {
            blocks[x, y, z] = b;
        }
    }

    private void CubeTop(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        Cube(block);
    }

    private void CubeNorth(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Cube(block);
    }

    private void CubeEast(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        Cube(block);
    }

    private void CubeSouth(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        Cube(block);
    }

    private void CubeWest(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        Cube(block);
    }

    private void CubeBot(int x, int y, int z, Vector2 block) {
        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        Cube(block);
    }

    private void Cube(Vector2 texturePos) {

        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(1, 1));
        newUV.Add(new Vector2(1, 1));
        newUV.Add(new Vector2(1, 1));
        newUV.Add(new Vector2(1, 1));

        faceCount++;
    }

    public void GenerateMesh() {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    bool isBlock = Block(x, y, z);
                    //If the block is solid
                    if (isBlock) {
                        //Block above is air
                        if (!Block(x, y + 1, z)) CubeTop(x, y, z, Vector2.zero);

                        //Block below is air
                        if (!Block(x, y - 1, z)) CubeBot(x, y, z, Vector2.zero);

                        //Block east is air
                        if (!Block(x + 1, y, z)) CubeEast(x, y, z, Vector2.zero);

                        //Block west is air
                        if (!Block(x - 1, y, z)) CubeWest(x, y, z, Vector2.zero);

                        //Block north is air
                        if (!Block(x, y, z + 1)) CubeNorth(x, y, z, Vector2.zero);

                        //Block south is air
                        if (!Block(x, y, z - 1)) CubeSouth(x, y, z, Vector2.zero);
                    }
                }
            }
        }
    }

    public void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();

        col.sharedMesh = mesh;

        //mesh.UploadMeshData(true);

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }
}
