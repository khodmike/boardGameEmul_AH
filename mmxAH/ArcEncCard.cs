using System;

namespace mmxAH
{
	public class ArcEncCard: Card
	{private GameEngine en;
		private short loc1=-1, loc2=-1, loc3=-1;
		private string Text1, Text2, Text3;
		private Effect eff1, eff2, eff3;
		private byte districtNum;
		public ArcEncCard (GameEngine eng, short pID)
		{  en=eng;
			ID=pID;




			;
		

		}



		public void Execute( short locnum)
		{   en.curs.resolvingArch.Add(this); 
			if (locnum == loc1)
			{
				en.io.ServerPrintTag (Environment.NewLine + Text1);
				eff1.Execute (en.clock.NextPlayer);

			} else if (locnum == loc2)
			{
				en.io.ServerPrintTag (Environment.NewLine + Text2);
				eff2.Execute (en.clock.NextPlayer);

			} else if( locnum==loc3)
			{ en.io.ServerPrintTag (Environment.NewLine + Text3);
				eff3.Execute (en.clock.NextPlayer);

			}

		}

		public void Discard()
		{ 
			en.archEncs [districtNum].Add (this);   
		}

		public bool FromTextFile( TextFileParser data, TextFileParser text, short l1,short l2, short l3, byte dn)
		{ loc1=l1;
		 loc2 = l2;
		 loc3 = l3;
		  districtNum = dn; 
			byte stringNum;

			eff1= Effect.FromTextFile(data, en); 
			if (eff1 == null)
				return false;

			eff2= Effect.FromTextFile(data, en); 
			if (eff2 == null)
				return false;
			eff3= Effect.FromTextFile(data, en); 
			if (eff3 == null)
				return false;

			if (! byte.TryParse (text.GetToken (), out stringNum))
				return false;
			for (int i=0; i< stringNum; i++)
				Text1 += text.GetCurString () + Environment.NewLine;

			if (! byte.TryParse (text.GetToken (), out stringNum))
				return false;
			for (int i=0; i< stringNum; i++)
				Text2 += text.GetCurString () + Environment.NewLine;

			if (! byte.TryParse (text.GetToken (), out stringNum))
				return false;
			for (int i=0; i< stringNum; i++)
				Text3 += text.GetCurString () + Environment.NewLine;

			return true;

		}
	}
}

