using System;

namespace Bad_XOR
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] A, B;
            int N, M;
            string output = "";
            Console.WriteLine("Input:");
            //Tesztesetek száma
            int testCases = Convert.ToInt32(Console.ReadLine());
            if(testCases < 1 || testCases > 20) throw new Exception("Test cases out of bounds (1 ≤ T ≤ 20)");
            for (int i = 0; i < testCases; i++)
            {
                //DP tömb a dinamikus programozáshoz
                //DP[i] azt tárolja, hogy hány DARAB részhalmaz van, aminek az XOR értéke i
                //Mivel minden érték 0-val lesz inicalizálva, elég lesz majd mindig ++-al növelni XOR értékű indexen.
                long[] DP = new long[1025];
                //DP[0] = 1 azért, mert az üres részhalmaz XOR értéke 0
                DP[0] = 1;
                //Minden teszteset első sora két egész számot tartalmaz: N és M (0 ≤ N, M ≤ 1000).
                string str = Console.ReadLine();
                N = Convert.ToInt32(str.Split(' ')[0]);
                M = Convert.ToInt32(str.Split(' ')[1]);
                //A következő sor az A tömb N darab elemét adja meg (0 ≤ Ai ≤ 1000).
                str = Console.ReadLine();
                if (str.Split(' ').Length > 1000) throw new Exception("N exceeds limit (1000)");
                if (str.Split(' ').Length != N) throw new Exception("N does not match the number of elements provided");
                A = new int[N];
                string[] elements = str.Split(' ');
                for (int j = 0; j < N; j++)
                {
                    if (Convert.ToInt32(elements[j]) > 1000 || Convert.ToInt32(elements[j]) < 0) throw new Exception("Element out of bounds (0 ≤ Ai ≤ 1000) (" + elements[j] + ")");
                    A[j] = Convert.ToInt32(elements[j]);
                }
                //Ezután egy sor a B tömb M darab elemével (0 ≤ Bi ≤ 1000).
                //Feltételezhető, hogy a B tömb minden eleme egyedi.
                str = Console.ReadLine();
                if (str.Split(' ').Length > 1000) throw new Exception("M exceeds limit (1000)");
                if (str.Split(' ').Length != M) throw new Exception("M does not match the number of elements provided");
                B = new int[M];
                elements = str.Split(' ');
                for (int j = 0; j < M; j++)
                {
                    if (Convert.ToInt32(elements[j]) > 1000 || Convert.ToInt32(elements[j]) < 0) throw new Exception("Element out of bounds (0 ≤ Ai ≤ 1000) (" + elements[j] + ")");
                    B[j] = Convert.ToInt32(elements[j]);
                }
                
                foreach (int val in A)
                {
                    long[] next = new long[DP.Length];
                    // Először: másoljuk a régi állapotot, mert ha NEM vesszük bele a val-t,
                    // akkor ugyanazok a XOR értékek maradnak.
                    for (int x = 0; x < DP.Length; x++) next[x] = DP[x];
                    // Most: ha BELEvesszük a val-t, akkor az összes régi XOR-ból új XOR lesz: x XOR val
                    for (int x = 0; x < DP.Length; x++)
                    {
                        if (DP[x] > 0)
                        {
                            int newXor = x ^ val;
                            next[newXor] += DP[x];
                        }
                    }
                    DP = next; // frissítjük az aktuális dp állapotot
                }
                //Kimenet
                //Minden tesztesethez írd ki a sorszámot és a jó részhalmazok számát.
                //Mivel az eredmény nagyon nagy is lehet, a 100000007 modult kell használni.
                long MOD = 1_000_000_007L;
                long result = 0;
                for (int j = 0; j < DP.Length; j++)
                {
                    bool found = false;
                    for (int g = 0; g < B.Length; g++)
                    {
                        if (j == B[g])
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found) result = (result + DP[j]) % MOD;
                }
                output += $"Case {i + 1}: {result}\n";
            }
            Console.WriteLine("Output:\n" + output);
        }
    }
}

