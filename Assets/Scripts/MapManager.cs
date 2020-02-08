using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public static readonly int mapWidth = 20;
    public static readonly int mapHeight = 7;

    public static float scale = 10;
    public static float frequency = 1;
    public static float amplitude = 2;
    public static int seed = 0;
    public static int octaves = 2;
    public GameObject chunkObj;
    public Chunk[,,] chunks { get; private set; }

    void Start() {
        chunks = new Chunk[mapWidth, mapHeight, mapWidth];
        for (int x = 0; x < mapWidth; x++) {
            for (int z = 0; z < mapWidth; z++) {
                for (int y = 0; y < mapHeight; y++) {
                    GameObject chunk = Instantiate(chunkObj, new Vector3(x * Chunk.chunkSize, y * Chunk.chunkSize, z * Chunk.chunkSize), Quaternion.identity);
                    chunks[x, y, z] = chunk.GetComponent<Chunk>();
                    chunk.transform.parent = transform;
                    chunk.transform.name = "Chunk: " + chunk.transform.position;
                    chunks[x, y, z].mapManager = GetComponent<MapManager>();
                    //chunks[x, y, z].isEmpty = x > (mapWidth-1) * 0.25f && x < (mapWidth-1) * 0.75f &&
                    //                          z > (mapWidth-1) * 0.25f && z < (mapWidth-1) * 0.75f;
                    chunks[x, y, z].isEmpty = false;
                }
            }
        }
    }

    private bool doUpdate = true;

    private void Update() {
        if (doUpdate) {
            GenerateWorld();
            for (int x = 0; x < mapWidth; x++) {
                for (int z = 0; z < mapWidth; z++) {
                    for (int y = 0; y < mapHeight; y++) {
                        chunks[x, y, z].doUpdate = true;
                    }
                }
            }
            doUpdate = false;
        }
    }

    private void GenerateWorld() {
        int LQXZ = (int)(0.25f * mapWidth * Chunk.chunkSize);
        int UQXZ = (int)(0.75f * mapWidth * Chunk.chunkSize);
        int MXZ = (int)(0.5f * mapWidth * Chunk.chunkSize);
        int MY = (int)(0.5f * mapHeight * Chunk.chunkSize);
        ClearSphere(LQXZ, MY, LQXZ, 20);
        ClearSphere(UQXZ, MY, UQXZ, 20);
        ClearSphere(UQXZ, MY, LQXZ, 20);
        ClearSphere(LQXZ, MY, UQXZ, 20);

        ClearCube(10, MY + 4, 10, 9);
        ClearCube(mapWidth * Chunk.chunkSize - 10, MY + 4, mapWidth * Chunk.chunkSize - 10, 9);
        ClearCube(mapWidth * Chunk.chunkSize - 10, MY + 4, 10, 9);
        ClearCube(10, MY + 4, mapWidth * Chunk.chunkSize - 10, 9);

        ClearCuboid(MXZ, MY, MXZ, 40, 40, (int)(mapHeight / 2 * Chunk.chunkSize * 0.9f));

        ClearPath(11, 11, LQXZ, LQXZ, MY, 5);
        ClearPath(LQXZ, UQXZ, 11, mapWidth * Chunk.chunkSize - 11, MY, 5);
        ClearPath(mapWidth * Chunk.chunkSize - 11, mapWidth * Chunk.chunkSize - 11, UQXZ, UQXZ, MY, 5);
        ClearPath(UQXZ, LQXZ, mapWidth * Chunk.chunkSize - 11, 11, MY, 5);
        ClearPath(LQXZ, LQXZ, MXZ, MXZ, MY, 5);
        ClearPath(MXZ, MXZ, LQXZ, UQXZ, MY, 5);
        ClearPath(UQXZ, UQXZ, MXZ, MXZ, MY, 5);
        ClearPath(MXZ, MXZ, UQXZ, LQXZ, MY, 5);

        PlaceCuboid(MXZ, (int)(mapHeight / 2 * Chunk.chunkSize * 0.25f) - 6, MXZ, 7, 7, (int)(0.4f * mapHeight * Chunk.chunkSize));
        PlaceCuboid(MXZ, (int)(mapHeight * Chunk.chunkSize * 0.5f - 1) - 6, MXZ, 40, 5, 1);
        PlaceCuboid(MXZ, (int)(mapHeight * Chunk.chunkSize * 0.5f - 1) - 6, MXZ, 5, 40, 1);

    }

    private Chunk GetChunkWorldSpace(int x, int y, int z) {
        int xPos = Mathf.FloorToInt(x / Chunk.chunkSize);
        int yPos = Mathf.FloorToInt(y / Chunk.chunkSize);
        int zPos = Mathf.FloorToInt(z / Chunk.chunkSize);
        if (xPos < mapWidth && xPos >= 0 && yPos >= 0 && yPos < mapHeight && zPos < mapWidth && zPos >= 0)
            return chunks[xPos, yPos, zPos];
        else return null;
    }

    private void ClearSphere(int x, int y, int z, int diameter) {
        int radius = diameter / 2;
        int radiusSquared = radius * radius;

        int CircleEquation(int xPos, int yPos, int zPos) { return (int)(Mathf.Pow(xPos - x, 2) + Mathf.Pow(yPos - y, 2) + Mathf.Pow(zPos - z, 2)); }

        for (int i = -radius; i <= radius; i++) {
            for (int j = -radius; j <= radius; j++) {
                for (int k = -radius; k <= radius; k++) {
                    if (CircleEquation(i + x, j + y, k + z) < radiusSquared) {
                        Chunk c = GetChunkWorldSpace(i + x, j + y, k + z);
                        if (c != null) c.SetBlock(Mathf.Abs(i + x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(k + z) % Chunk.chunkSize, false);
                    }
                }
            }
        }
    }

    private void ClearCube(int x, int y, int z, int length) {
        for (int i = -length; i <= length; i++) {
            for (int j = -length; j <= length; j++) {
                for (int k = -length; k <= length; k++) {
                    Chunk c = GetChunkWorldSpace(i + x, j + y, k + z);
                    if (c != null) c.SetBlock(Mathf.Abs(i + x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(k + z) % Chunk.chunkSize, false);
                }
            }
        }
    }

    private void ClearCuboid(int x, int y, int z, int length, int width, int height) {
        for (int i = -length; i <= length; i++) {
            for (int j = -height; j <= height; j++) {
                for (int k = -width; k <= width; k++) {
                    Chunk c = GetChunkWorldSpace(i + x, j + y, k + z);
                    if (c != null) c.SetBlock(Mathf.Abs(i + x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(k + z) % Chunk.chunkSize, false);
                }
            }
        }
    }

    private void PlaceCube(int x, int y, int z, int length) {
        for (int i = -length; i <= length; i++) {
            for (int j = -length; j <= length; j++) {
                for (int k = -length; k <= length; k++) {
                    Chunk c = GetChunkWorldSpace(i + x, j + y, k + z);
                    if (c != null) c.SetBlock(Mathf.Abs(i + x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(k + z) % Chunk.chunkSize, true);
                }
            }
        }
    }

    private void PlaceCuboid(int x, int y, int z, int length, int width, int height) {
        for (int i = -length; i <= length; i++) {
            for (int j = -height; j <= height; j++) {
                for (int k = -width; k <= width; k++) {
                    Chunk c = GetChunkWorldSpace(i + x, j + y, k + z);
                    if (c != null) c.SetBlock(Mathf.Abs(i + x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(k + z) % Chunk.chunkSize, true);
                }
            }
        }
    }

    private void ClearPath(int xs, int zs, int xf, int zf, int y, int length) {
        int xDiff = Mathf.Abs(xf - xs);
        int zDiff = Mathf.Abs(zf - zs);
        for (int i = 0; i < xDiff + length + 1; i++) {
            ClearPathSegment(xs < xf ? xs + i : xs - i, y, zs, length, false);
        }
        for (int i = 0; i < zDiff + length + 1; i++) {
            ClearPathSegment(xf, y, zs < zf ? zs + i : zs - i, length, true);
        }
    }

    private void ClearPathSegment(int x, int y, int z, int length, bool direction) {
        for (int i = -length; i <= length; i++) {
            for (int j = -length; j <= length; j++) {
                Chunk c = GetChunkWorldSpace(direction ? i + x : x, j + y, direction ? z : z + i);
                if (c != null) c.SetBlock(Mathf.Abs(direction ? i + x : x) % Chunk.chunkSize, Mathf.Abs(j + y) % Chunk.chunkSize, Mathf.Abs(direction ? z : z + i) % Chunk.chunkSize, false);
            }
        }
    }
}
