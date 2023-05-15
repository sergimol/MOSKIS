namespace GraphPrinter
{
    partial class GraphPrinter
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title7 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title8 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.graph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.SaveButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.graph)).BeginInit();
            this.SuspendLayout();
            // 
            // graph
            // 
            chartArea4.Name = "ChartArea1";
            this.graph.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.graph.Legends.Add(legend4);
            this.graph.Location = new System.Drawing.Point(2, 3);
            this.graph.Name = "graph";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Color = System.Drawing.Color.Red;
            series7.Legend = "Legend1";
            series7.Name = "Diseñada";
            series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series7.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Color = System.Drawing.Color.Lime;
            series8.Legend = "Legend1";
            series8.Name = "Obtenida";
            series8.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series8.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.graph.Series.Add(series7);
            this.graph.Series.Add(series8);
            this.graph.Size = new System.Drawing.Size(786, 300);
            this.graph.TabIndex = 0;
            this.graph.Text = "chart1";
            title7.Alignment = System.Drawing.ContentAlignment.BottomCenter;
            title7.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title7.Name = "EventX";
            title8.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title8.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title8.Name = "EventY";
            this.graph.Titles.Add(title7);
            this.graph.Titles.Add(title8);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(29, 384);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Guardar";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Location = new System.Drawing.Point(29, 355);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(75, 23);
            this.OpenButton.TabIndex = 2;
            this.OpenButton.Text = "Abrir archivo";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Archivos CSV (*.csv)|*.csv";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "png";
            this.saveFileDialog1.Filter = "Archivos PNG(*.png)|*.png";
            // 
            // GraphPrinter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.OpenButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.graph);
            this.Name = "GraphPrinter";
            this.Text = "GraphPrinter";
            ((System.ComponentModel.ISupportInitialize)(this.graph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart graph;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button OpenButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

