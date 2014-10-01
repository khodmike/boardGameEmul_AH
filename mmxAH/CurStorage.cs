using System;
using System.Collections.Generic;

namespace mmxAH
{
	public class CurStorage
	{ private GameEngine en;
		public MonsterIndivid curFight=null;
		public MythosEnv curEnv=null; 
		public MythosRumor curRumor=null; 
		public MythosCard resolvingMythos=null;
		public List<OWEncCard> resolvingOW;
		public  List<ArcEncCard>  resolvingArch; 
		public CurStorage (GameEngine eng)
		{ en=eng;
			resolvingOW = new List<OWEncCard> ();
			resolvingArch = new List<ArcEncCard> (); 
		}

		public void Reset()
		{ if (curFight != null)
				en.MonstersCup.Add (curFight);
			curFight = null; 
			if (resolvingMythos != null)
				en.mythosDeck.Add (resolvingMythos); 
			resolvingMythos = null; 
			if (curEnv != null)
			{
				curEnv.Stop ();
				en.mythosDeck.Add (curEnv);
			}
			curEnv = null; 
			if (curRumor != null)
			{
				curRumor.Reset ();
				en.mythosDeck.Add (curRumor);
			}
			curRumor = null; 
			DiscardEncounters (); 

		}

		public void DiscardEncounters()
		{ foreach (OWEncCard  c in  resolvingOW)
				en.owEnc.Discard (c);
			foreach (ArcEncCard  c in  resolvingArch)
				c.Discard ();
			resolvingOW.Clear ();
			resolvingArch.Clear (); 
		  

		}
	}
}

