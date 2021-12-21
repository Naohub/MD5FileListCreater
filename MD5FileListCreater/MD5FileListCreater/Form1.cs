using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MD5FileListCreater
{
    public partial class Form1 : Form
    {

        string _selectedPath = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void SelectFilesButton_Click(object sender, EventArgs e)
        {
            // ダイアログボックスの表示
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // リストボックスの初期化
                lstFiles.Items.Clear();

                // 選択されたファイルをテキストボックスに表示する
                foreach (string strFilePath in openFileDialog1.FileNames)
                {
                    // ファイルパスからファイル名を取得
                    string strFileName = System.IO.Path.GetFileName(strFilePath);

                    //パス取得
                    _selectedPath = System.IO.Path.GetDirectoryName(strFilePath);

                    // リストボックスにファイル名を表示
                    lstFiles.Items.Add(strFileName);
                }
            }
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {

            StringBuilder sb = new StringBuilder();
            sb.Clear();

            foreach (string str in lstFiles.Items) {
 
                sb.Append( CalcMD5(_selectedPath,str));
                sb.Append("  ");
                sb.Append(str);
                sb.Append("\n");
            }

            OutputFile(sb.ToString());
        }

        private String CalcMD5(string dirName, string fileName)
        {

            //MD5ハッシュ値を計算するファイル

            //ファイルを開く
            System.IO.FileStream fs = new System.IO.FileStream(
                System.IO.Path.Combine(dirName, fileName),
                System.IO.FileMode.Open,
                System.IO.FileAccess.Read,
                System.IO.FileShare.Read);

            //MD5CryptoServiceProviderオブジェクトを作成
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();

            //ハッシュ値を計算する
            byte[] bs = md5.ComputeHash(fs);

            //リソースを解放する
            md5.Clear();
            //ファイルを閉じる
            fs.Close();

            //byte型配列を16進数の文字列に変換
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            //ここの部分は次のようにもできる
            //string result = BitConverter.ToString(bs).ToLower().Replace("-","");

            //結果を表示
            return result.ToString();
        }

        private void OutputFile(string md5) {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true; // ユーザーが拡張子を省略したときに、自動的に拡張子を付けるか。規定値はtrue。
            dlg.CheckFileExists = false; // ユーザーが存在しないファイルを指定したときに、警告するか。規定値はfalse。
            dlg.CheckPathExists = true; // ユーザーが存在しないパスを指定したときに、警告するか。規定値はtrue。
            dlg.CreatePrompt = false; // ユーザーが存在しないファイルを指定したときに、作成の許可を求めるか。規定値はfalse。
            dlg.DefaultExt = string.Empty; // ダイアログに表示するファイルの拡張子。規定値はEmpty。
            dlg.DereferenceLinks = false; // ショートカットが参照先を返す場合はtrue。リンクファイルを返す場合はfalse。規定値はfalse。
            dlg.FileName = "calcmd5.txt"; // 選択されたファイルのフルパス。
            dlg.Filter = "テキスト文書|*.txt"; // ダイアログで表示するファイルの種類のフィルタを指定する文字列。
            dlg.FilterIndex = 1; // 選択されたFilterのインデックス。規定値は1。
            dlg.InitialDirectory = _selectedPath; // ダイアログの初期ディレクトリ。規定値はEmpty。
            dlg.OverwritePrompt = true; // 存在するファイルを指定したときに、警告するか。規定値はtrue。
            dlg.Title = "保存するファイル名を入力してください。"; // ダイアログのタイトル。
            dlg.ValidateNames = true; // ファイル名がWin32に適合するか検査するかどうか。規定値はfalse。

            if (DialogResult.OK == dlg.ShowDialog())
            {

                string filename = dlg.FileName;

                Encoding enc = Encoding.GetEncoding("shift_jis");
                try
                {
                    File.WriteAllText(filename, md5, enc);
                    MessageBox.Show("出力完了");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }
}
