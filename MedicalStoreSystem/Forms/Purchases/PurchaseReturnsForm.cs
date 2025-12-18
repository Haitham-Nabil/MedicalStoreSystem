using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Forms.Sales;
using MedicalStoreSystem.Helpers;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MedicalStoreSystem.Forms.Purchases
{
    public partial class PurchaseReturnsForm : Form
    {
        private PurchaseDAL purchaseDAL = new PurchaseDAL();

        public PurchaseReturnsForm()
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
            groupSearch.Text = "بحث عن فاتورة مشتريات";
            groupSearch.Location = new System.Drawing.Point(20, 10);
            groupSearch.Size = new System.Drawing.Size(600, 100);

            Label lblPurchaseNumber = new Label();
            lblPurchaseNumber.Text = "رقم الفاتورة:";
            lblPurchaseNumber.Location = new System.Drawing.Point(20, 30);
            lblPurchaseNumber.AutoSize = true;

            TextBox txtPurchaseNumber = new TextBox();
            txtPurchaseNumber.Name = "txtPurchaseNumber";
            txtPurchaseNumber.Location = new System.Drawing.Point(120, 28);
            txtPurchaseNumber.Size = new System.Drawing.Size(200, 25);

            Button btnSearch = new Button();
            btnSearch.Text = "بحث";
            btnSearch.Location = new System.Drawing.Point(330, 26);
            btnSearch.Size = new System.Drawing.Size(80, 30);
            btnSearch.Click += BtnSearch_Click;

            Label lblInfo = new Label();
            lblInfo.Name = "lblInfo";
            lblInfo.Text = "أدخل رقم فاتورة المشتريات للبحث...";
            lblInfo.Location = new System.Drawing.Point(20, 65);
            lblInfo.Size = new System.Drawing.Size(550, 25);
            lblInfo.ForeColor = System.Drawing.Color.Gray;

            groupSearch.Controls.Add(lblPurchaseNumber);
            groupSearch.Controls.Add(txtPurchaseNumber);
            groupSearch.Controls.Add(btnSearch);
            groupSearch.Controls.Add(lblInfo);
            panelTop.Controls.Add(groupSearch);

            // Panel للتفاصيل
            Panel panelDetails = new Panel();
            panelDetails.Name = "panelDetails";
            panelDetails.Dock = DockStyle.Fill;
            panelDetails.Visible = false;

            GroupBox groupItems = new GroupBox();
            groupItems.Text = "أصناف فاتورة المشتريات";
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
                TextBox txtPurchaseNumber = this.Controls.Find("txtPurchaseNumber", true)[0] as TextBox;
                string purchaseNumber = txtPurchaseNumber.Text.Trim();

                if (string.IsNullOrWhiteSpace(purchaseNumber))
                {
                    MessageBox.Show("يرجى إدخال رقم الفاتورة", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // البحث عن الفاتورة
                string query = @"SELECT PurchaseID FROM Purchases WHERE PurchaseNumber = @PurchaseNumber";
                SqlParameter[] parameters = {
                    new SqlParameter("@PurchaseNumber", purchaseNumber)
                };

                DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    int purchaseID = Convert.ToInt32(dt.Rows[0]["PurchaseID"]);
                    LoadPurchaseDetails(purchaseID);
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

        private void LoadPurchaseDetails(int purchaseID)
        {
            try
            {
                // جلب تفاصيل الفاتورة
                var purchase = purchaseDAL.GetPurchaseByID(purchaseID);

                if (purchase != null)
                {
                    Label lblInfo = this.Controls.Find("lblInfo", true)[0] as Label;
                    lblInfo.Text = $"الفاتورة: {purchase.PurchaseNumber} - المورد: {purchase.SupplierName} - التاريخ: {purchase.PurchaseDate:yyyy-MM-dd}";

                    // عرض الأصناف في DataGridView
                    DataGridView dgv = this.Controls.Find("dgvItems", true)[0] as DataGridView;
                    dgv.Columns.Clear();
                    dgv.Rows.Clear();

                    // إضافة الأعمدة
                    dgv.Columns.Add("PurchaseDetailID", "PurchaseDetailID");
                    dgv.Columns["PurchaseDetailID"].Visible = false;

                    dgv.Columns.Add("ProductID", "ProductID");
                    dgv.Columns["ProductID"].Visible = false;

                    dgv.Columns.Add("ProductName", "اسم المنتج");
                    dgv.Columns["ProductName"].ReadOnly = true;

                    dgv.Columns.Add("Quantity", "الكمية المشتراة");
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
                    foreach (var detail in purchase.Details)
                    {
                        int rowIndex = dgv.Rows.Add();
                        DataGridViewRow row = dgv.Rows[rowIndex];

                        row.Cells["PurchaseDetailID"].Value = detail.PurchaseDetailID;
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

                    // حفظ PurchaseID و SupplierID
                    panelDetails.Tag = new { PurchaseID = purchaseID, SupplierID = purchase.SupplierID };
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
                            MessageBox.Show("الكمية المرتجعة لا يمكن أن تكون أكبر من الكمية المشتراة!", "تنبيه",
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

                dynamic data = panelDetails.Tag;
                int purchaseID = data.PurchaseID;
                int supplierID = data.SupplierID;

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
                    ProcessReturn(purchaseID, supplierID, dgv, totalReturn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessReturn(int purchaseID, int supplierID, DataGridView dgv, decimal totalReturn)
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

                        // خصم من المخزون
                        string queryStock = @"UPDATE Products 
                                            SET CurrentStock = CurrentStock - @Quantity 
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
                        cmdMovement.Parameters.AddWithValue("@MovementType", "مرتجع مشتريات");
                        cmdMovement.Parameters.AddWithValue("@Quantity", -returnQty); // سالب
                        cmdMovement.Parameters.AddWithValue("@ReferenceType", "مرتجع مشتريات");
                        cmdMovement.Parameters.AddWithValue("@ReferenceID", purchaseID);
                        cmdMovement.Parameters.AddWithValue("@UserID", CurrentSession.LoggedInUser.UserID);
                        cmdMovement.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        cmdMovement.ExecuteNonQuery();
                    }
                }

                // إضافة للخزنة (استرداد المبلغ من المورد)
                string queryCash = @"INSERT INTO CashTransactions 
                                   (TransactionDate, TransactionType, Amount, Description,
                                   ReferenceType, ReferenceID, UserID, CreatedDate)
                                   VALUES
                                   (@TransactionDate, @TransactionType, @Amount, @Description,
                                   @ReferenceType, @ReferenceID, @UserID, @CreatedDate)";

                SqlCommand cmdCash = new SqlCommand(queryCash, connection, transaction);
                cmdCash.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                cmdCash.Parameters.AddWithValue("@TransactionType", "مرتجع مشتريات");
                cmdCash.Parameters.AddWithValue("@Amount", totalReturn); // موجب = إيداع
                cmdCash.Parameters.AddWithValue("@Description", $"مرتجع مشتريات - فاتورة رقم {purchaseID}");
                cmdCash.Parameters.AddWithValue("@ReferenceType", "مرتجع مشتريات");
                cmdCash.Parameters.AddWithValue("@ReferenceID", purchaseID);
                cmdCash.Parameters.AddWithValue("@UserID", CurrentSession.LoggedInUser.UserID);
                cmdCash.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmdCash.ExecuteNonQuery();

                // تحديث حساب المورد (تخفيض رصيده)
                string querySupplier = @"UPDATE Suppliers 
                                       SET CurrentBalance = CurrentBalance - @Amount 
                                       WHERE SupplierID = @SupplierID";

                SqlCommand cmdSupplier = new SqlCommand(querySupplier, connection, transaction);
                cmdSupplier.Parameters.AddWithValue("@Amount", totalReturn);
                cmdSupplier.Parameters.AddWithValue("@SupplierID", supplierID);
                cmdSupplier.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show("تم إتمام المرتجع بنجاح!\n\nتم إضافة المبلغ للخزنة وتحديث المخزون وحساب المورد",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إخفاء panel التفاصيل
                Panel panelDetails = this.Controls.Find("panelDetails", true)[0] as Panel;
                panelDetails.Visible = false;

                TextBox txtPurchaseNumber = this.Controls.Find("txtPurchaseNumber", true)[0] as TextBox;
                txtPurchaseNumber.Clear();
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
}
