using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalStoreSystem.DAL;
using MedicalStoreSystem.Helpers;

namespace MedicalStoreSystem.Forms.Products
{
    public partial class StockAdjustmentForm : Form
    {
        private ProductDAL productDAL = new ProductDAL();

        public StockAdjustmentForm()
        {
            InitializeComponent();

            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "تسوية المخزون";

            InitializeControls();
        }

        private void InitializeControls()
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = "بيانات التسوية";
            groupBox.Location = new System.Drawing.Point(20, 20);
            groupBox.Size = new System.Drawing.Size(540, 250);

            Label lblProduct = new Label();
            lblProduct.Text = "المنتج:";
            lblProduct.Location = new System.Drawing.Point(20, 30);
            lblProduct.AutoSize = true;

            ComboBox cmbProduct = new ComboBox();
            cmbProduct.Name = "cmbProduct";
            cmbProduct.Location = new System.Drawing.Point(20, 55);
            cmbProduct.Size = new System.Drawing.Size(490, 25);
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;

            Label lblCurrentStock = new Label();
            lblCurrentStock.Text = "الكمية الحالية:";
            lblCurrentStock.Location = new System.Drawing.Point(20, 95);
            lblCurrentStock.AutoSize = true;

            Label lblCurrentValue = new Label();
            lblCurrentValue.Name = "lblCurrentValue";
            lblCurrentValue.Text = "0";
            lblCurrentValue.Location = new System.Drawing.Point(150, 95);
            lblCurrentValue.AutoSize = true;
            lblCurrentValue.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);

            Label lblNewStock = new Label();
            lblNewStock.Text = "الكمية الصحيحة:";
            lblNewStock.Location = new System.Drawing.Point(20, 130);
            lblNewStock.AutoSize = true;

            NumericUpDown numNewStock = new NumericUpDown();
            numNewStock.Name = "numNewStock";
            numNewStock.Location = new System.Drawing.Point(150, 128);
            numNewStock.Size = new System.Drawing.Size(100, 25);
            numNewStock.Maximum = 999999;

            Label lblNotes = new Label();
            lblNotes.Text = "ملاحظات:";
            lblNotes.Location = new System.Drawing.Point(20, 165);
            lblNotes.AutoSize = true;

            TextBox txtNotes = new TextBox();
            txtNotes.Name = "txtNotes";
            txtNotes.Location = new System.Drawing.Point(20, 190);
            txtNotes.Size = new System.Drawing.Size(490, 25);
            txtNotes.Multiline = true;
            txtNotes.Height = 50;

            Button btnSave = new Button();
            btnSave.Text = "حفظ التسوية";
            btnSave.Location = new System.Drawing.Point(200, 290);
            btnSave.Size = new System.Drawing.Size(120, 35);
            btnSave.BackColor = System.Drawing.Color.Green;
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Click += BtnSave_Click;

            groupBox.Controls.Add(lblProduct);
            groupBox.Controls.Add(cmbProduct);
            groupBox.Controls.Add(lblCurrentStock);
            groupBox.Controls.Add(lblCurrentValue);
            groupBox.Controls.Add(lblNewStock);
            groupBox.Controls.Add(numNewStock);
            groupBox.Controls.Add(lblNotes);
            groupBox.Controls.Add(txtNotes);

            this.Controls.Add(groupBox);
            this.Controls.Add(btnSave);

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                ComboBox cmb = this.Controls.Find("cmbProduct", true)[0] as ComboBox;
                var products = productDAL.GetActiveProducts();
                cmb.DataSource = products;
                cmb.DisplayMember = "ProductName";
                cmb.ValueMember = "ProductID";
                cmb.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedIndex != -1)
            {
                var product = cmb.SelectedItem as MedicalStoreSystem.Models.Product;
                Label lbl = this.Controls.Find("lblCurrentValue", true)[0] as Label;
                lbl.Text = product.CurrentStock.ToString();

                NumericUpDown num = this.Controls.Find("numNewStock", true)[0] as NumericUpDown;
                num.Value = product.CurrentStock;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ComboBox cmbProduct = this.Controls.Find("cmbProduct", true)[0] as ComboBox;
                if (cmbProduct.SelectedIndex == -1)
                {
                    MessageBox.Show("يرجى اختيار المنتج", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var product = cmbProduct.SelectedItem as MedicalStoreSystem.Models.Product;
                NumericUpDown numNewStock = this.Controls.Find("numNewStock", true)[0] as NumericUpDown;
                TextBox txtNotes = this.Controls.Find("txtNotes", true)[0] as TextBox;

                int oldStock = product.CurrentStock;
                int newStock = (int)numNewStock.Value;
                int difference = newStock - oldStock;

                if (difference == 0)
                {
                    MessageBox.Show("لا يوجد فرق في الكمية", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    $"هل تريد تسوية مخزون '{product.ProductName}'?\n\n" +
                    $"الكمية الحالية: {oldStock}\n" +
                    $"الكمية الصحيحة: {newStock}\n" +
                    $"الفرق: {difference}",
                    "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // تحديث المخزون
                    string queryUpdate = @"UPDATE Products 
                                         SET CurrentStock = @NewStock 
                                         WHERE ProductID = @ProductID";

                    SqlParameter[] parameters = {
                        new SqlParameter("@NewStock", newStock),
                        new SqlParameter("@ProductID", product.ProductID)
                    };

                    int affected = DatabaseConnection.ExecuteNonQuery(queryUpdate, parameters);

                    if (affected > 0)
                    {
                        // تسجيل حركة المخزن
                        string queryMovement = @"INSERT INTO StockMovements 
                                               (MovementDate, ProductID, MovementType, Quantity, 
                                               ReferenceType, ReferenceID, Notes, UserID, CreatedDate)
                                               VALUES
                                               (@MovementDate, @ProductID, @MovementType, @Quantity,
                                               @ReferenceType, @ReferenceID, @Notes, @UserID, @CreatedDate)";

                        SqlParameter[] movementParams = {
                            new SqlParameter("@MovementDate", DateTime.Now),
                            new SqlParameter("@ProductID", product.ProductID),
                            new SqlParameter("@MovementType", "تسوية"),
                            new SqlParameter("@Quantity", difference),
                            new SqlParameter("@ReferenceType", "تسوية"),
                            new SqlParameter("@ReferenceID", DBNull.Value),
                            new SqlParameter("@Notes", txtNotes.Text ?? ""),
                            new SqlParameter("@UserID", CurrentSession.LoggedInUser.UserID),
                            new SqlParameter("@CreatedDate", DateTime.Now)
                        };

                        DatabaseConnection.ExecuteNonQuery(queryMovement, movementParams);

                        MessageBox.Show("تمت التسوية بنجاح", "نجاح",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
