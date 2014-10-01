using System;

namespace mmxAH
{
	public class TextWorker
	{ private GameEngine en;
		private const  byte special_card_count=5;
		public TextWorker (GameEngine eng)
		{ en=eng;
		}

		public bool TextInit (String DataFileName,  String LangFileName)
		{
			TextFileParser data = new TextFileParser (DataFileName, "//");
			TextFileParser text = new TextFileParser (LangFileName , "//");
			if (! data.Open ()) 
				return false;
			if( ! text.Open())
				return false;

			if( text.GetCurString().Trim ().ToUpper() == "MULTINAME")
				text.isMultiName=true; 

			en.sysstr.FromTextFile(text); 
			if (! en.ds.FromText (data, text))
				return false;
			if (! en.colors.FromText (data, text))
				return false;
			int cn;
			if (! en.LimitsFromText (data))
				return false;


			en.locs.Add ( new Litas(en)); 
			if (! en.ows.FromText (data,text))
				return false;

			if (! Int32.TryParse (data.GetToken (), out cn))
				return false;
			GatePrototype g = new GatePrototype (en);
			for (int i=0; i< cn; i++)
			{ if( ! g.FromText(data))
				return false;
				en.gates.Add (g);
				g = new GatePrototype (en);

			}
			en.gates.Shuffle(); 

			en.map= new MapOfCity(en);
			if( ! en.map.FromText(data, text))
				return false;

			for(int i=0; i<special_card_count; i++) 
				en.scTexts.Add (new SpecialCardText ().FromText (text));

			if (! en.InvestigatorsFromText (data, text))
				return false;

			if (! int.TryParse (data.GetToken (), out cn)) 
				return false;
			MonsterPrototype mpr;
			for (int i=0; i<cn; i++)
			{   mpr = new MonsterPrototype (en);
				if (! mpr.FromTextFile (data, text))
					return false;
				en.MonsterPrototypes.Add (mpr); 


			}

		

			if (! int.TryParse (data.GetToken (), out cn)) 
				return false;
			en.archEncs = new Deck<ArcEncCard> [cn];
			for (int i=0; i<cn; i++)
			{en.archEncs [i] = new Deck<ArcEncCard> (true); 
				if (! LoadDistricktEncounters (data,text))
					return false;
			}


			if (! int.TryParse (data.GetToken (), out cn)) 
				return false;
			MythosCard c;
			for (int i=0; i< cn; i++)
			{ switch (data.GetToken ().ToUpper() )
				{ case "HEAD": c = new MythosHead (en, (short)i); break; 
				  case "ENV": c = new MythosEnv (en, (short)i); break;
				  case "RUMOR": c = new MythosRumor (en, (short)i); break; 
				  default : return false;

				}

				if (! c.FromTextFile (data, text))
					return false;
				en.mythosDeck.Add (c);
			}




			data.Close ();
			text.Close ();


			return true;

		}

		private bool  LoadDistricktEncounters( TextFileParser data, TextFileParser text)
		{ byte distNum, distCount;
			short l1, l2, l3;
			if (! byte.TryParse (data.GetToken (), out distNum) )
				return false;
			if (distNum >= en.archEncs.Length)
				return false;
			l1 = en.map.GetNumberByCodeName (data.GetToken());
			l2 = en.map.GetNumberByCodeName (data.GetToken());
			l3 = en.map.GetNumberByCodeName (data.GetToken());
			if (l1 == -1 || l2 == -1 || l3 == -1)
				return false;
			if (! byte.TryParse (data.GetToken (), out distCount) )
				return false;
			ArcEncCard c; 
			for (byte i =0; i< distCount; i++)
			{ c = new ArcEncCard (en, i);
				if (! c.FromTextFile (data, text, l1, l2, l3, distNum ))
					return false;
				en.archEncs [distNum].Add (c);  

			}
			return true;
		}
	}
}

