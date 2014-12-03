using System;

namespace mmxAH
{
	public class OWMeneger
	{  private GameEngine en;
	   private byte OwCount;
		public OWMeneger ( GameEngine eng)
		{ en=eng;
		}

		public bool FromText(TextFileParser prs, TextFileParser text)
		{if (! Byte.TryParse (prs.GetToken (), out OwCount))
				return false;
			OWLoc ow;
			for (int i=0; i< OwCount; i++)
			{ ow= new OWLoc(en);
              if( ! ow.FromText(prs, text))
					return false;
			 ow.SetLocIndex((byte)(en.locs.Count));
				en.locs.Add(ow);
			}
			return true;
		}

		public void Print ()
		{ 
			foreach( Locathion l in en.locs)
				if( l.GetLocType()== LocathionType.OW ) 
			{ l.Print (1); en.io.Print( Environment.NewLine);}     

		}

		public int Index (string codename)
		{  for( int i=0; i< en.locs.Count ; i++  )
			 if(en.locs[i].GetCodeName()==codename && en.locs[i].GetLocType() == LocathionType.OW)
				return i;
			return -1;

		}


		public void FromBin( System.IO.BinaryReader rd)
		{ OwCount= rd.ReadByte();
			OWLoc l;
			for (int i=0; i< OwCount; i++)
			{  l = new OWLoc (en);
				l.FromBin (rd); 
				en.locs.Add(l);
			}

		}

		public void ToBin( System.IO.BinaryWriter wr)
		{ wr.Write(OwCount);
			foreach (Locathion l in en.locs)
				if (l.GetLocType () == LocathionType.OW) 
					l.ToBin (wr);

		}


	}
}

