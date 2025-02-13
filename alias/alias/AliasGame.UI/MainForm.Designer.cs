using System.ComponentModel;

namespace AliasGame.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtNickname;
    private System.Windows.Forms.Button btnJoin;
    private Label lblCurrentWord;
    private Label lblTimer;
    private ListBox lstPlayers;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnCorrect;
    private System.Windows.Forms.Button btnIncorrect;
    private System.Windows.Forms.Button btnRandomTeams;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        txtNickname = new System.Windows.Forms.TextBox();
        btnJoin = new System.Windows.Forms.Button();
        lblCurrentWord = new System.Windows.Forms.Label();
        lblTimer = new System.Windows.Forms.Label();
        lstPlayers = new System.Windows.Forms.ListBox();
        btnStart = new System.Windows.Forms.Button();
        btnCorrect = new System.Windows.Forms.Button();
        btnIncorrect = new System.Windows.Forms.Button();
        btnRandomTeams = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // txtNickname
        // 
        txtNickname.Location = new System.Drawing.Point(20, 20);
        txtNickname.MaxLength = 20;
        txtNickname.Name = "txtNickname";
        txtNickname.PlaceholderText = "Введите ваше имя";
        txtNickname.Size = new System.Drawing.Size(150, 23);
        txtNickname.TabIndex = 0;
        // 
        // btnJoin
        // 
        btnJoin.Location = new System.Drawing.Point(180, 20);
        btnJoin.Name = "btnJoin";
        btnJoin.Size = new System.Drawing.Size(150, 23);
        btnJoin.TabIndex = 1;
        btnJoin.Text = "Присоединиться";
        // 
        // lblCurrentWord
        // 
        lblCurrentWord.Location = new System.Drawing.Point(20, 60);
        lblCurrentWord.Name = "lblCurrentWord";
        lblCurrentWord.Size = new System.Drawing.Size(200, 30);
        lblCurrentWord.TabIndex = 2;
        lblCurrentWord.Text = "Ожидание слова...";
        // 
        // lblTimer
        // 
        lblTimer.Location = new System.Drawing.Point(230, 60);
        lblTimer.Name = "lblTimer";
        lblTimer.Size = new System.Drawing.Size(100, 23);
        lblTimer.TabIndex = 3;
        lblTimer.Text = "60";
        // 
        // lstPlayers
        // 
        lstPlayers.Location = new System.Drawing.Point(20, 100);
        lstPlayers.Name = "lstPlayers";
        lstPlayers.Size = new System.Drawing.Size(200, 144);
        lstPlayers.TabIndex = 4;
        // 
        // btnStart
        // 
        btnStart.Location = new System.Drawing.Point(20, 253);
        btnStart.Name = "btnStart";
        btnStart.Size = new System.Drawing.Size(95, 30);
        btnStart.TabIndex = 5;
        btnStart.Text = "Начать игру";
        // 
        // btnCorrect
        // 
        btnCorrect.Location = new System.Drawing.Point(230, 100);
        btnCorrect.Name = "btnCorrect";
        btnCorrect.Size = new System.Drawing.Size(100, 23);
        btnCorrect.TabIndex = 6;
        btnCorrect.Text = "Угадал";
        // 
        // btnIncorrect
        // 
        btnIncorrect.Location = new System.Drawing.Point(230, 130);
        btnIncorrect.Name = "btnIncorrect";
        btnIncorrect.Size = new System.Drawing.Size(100, 23);
        btnIncorrect.TabIndex = 7;
        btnIncorrect.Text = "Не угадал";
        // 
        // btnRandomTeams
        // 
        btnRandomTeams.Location = new System.Drawing.Point(180, 253);
        btnRandomTeams.Name = "btnRandomTeams";
        btnRandomTeams.Size = new System.Drawing.Size(150, 30);
        btnRandomTeams.TabIndex = 8;
        btnRandomTeams.Text = "Случайные команды";
        // 
        // MainForm
        // 
        AcceptButton = btnJoin;
        ClientSize = new System.Drawing.Size(354, 300);
        Controls.Add(txtNickname);
        Controls.Add(btnJoin);
        Controls.Add(lblCurrentWord);
        Controls.Add(lblTimer);
        Controls.Add(lstPlayers);
        Controls.Add(btnStart);
        Controls.Add(btnCorrect);
        Controls.Add(btnIncorrect);
        Controls.Add(btnRandomTeams);
        Text = "Alias Game";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}