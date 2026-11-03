namespace CatalogoRecursivo;

/// <summary>
/// Catálogo de la Tarea 2.2. Cada problema tiene su versión iterativa (original)
/// y su conversión a recursiva, con el caso base y el caso recursivo señalados.
/// </summary>
public static class Ejercicios
{
    // =====================================================================
    // 1. FACTORIAL: n! = n · (n − 1)!
    // =====================================================================

    public static long FactorialIterativo(int n)
    {
        long resultado = 1;
        for (int i = 2; i <= n; i++)
        {
            resultado *= i;
        }
        return resultado;
    }

    public static long FactorialRecursivo(int n, Traza t)
    {
        t.Entrar($"Factorial({n})");

        // CASO BASE: 0! = 1! = 1
        if (n <= 1)
            return t.Salir(1L, esCasoBase: true);

        // CASO RECURSIVO: el problema se reduce a uno de tamaño n − 1
        return t.Salir(n * FactorialRecursivo(n - 1, t));
    }

    // =====================================================================
    // 2. FIBONACCI: F(n) = F(n − 1) + F(n − 2)
    // =====================================================================

    public static long FibonacciIterativo(int n)
    {
        long anterior = 0, actual = 1;
        if (n == 0) return 0;
        for (int i = 2; i <= n; i++)
        {
            long siguiente = anterior + actual;
            anterior = actual;
            actual = siguiente;
        }
        return actual;
    }

    public static long FibonacciRecursivo(int n, Traza t)
    {
        t.Entrar($"Fibonacci({n})");

        // CASO BASE: F(0) = 0 y F(1) = 1
        if (n <= 1)
            return t.Salir((long)n, esCasoBase: true);

        // CASO RECURSIVO: dos llamadas con problemas más pequeños
        return t.Salir(FibonacciRecursivo(n - 1, t) + FibonacciRecursivo(n - 2, t));
    }

    // =====================================================================
    // 3. TORRES DE HANOI: mover n discos de origen a destino
    // =====================================================================

    /// <summary>
    /// Versión iterativa clásica: en cada paso se hace el único movimiento legal
    /// entre un par de postes, recorriendo los pares de forma cíclica.
    /// </summary>
    public static List<string> HanoiIterativo(int n, char origen, char destino, char auxiliar)
    {
        var movimientos = new List<string>();
        var postes = new Dictionary<char, Stack<int>>
        {
            [origen] = new Stack<int>(),
            [destino] = new Stack<int>(),
            [auxiliar] = new Stack<int>()
        };
        for (int disco = n; disco >= 1; disco--) postes[origen].Push(disco);

        // Si n es par, el ciclo de pares se recorre intercambiando destino y auxiliar.
        char a = origen, b = destino, c = auxiliar;
        if (n % 2 == 0) (b, c) = (c, b);

        long total = (1L << n) - 1;   // 2^n − 1 movimientos
        for (long i = 1; i <= total; i++)
        {
            switch (i % 3)
            {
                case 1: MoverLegal(postes, a, b, movimientos); break;
                case 2: MoverLegal(postes, a, c, movimientos); break;
                default: MoverLegal(postes, c, b, movimientos); break;
            }
        }
        return movimientos;
    }

    /// <summary>Mueve el disco más pequeño de los dos postes hacia el otro.</summary>
    private static void MoverLegal(Dictionary<char, Stack<int>> postes, char x, char y, List<string> movimientos)
    {
        Stack<int> px = postes[x], py = postes[y];
        if (py.Count == 0 || (px.Count > 0 && px.Peek() < py.Peek()))
            movimientos.Add($"Disco {MoverDisco(px, py)}: {x} → {y}");
        else
            movimientos.Add($"Disco {MoverDisco(py, px)}: {y} → {x}");
    }

    private static int MoverDisco(Stack<int> desde, Stack<int> hacia)
    {
        int disco = desde.Pop();
        hacia.Push(disco);
        return disco;
    }

    public static void HanoiRecursivo(int n, char origen, char destino, char auxiliar, List<string> movimientos, Traza t)
    {
        t.Entrar($"Hanoi({n}, {origen} → {destino}, aux {auxiliar})");

        // CASO BASE: un solo disco se mueve directamente
        if (n == 1)
        {
            movimientos.Add($"Disco 1: {origen} → {destino}");
            t.Nota($"mover disco 1: {origen} → {destino}");
            t.SalirSinValor(esCasoBase: true);
            return;
        }

        // CASO RECURSIVO: n − 1 discos al auxiliar, el disco n al destino, n − 1 discos al destino
        HanoiRecursivo(n - 1, origen, auxiliar, destino, movimientos, t);
        movimientos.Add($"Disco {n}: {origen} → {destino}");
        t.Nota($"mover disco {n}: {origen} → {destino}");
        HanoiRecursivo(n - 1, auxiliar, destino, origen, movimientos, t);
        t.SalirSinValor();
    }

    // =====================================================================
    // 4. SUMA DE LOS ELEMENTOS DE UN ARREGLO
    // =====================================================================

    public static long SumaIterativa(int[] datos)
    {
        long suma = 0;
        for (int i = 0; i < datos.Length; i++)
        {
            suma += datos[i];
        }
        return suma;
    }

    public static long SumaRecursiva(int[] datos, int indice, Traza t)
    {
        t.Entrar($"Suma(indice = {indice})");

        // CASO BASE: ya no quedan elementos por sumar
        if (indice == datos.Length)
            return t.Salir(0L, esCasoBase: true);

        // CASO RECURSIVO: el elemento actual más la suma del resto
        return t.Salir(datos[indice] + SumaRecursiva(datos, indice + 1, t));
    }

    // =====================================================================
    // 5. BÚSQUEDA LINEAL
    // =====================================================================

    public static int BusquedaIterativa(int[] datos, int objetivo)
    {
        for (int i = 0; i < datos.Length; i++)
        {
            if (datos[i] == objetivo) return i;
        }
        return -1;
    }

    public static int BusquedaRecursiva(int[] datos, int objetivo, int indice, Traza t)
    {
        t.Entrar($"Buscar({objetivo}, indice = {indice})");

        // CASO BASE 1: se recorrió todo el arreglo sin encontrarlo
        if (indice == datos.Length)
            return t.Salir(-1, esCasoBase: true);

        // CASO BASE 2: el elemento actual es el buscado
        if (datos[indice] == objetivo)
            return t.Salir(indice, esCasoBase: true);

        // CASO RECURSIVO: buscar en el resto del arreglo
        return t.Salir(BusquedaRecursiva(datos, objetivo, indice + 1, t));
    }
}