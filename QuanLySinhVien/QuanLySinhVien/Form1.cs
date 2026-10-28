using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace QuanLySinhVien
{
    public partial class Form1 : Form
    {
        // Doi Data Source neu may thi dung SQL Server instance khac
        private const string ConnStr =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=DuLieu;Integrated Security=True;TrustServerCertificate=True";

        public Form1()
        {
            InitializeComponent();
        }

        
      
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            DatTrangThaiBanDau();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT MaSinhVien, TenSinhVien, DiemTrungBinh FROM SinhVien", conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvSinhVien.DataSource = dt;
                }

                dgvSinhVien.Columns[0].HeaderText = "Mã Sinh Viên";
                dgvSinhVien.Columns[1].HeaderText = "Tên Sinh Viên";
                dgvSinhVien.Columns[2].HeaderText = "Điểm Trung Bình";
                dgvSinhVien.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void DatTrangThaiBanDau()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiem.Clear();
            txtMa.ReadOnly = false;

            btnThem.Enabled = true;
            btnThoat.Enabled = true;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;

            dgvSinhVien.ClearSelection();
            txtMa.Focus();
        }

        private void txtDiem_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;

            if (char.IsControl(c) || char.IsDigit(c))
            {
                return;
            }

          
            if ((c == '.' || c == ',') && txtDiem.Text.IndexOf('.') < 0 && txtDiem.Text.IndexOf(',') < 0)
            {
                return;
            }

            e.Handled = true;
        }

        private bool LaSo(string s)
        {
            double kq;
            return double.TryParse(s.Replace(',', '.'), NumberStyles.Float,
                CultureInfo.InvariantCulture, out kq);
        }

 
        private bool KiemTraNhapDayDu()
        {
            if (txtMa.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập mã sinh viên.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMa.Focus();
                return false;
            }

            if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập tên sinh viên.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTen.Focus();
                return false;
            }

            if (txtDiem.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập điểm trung bình.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                return false;
            }

            if (!LaSo(txtDiem.Text.Trim()))
            {
                MessageBox.Show("Điểm trung bình phải là số nguyên hoặc số thực.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiem.Focus();
                txtDiem.SelectAll();
                return false;
            }

            return true;
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult kq = MessageBox.Show("Bạn có muốn thoát không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (kq == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

  
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraNhapDayDu())
            {
                return;
            }

            string ma = txtMa.Text.Trim();
            string ten = txtTen.Text.Trim();
            string diem = txtDiem.Text.Trim().Replace(',', '.');

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

              
                    using (SqlCommand cmdKiemTra = new SqlCommand(
                        "SELECT COUNT(*) FROM SinhVien WHERE MaSinhVien = @Ma", conn))
                    {
                        cmdKiemTra.Parameters.AddWithValue("@Ma", ma);
                        int soLuong = Convert.ToInt32(cmdKiemTra.ExecuteScalar());

                        if (soLuong > 0)
                        {
                            MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập lại.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtMa.Focus();
                            txtMa.SelectAll();
                            return;
                        }
                    }

                    using (SqlCommand cmdThem = new SqlCommand(
                        "INSERT INTO SinhVien (MaSinhVien, TenSinhVien, DiemTrungBinh) " +
                        "VALUES (@Ma, @Ten, @Diem)", conn))
                    {
                        cmdThem.Parameters.AddWithValue("@Ma", ma);
                        cmdThem.Parameters.AddWithValue("@Ten", ten);
                        cmdThem.Parameters.AddWithValue("@Diem", diem);
                        cmdThem.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Thêm sinh viên thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                DatTrangThaiBanDau();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yeu cau 5: chon mot sinh vien tren luoi
        private void dgvSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];
            txtMa.Text = Convert.ToString(row.Cells["MaSinhVien"].Value);
            txtTen.Text = Convert.ToString(row.Cells["TenSinhVien"].Value);
            txtDiem.Text = Convert.ToString(row.Cells["DiemTrungBinh"].Value);

            // Ma la khoa chinh nen khong cho sua khi dang chon mot dong
            txtMa.ReadOnly = true;

            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnThem.Enabled = false;
        }

        // Yeu cau 6: Xoa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult kq = MessageBox.Show("Bạn có muốn xóa sinh viên này không?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (kq != DialogResult.Yes)
            {
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(
                    "DELETE FROM SinhVien WHERE MaSinhVien = @Ma", conn))
                {
                    cmd.Parameters.AddWithValue("@Ma", txtMa.Text.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadData();
                DatTrangThaiBanDau();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Yeu cau 7: Sua
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!KiemTraNhapDayDu())
            {
                return;
            }

            string ma = txtMa.Text.Trim();
            string ten = txtTen.Text.Trim();
            string diem = txtDiem.Text.Trim().Replace(',', '.');

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE SinhVien SET TenSinhVien = @Ten, DiemTrungBinh = @Diem " +
                    "WHERE MaSinhVien = @Ma", conn))
                {
                    cmd.Parameters.AddWithValue("@Ma", ma);
                    cmd.Parameters.AddWithValue("@Ten", ten);
                    cmd.Parameters.AddWithValue("@Diem", diem);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật sinh viên thành công.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadData();
                DatTrangThaiBanDau();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
