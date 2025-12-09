using System;
using System.Collections.Generic;

namespace Almost_Shortest_Path
{
    internal class Program
    {
        private const int INF = int.MaxValue;
        class Edge
        {
            /// <summary>
            /// Él: Honnan
            /// </summary>
            public int U;
            /// <summary>
            /// Él: Hova
            /// </summary>
            public int V;
            /// <summary>
            /// Él: Költség
            /// </summary>
            public int P;
            /// <summary>
            /// Új él a gráfban
            /// </summary>
            /// <param name="from">Honnan indul</param>
            /// <param name="to">Hova megy</param>
            /// <param name="weight">Mennyi a költsége</param>
            public Edge(int from, int to, int weight)
            {
                U = from;
                V = to;
                P = weight;
            }
        }
        static void Main(string[] args)
        {
            string str = String.Empty, output = String.Empty;
            int N, M, S, D;
            Console.WriteLine("Input:");
            while(true)
            {
                #region INPUT
                str = Console.ReadLine();
                // N: pontok darabszáma
                N = int.Parse(str.Split(' ')[0]);
                // M: egyirányú élek darabszáma melyek összekötnek két pontot
                M = int.Parse(str.Split(' ')[1]);
                if ((N == 0) && (M == 0)) break;
                if((N < 2) || (N > 500) || (M < 1) || (M > Math.Pow(10d, 4d))) throw new Exception("Input out of range");
                str = Console.ReadLine();
                // S: kezdőpont
                S = int.Parse(str.Split(' ')[0]);
                // D: célpont
                D = int.Parse(str.Split(' ')[1]);
                if((S < 0) || (N <= D)) throw new Exception("Input out of range");
                List<Edge> edges = new List<Edge>();
                for (int j = 0; j < M; j++)
                {
                    str = Console.ReadLine();
                    int u = int.Parse(str.Split(' ')[0]);
                    int v = int.Parse(str.Split(' ')[1]);
                    int p = int.Parse(str.Split(' ')[2]);
                    if ((u == v) || (u < 0) || (N <= v) || (p < 1) || (p > Math.Pow(10d, 3d))) throw new Exception("Input out of range");
                    edges.Add(new Edge(u, v, p));
                }
                #endregion
                int result = AlmostShortestPath(N, M, S, D, edges);
                output += result.ToString() + "\n";
            }
            Console.WriteLine("Output:\n" + output);
        }
        /// <summary>
        /// Dijkstra kicsit átalakított algoritmusa.<br></br>
        /// Eltárolja az összes lehetséges szülőt is, hogy később vissza tudjuk követni a legrövidebb utat<br></br>
        /// Használ egy tiltólistát is, mely megmondja, hogy mely éleket ne vegye figyelembe.<br></br>
        /// Ezzel meg tudjuk határozni a második legrövidebb uta(ka)t is.
        /// </summary>
        /// <param name="graph">A gráf mellyel dolgozunk</param>
        /// <param name="removed">A tiltólistás élek, avagy a leggyorsabb útvonal élei</param>
        /// <param name="N">Pontok száma</param>
        /// <param name="S">Két pontot összekötő egyirányú élek száma</param>
        /// <param name="distances">A távolságokat tartalmazó tömb</param>
        /// <param name="parent"></param>
        private static void Dijkstra(List<(int v, int w)>[] graph, bool[,] removed, int N, int S, int[] distances, List<int>[] parent)
        {
            //jelzés, hogy feldolgoztuk-e az adott pontot
            bool[] visited = new bool[N];
            // minden távolságot végtelenre állítunk
            for (int i = 0; i < N; i++) distances[i] = INF;
            distances[S] = 0;
            for (int iter = 0; iter < N; iter++)
            {
                int u = -1;
                int best = INF;
                //Amúgy min-heapet használtam volna de nincs implementálva C#-ban alapból.
                //Kézzel megkeressük a legkisebb távolságú csúcsot, amit még nem dolgoztunk fel.
                for (int i = 0; i < N; i++)
                {
                    if (!visited[i] && distances[i] < best)
                    {
                        best = distances[i];
                        u = i;
                    }
                }
                //Ha nincs több elérhető csúcs akkor kilépünk
                if (u == -1) break;
                visited[u] = true;
                foreach (var edge in graph[u])
                {
                    int v = edge.v;
                    int w = edge.w;
                    if (removed != null && removed[u, v]) continue; // tiltott él
                    int nd = distances[u] + w;
                    if (nd < distances[v])
                    {
                        distances[v] = nd;
                        parent[v].Clear();
                        parent[v].Add(u);
                    }
                    else if (nd == distances[v])
                    {
                        parent[v].Add(u);
                    }
                }
            }
        }
        private static int AlmostShortestPath(int N, int M, int S, int D, List<Edge> edges)
        {
            // Megépítjük a gráfot
            List<(int, int)>[] graph = new List<(int v, int w)>[N];
            for (int i = 0; i < N; i++) graph[i] = new List<(int, int)>();
            foreach (var edge in edges) graph[edge.U].Add((edge.V, edge.P));
            int[] distances = new int[N];
            List<int>[] parent = new List<int>[N];
            for (int i = 0; i < N; i++) parent[i] = new List<int>();

            // Futtatjuk Dijkstra algoritmusát, hogy megtaláljuk a legrövidebb utakat.
            Dijkstra(graph, null, N, S, distances, parent);
            // Ha nincs út S-ből D-be, akkor -1-et adunk vissza.
            if (distances[D] == INF) return -1;

            // Tiltólistára rakjuk a legrövidebb utak éleit
            bool[,] removed = new bool[N, N];
            Queue<int> q = new Queue<int>();
            q.Enqueue(D);
            while (q.Count > 0)
            {
                int cur = q.Dequeue();
                foreach (int p in parent[cur])
                {
                    if (!removed[p, cur])
                    {
                        removed[p, cur] = true;
                        q.Enqueue(p);
                    }
                }
            }
            
            // Második Dijkstra, a tiltólistával
            int[] dist2 = new int[N];
            List<int>[] dummyParents = new List<int>[N];
            for (int i = 0; i < N; i++) dummyParents[i] = new List<int>();
            Dijkstra(graph, removed, N, S, dist2, dummyParents);

            return dist2[D] == INF ? -1 : dist2[D];
        }
    }
}
