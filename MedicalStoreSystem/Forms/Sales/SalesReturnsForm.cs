using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Sales
{
    public partial class SalesReturnsForm : Form
    {
        private SaleDAL saleDAL = new SaleDAL();

        public SalesReturnsForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;

            InitializeControls();
        }

        private void InitializeControls()
        {
            // Panel العلوي
            Panel panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 120;
            panelTop.BackColor = System.Drawing.Color.WhiteSmoke;

            GroupBox groupSearch = new GroupBox();
            groupSearch.Text = "بحث عن فاتورة";
            groupSearch.Location = new System.Drawing.Point(20, 10);
            groupSearch.Size = new System.Drawing.Size(600, 100);

            Label lblSaleNumber = new Label();
            lblSaleNumber.Text = "رقم الفاتورة:";
            lblSaleNumber.Location = new System.Drawing.Point(20, 30);
            lblSaleNumber.AutoSize = true;

            TextBox txtSaleNumber = new TextBox();
            txtSaleNumber.Name = "txtSaleNumber";
            txtSaleNumber.Location = new System.Drawing.Point(120, 28);
            txtSaleNumber.Size = new System.Drawing.Size(200, 25);

            Button btnSearch = new Button();
            btnSearch.Text = "بحث";
            btnSearch.Location = new System.Drawing.Point(330, 26);
            btnSearch.Size = new System.Drawing.Size(80, 30);
            btnSearch.Click += BtnSearch_Click;

            Label lblInfo = new Label();
            lblInfo.Name = "lblInfo";
            lblInfo.Text = "أدخل رقم الفاتورة للبحث...";
            lblInfo.Location = new System.Drawing.Point(20, 65);
            lblInfo.Size = new System.Drawing.Size(550, 25);
            lblInfo.ForeColor = System.Drawing.Color.Gray;

            groupSearch.Controls.Add(lblSaleNumber);
            groupSearch.Controls.Add(txtSaleNumber);
            groupSearch.Controls.Add(btnSearch);
            groupSearch.Controls.Add(lblInfo);
            panelTop.Controls.Add(groupSearch);

            // Panel للتفاصيل
            Panel panelDetails = new Panel();
            panelDetails.Name = "panelDetails";
            panelDetails.Dock = DockStyle.Fill;
            panelDetails.Visible = false;

            GroupBox groupItems = new GroupBox();
            groupItems.Text = "أصناف الفاتورة";
            groupItems.Dock = DockStyle.Fill;
            groupItems.Padding = new Padding(10);

            DataGridView dgvItems = new DataGridView();
            dgvItems.Name = "dgvItems";
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.ReadOnly = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            groupItems.Controls.Add(dgvItems);

            // Panel للأزرار السفلية
            Panel panelBottom = new Panel();
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Height = 100;
            panelBottom.BackColor = System.Drawing.Color.WhiteSmoke;

            GroupBox groupReturn = new GroupBox();
            groupReturn.Text = "المرتجع";
            groupReturn.Location = new System.Drawing.Point(20, 10);
            groupReturn.Size = new System.Drawing.Size(700, 80);

            Label lblReturnAmount = new Label();
            lblReturnAmount.Text = "قيمة المرتجع:";
            lblReturnAmount.Location = new System.Drawing.Point(20, 30);
            lblReturnAmount.AutoSize = true;

            Label lblReturnValue = new Label();
            lblReturnValue.Name = "lblReturnValue";
            lblReturnValue.Text = "0.00 جنيه";
            lblReturnValue.Location = new System.Drawing.Point(120, 30);
            lblReturnValue.Size = new System.Drawing.Size(150, 25);
            lblReturnValue.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            lblReturnValue.ForeColor = System.Drawing.Color.Red;

            Button btnProcessReturn = new Button();
            btnProcessReturn.Name = "btnProcessReturn";
            btnProcessReturn.Text = "إتمام المرتجع";
            btnProcessReturn.Location = new System.Drawing.Point(300, 20);
            btnProcessReturn.Size = new System.Drawing.Size(150, 40);
            btnProcessReturn.BackColor = System.Drawing.Color.Red;
            btnProcessReturn.ForeColor = System.Drawing.Color.White;
            btnProcessReturn.Click += BtnProcessReturn_Click;

            groupReturn.Controls.Add(lblReturnAmount);
            groupReturn.Controls.Add(lblReturnValue);
            groupReturn.Controls.Add(btnProcessReturn);
            panelBottom.Controls.Add(groupReturn);

            panelDetails.Controls.Add(groupItems);
            panelDetails.Controls.Add(panelBottom);

            this.Controls.Add(panelDetails);
            this.Controls.Add(panelTop);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox txtSaleNumber = this.Controls.Find("txtSaleNumber", true)[0] as TextBox;
                string saleNumber = txtSaleNumber.Text.Trim();

                if (string.IsNullOrWhiteSpace(saleNumber))
                {
                    MessageBox.Show("يرجى إدخال رقم الفاتورة", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // البحث عن الفاتورة
                string query = @"SELECT SaleID FROM Sales WHERE SaleNumber = @SaleNumber";
                SqlParameter[] parameters = {
                    new SqlParameter("@SaleNumber", saleNumber)
                };

                DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int saleID = Convert.ToInt32(dt.Rows[0]["SaleID"]);
                    LoadSaleDetails(saleID);
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على الفاتورة", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSaleDetails(int saleID)
        {
            try
            {
                // جلب تفاصيل الفاتورة
                var sale = saleDAL.GetSaleByID(saleID);

                if (sale != null)
                {
                    Label lblInfo = this.Controls.Find("lblInfo", true)[0] as Label;
                    lblInfo.Text = $"الفاتورة: {sale.SaleNumber} - العميل: {sale.CustomerName} - التاريخ: {sale.SaleDate:yyyy-MM-dd}";

                    // عرض الأصناف في DataGridView
                    DataGridView dgv = this.Controls.Find("dgvItems", true)[0] as DataGridView;
                    dgv.Columns.Clear();
                    dgv.Rows.Clear();

                    // إضافة الأعمدة
                    dgv.Columns.Add("SaleDetailID", "SaleDetailID");
                    dgv.Columns["SaleDetailID"].Visible = false;

                    dgv.Columns.Add("ProductID", "ProductID");
                    dgv.Columns["ProductID"].Visible = false;

                    dgv.Columns.Add("ProductName", "اسم المنتج");
                    dgv.Columns["ProductName"].ReadOnly = true;

                    dgv.Columns.Add("Quantity", "الكمية المباعة");
                    dgv.Columns["Quantity"].ReadOnly = true;

                    dgv.Columns.Add("UnitPrice", "السعر");
                    dgv.Columns["UnitPrice"].ReadOnly = true;

                    // عمود الكمية المرتجعة (قابل للتعديل)
                    DataGridViewNumericUpDownColumn colReturnQty = new DataGridViewNumericUpDownColumn();
                    colReturnQty.Name = "ReturnQuantity";
                    colReturnQty.HeaderText = "الكمية المرتجعة";
                    colReturnQty.Minimum = 0;
                    dgv.Columns.Add(colReturnQty);

                    dgv.Columns.Add("ReturnValue", "قيمة المرتجع");
                    dgv.Columns["ReturnValue"].ReadOnly = true;

                    // إضافة الصفوف
                    foreach (var detail in sale.Details)
                    {
                        int rowIndex = dgv.Rows.Add();
                        DataGridViewRow row = dgv.Rows[rowIndex];

                        row.Cells["SaleDetailID"].Value = detail.SaleDetailID;
                        row.Cells["ProductID"].Value = detail.ProductID;
                        row.Cells["ProductName"].Value = detail.ProductName;
                        row.Cells["Quantity"].Value = detail.Quantity;
                        row.Cells["UnitPrice"].Value = detail.UnitPrice;
                        row.Cells["ReturnQuantity"].Value = 0;
                        row.Cells["ReturnValue"].Value = 0.00m;
                    }

                    dgv.CellValueChanged += DgvItems_CellValueChanged;
                    dgv.CurrentCellDirtyStateChanged += (s, ev) => {
                        if (dgv.IsCurrentCellDirty)
                            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    };

                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    // إظهار panel التفاصيل
                    Panel panelDetails = this.Controls.Find("panelDetails", true)[0] as Panel;
                    panelDetails.Visible = true;

                    // حفظ SaleID
                    panelDetails.Tag = saleID;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = sender as DataGridView;

                if (dgv.Columns[e.ColumnIndex].Name == "ReturnQuantity")
                {
                    try
                    {
                        DataGridViewRow row = dgv.Rows[e.RowIndex];

                        int maxQty = Convert.ToInt32(row.Cells["Quantity"].Value);
                        int returnQty = Convert.ToInt32(row.Cells["ReturnQuantity"].Value);
                        decimal unitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value);

                        // التحقق من عدم تجاوز الكمية
                        if (returnQty > maxQty)
                        {
                            MessageBox.Show("الكمية المرتجعة لا يمكن أن تكون أكبر من الكمية المباعة!", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            row.Cells["ReturnQuantity"].Value = 0;
                            return;
                        }

                        // حساب قيمة المرتجع
                        decimal returnValue = returnQty * unitPrice;
                        row.Cells["ReturnValue"].Value = returnValue;

                        // حساب الإجمالي
                        CalculateTotalReturn();
                    }
                    catch { }
                }
            }
        }

        private void CalculateTotalReturn()
        {
            try
            {
                DataGridView dgv = this.Controls.Find("dgvItems", true)[0] as DataGridView;
                decimal totalReturn = 0;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["ReturnValue"].Value != null)
                    {
                        totalReturn += Convert.ToDecimal(row.Cells["ReturnValue"].Value);
                    }
                }

                Label lblReturnValue = this.Controls.Find("lblReturnValue", true)[0] as Label;
                lblReturnValue.Text = totalReturn.ToString("F2") + " جنيه";
            }
            catch { }
        }

        private void BtnProcessReturn_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridView dgv = this.Controls.Find("dgvItems", true)[0] as DataGridView;
                Panel panelDetails = this.Controls.Find("panelDetails", true)[0] as Panel;
                int saleID = Convert.ToInt32(panelDetails.Tag);

                // التحقق من وجود أصناف مرتجعة
                bool hasReturns = false;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    int returnQty = Convert.ToInt32(row.Cells["ReturnQuantity"].Value);
                    if (returnQty > 0)
                    {
                        hasReturns = true;
                        break;
                    }
                }

                if (!hasReturns)
                {
                    MessageBox.Show("يرجى إدخال الكميات المرتجعة", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal totalReturn = 0;
                Label lblReturnValue = this.Controls.Find("lblReturnValue", true)[0] as Label;
                totalReturn = decimal.Parse(lblReturnValue.Text.Replace(" جنيه", ""));

                DialogResult result = MessageBox.Show(
                    $"هل تريد إتمام عملية المرتجع؟\n\nقيمة المرتجع: {totalReturn:F2} جنيه",
                    "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    ProcessReturn(saleID, dgv, totalReturn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessReturn(int saleID, DataGridView dgv, decimal totalReturn)
        {
            SqlConnection connection = DatabaseConnection.GetConnection();
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    int returnQty = Convert.ToInt32(row.Cells["ReturnQuantity"].Value);

                    if (returnQty > 0)
                    {
                        int productID = Convert.ToInt32(row.Cells["ProductID"].Value);

                        // إرجاع الكمية للمخزون
                        string queryStock = @"UPDATE Products 
                                            SET CurrentStock = CurrentStock + @Quantity 
                                            WHERE ProductID = @ProductID";

                        SqlCommand cmdStock = new SqlCommand(queryStock, connection, transaction);
                        cmdStock.Parameters.AddWithValue("@Quantity", returnQty);
                        cmdStock.Parameters.AddWithValue("@ProductID", productID);
                        cmdStock.ExecuteNonQuery();

                        // تسجيل حركة المخزن
                        string queryMovement = @"INSERT INTO StockMovements 
                                               (MovementDate, ProductID, MovementType, Quantity, 
                                               ReferenceType, ReferenceID, UserID, CreatedDate)
                                               VALUES
                                               (@MovementDate, @ProductID, @MovementType, @Quantity,
                                               @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                        SqlCommand cmdMovement = new SqlCommand(queryMovement, connection, transaction);
                        cmdMovement.Parameters.AddWithValue("@MovementDate", DateTime.Now);
                        cmdMovement.Parameters.AddWithValue("@ProductID", productID);
                        cmdMovement.Parameters.AddWithValue("@MovementType", "مرتجع مبيعات");
                        cmdMovement.Parameters.AddWithValue("@Quantity", returnQty);
                        cmdMovement.Parameters.AddWithValue("@ReferenceType", "مرتجع مبيعات");
                        cmdMovement.Parameters.AddWithValue("@ReferenceID", saleID);
                        cmdMovement.Parameters.AddWithValue("@UserID", CurrentSession.LoggedInUser.UserID);
                        cmdMovement.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmdMovement.ExecuteNonQuery();
                    }
                }

                // سحب من الخزنة
                string queryCash = @"INSERT INTO CashTransactions 
                                   (TransactionDate, TransactionType, Amount, Description,
                                   ReferenceType, ReferenceID, UserID, CreatedDate)
                                   VALUES
                                   (@TransactionDate, @TransactionType, @Amount, @Description,
                                   @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                SqlCommand cmdCash = new SqlCommand(queryCash, connection, transaction);
                cmdCash.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                cmdCash.Parameters.AddWithValue("@TransactionType", "مرتجع مبيعات");
                cmdCash.Parameters.AddWithValue("@Amount", -totalReturn); // سالب = صرف
                cmdCash.Parameters.AddWithValue("@Description", $"مرتجع مبيعات - فاتورة رقم {saleID}");
                cmdCash.Parameters.AddWithValue("@ReferenceType", "مرتجع مبيعات");
                cmdCash.Parameters.AddWithValue("@ReferenceID", saleID);
                cmdCash.Parameters.AddWithValue("@UserID", CurrentSession.LoggedInUser.UserID);
                cmdCash.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmdCash.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show("تم إتمام المرتجع بنجاح!\n\nتم إرجاع المبلغ وتحديث المخزون",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إخفاء panel التفاصيل
                Panel panelDetails = this.Controls.Find("panelDetails", true)[0] as Panel;
                panelDetails.Visible = false;

                TextBox txtSaleNumber = this.Controls.Find("txtSaleNumber", true)[0] as TextBox;
                txtSaleNumber.Clear();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }
    }

    // كلاس مساعد لـ NumericUpDown في DataGridView
    public class DataGridViewNumericUpDownColumn : DataGridViewColumn
    {
        public DataGridViewNumericUpDownColumn() : base(new DataGridViewNumericUpDownCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewNumericUpDownCell)))
                {
                    throw new InvalidCastException("Must be a DataGridViewNumericUpDownCell");
                }
                base.CellTemplate = value;
            }
        }

        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; } = 999999;
    }

    public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell
    {
        public DataGridViewNumericUpDownCell() : base()
        {
            this.Style.Format = "N0";
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            NumericUpDown ctl = DataGridView.EditingControl as NumericUpDown;
            if (ctl != null)
            {
                ctl.DecimalPlaces = 0;
                ctl.Minimum = 0;
                ctl.Maximum = 999999;

                if (this.Value != null && this.Value != DBNull.Value)
                    ctl.Value = Convert.ToDecimal(this.Value);
                else
                    ctl.Value = 0;
            }
        }

        public override Type EditType
        {
            get { return typeof(DataGridViewNumericUpDownEditingControl); }
        }

        public override Type ValueType
        {
            get { return typeof(decimal); }
        }

        public override object DefaultNewRowValue
        {
            get { return 0; }
        }
    }

    public class DataGridViewNumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public DataGridViewNumericUpDownEditingControl()
        {
            this.DecimalPlaces = 0;
        }

        public object EditingControlFormattedValue
        {
            get { return this.Value.ToString(); }
            set
            {
                if (value is String)
                {
                    try
                    {
                        this.Value = decimal.Parse((String)value);
                    }
                    catch
                    {
                        this.Value = 0;
                    }
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
                this.Select(0, this.Text.Length);
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        protected override void OnValueChanged(EventArgs eventargs)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }
    }
}
