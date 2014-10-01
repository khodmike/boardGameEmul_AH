using System;

namespace mmxAH
{
	public class OWEncCard: Card
	{  private byte color;
		private GameEngine en;
		private short loc1=-1, loc2=-1;
		private string Text1, Text2, TextOther;
		private Effect eff1, eff2, effOther;
		public OWEncCard (GameEngine eng, short pID)
		{  en=eng;
			ID=pID;
		}

		public byte GetColor()
		{
			return color;
		}


		public void Execute(short locnum)
		{  en.curs.resolvingOW.Add(this); 
			if (locnum == loc1)
			{
				en.io.ServerPrintTag (Environment.NewLine + Text1);
				eff1.Execute (en.clock.NextPlayer);

			} else if (locnum == loc2)
			{
				en.io.ServerPrintTag (Environment.NewLine + Text2);
				eff2.Execute (en.clock.NextPlayer);

			} else
			{ en.io.ServerPrintTag (Environment.NewLine + TextOther);
				effOther.Execute (en.clock.NextPlayer);

			}

		}


	}
}

