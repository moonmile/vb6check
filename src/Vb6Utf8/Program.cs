using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vb6Utf8
{
    class Program
    {
        /// <summary>
        /// VBプロジェクトの内容を VSCode で読めるように UTF8 コードに変換する
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if ( args.Length != 2 )
            {
                Console.WriteLine("VB6UTF8 [変換対象VB6フォルダ] [出力先フォルダ]");
                Console.WriteLine("ex. VB6UTF8 . out");
                return;
            }
            var app = new Program();
            app.Run(args[0], args[1]);
        }

        void Run( string infolder, string outfolder )
        {
            Console.WriteLine("変換元 " + infolder);
            Console.WriteLine("変換先 " + outfolder);
            /// 変換対象のファイル(*.frm, *.bas, *.vbp) を再帰的に取得
            this.Files = new List<string>();
            search(infolder);
            /// 変換先のファイルを再帰的に取得
            var curdir = Environment.CurrentDirectory;

            foreach ( var it in Files )
            {
                var text = System.IO.File.ReadAllText(it, Encoding.GetEncoding("shift_jis"));
                var outfile = it.Replace(curdir, "");
                outfile = outfolder + "\\" + outfile;

                var folder = System.IO.Path.GetDirectoryName(outfile);
                if (!System.IO.Directory.Exists( folder ))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }
                Console.WriteLine("write: " + outfile);
                System.IO.File.WriteAllText(outfile, text);
            }
        }
        List<string> Files;

        void search( string path )
        {
            if ( System.IO.Directory.Exists( path ) )
            {
                foreach( var it in System.IO.Directory.GetDirectories( path ) )
                {
                    search(it);
                }
                foreach (var it in System.IO.Directory.GetFiles(path))
                {
                    search(it);
                }
            }
            if ( System.IO.File.Exists( path ))
            {
                var ext = System.IO.Path.GetExtension(path).ToLower();
                if (ext == ".bas" || ext == ".frm" || ext == ".vbp")
                {
                    this.Files.Add(path);
                }
            }
        }
    }
}
