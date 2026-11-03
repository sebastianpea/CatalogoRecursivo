namespace CatalogoRecursivo;

/// <summary>
/// Registra las llamadas recursivas (entradas y salidas) para mostrarlas como prueba de escritorio.
/// Cuando no está activa solo cuenta las llamadas, para que la medición de tiempo sea justa.
/// </summary>
public class Traza
{
    private const int MaximoDeLineas = 400;
    private readonly List<string> lineas = new();
    private int nivel;

    public bool Activa { get; }
    public long Llamadas { get; private set; }
    public int ProfundidadMaxima { get; private set; }
    public bool Recortada { get; private set; }
    public IReadOnlyList<string> Lineas => lineas;

    public Traza(bool activa)
    {
        Activa = activa;
    }

    /// <summary>Se invoca al comenzar cada llamada: aumenta la profundidad de la pila.</summary>
    public void Entrar(string llamada)
    {
        Llamadas++;
        nivel++;
        if (nivel > ProfundidadMaxima) ProfundidadMaxima = nivel;
        Agregar($"{Sangria()}→ {llamada}");
    }

    /// <summary>Se invoca al terminar una llamada que devuelve un valor: la pila disminuye.</summary>
    public T Salir<T>(T resultado, bool esCasoBase = false)
    {
        Agregar($"{Sangria()}← devuelve {resultado}{(esCasoBase ? "   [caso base]" : "")}");
        nivel--;
        return resultado;
    }

    /// <summary>Se invoca al terminar una llamada que no devuelve valor (por ejemplo, Hanoi).</summary>
    public void SalirSinValor(bool esCasoBase = false)
    {
        Agregar($"{Sangria()}← termina{(esCasoBase ? "   [caso base]" : "")}");
        nivel--;
    }

    /// <summary>Agrega un comentario dentro de la llamada actual.</summary>
    public void Nota(string texto) => Agregar($"{Sangria()}   {texto}");

    private string Sangria() => new string(' ', Math.Max(0, nivel - 1) * 3);

    private void Agregar(string linea)
    {
        if (!Activa) return;
        if (lineas.Count < MaximoDeLineas) lineas.Add(linea);
        else Recortada = true;
    }
}
