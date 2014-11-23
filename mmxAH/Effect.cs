using System;

namespace mmxAH
{
	public abstract  class Effect
	{  protected GameEngine en;
		protected  Func rp;
		protected byte invnum;
		public Effect( GameEngine eng) 
		{  en=eng;

		}

		public virtual void Execute( Func f,  byte pInvNum=40)
		{ rp = f;
			if (pInvNum == 40)
			invnum = en.clock.GetCurPlayer ();
			else
				invnum = pInvNum;  

		}
		protected virtual bool ReadFromTextIndivid(TextFileParser data)
		{
			return true;
		}
		public static Effect FromTextFile(TextFileParser data, GameEngine eng)
		{  Effect res;
			switch (data.GetToken ().ToUpper ())
		{ case "NO": res = new EffNothing (eng); break; 
			case "TONEARGATE": res = new EffToNearGate (eng); break; 
			case "TONEARINVEST": res = new EffToNearInvest (eng); break; 
		  case "SANLOSE": res = new EffSanityLose (eng); break; 
		  case "STALOSE": res = new EffStaminaLose (eng); break;  
		  case "MONSTER": res = new EffMonsterApears  (eng); break;
		  case "MOVEROLL": res = new EffMonsterMoveRoll (eng);break;   
		  case "LITAS": res = new EffLitas  (eng);break;   
			  default: return null;
			}

			if (! res.ReadFromTextIndivid (data))
				return null;
			return res;

		}


 }


	public  class EffNothing : Effect
	{   public EffNothing( GameEngine eng ) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			f();

		}
	}


	public  class EffLitas : Effect
	{   public EffLitas( GameEngine eng ) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			en.ActiveInvistigators [invnum].SetLocathion (0);
			//тут проверки на попадание в Litas

			f();

		}
	}


	public  class EffMonsterMoveRoll : Effect
	{   private Effect effToAll;
		public EffMonsterMoveRoll( GameEngine eng ) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			//СДЕЛАТЬ
			f();

		}

		protected override bool ReadFromTextIndivid (TextFileParser data)
		{
			effToAll = Effect.FromTextFile (data, en);
			if (effToAll == null)
				return false;
			return true;
		}
	}
	public  class EffToNearGate : Effect
	{   public EffToNearGate( GameEngine eng) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			if (en.locs [en.ActiveInvistigators [invnum].GetLocathion()].GetLocType () == LocathionType.OW)
			{
				en.ga.ReturnToArchem (en.ActiveInvistigators [invnum].GetLocathion (), f, invnum); 
			} else
			{ System.Collections.Generic.List<short> potLocs = new PathFinder (en).Find (en.ActiveInvistigators [invnum].GetLocathion (), Check);  
				if (potLocs.Count == 0)
				{
					f ();
					return;
				}
				if (potLocs.Count == 1)
				{ ((ArchemUnstableLoc)en.locs [potLocs [0]]).DrawFromGate (invnum);
					f ();

				}
				//Выбор
			}

		}

		private bool Check(short loc)
		{ if (en.locs [loc].GetLocType() == LocathionType.ArchamUnstable && ((ArchemUnstableLoc)en.locs [loc]).isGate ())  
			return true;
			else
				return false;

		}
	}


	public  class EffToNearInvest : Effect
	{   public EffToNearInvest( GameEngine eng ) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			f();

		}
	}

	public  abstract class EffLose : Effect
	{   
		protected short demageCount;
		public EffLose( GameEngine eng ) : base(eng)
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
		public  EffStaminaLose( GameEngine eng):  base(eng)
		{
		}
		


		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum);
			if (demageCount < 0) 
				demageCount = 0;
			if(en.ActiveInvistigators[invnum].LoseStamina((byte)demageCount))
				rp ();
		}

	


	}

	public  class EffSanityLose : EffLose
	{   

		public  EffSanityLose( GameEngine eng): base(eng) 
		{

		}


		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum); 
			if (demageCount < 0) 
				demageCount = 0;
			if(en.ActiveInvistigators[invnum].LoseSanity((byte)demageCount))
				rp ();
		}




	}


	public  class EffMonsterApears : Effect
	{   public EffMonsterApears( GameEngine eng ) : base(eng)
		{  
		}

		public override void Execute (Func f, byte pInvnum=40)
		{  base.Execute(f, pInvnum);  
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


	
