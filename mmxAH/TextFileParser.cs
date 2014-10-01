using System;
using System.IO;
namespace mmxAH
{
	public class TextFileParser
	{  private string WorkFileName;
		private string CommentString;
		private FileStream fs;
		StreamReader rd;
		private string CurStr;
		public bool isMultiName=false;

		public TextFileParser (string FileName, string pComment="//" )
		{
			WorkFileName =CreatePath(FileName);
			CommentString= pComment; 


		}

		public bool Open ()
		{
			if (! File.Exists (WorkFileName))
			{

				return false; 
			}

			fs = new FileStream (WorkFileName, FileMode.Open, FileAccess.Read);
			rd = new StreamReader (fs);
			CurStr=rd.ReadLine();
			if( CurStr == null)
				return false;

			return true;
		}

		public string GetToken()
		{




				GetNewString ();
				int index=CurStr.IndexOf(" ");
				if( index == -1)
				{ string a=CurStr;
					CurStr="";

					return a; 
				}
				string tok=CurStr.Substring (0, index);
				CurStr=CurStr.Remove (0, index);
				return tok;









		}


		public string GetCurString ()
		{string res;
			GetNewString();
		    res= CurStr;
			CurStr="";
			return res;

		}




		private void GetNewString ()
		{ do
			{

				CurStr=CurStr.Trim ();
				if(!(CurStr=="" || CurStr.StartsWith(CommentString)))
					return;








			}
			while ((CurStr=rd.ReadLine())  != null);


		}

		public  void Close()
		{rd.Close ();
		 fs.Close ();

		}

		public  static  string CreatePath (string filename)
		{
			filename = filename.Trim ();
			if (filename [0] == '/' || filename [0] == '\\')
			{
				filename=filename.Remove(0,1); 

			};
			 filename=filename.Replace('\\', '/');
			filename=filename.Replace ('/', System.IO.Path.DirectorySeparatorChar); 

			return System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, filename);

		}



	}
}

