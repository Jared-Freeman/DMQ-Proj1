using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuloGames.UI;
using System.Linq;

public class FR_LGP_Generator : MonoBehaviour
{
    #region Members

    public static bool s_FLAG_DEBUG = false;

    //helper structs
    #region Structs
    [System.Serializable]
    public class Opt
    {
        public Vector2Int GenerationDimensions = new Vector2Int(7,7);
        /// <summary>
        /// Size of 2D grid spacing
        /// </summary>
        public float GridSize = 40f;
    }
    [System.Serializable]
    public struct CR
    {
        [Header("Set FLAG_UseResourcePaths to true to use Resources/ folders of prefabs")]
        public ConLists ConnectionLists;
        public RscFilePaths ResourcePaths;
    }
    [System.Serializable]
    public struct ConLists
    {
        //Set of all possible connection linkages
        //These are intended to be rotated / mirrored to fit any gen needed
        public List<FR_LGP_Connections> N_Connections;
        public List<FR_LGP_Connections> NS_Connections;
        public List<FR_LGP_Connections> NE_Connections;
        public List<FR_LGP_Connections> WNE_Connections;
        public List<FR_LGP_Connections> NESW_Connections;
    }
    [System.Serializable]
    public struct RscFilePaths
    {
        public string N_Connections;
        public string NS_Connections;
        public string NE_Connections;
        public string WNE_Connections;
        public string NESW_Connections;
    }
    #endregion

    //flags
    public bool FLAG_Debug = false;
    public bool FLAG_UseResourcePaths = false; //can specify resource paths instead of loading objects manually

    //instantiated helper structs
    public Opt Options;
    public CR C_Repo;

    //refs
    public GameObject Parent;
    public GameObject Instance;

    public GameObject PlayerSpawner;
    public GameObject ExitPrefab;

    //internal
    #region Helper Classes
    private class GridEntry
    {
        public bool[] NESW = new bool[4] { true, true, true, true };
        public bool [] Cs = new bool[5] { true, true, true, true, true };

        public GridEntry() { }

        public GridEntry(bool N, bool E = true, bool S = true, bool W = true)
        {
            int i = 0;
            NESW[i++] = N;
            NESW[i++] = E;
            NESW[i++] = S;
            NESW[i++] = W;
        }

        /// <summary>
        /// Returns true if any connections exist to this grid entry. 
        /// Existing connections imply that a room is instantiated on this grid entry.
        /// </summary>
        public bool AnyConnectionExists
        {
            get
            {
                foreach (bool b in NESW) { if (b == true) return true; }
                return false;
            }
        }
    }
    private class GridMask
    {
        //members
        public int NumRows, NumCols;
        public GridEntry[,] Grid; //2d array in c# is weird

        /// <summary>
        /// The list of all [i,j] values that have connections (and thus will contain instantiated rooms).
        /// </summary>
        public List<KeyValuePair<int, int>> PopulatedGridEntries { get; private set; }


        //ctor
        public GridMask(int cols, int rows)
        {
            NumRows = rows;
            NumCols = cols;
            Grid = new GridEntry[cols, rows];

            //instantiate GridEntry's
            int i = 0, j = 0;
            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    Grid[i, j] = new GridEntry();
                }
            }

            //foreach (GridEntry GE in Grid)
            //{
            //    Debug.Log(GE.NESW[0]);
            //}

            //cull adjacencies around borders
            CullBorders();
        }

        private void CullBorders()
        {
            int i = 0, j = 0;
            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    CullBorderAdjacency(i, j);
                }
            }
        }

        private void CullBorderAdjacency(int i, int j)
        {
            if(IndicesInBounds(i,j))
            {
                int N = 0, E = 1, S = 2, W = 3;

                if (i == 0)         Grid[i, j].NESW[W] = false;
                if (i == NumCols-1) Grid[i, j].NESW[E] = false;
                if (j == 0)         Grid[i, j].NESW[S] = false;
                if (j == NumRows-1) Grid[i, j].NESW[N] = false;
            }
        }

        private void InvertMask()
        {
            int i = 0, j = 0;
            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    for (int k = 0; k < Grid[i, j].NESW.Length; k++) Grid[i, j].NESW[k] = !Grid[i, j].NESW[k];
                }
            }

        }

        private bool IndicesInBounds(int i, int j)
        {
            if (i >= 0 && i < NumCols && j >= 0 && j < NumRows) return true;
            else return false;
        }

        // returns true if a connection was updated
        private bool UpdateConnections(int i, int j)
        {
            if (IndicesInBounds(i, j))
            {
                bool ConnectionUpdated = false;

                int N = 0, E = 1, S = 2, W = 3; 

                //up
                if (IndicesInBounds(i, j + 1))
                {
                    if(Grid[i,j].NESW[N] != Grid[i, j + 1].NESW[S])
                    {
                        ConnectionUpdated = true;
                        Grid[i, j + 1].NESW[S] = Grid[i, j].NESW[N];
                    }
                }

                //right
                if (IndicesInBounds(i + 1, j))
                {
                    if (Grid[i, j].NESW[E] != Grid[i + 1, j].NESW[W])
                    {
                        ConnectionUpdated = true;
                        Grid[i + 1, j].NESW[W] = Grid[i, j].NESW[E];
                    }
                }

                //down
                if (IndicesInBounds(i, j - 1))
                {
                    if (Grid[i, j].NESW[S] != Grid[i, j - 1].NESW[N])
                    {
                        ConnectionUpdated = true;
                        Grid[i, j - 1].NESW[N] = Grid[i, j].NESW[S];
                    }
                }

                //left
                if (IndicesInBounds(i - 1, j))
                {
                    if (Grid[i, j].NESW[W] != Grid[i - 1, j].NESW[E])
                    {
                        ConnectionUpdated = true;
                        Grid[i - 1, j].NESW[E] = Grid[i, j].NESW[W];
                    }
                }

                return ConnectionUpdated;
            }
            else return false;
        }

        //recursive
        public void PropagateAdjacency(int i, int j)
        {
            if(IndicesInBounds(i,j))
            {
                //update connections to match
                bool Updated = UpdateConnections(i, j);
                //call propagate on these too
                if (Updated)
                {
                    PropagateAdjacency(i - 1, j);
                    PropagateAdjacency(i + 1, j);
                    PropagateAdjacency(i, j - 1);
                    PropagateAdjacency(i, j + 1);
                }
            }
        }

        public void PlaceGridMask(int i, int j, GridEntry Entry)
        {
            if (IndicesInBounds(i, j))
            {
                Grid[i, j] = Entry;
                PropagateAdjacency(i, j);
            }
            //else Debug.Log("Level Generator: Grid Index OOB");

        }

        public void DebugPopulateGridSequential()
        {
            int i = 0, j = 0;
            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    GridEntry Insert_Entry = new GridEntry();

                    //random params
                    int cutoff = 0;
                    int min = -2;
                    int max = 2;

                    int N = 0, E = 1, S = 2, W = 3;

                    //randomly turn on connections
                    if (Grid[i, j].NESW[N])
                    {
                        if (Random.Range(min, max) > cutoff) Insert_Entry.NESW[N] = false;
                    } 
                    else Insert_Entry.NESW[N] = false;

                    if (Grid[i, j].NESW[S])
                    {
                        if (Random.Range(min, max) > cutoff) Insert_Entry.NESW[S] = false;
                    }
                    else Insert_Entry.NESW[S] = false;

                    if (Grid[i, j].NESW[E])
                    {
                        if (Random.Range(min, max) > cutoff) Insert_Entry.NESW[E] = false;
                    }
                    else Insert_Entry.NESW[E] = false;

                    if (Grid[i, j].NESW[W])
                    {
                        if (Random.Range(min, max) > cutoff) Insert_Entry.NESW[W] = false;
                    }
                    else Insert_Entry.NESW[W] = false;


                    //place object
                    PlaceGridMask(i, j, Insert_Entry);
                }
            }




            CircularizeMaskPerlin();




            var Islands = ConstructIslands();

            //just in case
            if (Islands.Count < 1) return;

            var SortedIslands = Islands.OrderBy(t => t.Count);

            var LargestIsland = SortedIslands.First();
            foreach (var I in SortedIslands)
            {
                if (I.Count > LargestIsland.Count) LargestIsland = I;
            }

            // Record for reference outside this class.
            PopulatedGridEntries = LargestIsland;

            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    if (!LargestIsland.Contains(new KeyValuePair<int, int>(i, j)))
                    {
                        for (int k = 0; k < Grid[i, j].NESW.Length; k++) Grid[i, j].NESW[k] = false;
                    }
                }
            }

            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    UpdateConnections(i, j);
                }
            }

        }

        private void CircularizeMaskPerlin()
        {
            int cx = NumCols / 2, cy = NumRows / 2;
            float radius = cx; //TODO: Consider elliptical radii


            float[,] Mask = new float[NumCols, NumRows];

            for (int i = 0; i < NumCols; i++)
            {
                for(int j=0; j<NumRows; j++)
                {
                    if (Mathf.Abs(i - cx) < radius && Mathf.Abs(j - cy) < radius && false) //not needed anymore
                    {
                        Mask[i, j] = 1;
                    }
                    else
                    {
                        //Mask[i, j] = Mathf.Clamp(1 / Mathf.Log(1 + Mathf.Pow(( i - cx ), 2) + Mathf.Pow(( j - cy ), 2)), 0 , 1);

                        //Gaussian

                        float A = 1; //defines peak
                        float SigmaX = 1 / radius, SigmaY = 1 / radius;
                        //float SigmaX = 1, SigmaY = 1;

                        //Mask[i, j] = A * Mathf.Exp(- ((Mathf.Pow((i - cx),2) / 2 * Mathf.Pow(SigmaX, 2)) + (Mathf.Pow((j - cy), 2) / 2 * Mathf.Pow(SigmaY, 2))) );

                        Mask[i, j] = A * Mathf.Exp(
                            -1 * 
                            (
                            (Mathf.Pow((i - cx), 2) / 2 * Mathf.Pow(SigmaX, 2))
                            + 
                            (Mathf.Pow((j - cy), 2) / 2 * Mathf.Pow(SigmaY, 2))
                            )
                            );
                        


                    }
                }
            }
            

            float Resolution = 1.01f, HeightAmplifier = .8f;

            for (int i = 0; i < NumCols; i++)
            {
                for (int j = 0; j < NumRows; j++)
                {
                    if (Mathf.Abs(i - cx) > radius && Mathf.Abs(j - cy) > radius || true) //not needed anymore
                    {
                        Mask[i, j] *= Mathf.Clamp(Mathf.PerlinNoise((float)i / Resolution, (float)j / Resolution), 0, 1) * HeightAmplifier;
                    }

                }
            }

            float min = Mathf.Infinity, max = Mathf.NegativeInfinity;
            for (int i = 0; i < NumCols; i++)
            {
                for (int j = 0; j < NumRows; j++)
                {
                    if (Mask[i, j] < min) min = Mask[i, j];
                    if (Mask[i, j] > max) max = Mask[i, j];
                }
            }

            if(s_FLAG_DEBUG) Debug.Log("MAX: " + max + ", MIN: " + min + ", RATIO: " + max / min);

            for (int i = 0; i < NumCols; i++)
            {
                for (int j = 0; j < NumRows; j++)
                {
                    Mask[i, j] = (((Mask[i, j] - min) * (1)) / (max-min)); //remap

                    float sz_spacing = 1f;
                    float sz = sz_spacing * .5f * .9f;
                    float scale = 3f;
                    Vector3 SpawnPosition = new Vector3(i * sz_spacing, 0, j * sz_spacing);

                    if (Mask[i, j] < .5f && (Mathf.Abs(i - cx) > radius/2 && Mathf.Abs(j - cy) > radius/2))
                    {
                        for (int k = 0; k < Grid[i, j].NESW.Length; k++) Grid[i, j].NESW[k] = false;

                        bool Draw = false;
                        if (Draw) Debug.DrawRay(SpawnPosition, Vector3.up * Mask[i, j] * scale, Color.blue, Mathf.Infinity);
                    }
                }
            }


            //float min = Mathf.Infinity, max = Mathf.NegativeInfinity;
            //for (int i = 0; i < NumCols; i++)
            //{
            //    for (int j = 0; j < NumRows; j++)
            //    {
            //        if (Mathf.Abs(i - cx) > radius && Mathf.Abs(j - cy) > radius)
            //        {
            //            if (Mask[i, j] < min) min = Mask[i, j];
            //            if (Mask[i, j] > max) max = Mask[i, j];
            //        }
            //    }
            //}


            //for (int i = 0; i < NumCols; i++)
            //{
            //    for (int j = 0; j < NumRows; j++)
            //    {
            //        Mask[i, j] = Freeman_Utilities.MapValueFromRangeToRange(Mask[i, j], min, max, 0, 1);

            //        Debug.Log(Mask[i, j]);


            //        float sz_spacing = 1f;
            //        float sz = sz_spacing * .5f * .9f;
            //        float scale = 3f;
            //        Vector3 SpawnPosition = new Vector3(i * sz_spacing, 0, j * sz_spacing);


            //        Debug.DrawRay(SpawnPosition, Vector3.up * Mask[i, j] * scale, Color.blue, Mathf.Infinity);
            //    }
            //}




        }

        private List<List<KeyValuePair<int, int>>> ConstructIslands()
        {
            var Islands = new List<List<KeyValuePair<int, int>>>();

            List<KeyValuePair<int, int>> RemainingLocations = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < NumCols; i++)
            {
                for (int j = 0; j < NumRows; j++)
                {
                    RemainingLocations.Add(new KeyValuePair<int, int>(i, j));
                }
            }

            int count = 0;
            while(RemainingLocations.Count > 0)
            {
                var NextIslandSet = ExploreIsland(RemainingLocations[0].Key, RemainingLocations[0].Value, ref RemainingLocations);
                foreach(KeyValuePair<int,int> IND in NextIslandSet)
                {
                    RemainingLocations.Remove(IND);
                }
                if(NextIslandSet.Count > 1)
                    count++;

                Islands.Add(NextIslandSet);
            }
            //Debug.Log("Island count: " + count);

            return Islands;
        }


        private List<KeyValuePair<int, int>> ExploreIsland(int i, int j, ref List<KeyValuePair<int, int>> RemainingLocations)
        {
            var IslandSet = new List<KeyValuePair<int, int>>();

            ContinueExploreIsland(i, j, ref IslandSet);

            return IslandSet;
        }

        //assumes NESW boolean constraints are already valid!!!
        private void ContinueExploreIsland(int i, int j, ref List<KeyValuePair<int, int>> ExploredSet)
        {
            //mark this index as explored
            ExploredSet.Add(new KeyValuePair<int, int>(i, j));

            int N = 0, E = 1, S = 2, W = 3;

            //if there exists a valid, unexplored space in any direction, explore there
            if (Grid[i, j].NESW[N] && !ExploredSet.Contains(new KeyValuePair<int, int>(i, j+1)))
            {
                ContinueExploreIsland(i, j + 1, ref ExploredSet);
            }
            if (Grid[i, j].NESW[S] && !ExploredSet.Contains(new KeyValuePair<int, int>(i, j-1)))
            {
                ContinueExploreIsland(i, j - 1, ref ExploredSet);
            }
            if (Grid[i, j].NESW[E] && !ExploredSet.Contains(new KeyValuePair<int, int>(i+1, j)))
            {
                ContinueExploreIsland(i + 1, j, ref ExploredSet);
            }
            if (Grid[i, j].NESW[W] && !ExploredSet.Contains(new KeyValuePair<int, int>(i-1, j)))
            {
                ContinueExploreIsland(i - 1, j, ref ExploredSet);
            }
        }


        public void DebugPopulateGridRandom()
        {
            int i = 0, j = 0;
            for (i = 0; i < NumCols; i++)
            {
                for (j = 0; j < NumRows; j++)
                {
                    //select random remaining option

                    //place object
                }
            }
        }

        public void DebugRenderGrid(GameObject Parent, GameObject InstanceTemplate, float Spacing = 2f)
        {
            float sz_spacing = Spacing;

            for (int i = 0; i < NumCols; i++)
            {
                for (int j = 0; j < NumRows; j++)
                {
                    float sz = sz_spacing * .9f;
                    float sz_scale = .1f;
                    Vector3 SpawnPosition = new Vector3(i * sz_spacing, 0, j * sz_spacing);

                    GameObject GO = Instantiate(InstanceTemplate, Parent.transform);
                    GO.name = "Debug Instance";
                    GO.transform.position = SpawnPosition;

                    //draw connection rays
                    int N = 0, E = 1, S = 2, W = 3;

                    if (Grid[i, j].NESW[N]) Debug.DrawRay(GO.transform.position, GO.transform.forward * sz, Color.green, Mathf.Infinity);
                    else Debug.DrawRay(GO.transform.position, GO.transform.forward * sz * sz_scale, Color.red, Mathf.Infinity);
                    
                    if (Grid[i, j].NESW[E]) Debug.DrawRay(GO.transform.position, GO.transform.right * sz, Color.green, Mathf.Infinity);
                    else Debug.DrawRay(GO.transform.position, GO.transform.right * sz * sz_scale, Color.red, Mathf.Infinity);

                    if (Grid[i, j].NESW[S]) Debug.DrawRay(GO.transform.position, -GO.transform.forward * sz, Color.green, Mathf.Infinity);
                    else Debug.DrawRay(GO.transform.position, -GO.transform.forward * sz * sz_scale, Color.red, Mathf.Infinity);

                    if (Grid[i, j].NESW[W]) Debug.DrawRay(GO.transform.position, -GO.transform.right * sz, Color.green, Mathf.Infinity);
                    else Debug.DrawRay(GO.transform.position, -GO.transform.right * sz * sz_scale, Color.red, Mathf.Infinity);
                }
            }

        }

    }
    #endregion

    private GridMask GMask;



    #endregion


    private void Awake()
    {
        if(FLAG_UseResourcePaths)
        {
            C_Repo.ConnectionLists.N_Connections = Resources.LoadAll<FR_LGP_Connections>(C_Repo.ResourcePaths.N_Connections).ToList();
            C_Repo.ConnectionLists.NE_Connections = Resources.LoadAll<FR_LGP_Connections>(C_Repo.ResourcePaths.NE_Connections).ToList();
            C_Repo.ConnectionLists.NS_Connections = Resources.LoadAll<FR_LGP_Connections>(C_Repo.ResourcePaths.NS_Connections).ToList();
            C_Repo.ConnectionLists.WNE_Connections = Resources.LoadAll<FR_LGP_Connections>(C_Repo.ResourcePaths.WNE_Connections).ToList();
            C_Repo.ConnectionLists.NESW_Connections = Resources.LoadAll<FR_LGP_Connections>(C_Repo.ResourcePaths.NESW_Connections).ToList();
        }

        if ( C_Repo.ConnectionLists.N_Connections.Count < 1
            || C_Repo.ConnectionLists.N_Connections.Count < 1
            || C_Repo.ConnectionLists.NS_Connections.Count < 1
            || C_Repo.ConnectionLists.NE_Connections.Count < 1
            || C_Repo.ConnectionLists.WNE_Connections.Count < 1
            || C_Repo.ConnectionLists.NESW_Connections.Count < 1)
        {
            Debug.LogError("Must have at least 1 Connection pattern specified per category!");
        }
    }

    private void Start()
    {
        DoGeneration();
    }

    void DoGeneration()
    {
        //Init board
        //Remove options from borders (handled at construction-time)
        GMask = new GridMask(Options.GenerationDimensions.x, Options.GenerationDimensions.y);

        {
            int i = 0;
            foreach (GridEntry GE in GMask.Grid)
            {
                i++;
            }
            //Debug.Log(i);
        }

        //GMask.PlaceGridMask(2, 2, new GridEntry(false, false, false, true));
        //GMask.PlaceGridMask(2, 3, new GridEntry(false, true, true, true));
        //GMask.PlaceGridMask(2, 4, new GridEntry(false, false, false, true));

        GMask.DebugPopulateGridSequential();

        //GMask.DebugRenderGrid(Parent, Instance);
        InstantiateConnections(Parent, Options.GridSize);

        //TODO: Make this more elegant
        InstantiateSpawnAndExit();

        //Loop: Preplace object, Link objects

        //
    }



    private void RotateUntilMatch(int i, int j, FR_LGP_Connections C)
    {
        int c = 0, max = 4;

        int N = 0, E = 1, S = 2, W = 3;

        for (; c < max; c++)
        {
            if (
                (C.Connections[N] == GMask.Grid[i, j].NESW[N])
                && (C.Connections[E] == GMask.Grid[i, j].NESW[E])
                && (C.Connections[S] == GMask.Grid[i, j].NESW[S])
                && (C.Connections[W] == GMask.Grid[i, j].NESW[W])
                )
            {
                break;
            }
            else
            {
                C.gameObject.transform.Rotate(new Vector3(0, -90, 0));

                //use dirs as circular array
                bool temp = C.Connections[0];
                for (int k = 0; k < max; k++)
                {
                    C.Connections[k] = C.Connections[(k + 1) % max];
                }
                C.Connections[max - 1] = temp;
            }
        }

    }

    public void InstantiateConnections(GameObject Parent, float Spacing = 2f)
    {
        float sz_spacing = Spacing;
        float sz = sz_spacing * .9f;
        //float sz_scale = .1f;

        int N = 0, E = 1, S = 2, W = 3;

        for (int i = 0; i < GMask.NumCols; i++)
        {
            for (int j = 0; j < GMask.NumRows; j++)
            {
                Vector3 SpawnPosition = new Vector3(i * sz_spacing, 0, j * sz_spacing);
                //get connections from Grid
                bool[] C = new bool[4];
                for (int k = 0; k < GMask.Grid[i, j].NESW.Length; k++) C[k] = GMask.Grid[i, j].NESW[k];

                GameObject GO;
                //Construct bit mask
                byte Bt = 0b00000000;
                byte BN = 0b00000001, BE = 0b00000010, BS = 0b00000100, BW = 0b00001000;
                if (C[N]) Bt |= BN;
                if (C[E]) Bt |= BE;
                if (C[S]) Bt |= BS;
                if (C[W]) Bt |= BW;

                //Type 1 (N)
                if (Bt == BN || Bt == BE || Bt == BS || Bt == BW)
                {
                    int k = Random.Range(0, C_Repo.ConnectionLists.N_Connections.Count);
                    GO = Instantiate(C_Repo.ConnectionLists.N_Connections[k].gameObject);
                }

                //Type 2 (NS)
                else if (Bt == (BN | BS) || Bt == (BE | BW))
                {
                    int k = Random.Range(0, C_Repo.ConnectionLists.NS_Connections.Count);
                    GO = Instantiate(C_Repo.ConnectionLists.NS_Connections[k].gameObject);
                }

                //Type 3 (NE)
                else if (Bt == (BN | BE) || Bt == (BE | BS) || Bt == (BS | BW) || Bt == (BW | BN))
                {
                    int k = Random.Range(0, C_Repo.ConnectionLists.NE_Connections.Count);
                    GO = Instantiate(C_Repo.ConnectionLists.NE_Connections[k].gameObject);
                }

                //Type 4 (WNE)
                else if (Bt == (BN | BE | BS) || Bt == (BE | BS | BW) || Bt == (BS | BW | BN) || Bt == (BW | BN | BE))
                {
                    int k = Random.Range(0, C_Repo.ConnectionLists.WNE_Connections.Count);
                    GO = Instantiate(C_Repo.ConnectionLists.WNE_Connections[k].gameObject);
                }

                //Type 5 (NESW)
                else if (Bt == (BN | BE | BS | BW))
                {
                    int k = Random.Range(0, C_Repo.ConnectionLists.NESW_Connections.Count);
                    GO = Instantiate(C_Repo.ConnectionLists.NESW_Connections[k].gameObject);
                }
                else
                {
                    GO = null;
                }
                
                if(GO != null)
                {
                    RotateUntilMatch(i, j, GO.GetComponent<FR_LGP_Connections>());

                    GO.transform.parent = Parent.transform;
                    GO.name = "Room Instance";
                    GO.transform.position = SpawnPosition;

                    ////draw connection rays

                    //if (GMask.Grid[i, j].NESW[N]) Debug.DrawRay(GO.transform.position, GO.transform.forward * sz, Color.green, Mathf.Infinity);
                    //else Debug.DrawRay(GO.transform.position, GO.transform.forward * sz * sz_scale, Color.red, Mathf.Infinity);

                    //if (GMask.Grid[i, j].NESW[E]) Debug.DrawRay(GO.transform.position, GO.transform.right * sz, Color.green, Mathf.Infinity);
                    //else Debug.DrawRay(GO.transform.position, GO.transform.right * sz * sz_scale, Color.red, Mathf.Infinity);

                    //if (GMask.Grid[i, j].NESW[S]) Debug.DrawRay(GO.transform.position, -GO.transform.forward * sz, Color.green, Mathf.Infinity);
                    //else Debug.DrawRay(GO.transform.position, -GO.transform.forward * sz * sz_scale, Color.red, Mathf.Infinity);

                    //if (GMask.Grid[i, j].NESW[W]) Debug.DrawRay(GO.transform.position, -GO.transform.right * sz, Color.green, Mathf.Infinity);
                    //else Debug.DrawRay(GO.transform.position, -GO.transform.right * sz * sz_scale, Color.red, Mathf.Infinity);
                }

            }
        }

    }

    public void InstantiateSpawnAndExit()
    {

        var remainingEntries = GMask.PopulatedGridEntries;

        {
            //this is an INDEX to the List of key-value pairs.
            int index = Random.Range(0, remainingEntries.Count);

            int i, j;
            i = remainingEntries[index].Key;
            j = remainingEntries[index].Value;

            Vector3 SpawnPosition = new Vector3(i * Options.GridSize, 0, j * Options.GridSize);
            
            var GO = Instantiate(PlayerSpawner);
            if (GO != null)
                GO.transform.position = SpawnPosition;
            
            //This grid space can no longer be used
            remainingEntries.Remove(remainingEntries[index]);
        }


        {
            //this is an INDEX to the List of key-value pairs.
            int index = Random.Range(0, GMask.PopulatedGridEntries.Count);

            int i, j;
            i = GMask.PopulatedGridEntries[index].Key;
            j = GMask.PopulatedGridEntries[index].Value;

            Vector3 SpawnPosition = new Vector3(i * Options.GridSize, 0, j * Options.GridSize);

            var GO = Instantiate(ExitPrefab);
            if(GO != null)
                GO.transform.position = SpawnPosition;
        }

    }
}
