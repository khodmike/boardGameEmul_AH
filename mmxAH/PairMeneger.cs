using System;
using System.Collections.Generic;
namespace mmxAH
{
	public class PairMeneger
	{

		private List< string> codes;
		private List<string> values;
		public PairMeneger()
		{
			codes= new List<string>();
			values= new List<string>();
		}

		public bool FromText (TextFileParser prs, TextFileParser text)
		{
			int dsc;
			if (! Int32.TryParse (prs.GetToken (), out dsc))
				return false;
			string str, str2;
			for (int i=0; i< dsc; i++)
				if ((str = prs.GetToken ()) == null || (str2 = text.GetToken ()) == null)
					return false;
				else
				{
					codes.Add (str);
				    values.Add (str2);

				}
			return true;

		}

		public override string ToString()
		{ string res= "==Pairs ( "+ codes.Count+ ")=="+ Environment.NewLine  ;
			for(int i=0; i<codes.Count; i++)
				res+="code: "+ codes[i]+ " title: "+ values[i] + Environment.NewLine;

			return res;

		}

		public int GetIndex( string code)
		{ return  codes.IndexOf(code); 
		}

		public string GetCode( byte ind)
		{ return codes[ind];
		}

		public string GetTitle ( byte ind)
		{ return values[ind];

		}

		public string GetTitle (string Code)
		{ int ind=codes.IndexOf(Code);
			if(ind == -1)
				return null;
			return values[ind];

		}

		public void ToBinary( System.IO.BinaryWriter wr)
		{ wr.Write(codes.Count);
          for (int i=0; i< codes.Count; i++)
			{ wr.Write(codes[i]);
			  wr.Write(values[i]);

			}

		}

		public void FromBinary (System.IO.BinaryReader rd)
		{
			int dcs = rd.ReadInt32 ();
			for (int i=0; i< dcs; i++)
			{ codes.Add ( rd.ReadString());
			  values.Add (rd.ReadString());

			}

		}
		}
	}


