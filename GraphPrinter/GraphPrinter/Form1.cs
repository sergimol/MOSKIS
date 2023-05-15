using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GraphPrinter
{
    public partial class GraphPrinter : Form
    {
        string fileName = "";
        bool showingGraph = false;

        public GraphPrinter()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            try 
            {
                // Mostrar el cuadro de diálogo para abrir el archivo
                openFileDialog1.ShowDialog();
                // Abrir el archivo asegurando que tenga la extensión .csv
                fileName = openFileDialog1.FileName;
                if (Path.GetExtension(fileName) != ".csv")
                    throw new InvalidOperationException("El archivo a leer debe ser tipo CSV");
                
                // Leer la primera línea para obtener los nombres de los eventos
                IEnumerable<string> lines = File.ReadLines(fileName);
                string[] events = lines.First().Split(',');
                graph.Titles.First().Text = events[0];
                graph.Titles.Last().Text = events[1];

                // Asigna el valor mínimo al espacio para las gráficas para que no empiece en -1
                graph.ChartAreas[0].AxisX.Minimum = 0;
                graph.ChartAreas[0].AxisY.Minimum = 0;

                // Pasar los valores de los puntos a una lista y eliminar el primer elemento para iterar
                List<string> values = lines.ToList();
                values.Remove(values.First());
                foreach (string value in values)
                {
                    // Obtener los valores de los puntos y añadirlos a cada gráfica
                    string[] positions = value.Split(',');
                    double x = double.Parse(positions[0]);
                    double yObt = double.Parse(positions[1]);
                    double yDes = double.Parse(positions[2]);
                    graph.Series.FindByName("Obtenida").Points.AddXY(x, yObt);
                    graph.Series.FindByName("Diseñada").Points.AddXY(x, yDes);

                    // Comprobar si las gráficas tienen nuevos valores máximos
                    graph.ChartAreas[0].AxisX.Maximum = Math.Max(graph.ChartAreas[0].AxisX.Maximum, x);
                    double maxYCandidate = Math.Max(yObt, yDes);
                    graph.ChartAreas[0].AxisY.Maximum = Math.Max(graph.ChartAreas[0].AxisY.Maximum, maxYCandidate);
                }
                showingGraph = true;
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.Message);
                showingGraph = false;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (showingGraph)
            {
                string imageName = fileName.Split('.')[0];
                graph.SaveImage(fileName + ".png", ChartImageFormat.Png);
            }
        }
    }
}
