using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatalogoRecursivo
{
    public partial class FormPrincipal : Form
    {
        private readonly ListBox lstEjercicios = new() { Dock = DockStyle.Fill, IntegralHeight = false };
        private readonly Label lblDescripcion = new() { Dock = DockStyle.Fill, AutoSize = false, Padding = new Padding(6), BackColor = Color.AliceBlue };

        private readonly Label lblN = new() { Text = "n:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) };
        private readonly NumericUpDown nudN = new() { Minimum = 0, Maximum = 20, Value = 4, Width = 70 };
        private readonly Label lblArreglo = new() { Text = "Arreglo:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) };
        private readonly TextBox txtArreglo = new() { Text = "3, 7, 2, 5", Width = 220 };
        private readonly Label lblObjetivo = new() { Text = "Buscar:", AutoSize = true, Margin = new Padding(3, 9, 3, 3) };
        private readonly NumericUpDown nudObjetivo = new() { Minimum = -100000, Maximum = 100000, Value = 2, Width = 80 };
        private readonly Button btnEjecutar = new() { Text = "Ejecutar", AutoSize = true };

        private readonly Label lblResultados = new() { Dock = DockStyle.Fill, AutoSize = false, Padding = new Padding(6), Font = new Font("Consolas", 10F) };
        private readonly TextBox txtTraza = new()
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            Font = new Font("Consolas", 10F),
            BackColor = Color.White
        };

        // Descripción de cada ejercicio: nombre, caso base y caso recursivo.
        private static readonly (string Nombre, string Base, string Recursivo)[] Catalogo =
        {
        ("1. Factorial", "n ≤ 1  →  devuelve 1", "n · Factorial(n − 1)"),
        ("2. Fibonacci", "n ≤ 1  →  devuelve n", "Fibonacci(n − 1) + Fibonacci(n − 2)"),
        ("3. Torres de Hanoi", "n = 1  →  mover el disco directamente", "Hanoi(n − 1, origen → auxiliar), mover disco n, Hanoi(n − 1, auxiliar → destino)"),
        ("4. Suma de un arreglo", "índice = longitud  →  devuelve 0", "datos[índice] + Suma(índice + 1)"),
        ("5. Búsqueda lineal", "índice = longitud → −1;  datos[índice] = objetivo → índice", "Buscar(objetivo, índice + 1)")
    };

        public FormPrincipal()
        {
            Text = "Tarea 2.2 - Catálogo de ejercicios: de iterativo a recursivo";
            Size = new Size(1100, 720);
            MinimumSize = new Size(850, 550);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10F);

            foreach (var ejercicio in Catalogo) lstEjercicios.Items.Add(ejercicio.Nombre);

            var parametros = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            parametros.Controls.AddRange(new Control[] { lblN, nudN, lblArreglo, txtArreglo, lblObjetivo, nudObjetivo, btnEjecutar });

            var derecha = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            derecha.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            derecha.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            derecha.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            derecha.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            derecha.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            derecha.Controls.Add(lblDescripcion, 0, 0);
            derecha.Controls.Add(parametros, 0, 1);
            derecha.Controls.Add(lblResultados, 0, 2);
            derecha.Controls.Add(txtTraza, 0, 3);

            var principal = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(6) };
            principal.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            principal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            principal.Controls.Add(lstEjercicios, 0, 0);
            principal.Controls.Add(derecha, 1, 0);
            Controls.Add(principal);

            lstEjercicios.SelectedIndexChanged += (s, e) => ConfigurarEjercicio();
            btnEjecutar.Click += BtnEjecutar_Click;
            lstEjercicios.SelectedIndex = 0;
        }

        /// <summary>Muestra solo los parámetros que necesita el ejercicio elegido.</summary>
        private void ConfigurarEjercicio()
        {
            int i = lstEjercicios.SelectedIndex;
            if (i < 0) return;

            var (nombre, casoBase, casoRecursivo) = Catalogo[i];
            lblDescripcion.Text = $"{nombre}{Environment.NewLine}Caso base: {casoBase}{Environment.NewLine}Caso recursivo: {casoRecursivo}";

            bool usaN = i <= 2;
            lblN.Visible = nudN.Visible = usaN;
            lblArreglo.Visible = txtArreglo.Visible = !usaN;
            lblObjetivo.Visible = nudObjetivo.Visible = i == 4;

            // Límites para que el resultado quepa en un long y la traza siga siendo legible.
            nudN.Maximum = i switch { 0 => 20, 1 => 30, _ => 15 };
            nudN.Minimum = i == 2 ? 1 : 0;
            lblResultados.Text = "";
            txtTraza.Clear();
        }

        private void BtnEjecutar_Click(object? sender, EventArgs e)
        {
            int ejercicio = lstEjercicios.SelectedIndex;
            int n = (int)nudN.Value;
            int[] datos = Array.Empty<int>();

            if (ejercicio >= 3 && !IntentarLeerArreglo(out datos))
            {
                MessageBox.Show("Escribe números enteros separados por comas.", "Arreglo no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                // 1) Versión iterativa
                var cronometro = Stopwatch.StartNew();
                string iterativo = EjecutarIterativo(ejercicio, n, datos);
                double msIterativo = cronometro.Elapsed.TotalMilliseconds;

                // 2) Versión recursiva sin traza (para medir el tiempo y contar las llamadas)
                var conteo = new Traza(activa: false);
                cronometro.Restart();
                string recursivo = EjecutarRecursivo(ejercicio, n, datos, conteo);
                double msRecursivo = cronometro.Elapsed.TotalMilliseconds;

                // 3) Versión recursiva con traza (prueba de escritorio)
                var traza = new Traza(activa: true);
                EjecutarRecursivo(ejercicio, n, datos, traza);

                bool coinciden = iterativo == recursivo;
                lblResultados.Text =
                    $"Iterativo : {Resumir(iterativo)}   ({msIterativo:F3} ms){Environment.NewLine}" +
                    $"Recursivo : {Resumir(recursivo)}   ({msRecursivo:F3} ms){Environment.NewLine}" +
                    $"¿Coinciden? {(coinciden ? "Sí" : "NO")}{Environment.NewLine}" +
                    $"Llamadas recursivas: {conteo.Llamadas:N0}    Profundidad máxima de la pila: {conteo.ProfundidadMaxima:N0}";

                var lineas = new List<string>(traza.Lineas);
                if (traza.Recortada) lineas.Add($"... (la traza se recortó a {traza.Lineas.Count} líneas)");
                txtTraza.Lines = lineas.ToArray();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private string EjecutarIterativo(int ejercicio, int n, int[] datos) => ejercicio switch
        {
            0 => Ejercicios.FactorialIterativo(n).ToString(),
            1 => Ejercicios.FibonacciIterativo(n).ToString(),
            2 => string.Join(" | ", Ejercicios.HanoiIterativo(n, 'A', 'C', 'B')),
            3 => Ejercicios.SumaIterativa(datos).ToString(),
            _ => Ejercicios.BusquedaIterativa(datos, (int)nudObjetivo.Value).ToString()
        };

        private string EjecutarRecursivo(int ejercicio, int n, int[] datos, Traza t)
        {
            switch (ejercicio)
            {
                case 0: return Ejercicios.FactorialRecursivo(n, t).ToString();
                case 1: return Ejercicios.FibonacciRecursivo(n, t).ToString();
                case 2:
                    var movimientos = new List<string>();
                    Ejercicios.HanoiRecursivo(n, 'A', 'C', 'B', movimientos, t);
                    return string.Join(" | ", movimientos);
                case 3: return Ejercicios.SumaRecursiva(datos, 0, t).ToString();
                default: return Ejercicios.BusquedaRecursiva(datos, (int)nudObjetivo.Value, 0, t).ToString();
            }
        }

        /// <summary>Convierte el texto "3, 7, 2, 5" en un arreglo de enteros.</summary>
        private bool IntentarLeerArreglo(out int[] datos)
        {
            var partes = txtArreglo.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            datos = new int[partes.Length];
            for (int i = 0; i < partes.Length; i++)
            {
                if (!int.TryParse(partes[i], out datos[i])) return false;
            }
            return datos.Length > 0;
        }

        /// <summary>Para Hanoi el resultado es muy largo: se muestra solo el número de movimientos.</summary>
        private static string Resumir(string resultado)
        {
            if (!resultado.Contains('|')) return resultado;
            int movimientos = resultado.Split('|').Length;
            return $"{movimientos:N0} movimientos (2^n − 1)";
        }
    }
}
