namespace WindowsFormsApp1.Forms
{
    partial class SettingsDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInstagramApi = new System.Windows.Forms.TabPage();
            this.grpApiSettings = new System.Windows.Forms.GroupBox();
            this.txtBloksVersioningId = new System.Windows.Forms.TextBox();
            this.lblBloksVersioningId = new System.Windows.Forms.Label();
            this.txtUserAgent = new System.Windows.Forms.TextBox();
            this.lblUserAgent = new System.Windows.Forms.Label();
            this.numRequestTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblRequestTimeout = new System.Windows.Forms.Label();
            this.txtPublicKey = new System.Windows.Forms.TextBox();
            this.lblPublicKey = new System.Windows.Forms.Label();
            this.numEncryptionKeyId = new System.Windows.Forms.NumericUpDown();
            this.lblEncryptionKeyId = new System.Windows.Forms.Label();
            this.tabApplication = new System.Windows.Forms.TabPage();
            this.grpAppSettings = new System.Windows.Forms.GroupBox();
            this.chkEnableDebugLogging = new System.Windows.Forms.CheckBox();
            this.numRetryDelay = new System.Windows.Forms.NumericUpDown();
            this.lblRetryDelay = new System.Windows.Forms.Label();
            this.numMaxRetries = new System.Windows.Forms.NumericUpDown();
            this.lblMaxRetries = new System.Windows.Forms.Label();
            this.txtAvatarFolder = new System.Windows.Forms.TextBox();
            this.lblAvatarFolder = new System.Windows.Forms.Label();
            this.chkDownloadAvatars = new System.Windows.Forms.CheckBox();
            this.tabProxy = new System.Windows.Forms.TabPage();
            this.grpProxySettings = new System.Windows.Forms.GroupBox();
            this.txtProxyPassword = new System.Windows.Forms.TextBox();
            this.lblProxyPassword = new System.Windows.Forms.Label();
            this.txtProxyUsername = new System.Windows.Forms.TextBox();
            this.lblProxyUsername = new System.Windows.Forms.Label();
            this.cmbProxyType = new System.Windows.Forms.ComboBox();
            this.lblProxyType = new System.Windows.Forms.Label();
            this.numProxyPort = new System.Windows.Forms.NumericUpDown();
            this.lblProxyPort = new System.Windows.Forms.Label();
            this.txtProxyHost = new System.Windows.Forms.TextBox();
            this.lblProxyHost = new System.Windows.Forms.Label();
            this.chkEnableProxy = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabInstagramApi.SuspendLayout();
            this.grpApiSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRequestTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEncryptionKeyId)).BeginInit();
            this.tabApplication.SuspendLayout();
            this.grpAppSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRetries)).BeginInit();
            this.tabProxy.SuspendLayout();
            this.grpProxySettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProxyPort)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInstagramApi);
            this.tabControl.Controls.Add(this.tabApplication);
            this.tabControl.Controls.Add(this.tabProxy);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(660, 450);
            this.tabControl.TabIndex = 0;
            // 
            // tabInstagramApi
            // 
            this.tabInstagramApi.Controls.Add(this.grpApiSettings);
            this.tabInstagramApi.Location = new System.Drawing.Point(4, 25);
            this.tabInstagramApi.Name = "tabInstagramApi";
            this.tabInstagramApi.Padding = new System.Windows.Forms.Padding(3);
            this.tabInstagramApi.Size = new System.Drawing.Size(652, 421);
            this.tabInstagramApi.TabIndex = 0;
            this.tabInstagramApi.Text = "Instagram API";
            this.tabInstagramApi.UseVisualStyleBackColor = true;
            // 
            // grpApiSettings
            // 
            this.grpApiSettings.Controls.Add(this.txtBloksVersioningId);
            this.grpApiSettings.Controls.Add(this.lblBloksVersioningId);
            this.grpApiSettings.Controls.Add(this.txtUserAgent);
            this.grpApiSettings.Controls.Add(this.lblUserAgent);
            this.grpApiSettings.Controls.Add(this.numRequestTimeout);
            this.grpApiSettings.Controls.Add(this.lblRequestTimeout);
            this.grpApiSettings.Controls.Add(this.txtPublicKey);
            this.grpApiSettings.Controls.Add(this.lblPublicKey);
            this.grpApiSettings.Controls.Add(this.numEncryptionKeyId);
            this.grpApiSettings.Controls.Add(this.lblEncryptionKeyId);
            this.grpApiSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpApiSettings.Location = new System.Drawing.Point(3, 3);
            this.grpApiSettings.Name = "grpApiSettings";
            this.grpApiSettings.Size = new System.Drawing.Size(646, 415);
            this.grpApiSettings.TabIndex = 0;
            this.grpApiSettings.TabStop = false;
            this.grpApiSettings.Text = "Cài đặt Instagram API";
            // 
            // txtBloksVersioningId
            // 
            this.txtBloksVersioningId.Location = new System.Drawing.Point(180, 360);
            this.txtBloksVersioningId.Name = "txtBloksVersioningId";
            this.txtBloksVersioningId.Size = new System.Drawing.Size(450, 22);
            this.txtBloksVersioningId.TabIndex = 9;
            // 
            // lblBloksVersioningId
            // 
            this.lblBloksVersioningId.AutoSize = true;
            this.lblBloksVersioningId.Location = new System.Drawing.Point(20, 363);
            this.lblBloksVersioningId.Name = "lblBloksVersioningId";
            this.lblBloksVersioningId.Size = new System.Drawing.Size(138, 16);
            this.lblBloksVersioningId.TabIndex = 8;
            this.lblBloksVersioningId.Text = "Bloks Versioning ID:";
            // 
            // txtUserAgent
            // 
            this.txtUserAgent.Location = new System.Drawing.Point(180, 320);
            this.txtUserAgent.Name = "txtUserAgent";
            this.txtUserAgent.Size = new System.Drawing.Size(450, 22);
            this.txtUserAgent.TabIndex = 7;
            // 
            // lblUserAgent
            // 
            this.lblUserAgent.AutoSize = true;
            this.lblUserAgent.Location = new System.Drawing.Point(20, 323);
            this.lblUserAgent.Name = "lblUserAgent";
            this.lblUserAgent.Size = new System.Drawing.Size(80, 16);
            this.lblUserAgent.TabIndex = 6;
            this.lblUserAgent.Text = "User-Agent:";
            // 
            // numRequestTimeout
            // 
            this.numRequestTimeout.Location = new System.Drawing.Point(180, 280);
            this.numRequestTimeout.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numRequestTimeout.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numRequestTimeout.Name = "numRequestTimeout";
            this.numRequestTimeout.Size = new System.Drawing.Size(120, 22);
            this.numRequestTimeout.TabIndex = 5;
            this.numRequestTimeout.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // lblRequestTimeout
            // 
            this.lblRequestTimeout.AutoSize = true;
            this.lblRequestTimeout.Location = new System.Drawing.Point(20, 282);
            this.lblRequestTimeout.Name = "lblRequestTimeout";
            this.lblRequestTimeout.Size = new System.Drawing.Size(154, 16);
            this.lblRequestTimeout.TabIndex = 4;
            this.lblRequestTimeout.Text = "Request Timeout (giây):";
            // 
            // txtPublicKey
            // 
            this.txtPublicKey.Location = new System.Drawing.Point(180, 80);
            this.txtPublicKey.Multiline = true;
            this.txtPublicKey.Name = "txtPublicKey";
            this.txtPublicKey.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPublicKey.Size = new System.Drawing.Size(450, 180);
            this.txtPublicKey.TabIndex = 3;
            // 
            // lblPublicKey
            // 
            this.lblPublicKey.AutoSize = true;
            this.lblPublicKey.Location = new System.Drawing.Point(20, 83);
            this.lblPublicKey.Name = "lblPublicKey";
            this.lblPublicKey.Size = new System.Drawing.Size(145, 16);
            this.lblPublicKey.TabIndex = 2;
            this.lblPublicKey.Text = "Public Key (Base64):";
            // 
            // numEncryptionKeyId
            // 
            this.numEncryptionKeyId.Location = new System.Drawing.Point(180, 40);
            this.numEncryptionKeyId.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numEncryptionKeyId.Name = "numEncryptionKeyId";
            this.numEncryptionKeyId.Size = new System.Drawing.Size(120, 22);
            this.numEncryptionKeyId.TabIndex = 1;
            this.numEncryptionKeyId.Value = new decimal(new int[] {
            228,
            0,
            0,
            0});
            // 
            // lblEncryptionKeyId
            // 
            this.lblEncryptionKeyId.AutoSize = true;
            this.lblEncryptionKeyId.Location = new System.Drawing.Point(20, 42);
            this.lblEncryptionKeyId.Name = "lblEncryptionKeyId";
            this.lblEncryptionKeyId.Size = new System.Drawing.Size(122, 16);
            this.lblEncryptionKeyId.TabIndex = 0;
            this.lblEncryptionKeyId.Text = "Encryption Key ID:";
            // 
            // tabApplication
            // 
            this.tabApplication.Controls.Add(this.grpAppSettings);
            this.tabApplication.Location = new System.Drawing.Point(4, 25);
            this.tabApplication.Name = "tabApplication";
            this.tabApplication.Padding = new System.Windows.Forms.Padding(3);
            this.tabApplication.Size = new System.Drawing.Size(652, 421);
            this.tabApplication.TabIndex = 1;
            this.tabApplication.Text = "Ứng dụng";
            this.tabApplication.UseVisualStyleBackColor = true;
            // 
            // grpAppSettings
            // 
            this.grpAppSettings.Controls.Add(this.chkEnableDebugLogging);
            this.grpAppSettings.Controls.Add(this.numRetryDelay);
            this.grpAppSettings.Controls.Add(this.lblRetryDelay);
            this.grpAppSettings.Controls.Add(this.numMaxRetries);
            this.grpAppSettings.Controls.Add(this.lblMaxRetries);
            this.grpAppSettings.Controls.Add(this.txtAvatarFolder);
            this.grpAppSettings.Controls.Add(this.lblAvatarFolder);
            this.grpAppSettings.Controls.Add(this.chkDownloadAvatars);
            this.grpAppSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAppSettings.Location = new System.Drawing.Point(3, 3);
            this.grpAppSettings.Name = "grpAppSettings";
            this.grpAppSettings.Size = new System.Drawing.Size(646, 415);
            this.grpAppSettings.TabIndex = 0;
            this.grpAppSettings.TabStop = false;
            this.grpAppSettings.Text = "Cài đặt ứng dụng";
            // 
            // chkEnableDebugLogging
            // 
            this.chkEnableDebugLogging.AutoSize = true;
            this.chkEnableDebugLogging.Location = new System.Drawing.Point(23, 220);
            this.chkEnableDebugLogging.Name = "chkEnableDebugLogging";
            this.chkEnableDebugLogging.Size = new System.Drawing.Size(182, 20);
            this.chkEnableDebugLogging.TabIndex = 8;
            this.chkEnableDebugLogging.Text = "Bật ghi log debug (dev)";
            this.chkEnableDebugLogging.UseVisualStyleBackColor = true;
            // 
            // numRetryDelay
            // 
            this.numRetryDelay.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numRetryDelay.Location = new System.Drawing.Point(230, 180);
            this.numRetryDelay.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.numRetryDelay.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numRetryDelay.Name = "numRetryDelay";
            this.numRetryDelay.Size = new System.Drawing.Size(120, 22);
            this.numRetryDelay.TabIndex = 7;
            this.numRetryDelay.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // lblRetryDelay
            // 
            this.lblRetryDelay.AutoSize = true;
            this.lblRetryDelay.Location = new System.Drawing.Point(20, 182);
            this.lblRetryDelay.Name = "lblRetryDelay";
            this.lblRetryDelay.Size = new System.Drawing.Size(204, 16);
            this.lblRetryDelay.TabIndex = 6;
            this.lblRetryDelay.Text = "Thời gian chờ retry (ms):";
            // 
            // numMaxRetries
            // 
            this.numMaxRetries.Location = new System.Drawing.Point(230, 140);
            this.numMaxRetries.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMaxRetries.Name = "numMaxRetries";
            this.numMaxRetries.Size = new System.Drawing.Size(120, 22);
            this.numMaxRetries.TabIndex = 5;
            this.numMaxRetries.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // lblMaxRetries
            // 
            this.lblMaxRetries.AutoSize = true;
            this.lblMaxRetries.Location = new System.Drawing.Point(20, 142);
            this.lblMaxRetries.Name = "lblMaxRetries";
            this.lblMaxRetries.Size = new System.Drawing.Size(183, 16);
            this.lblMaxRetries.TabIndex = 4;
            this.lblMaxRetries.Text = "Số lần retry tối đa (login):";
            // 
            // txtAvatarFolder
            // 
            this.txtAvatarFolder.Location = new System.Drawing.Point(230, 100);
            this.txtAvatarFolder.Name = "txtAvatarFolder";
            this.txtAvatarFolder.Size = new System.Drawing.Size(300, 22);
            this.txtAvatarFolder.TabIndex = 3;
            // 
            // lblAvatarFolder
            // 
            this.lblAvatarFolder.AutoSize = true;
            this.lblAvatarFolder.Location = new System.Drawing.Point(20, 103);
            this.lblAvatarFolder.Name = "lblAvatarFolder";
            this.lblAvatarFolder.Size = new System.Drawing.Size(138, 16);
            this.lblAvatarFolder.TabIndex = 2;
            this.lblAvatarFolder.Text = "Thư mục lưu avatar:";
            // 
            // chkDownloadAvatars
            // 
            this.chkDownloadAvatars.AutoSize = true;
            this.chkDownloadAvatars.Checked = true;
            this.chkDownloadAvatars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDownloadAvatars.Location = new System.Drawing.Point(23, 60);
            this.chkDownloadAvatars.Name = "chkDownloadAvatars";
            this.chkDownloadAvatars.Size = new System.Drawing.Size(208, 20);
            this.chkDownloadAvatars.TabIndex = 1;
            this.chkDownloadAvatars.Text = "Tự động tải ảnh đại diện";
            this.chkDownloadAvatars.UseVisualStyleBackColor = true;
            // 
            // tabProxy
            // 
            this.tabProxy.Controls.Add(this.grpProxySettings);
            this.tabProxy.Location = new System.Drawing.Point(4, 25);
            this.tabProxy.Name = "tabProxy";
            this.tabProxy.Size = new System.Drawing.Size(652, 421);
            this.tabProxy.TabIndex = 2;
            this.tabProxy.Text = "Proxy";
            this.tabProxy.UseVisualStyleBackColor = true;
            // 
            // grpProxySettings
            // 
            this.grpProxySettings.Controls.Add(this.txtProxyPassword);
            this.grpProxySettings.Controls.Add(this.lblProxyPassword);
            this.grpProxySettings.Controls.Add(this.txtProxyUsername);
            this.grpProxySettings.Controls.Add(this.lblProxyUsername);
            this.grpProxySettings.Controls.Add(this.cmbProxyType);
            this.grpProxySettings.Controls.Add(this.lblProxyType);
            this.grpProxySettings.Controls.Add(this.numProxyPort);
            this.grpProxySettings.Controls.Add(this.lblProxyPort);
            this.grpProxySettings.Controls.Add(this.txtProxyHost);
            this.grpProxySettings.Controls.Add(this.lblProxyHost);
            this.grpProxySettings.Controls.Add(this.chkEnableProxy);
            this.grpProxySettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProxySettings.Location = new System.Drawing.Point(0, 0);
            this.grpProxySettings.Name = "grpProxySettings";
            this.grpProxySettings.Size = new System.Drawing.Size(652, 421);
            this.grpProxySettings.TabIndex = 0;
            this.grpProxySettings.TabStop = false;
            this.grpProxySettings.Text = "Cài đặt Proxy";
            // 
            // txtProxyPassword
            // 
            this.txtProxyPassword.Location = new System.Drawing.Point(200, 220);
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.PasswordChar = '*';
            this.txtProxyPassword.Size = new System.Drawing.Size(300, 22);
            this.txtProxyPassword.TabIndex = 10;
            // 
            // lblProxyPassword
            // 
            this.lblProxyPassword.AutoSize = true;
            this.lblProxyPassword.Location = new System.Drawing.Point(20, 223);
            this.lblProxyPassword.Name = "lblProxyPassword";
            this.lblProxyPassword.Size = new System.Drawing.Size(73, 16);
            this.lblProxyPassword.TabIndex = 9;
            this.lblProxyPassword.Text = "Mật khẩu:";
            // 
            // txtProxyUsername
            // 
            this.txtProxyUsername.Location = new System.Drawing.Point(200, 180);
            this.txtProxyUsername.Name = "txtProxyUsername";
            this.txtProxyUsername.Size = new System.Drawing.Size(300, 22);
            this.txtProxyUsername.TabIndex = 8;
            // 
            // lblProxyUsername
            // 
            this.lblProxyUsername.AutoSize = true;
            this.lblProxyUsername.Location = new System.Drawing.Point(20, 183);
            this.lblProxyUsername.Name = "lblProxyUsername";
            this.lblProxyUsername.Size = new System.Drawing.Size(113, 16);
            this.lblProxyUsername.TabIndex = 7;
            this.lblProxyUsername.Text = "Tên đăng nhập:";
            // 
            // cmbProxyType
            // 
            this.cmbProxyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProxyType.FormattingEnabled = true;
            this.cmbProxyType.Items.AddRange(new object[] {
            "HTTP",
            "HTTPS",
            "SOCKS4",
            "SOCKS5"});
            this.cmbProxyType.Location = new System.Drawing.Point(200, 140);
            this.cmbProxyType.Name = "cmbProxyType";
            this.cmbProxyType.Size = new System.Drawing.Size(150, 24);
            this.cmbProxyType.TabIndex = 6;
            // 
            // lblProxyType
            // 
            this.lblProxyType.AutoSize = true;
            this.lblProxyType.Location = new System.Drawing.Point(20, 143);
            this.lblProxyType.Name = "lblProxyType";
            this.lblProxyType.Size = new System.Drawing.Size(84, 16);
            this.lblProxyType.TabIndex = 5;
            this.lblProxyType.Text = "Loại Proxy:";
            // 
            // numProxyPort
            // 
            this.numProxyPort.Location = new System.Drawing.Point(200, 100);
            this.numProxyPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numProxyPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numProxyPort.Name = "numProxyPort";
            this.numProxyPort.Size = new System.Drawing.Size(120, 22);
            this.numProxyPort.TabIndex = 4;
            this.numProxyPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            // 
            // lblProxyPort
            // 
            this.lblProxyPort.AutoSize = true;
            this.lblProxyPort.Location = new System.Drawing.Point(20, 102);
            this.lblProxyPort.Name = "lblProxyPort";
            this.lblProxyPort.Size = new System.Drawing.Size(38, 16);
            this.lblProxyPort.TabIndex = 3;
            this.lblProxyPort.Text = "Port:";
            // 
            // txtProxyHost
            // 
            this.txtProxyHost.Location = new System.Drawing.Point(200, 60);
            this.txtProxyHost.Name = "txtProxyHost";
            this.txtProxyHost.Size = new System.Drawing.Size(300, 22);
            this.txtProxyHost.TabIndex = 2;
            // 
            // lblProxyHost
            // 
            this.lblProxyHost.AutoSize = true;
            this.lblProxyHost.Location = new System.Drawing.Point(20, 63);
            this.lblProxyHost.Name = "lblProxyHost";
            this.lblProxyHost.Size = new System.Drawing.Size(99, 16);
            this.lblProxyHost.TabIndex = 1;
            this.lblProxyHost.Text = "Proxy Server:";
            // 
            // chkEnableProxy
            // 
            this.chkEnableProxy.AutoSize = true;
            this.chkEnableProxy.Location = new System.Drawing.Point(23, 30);
            this.chkEnableProxy.Name = "chkEnableProxy";
            this.chkEnableProxy.Size = new System.Drawing.Size(163, 20);
            this.chkEnableProxy.TabIndex = 0;
            this.chkEnableProxy.Text = "Bật Proxy (nâng cao)";
            this.chkEnableProxy.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(420, 475);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "💾 Lưu";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(552, 475);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "❌ Hủy";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.Black;
            this.btnReset.Location = new System.Drawing.Point(12, 475);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(120, 35);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "🔄 Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(145, 475);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 35);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "📤 Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(245, 475);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(90, 35);
            this.btnImport.TabIndex = 5;
            this.btnImport.Text = "📥 Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // SettingsDialog
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(684, 521);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "⚙️ Cài Đặt";
            this.Load += new System.EventHandler(this.SettingsDialog_Load);
            this.tabControl.ResumeLayout(false);
            this.tabInstagramApi.ResumeLayout(false);
            this.grpApiSettings.ResumeLayout(false);
            this.grpApiSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRequestTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEncryptionKeyId)).EndInit();
            this.tabApplication.ResumeLayout(false);
            this.grpAppSettings.ResumeLayout(false);
            this.grpAppSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRetryDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRetries)).EndInit();
            this.tabProxy.ResumeLayout(false);
            this.grpProxySettings.ResumeLayout(false);
            this.grpProxySettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProxyPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInstagramApi;
        private System.Windows.Forms.TabPage tabApplication;
        private System.Windows.Forms.TabPage tabProxy;
        private System.Windows.Forms.GroupBox grpApiSettings;
        private System.Windows.Forms.NumericUpDown numEncryptionKeyId;
        private System.Windows.Forms.Label lblEncryptionKeyId;
        private System.Windows.Forms.TextBox txtPublicKey;
        private System.Windows.Forms.Label lblPublicKey;
        private System.Windows.Forms.NumericUpDown numRequestTimeout;
        private System.Windows.Forms.Label lblRequestTimeout;
        private System.Windows.Forms.TextBox txtUserAgent;
        private System.Windows.Forms.Label lblUserAgent;
        private System.Windows.Forms.TextBox txtBloksVersioningId;
        private System.Windows.Forms.Label lblBloksVersioningId;
        private System.Windows.Forms.GroupBox grpAppSettings;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.CheckBox chkDownloadAvatars;
        private System.Windows.Forms.TextBox txtAvatarFolder;
        private System.Windows.Forms.Label lblAvatarFolder;
        private System.Windows.Forms.NumericUpDown numMaxRetries;
        private System.Windows.Forms.Label lblMaxRetries;
        private System.Windows.Forms.NumericUpDown numRetryDelay;
        private System.Windows.Forms.Label lblRetryDelay;
        private System.Windows.Forms.CheckBox chkEnableDebugLogging;
        private System.Windows.Forms.GroupBox grpProxySettings;
        private System.Windows.Forms.CheckBox chkEnableProxy;
        private System.Windows.Forms.TextBox txtProxyHost;
        private System.Windows.Forms.Label lblProxyHost;
        private System.Windows.Forms.NumericUpDown numProxyPort;
        private System.Windows.Forms.Label lblProxyPort;
        private System.Windows.Forms.ComboBox cmbProxyType;
        private System.Windows.Forms.Label lblProxyType;
        private System.Windows.Forms.TextBox txtProxyUsername;
        private System.Windows.Forms.Label lblProxyUsername;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.Label lblProxyPassword;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
    }
}
