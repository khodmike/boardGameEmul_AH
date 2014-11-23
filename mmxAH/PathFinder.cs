using System;
using System.Collections.Generic;

namespace mmxAH
{
	public class PathFinder
	{ private GameEngine en;
		private List<short> doneLoc, curLoc, nextLoc, res;
		public PathFinder ( GameEngine eng)
		{ en = eng;
		}

		public List<short> Find( short startLoc, FuncBoolReturn checkFunc)
		{ doneLoc = new List<short> ();
			curLoc = new List<short> ();
			res = new List<short> ();
			curLoc.Add (startLoc);
			doneLoc.Add (startLoc);
			do
			{nextLoc = new List<short> ();
              foreach( short loc in curLoc)
					if( checkFunc(loc))
						res.Add(loc);
				    else
					{ foreach( short newLoc in  ( ( ArchemArea) en.locs[loc]).GetLinkedLocs())
					    if( ! doneLoc.Contains(newLoc)  )
						{ doneLoc.Add(newLoc);
						nextLoc.Add(newLoc); 
						}

					}



			curLoc=nextLoc;   
			} while( res.Count == 0 && curLoc.Count != 0);  
			return res;

		}

	}
}

