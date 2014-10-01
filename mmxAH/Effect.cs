using System;

namespace mmxAH
{
	public abstract  class Effect
	{  protected GameEngine en;
		protected  Func rp;
		protected byte invnum;
		public Effect( GameEngine eng, byte pInvnum=40 ) 
		{  en=eng;
			if (pInvnum == 40)
				invnum = en.clock.GetCurPlayer ();
			else
				invnum = pInvnum;  
		}

		public abstract void Execute( Func f);
		protected virtual bool ReadFromTextIndivid(TextFileParser data)
		{
			return true;
		}
		public static Effect FromTextFile(TextFileParser data, GameEngine eng)
		{  Effect res;
			switch (data.GetToken ().ToUpper ())
		{ case "NO": res = new EffNothing (eng); break; 
			case "TONEARGATE": res = new EffToNearGate (eng); break; 
		  case "SANLOSE": res = new EffSanityLose (eng); break; 
		  case "STALOSE": res = new EffStaminaLose (eng); break;  
		  case "MONSTER": res = new EffMonsterApears  (eng); break;  
			
			  default: return null;
			}

			if (! res.ReadFromTextIndivid (data))
				return null;
			return res;

		}


 }


	public  class EffNothing : Effect
	{   public EffNothing( GameEngine eng,  byte pInvnum=40 ) : base(eng, pInvnum)
		{  
		}

		public override void Execute (Func f)
		{ f();

		}
	}
	public  class EffToNearGate : Effect
	{   public EffToNearGate( GameEngine eng,  byte pInvnum=40 ) : base(eng, pInvnum)
		{  
		}

		public override void Execute (Func f)
		{ rp=f;

		}
	}


	public  abstract class EffLose : Effect
	{   
		protected short demageCount;
		public EffLose( GameEngine eng, byte pInvnum=40 ) : base(eng, pInvnum)
		{  
		}


		protected override bool ReadFromTextIndivid (TextFileParser data)
		{ if( ! short.TryParse(data.GetToken(),out demageCount))
			     return false;
			return true;
		}


		public void ChangeDemageCount(short modif)
		{
			demageCount += modif; 
		}
	}


	public  class EffStaminaLose : EffLose
	{   
		public  EffStaminaLose( GameEngine eng, byte pInvnum=40 ):  base(eng, pInvnum)
		{
		}
		


		public override void Execute (Func f)
		{ rp=f;
			if (demageCount < 0) 
				demageCount = 0;
			if(en.ActiveInvistigators[invnum].LoseStamina((byte)demageCount))
				rp ();
		}

	


	}

	public  class EffSanityLose : EffLose
	{   

		public  EffSanityLose( GameEngine eng, byte pInvnum=40 ): base(eng, pInvnum) 
		{

		}


		public override void Execute (Func f)
		{ rp=f;
			if (demageCount < 0) 
				demageCount = 0;
			if(en.ActiveInvistigators[invnum].LoseSanity((byte)demageCount))
				rp ();
		}




	}


	public  class EffMonsterApears : Effect
	{   public EffMonsterApears( GameEngine eng,  byte pInvnum=40 ) : base(eng, pInvnum)
		{  
		}

		public override void Execute (Func f)
		{ rp=f;
			MonsterIndivid m = en.MonstersCup.Draw ();
			if (m != null)
			{
				m.SetRPAfterEncounter (Clear); 
				m.BeginEncounter ();
			}
			else
			  en.ga.Awekeen (); 


		}

		private void Clear()
		{ if (en.curs.curFight != null)
			{
				en.MonstersCup.Add (en.curs.curFight);
				en.curs.curFight = null;
			}
			rp ();
		}

	}







	}


	
