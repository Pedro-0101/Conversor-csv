namespace csvConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBoxConteudo_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using var openFileDialog = new OpenFileDialog()
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    RestoreDirectory = true,
                    InitialDirectory = @"C:\"
                };

                var result = openFileDialog.ShowDialog();

                // 1. Sai se o usuário não selecionar OK
                if (result != DialogResult.OK)
                    return;

                var filePath = openFileDialog.FileName;

                // 2. Verifica se o caminho é válido e o arquivo existe
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    MessageBox.Show("Caminho do arquivo inválido ou arquivo não encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Lê todas as linhas. File.ReadLines é eficiente para arquivos grandes.
                var lines = File.ReadLines(filePath);

                // 4. Sai se o arquivo estiver vazio
                if (!lines.Any())
                    return;

                // Limpa todas as linhas existentes
                dataGridView1.Rows.Clear();

                var targetColumnCount = dataGridView1.ColumnCount;
                const int ColumnIndexToIgnore = 1; // Coluna a ser ignorada (índice 1 = 2ª coluna)

                // Acumula as linhas processadas para ordenar pela coluna 0 depois
                var processedRows = new List<string[]>();

                foreach (var line in lines)
                {
                    var allColumns = line.Split(';');

                    string[] processedColumns;
                    if (allColumns.Length > ColumnIndexToIgnore)
                    {
                        // Cria um novo array de colunas, pulando o elemento no índice 1.
                        processedColumns = allColumns
                            .Where((col, index) => index != ColumnIndexToIgnore)
                            .ToArray();
                    }
                    else
                    {
                        processedColumns = allColumns;
                    }

                    // Ajusta o número de colunas para corresponder ao DataGridView
                    if (processedColumns.Length < targetColumnCount)
                    {
                        // Preenche com strings vazias
                        var padded = processedColumns.Concat(Enumerable.Repeat(string.Empty, targetColumnCount - processedColumns.Length)).ToArray();
                        processedRows.Add(padded);
                    }
                    else if (processedColumns.Length > targetColumnCount)
                    {
                        // Trunca (corta) o array
                        processedRows.Add(processedColumns.Take(targetColumnCount).ToArray());
                    }
                    else
                    {
                        // O número de colunas processadas é exato
                        processedRows.Add(processedColumns);
                    }
                }

                // Ordena as linhas pelo valor da coluna 0 em ordem alfabética (case-insensitive)
                var orderedRows = processedRows
                    .OrderBy(r => r.Length > 0 ? (r[0] ?? string.Empty) : string.Empty, StringComparer.CurrentCultureIgnoreCase);

                // Adiciona ao DataGridView na ordem ordenada
                foreach (var row in orderedRows)
                {
                    dataGridView1.Rows.Add(row);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Erro ao processar arquivo CSV: " + err.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Erro ao copiar dados: Nao existe dados na tabela", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                ReadDataGridViewContent();
            }
        }

        private void ReadDataGridViewContent()
        {
            // Limpa a saída de depuração para um novo início
            string data = "";
            int columnAlmoco = 1;
            int columnLocal = 2;
            string valuePedeAlmoco = "SIM";
            string valueLocalSaoJoao = "SÃO JOÃO";

            // 1. Itera sobre cada LINHA (DataGridViewRow)
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                // Verifica se a linha não é a linha de cabeçalho ou a linha vazia de novo registro
                if (!row.IsNewRow)
                {
                    string rowData = "";
                    Boolean pedidoAlmoco = string.Equals(row.Cells[columnAlmoco].Value?.ToString()?.Replace("\"", "")?.Trim(), valuePedeAlmoco, StringComparison.OrdinalIgnoreCase);
                    Boolean almocoSaoJoao = string.Equals(row.Cells[columnLocal].Value?.ToString()?.Replace("\"", "")?.Trim(), valueLocalSaoJoao, StringComparison.OrdinalIgnoreCase);

                    if (pedidoAlmoco && almocoSaoJoao)
                    {
                        rowData = "1\t1\n"; // \t para adicionar tabulacao e \n para quebra de linha

                    } else
                    {

                        rowData = "0\t0\n"; // \t para adicionar tabulacao e \n para quebra de linha

                    }

                        // Adiciona o conteudo da linha na data
                        data += rowData;
                }
            }

            // Adiciona o conteudo no clipboard
            Clipboard.SetText(data);
            MessageBox.Show("Conteudo copiado para o clipboard!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
