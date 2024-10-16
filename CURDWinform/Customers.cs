using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CURDWinform
{
    public partial class Customer : Form
    {
        string constr = @"Data Source=DESKTOP-NHH\SQLEXPRESS;Initial Catalog=CURD;Integrated Security=True; TrustServerCertificate=True";

        public Customer()
        {
            InitializeComponent();
        }

        private void Customers_Load(object sender, EventArgs e)
        {
            GetCustomer();
            btnBoQua_Click(sender,e);
        }

        public void GetCustomer()
        {
            using (SqlConnection cnn = new SqlConnection(constr))
            {
                // Mở kết nối
                cnn.Open();
                string query = "SELECT * FROM dbo.Customer";
                SqlDataAdapter da = new SqlDataAdapter(query, cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvKhachHang.DataSource = dt;
            }
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            // Kiểm tra Tag có null không trước khi gọi ToString()
            string id = btnNhap.Tag != null ? btnNhap.Tag.ToString() : string.Empty;
            string procedureName = string.IsNullOrEmpty(id) ? "spKH_insert" : "spKH_update";

            using (SqlConnection cnn = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(procedureName, cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrEmpty(id))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                    }
                    // Thêm các tham số khác cho Stored Procedure
                    cmd.Parameters.AddWithValue("@Name", txtTenKH.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtSDT.Text);

                    cnn.Open();
                    cmd.ExecuteNonQuery();
                    cnn.Close();

                    btnBoQua_Click(sender, e);
                    GetCustomer(); 
                }
            }
        }

        private void btnBoQua_Click(object sender, EventArgs e)
        {
            txtTenKH.Text
            = txtSDT.Text
            = txtMaKH.Text
            = txtEmail.Text
            = txtSDT.Text
            = string.Empty;
            txtMaKH.Focus();

            btnNhap.Text = "Thêm Mới";
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnNhap.Tag = string.Empty;
            GetCustomer();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xóa khách hàng này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                // Lấy DataRowView từ DataSource
                DataRowView drvKhachHang = GetCurrentCustomer();
                if (drvKhachHang == null)
                {
                    MessageBox.Show("Không tìm thấy khách hàng hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xóa khách hàng
                using (SqlConnection cnn = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand("spKH_delete", cnn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", drvKhachHang["Id"]);

                    cnn.Open();
                    cmd.ExecuteNonQuery();
                }
                GetCustomer();
            }
            catch (SqlException sqlEx) when (sqlEx.Message.Contains("REFERENCE"))
            {
                MessageBox.Show("Khách hàng đang có dữ liệu liên quan, không xóa được", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Đã có lỗi xảy ra, hãy liên hệ với đội ngũ kỹ thuật", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            DataRowView drvKhachHang = GetCurrentCustomer();
            if (drvKhachHang != null)
            {
                chuyenTrangThaiSua(drvKhachHang);
            }
            else
            {
                MessageBox.Show("Không tìm thấy khách hàng hiện tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataRowView GetCurrentCustomer()
        {
            if (dgvKhachHang.DataSource is DataTable dt)
            {
                DataView dvKhachHang = dt.DefaultView;
                return dvKhachHang[dgvKhachHang.CurrentRow.Index];
            }
            else if (dgvKhachHang.DataSource is DataView dv)
            {
                return dv[dgvKhachHang.CurrentRow.Index];
            }
            return null;
        }

        private void chuyenTrangThaiSua(DataRowView drvKhachHang)
        {
            txtTenKH.Text = drvKhachHang["Name"].ToString();
            txtEmail.Text = drvKhachHang["Email"].ToString();
            txtSDT.Text = drvKhachHang["PhoneNumber"].ToString();
            txtMaKH.Text = drvKhachHang["Id"].ToString();

            btnNhap.Text = "Cập nhật";
            btnNhap.Enabled = true;
            btnNhap.Tag = drvKhachHang["Id"].ToString();

            btnSua.Enabled = btnXoa.Enabled = false;
        }

    }
}
