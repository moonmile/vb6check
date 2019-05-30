using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Vb6Step.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private string _Filename = "";
        public string Filename { get => _Filename; set => SetProperty(ref _Filename, value, nameof(Filename)); }
        private List<VBFile> _Files;
        public List<VBFile> Files { get => _Files; set => SetProperty(ref _Files, value, nameof(Files)); }
        /// <summary>
        /// ステップカウント
        /// </summary>
        internal void StepCount()
        {
            if (string.IsNullOrEmpty(Filename))
                return;

            if ( System.IO.Path.GetExtension( Filename) == ".vbp" )
            {
                /// *.vbp ファイルから、*.frm, *.bas を抽出
                var vbp = new VBPFile(this.Filename);
                for (int i = 0; i < vbp.Files.Count; i++) { vbp.Files[i].Num = i + 1; }
                this.Files = vbp.Files;
                return;
            }
            if ( System.IO.Directory.Exists(Filename) == true )
            {
                /// フォルダーの場合は、フォルダー内の *.frm, *.bas を全てチェックする
                this.Files = new List<VBFile>();
                StepCount(Filename);
                for (int i = 0; i < this.Files.Count; i++) { this.Files[i].Num = i + 1; }
            }
        }
        void StepCount( string path )
        {
            if ( System.IO.Directory.Exists( path ) )
            {
                foreach( var it in System.IO.Directory.GetDirectories(path))
                {
                    StepCount(it);
                }
                var files = System.IO.Directory.GetFiles(path);
                foreach ( var it in files )
                {
                    StepCount(it);
                }
            }
            else
            {
                var ext = System.IO.Path.GetExtension(path).ToLower();
                if ( ext == ".bas" || ext == ".frm")
                {
                    this.Files.Add(new VBFile(path));
                }
            }
        }

        /// <summary>
        /// CSV形式で保存
        /// </summary>
        /// <param name="path"></param>
        public void SaveCsv( string path )
        {
            var text = "項番,フォルダー名,ファイル名,全ステップ,コードのみ,空白抜き\n";
            foreach( var it in this.Files )
            {
                text += $"{it.Num},{it.DirectoryName},{it.FileName},{it.LineOfCount},{it.LineOfCodeOnly},{it.LineOfNoSpace}\n";
            }
            System.IO.File.WriteAllText(path, text, Encoding.GetEncoding("shift_jis"));
        }
    }

    public class VBPFile
    {
        public string Fullpath { get; set; }
        public string FileName => System.IO.Path.GetFileName(this.Fullpath);
        public string DirectoryName => System.IO.Path.GetDirectoryName(this.Fullpath);
        public List<VBFile> Files { get; private set; }

        public VBPFile( string fullpath )
        {
            this.Fullpath = fullpath;

            
            var lines = System.IO.File.ReadAllLines(fullpath, Encoding.GetEncoding("Shift_JIS"));
            this.Files = new List<VBFile>();

            foreach ( var l in lines )
            {
                if ( l.StartsWith("Module=") )
                {
                    var w = l.Split(new string[] { ";" }, StringSplitOptions.None);
                    var name = w[1].Trim();
                    this.Files.Add(new VBFile(this.DirectoryName + "\\" + name));
                }
                if (l.StartsWith("Form="))
                {
                    var w = l.Split(new string[] { "=" }, StringSplitOptions.None);
                    var name = w[1].Trim();
                    this.Files.Add(new VBFile(this.DirectoryName + "\\" + name));
                }
            }
        }
    }
    public class VBFile
    {
        public string Fullpath { get; set; }
        public string FileName => System.IO.Path.GetFileName(this.Fullpath);
        public string DirectoryName => System.IO.Path.GetDirectoryName(this.Fullpath);
        public int LineOfCount { get; private set; }
        public int LineOfCodeOnly { get; private set; }
        public int LineOfNoSpace { get; private set; }
        public int Num { get; set; }
        public VBFile(string fullpath)
        {
            this.Fullpath = fullpath;
            if ( System.IO.File.Exists( fullpath ) == false )
            {
                return;
            }
            // 相対パスを絶対パスに直す
            var fs = new System.IO.FileStream(fullpath, System.IO.FileMode.Open);
            this.Fullpath = fs.Name;
            fs.Close();

            var lines = System.IO.File.ReadAllLines(fullpath, Encoding.GetEncoding("Shift_JIS"));
            this.LineOfCount = lines.Length;
            this.LineOfCodeOnly = 0;
            this.LineOfNoSpace = 0;

            /// コードだけを調べる
            if (System.IO.Path.GetExtension(fullpath).ToLower() == ".frm")
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("Attribute"))
                    {
                        for (; i < lines.Length; i++)
                        {
                            if (!lines[i].StartsWith("Attribute")) break;
                        }
                        this.LineOfCodeOnly = this.LineOfCount - i;
                        break;
                    }
                }
            }
            else
            {
                this.LineOfCodeOnly = this.LineOfCount;
            }
            /// 空白とコメントを飛ばす
            this.LineOfNoSpace = 0;
            for (int i = lines.Length - this.LineOfCodeOnly; i < lines.Length; i++)
            {
                var it = lines[i].Trim();
                if (!(it == "" || it.StartsWith("'")))
                {
                    this.LineOfNoSpace++;
                }
            }
        }
    }
}
